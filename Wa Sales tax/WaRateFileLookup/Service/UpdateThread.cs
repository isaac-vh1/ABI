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
	public class UpdateThread
	{
		private IRateService m_srv;
		private Timer m_timer;
		private string m_localDirectory;

		public UpdateThread(IRateService srv, string localDirectory, int updateFreqSeconds)
		{
			m_srv = srv;
			m_localDirectory = localDirectory;

			TimerCallback timerDelegate = new TimerCallback(CheckForUpdates);

			// runs immediately and then every hour
			m_timer = new Timer(timerDelegate, null, 1, 1000 * updateFreqSeconds);
		}

		public void Stop()
		{
			lock (this)
			{
				m_timer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		private void CheckForUpdates(Object stateInfo)
		{
            try
            {
                lock (this)
                {
                    if (FileMaintenance.IsUpdateAvailable(m_localDirectory))
                    {
                        FileMaintenance.UpdateFiles(m_localDirectory);
                        LogFile.SysWriteLog("New data files downloaded");
                    }

                    Period curPeriod = Period.CurrentPeriod();
                    if (!m_srv.Locator.HadData || !curPeriod.Equals(m_srv.Locator.Period))
                    {
                        string addrFileName;
                        string rateFileName;
                        string zipFileName;

                        FileMaintenance.GetLocalFileBaseNames(curPeriod, out addrFileName, out rateFileName, out zipFileName);
                        addrFileName = m_localDirectory + addrFileName + ".csv";
                        rateFileName = m_localDirectory + rateFileName + ".csv";
                        zipFileName = m_localDirectory + zipFileName + ".csv";

                        m_srv.Locator.ReLoad(addrFileName, rateFileName, zipFileName);
                        LogFile.SysWriteLog("New data loaded for period " + curPeriod.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.SysWriteLog("UpdateThread::CheckForUpdates", ex);
            }
		}
	}
}
