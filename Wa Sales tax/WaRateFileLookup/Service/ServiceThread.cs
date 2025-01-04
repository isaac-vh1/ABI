/*
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using WaRateFiles.Support;
using WaRateFiles;

namespace RateLookupService
{
	/// <summary>
	/// Single threaded rate lookup service.  Set USE_THREADPOOL to false in the app.config to use this version.
	/// </summary>
	internal class ServiceThread : IRateService
	{
		private static ObjectPool<StringBuilder> m_sbpool = new ObjectPool<StringBuilder>();

		private int m_timeout;
		private int m_port;
		private RateLookup m_lookup;
		private PerformanceCounters m_counters;
		private Thread m_thread;
		private volatile bool m_running = false;
		private TcpListener m_sockListener;

		private DateTime m_inception = DateTime.Now;
		private long m_requestCount;

		public RateLookup Locator 
		{
			get { return m_lookup; }
		}

		public ServiceThread(int timeoutSec, int port, string addrfn, string ratefn, string zipfn, bool useShortcutEval, PerformanceCounters counters)
		{
			m_counters = counters;
			m_timeout = timeoutSec;
			m_port = port;
			m_sockListener = new TcpListener(IPAddress.Any, m_port);
			m_lookup = new RateLookup(addrfn, ratefn, zipfn, RateLookupEngine.STANDARDIZER, useShortcutEval);
		}

		public ServiceThread(int timeoutSec, int port, bool useShortcutEval, PerformanceCounters counters)
		{
			m_counters = counters;
			m_timeout = timeoutSec;
			m_port = port;
			m_sockListener = new TcpListener(IPAddress.Any, m_port);
			m_lookup = new RateLookup(RateLookupEngine.STANDARDIZER, useShortcutEval);
		}

		public void Start()
		{
			if (m_running)
			{
				throw new Exception("Already running");
			}
			m_sockListener.Start(50);
			m_running = true;
			ThreadStart ts = new ThreadStart(Run);
			m_thread = new Thread(ts);
			m_thread.IsBackground = false;
			m_thread.Start();
		}

		public void Stop()
		{
			m_running = false;
			m_sockListener.Stop();
			m_thread.Join();
		}

		internal static string ProcessRequest(int port, NetworkStream ns, RateLookup lookup, DateTime inception, long requestCount)
		{
			StreamReader sr = new StreamReader(ns);
			string httpHeaderLine = sr.ReadLine();
			string[] uri = httpHeaderLine.Split(new char[] { ' ' });

			bool useXml = false;
			if (uri[1].StartsWith("/xml?"))
			{
				useXml = true;
			}

			string addr = null;
			string city = null;
			string zip = null;

			string[] qargs = uri[1].Substring(uri[1].IndexOf('?') + 1).Split(new char[] { '&' });
			for (int x = 0; x < qargs.Length; x++)
			{
				int eqpos = qargs[x].IndexOf('=');
				if (0 > eqpos)
				{
					continue;
				}
				string key = UrlDecode(qargs[x].Substring(0, eqpos));
				string val = UrlDecode(qargs[x].Substring(eqpos + 1));

				switch (key)
				{
					case "addr":
						addr = val.Trim();
						break;
					case "city":
						city = val.Trim();
						break;
					case "zip":
						zip = val.Trim();
						break;
				}
			}

			StringBuilder httpHeader = m_sbpool.Get();
			StringBuilder httpBody = m_sbpool.Get();
			httpHeader.Length = 0;
			httpHeader.Append("HTTP/1.0 200 OK\r\n");
			httpHeader.Append("Server: WA Rate Service\r\n");
			httpHeader.Append("Expires: 0\r\n");
			httpHeader.Append("Cache-Control: no-cache\r\n");
			httpHeader.Append("Accept: text/*\r\n");

			httpBody.Length = 0;
			httpBody.Append("\r\n");

			if (useXml)
			{
				httpHeader.Append("Content-Type: text/xml\r\n");
				httpBody.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
			}
			else
			{
				httpHeader.Append("Content-Type: text/html\r\n");
			}

			if (uri[1] == "/")
			{
				// Send search page
				GetSearchPageHtml(httpBody, port);
			}
			else if (uri[1] == "/stats")
			{
				GetStatsPageHtml(httpBody, inception, requestCount, lookup.Period);
			}
			else
			{
				if (zip != null)
				{
					if (!ZIP.IsZip(zip))
					{
						httpBody.Append(FormatResult(LocationSource.INVALID_ARGS, useXml, "Invalid ZIP"));
					}
					else
					{
						AddressLine addrline;
						Rate rate = null;
						LocationSource loctype = LocationSource.NONE;
						if (lookup.FindRate(addr, city, zip, out addrline, ref rate, out loctype))
						{
							httpBody.Append(FormatResult(addrline, rate, loctype, useXml));
						}
						else
						{
							httpBody.Append(FormatResult(loctype, useXml, ""));
						}
					}
				}
				else
				{
					httpBody.Append(FormatResult(LocationSource.INVALID_ARGS, useXml, "ZIP code missing."));
				}
			}
			httpBody.Append("\r\n");
			httpHeader.Append("Content-Length: ");
			httpHeader.Append(httpBody.Length - 2);
			httpHeader.Append("\r\n");

			string httpResp = httpHeader.ToString() + httpBody.ToString();

			m_sbpool.Release(httpHeader);
			m_sbpool.Release(httpBody);

			return httpResp;
		}

		private void Run()
		{
			LogFile.SysWriteLog("ServiceThread.run", "Inception " + DateTime.Now.ToString());

			while (m_running)
			{
				try
				{
					TcpClient sock = m_sockListener.AcceptTcpClient();

					long startTicks = PerformanceCounters.CurrentTick;

					m_requestCount++;

					// set short timeout since clients can hang the server
					sock.ReceiveTimeout = m_timeout;
					sock.SendTimeout = m_timeout;

					NetworkStream ns = sock.GetStream();
					StreamWriter writer = new StreamWriter(ns);
					writer.Write(ProcessRequest(m_port, ns, m_lookup, m_inception, m_requestCount));
					writer.Flush();
					writer.Close();
					writer.Dispose();
					ns.Dispose();
					sock.Close();

					if (null != m_counters)
					{
						m_counters.Update(PerformanceCounters.CurrentTick - startTicks);
					}
				}
				catch (Exception ex)
				{
					LogFile.SysWriteLog("ServiceThread.Run", ex);
				}
			}
			m_running = false;
			LogFile.SysWriteLog("ServiceThread.run", "Shutting down " + DateTime.Now.ToString());
		}

		private static string FormatResult(AddressLine addr, Rate rate, LocationSource locsrc, bool xml)
		{
			if (xml)
			{
				return "<response loccode=\"" + rate.LocationCode + "\" localrate=\"" + rate.LocalRate.ToString() + "\" rate=\"" + rate.TotalRate.ToString() + "\" code=\"" + ((int)(locsrc)).ToString() + "\">" +
					((null == addr) ? "<addressline/>" : addr.ToXml()) + rate.ToXml() + "</response>";
			}
			return "LocationCode=" + rate.LocationCode + " Rate=" + rate.TotalRate.ToString() + " ResultCode=" + ((int)locsrc).ToString();
		}

		private static string FormatResult(LocationSource locsrc, bool xml, string msg)
		{
			if (xml)
			{
				return "<response loccode=\"\" localrate=\"\" rate=\"\" code=\"" + ((int)locsrc) + "\" debughint=\"" + msg + "\"><addressline/><rate/></response>";
			}
			else
			{
				return "LocationCode=-1 Rate=-1 ResultCode=" + (int)locsrc + " debughint=" + msg;
			}
		}

		private static string FormatResult(bool xml)
		{
			if (xml)
			{
				return "<response loccode=\"\" localrate=\"\" rate=\"\" code=\"4\" debughint=\"Problem with the request format\"><addressline/><rate/></response>";
			}
			else
			{
				return "LocationCode=-1 Rate=-1 ResultCode=4 debughint=Problem with the request format";
			}
		}

		private static char[] m_encodableChars = new char[] { '%', '&', '+' };

		private static string UrlDecode(string str)
		{
			if (str.IndexOfAny(m_encodableChars) < 0)
			{
				return str;
			}
			return str.Replace("%20", " ").Replace("%23", "#").Replace("&&", "&").Replace("%%", "%").Replace('+', ' ');
		}

		private static void GetStatsPageHtml(StringBuilder http, DateTime inception, long requestCount, Period period)
		{
			string enabled = ConfigurationManager.AppSettings["REMOTE_STATS_ENABLED"];
			if (null != enabled && "true" != enabled)
			{
				http.Append("<html><body>Verboten</body></html>");
				return;
			}
			TimeSpan uptime = DateTime.Now.Subtract(inception);

			http.Append("<html><body>Inception: ");
			http.Append(inception.ToString());
			http.Append("<br />Up time: ");
			http.Append(uptime.ToString());
			http.Append("<br />Period: ");
			http.Append(period.ToString());
			http.Append("<br />Total Requests: ");
			http.Append(requestCount);
			http.Append("<br />Average Requests/min: ");
			http.Append(requestCount / (uptime.TotalSeconds / 60));
			http.Append("<br />");

			try
			{
				ManagementClass memoryClass = new ManagementClass("Win32_OperatingSystem");
				ManagementObjectCollection memory = memoryClass.GetInstances();
				foreach (ManagementObject mo in memory)
				{
					foreach (PropertyData pd in mo.Properties)
					{
						http.Append(pd.Name);
						http.Append(": ");
						http.Append(pd.Value);
						http.Append("<br />");
					}
				}
			}
			catch (Exception)
			{
			}

			http.Append("</body></html>");
		}

		private static string m_searchHtml = null;
		private static object m_searchHtmlLock = new object();

		private static void GetSearchPageHtml(StringBuilder http, int port)
		{
			if (null == m_searchHtml)
			{
				lock (m_searchHtmlLock)
				{
					if (null == m_searchHtml)
					{
						string host = ConfigurationManager.AppSettings["SEARCHPAGE_HOST_NAME"];
						if (null == host || "" == host)
						{
							host = Environment.MachineName;
						}
						string pagePath = ConfigurationManager.AppSettings["SEARCHPAGE"];
						if (!File.Exists(pagePath))
						{
							return;
						}
						m_searchHtml = File.ReadAllText(pagePath);
						m_searchHtml = m_searchHtml.Replace("dorwebgis2", host);
						m_searchHtml = m_searchHtml.Replace("8080", port.ToString());
					}
				}
			}
			http.Append(m_searchHtml);
		}
	}
}
