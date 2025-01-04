using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

using WaRateFiles.Support;

namespace WaRateFiles
{
	/// <summary>
	/// Provides tools for checking and downloading address and
	/// rate updates from DOR.
	/// </summary>
	public class FileMaintenance
	{
		private static string m_dldPageUrl = "/content/FindTaxesAndRates/SalesAndUseTaxRates/stdownloads.aspx";
		private static string m_dldUrl = "/downloads/Add_Data/";

		private static string DownloadPageUrl
		{
			get
			{
				string dorwagov = ConfigurationManager.AppSettings["DORWAGOV_URL"];
				if (null == dorwagov)
				{
					dorwagov = "http://dor.wa.gov";
				}
				Debug.Assert(!dorwagov.EndsWith("\\") && !dorwagov.EndsWith("/"));

				return dorwagov + m_dldPageUrl;
			}
		}

		public static string DownloadSite
		{
			get
			{
				string dorwagov = ConfigurationManager.AppSettings["DORWAGOV_URL"];
				if (null == dorwagov)
				{
					dorwagov = "http://dor.wa.gov";
				}
				Debug.Assert(!dorwagov.EndsWith("\\") && !dorwagov.EndsWith("/"));

				return dorwagov + m_dldUrl;
			}
		}

       		/// <summary>
        	/// Looks in the downloadPageHTML for the string passed in filename
        	/// </summary>
		private static bool PingFile(string filename, string downloadPageHTML)
		{
            		return downloadPageHTML.ToUpper().IndexOf(filename.ToUpper()) > -1;
		}


		public static void GetRemoteFileBaseNames(Period period, out string stateBaseFile, out string rateBaseFile, out string zipBaseFile)
		{
			if (period.Year == 2008 && period.PeriodNum == 2)
			{
				stateBaseFile = "state";
				zipBaseFile = "ZIP4RATES" + period.Year.ToString() + "Q2";
			}
			else
			{
				stateBaseFile = "State_" + period.Year.ToString().Substring(2) + "Q" + period.PeriodNum;
				zipBaseFile = "ZIP4Q" + period.PeriodNum.ToString() + period.Year.ToString().Substring(2) + "C";
			}
			rateBaseFile = "Rates" + period.Year.ToString() + "Q" + period.PeriodNum;
		}

		public static void GetLocalFileBaseNames(Period period, out string stateBaseFile, out string rateBaseFile, out string zipBaseFile)
		{
			stateBaseFile = "State_" + period.Year.ToString().Substring(2) + "Q" + period.PeriodNum;
			zipBaseFile = "ZIP4Q" + period.PeriodNum.ToString() + period.Year.ToString().Substring(2) + "C";
			rateBaseFile = "Rates" + period.Year.ToString() + "Q" + period.PeriodNum;
		}
        
        private static string GetDownloadPageHTML()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(DownloadPageUrl);
            req.AllowAutoRedirect = false;
            req.KeepAlive = false;
            req.Method = "GET";

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            string downloadPageHTML = reader.ReadToEnd();
            reader.Close();
            if (HttpStatusCode.OK != resp.StatusCode)
            {
                resp.Close();
                throw new Exception("Download page returned status code " + resp.StatusCode.ToString() + " " + DownloadPageUrl);
            }
            resp.Close();

            return downloadPageHTML;
        }

		public static bool IsUpdateAvailable(string localDirectory)
		{
			Period period = Period.CurrentPeriod();
			string localStateBaseFile;
			string localRateBaseFile;
			string localZipBaseFile;
			string remoteStateBaseFile;
			string remoteRateBaseFile;
			string remoteZipBaseFile;
            		string downloadPageHTML = "";

			localDirectory = StringHelper.EnsureTrailingChar(localDirectory, Path.DirectorySeparatorChar);

			GetLocalFileBaseNames(period, out localStateBaseFile, out localRateBaseFile, out localZipBaseFile);
			GetRemoteFileBaseNames(period, out remoteStateBaseFile, out remoteRateBaseFile, out remoteZipBaseFile);

            		//web request to get the contents of the download files page
            		//this was changed by Ryan Dolby so there is only 1 web request per call to IsUpdateAvailable().
            		downloadPageHTML = GetDownloadPageHTML();

			if (!File.Exists(localDirectory + localStateBaseFile + ".csv"))
			{
                		if (PingFile(remoteStateBaseFile + ".zip", downloadPageHTML))
				{
					return true;
				}
			}
			if (!File.Exists(localDirectory + localRateBaseFile + ".csv"))
			{
                		if (PingFile(remoteRateBaseFile + ".zip", downloadPageHTML))
                		{
					return true;
				}
			}
			if (!File.Exists(localDirectory + localZipBaseFile + ".csv"))
			{
                		if (PingFile(remoteZipBaseFile + ".zip", downloadPageHTML))
                		{
					return true;
				}
			}

			// Check the next period
			period = period.NextPeriod;

			GetLocalFileBaseNames(period, out localStateBaseFile, out localRateBaseFile, out localZipBaseFile);
			GetRemoteFileBaseNames(period, out remoteStateBaseFile, out remoteRateBaseFile, out remoteZipBaseFile);

			if (!File.Exists(localDirectory + localStateBaseFile + ".csv"))
			{
                		if (PingFile(remoteStateBaseFile + ".zip", downloadPageHTML))
                		{
					return true;
				}
			}
			if (!File.Exists(localDirectory + localRateBaseFile + ".csv"))
			{
                		if (PingFile(remoteRateBaseFile + ".zip", downloadPageHTML))
                		{
					return true;
				}
			}
			if (!File.Exists(localDirectory + localZipBaseFile + ".csv"))
			{
                		if (PingFile(remoteZipBaseFile + ".zip", downloadPageHTML))
                		{
					return true;
				}
			}

			return false;
		}

		private static void DownloadFile(string localDirectory, string localBaseFilename, string remoteBaseFilename)
		{
			string localFilename = localDirectory + localBaseFilename + ".csv";
			if (File.Exists(localFilename))
			{
				throw new Exception(localFilename + " exists");
			}

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(DownloadSite + remoteBaseFilename + ".zip");
			req.AllowAutoRedirect = false;
			req.KeepAlive = false;
			req.Method = "GET";

			HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
			if (resp.ContentType != "application/x-zip-compressed")
			{
				resp.Close();
				throw new Exception(req.Address.ToString() + " returned content of " + resp.ContentType);
			}
			ZipInputStream reader = new ZipInputStream(resp.GetResponseStream());
			if (HttpStatusCode.OK != resp.StatusCode)
			{
			    throw new Exception("Download of " + req.Address.ToString() + " returned status code " + resp.StatusCode.ToString());
			}

			ZipEntry zipentry = reader.GetNextEntry();

			FileStream writer = File.OpenWrite(localFilename);

			try
			{
				int count;
				byte[] buf = new byte[1024];

				while ((count = reader.Read(buf, 0, buf.Length)) > 0)
				{
					writer.Write(buf, 0, count);
				}
			}
			finally
			{
				reader.Close();
				resp.Close();
				writer.Close();
			}
		}

		public static void UpdateFiles(string localDirectory)
		{
			Period period = Period.CurrentPeriod();
			string localStateBaseFile;
			string localRateBaseFile;
			string localZipBaseFile;
			string remoteStateBaseFile;
			string remoteRateBaseFile;
			string remoteZipBaseFile;
            		string downloadPageHTML = "";

			localDirectory = StringHelper.EnsureTrailingChar(localDirectory, Path.DirectorySeparatorChar);

			GetLocalFileBaseNames(period, out localStateBaseFile, out localRateBaseFile, out localZipBaseFile);
			GetRemoteFileBaseNames(period, out remoteStateBaseFile, out remoteRateBaseFile, out remoteZipBaseFile);
            
            		downloadPageHTML = GetDownloadPageHTML();

			if (!File.Exists(localDirectory + localStateBaseFile + ".csv"))
			{
				if (PingFile(remoteStateBaseFile + ".zip", downloadPageHTML))
				{
					DownloadFile(localDirectory, localStateBaseFile, remoteStateBaseFile);
				}
			}

			if (!File.Exists(localDirectory + localRateBaseFile + ".csv"))
			{
				if (PingFile(remoteRateBaseFile + ".zip", downloadPageHTML))
				{
					DownloadFile(localDirectory, localRateBaseFile, remoteRateBaseFile);
				}
			}

			if (!File.Exists(localDirectory + localZipBaseFile + ".csv"))
			{
				if (PingFile(remoteZipBaseFile + ".zip", downloadPageHTML))
				{
					DownloadFile(localDirectory, localZipBaseFile, remoteZipBaseFile);
				}
			}

			// Check the next period
			period = period.NextPeriod;

			GetLocalFileBaseNames(period, out localStateBaseFile, out localRateBaseFile, out localZipBaseFile);
			GetRemoteFileBaseNames(period, out remoteStateBaseFile, out remoteRateBaseFile, out remoteZipBaseFile);

			if (!File.Exists(localDirectory + localStateBaseFile + ".csv"))
			{
				if (PingFile(remoteStateBaseFile + ".zip", downloadPageHTML))
				{
					DownloadFile(localDirectory, localStateBaseFile, remoteStateBaseFile);
				}
			}

			if (!File.Exists(localDirectory + localRateBaseFile + ".csv"))
			{
				if (PingFile(remoteRateBaseFile + ".zip", downloadPageHTML))
				{
					DownloadFile(localDirectory, localRateBaseFile, remoteRateBaseFile);
				}
			}

			if (!File.Exists(localDirectory + localZipBaseFile + ".csv"))
			{
				if (PingFile(remoteZipBaseFile + ".zip", downloadPageHTML))
				{
					DownloadFile(localDirectory, localZipBaseFile, remoteZipBaseFile);
				}
			}
		}
	}
}
