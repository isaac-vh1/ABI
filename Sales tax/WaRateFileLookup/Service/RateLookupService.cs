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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;

using WaRateFiles;
using WaRateFiles.Support;

namespace RateLookupService
{
	public partial class RateLookupService : ServiceBase
	{
		private IRateService m_srv;
		private UpdateThread m_updater;

		public RateLookupService()
		{
		}

		public void OnStartDebug(string[] args)
		{
			OnStart(args);
		}

		public void OnStopDebug()
		{
			OnStop();
		}

		protected override void OnStart(string[] args)
		{
            try
            {
                string debugPeriod = ConfigurationManager.AppSettings["DEBUG_PERIOD"];
                if (null != debugPeriod && debugPeriod != "")
                {
                    LogFile.SysWriteLog("DEBUG: setting current period to " + debugPeriod + " -- NOT FOR PRODUCTION USE");
                    Period.DebugCurrentPeriod = Period.Parse(debugPeriod);
                }

                string sport = ConfigurationManager.AppSettings["PORT"];
                if (null == sport)
                {
                    throw new ConfigurationErrorsException("Set PORT in the app.config file");
                }
                if (!StringHelper.IsInt(sport))
                {
                    throw new ConfigurationErrorsException("PORT in the app.config file must be an integer.");
                }

                string datadir = ConfigurationManager.AppSettings["DATAFILE_DIR"];
                if (null == datadir)
                {
                    throw new Exception("Set DATAFILE_DIR in the app.config file");
                }
                if (datadir.EndsWith("\\") || datadir.EndsWith("/"))
                {
                    datadir = datadir.Substring(0, datadir.Length - 1);
                }
                if (!Directory.Exists(datadir))
                {
                    throw new Exception("DATAFILE_DIR \"" + datadir + "\" doesn't exist");
                }
                datadir = StringHelper.EnsureTrailingChar(datadir, Path.DirectorySeparatorChar);

                if ("true" == ConfigurationManager.AppSettings["DOWNLOAD_FILES_ON_STARTUP"])
                {
                    FileMaintenance.UpdateFiles(datadir);
                }

                string addrFileName;
                string rateFileName;
                string zipFileName;

                FileMaintenance.GetLocalFileBaseNames(Period.CurrentPeriod(), out addrFileName, out rateFileName, out zipFileName);
                addrFileName = datadir + addrFileName + ".csv";
                rateFileName = datadir + rateFileName + ".csv";
                zipFileName = datadir + zipFileName + ".csv";

                int timeout = 30000;
                string stimeout = ConfigurationManager.AppSettings["TIMEOUT_SEC"];
                if (null == stimeout || !StringHelper.IsInt(stimeout))
                {
                    timeout = Int32.Parse(stimeout) * 1000;
                }

                PerformanceCounters counters = null;
                if ("true" == ConfigurationManager.AppSettings["ENABLE_PERFORMANCE_COUNTERS"])
                {
                    string freeThread = ConfigurationManager.AppSettings["ENABLE_PERFORMANCE_COUNTERS_PERFORMANCE_OVER_ACCURACY"];
                    counters = new PerformanceCounters("WA Tax Rate Service", "WA Tax Rate Lookup Service", "true" != freeThread);
                }

                string suseShortcutEval = ConfigurationManager.AppSettings["USE_SHORTCUT_ADDRESS_EVALUATION"];
                bool useShortcutEval = suseShortcutEval == "true";
                bool threadpooled = ConfigurationManager.AppSettings["USE_THREADPOOL"] == "true";

                if (!File.Exists(addrFileName) || !File.Exists(rateFileName) || !File.Exists(zipFileName))
                {
                    // Let the update thread download the files.
                    if (threadpooled)
                    {
                        m_srv = new ServiceThreadPooled(timeout, Int32.Parse(sport), useShortcutEval, counters);
                    }
                    else
                    {
                        m_srv = new ServiceThread(timeout, Int32.Parse(sport), useShortcutEval, counters);
                    }
                }
                else
                {
                    if (threadpooled)
                    {
                        m_srv = new ServiceThreadPooled(timeout, Int32.Parse(sport), addrFileName, rateFileName, zipFileName, useShortcutEval, counters);
                    }
                    else
                    {
                        m_srv = new ServiceThread(timeout, Int32.Parse(sport), addrFileName, rateFileName, zipFileName, useShortcutEval, counters);
                    }
                }

                // Start the background data file update
                string pingFreqMin = ConfigurationManager.AppSettings["DATAFILE_UPDATE_CHECK_FREQ_MINUTES"];
                if (pingFreqMin == null || !StringHelper.IsInt(pingFreqMin))
                {
                    pingFreqMin = "60";
                }
                m_updater = new UpdateThread(m_srv, datadir, 60 * Int32.Parse(pingFreqMin));

                // Start the service
                m_srv.Start();
            }
            catch (Exception ex)
            {
                LogFile.SysWriteLog("RateLookupService::Run", ex);
                throw ex;
            }
		}

		protected override void OnStop()
		{
			m_updater.Stop();
			m_srv.Stop();
		}
	}
}
