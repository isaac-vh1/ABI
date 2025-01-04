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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using WaRateFiles.Support;
using WaRateFiles;

namespace RateLookupService
{
	internal class LookupTask
	{
		private TcpClient m_sock;
		private RateLookup m_lookup;
		private int m_port;
		private PerformanceCounters m_counters;
		private long m_startTicks;
		
		private DateTime m_inception;
		private long m_requestCount;

		public LookupTask
		(
			 TcpClient sock, 
			 RateLookup lookup, 
			 int port, 
			 PerformanceCounters counters, 
			 long startTicks,
			 DateTime inception,
			 long requestCount
		)
		{
			m_sock = sock;
			m_lookup = lookup;
			m_port = port;
			m_counters = counters;
			m_startTicks = startTicks;
			m_inception = inception;
			m_requestCount = requestCount;
		}

		// Wrapper method for use with thread pool.
		public void ThreadPoolCallback(Object threadContext)
		{
			StreamWriter writer = null;
			NetworkStream ns = null;
			try
			{
				ns = m_sock.GetStream();
				writer = new StreamWriter(ns);
				writer.Write(ServiceThread.ProcessRequest(m_port, ns, m_lookup, m_inception, m_requestCount));
				writer.Flush();
			}
			catch (Exception ex)
			{
				LogFile.SysWriteLog("LookupTask", ex);
				try
				{
					if (null == writer)
					{
						writer = new StreamWriter(m_sock.GetStream());
						writer.Write("HTTP/1.0 500 ERROR\r\nExpires: 0\r\nCache-Control: no-cache\r\n\r\n" + ex.ToString() + "\r\n\r\n");
						writer.Flush();
						writer.Close();
					}
				}
				catch (Exception)
				{
				}
			}
			m_sock.Client.Close();
			m_sock = null;
			m_lookup = null;

			if (null != m_counters)
			{
				m_counters.Update(PerformanceCounters.CurrentTick - m_startTicks);
			}
		}
	}
}
