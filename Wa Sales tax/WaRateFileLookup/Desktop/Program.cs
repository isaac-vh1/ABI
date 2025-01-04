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
using System.Windows.Forms;

namespace WaRateFiles
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			string debugCurrentPeriod = ConfigurationManager.AppSettings["DEBUG_PERIOD"];
			if (null != debugCurrentPeriod && debugCurrentPeriod.Length > 0)
			{
				Period.DebugCurrentPeriod = Period.Parse(debugCurrentPeriod);
			}

			if (FileMaintenance.IsUpdateAvailable("." + Path.DirectorySeparatorChar))
			{
				FileMaintenance.UpdateFiles("." + Path.DirectorySeparatorChar);
			}

			// For debugging the unit tests
			WaRateFiles.UnitTest.LexTest lextest = new WaRateFiles.UnitTest.LexTest();
			lextest.TestLex1();
			lextest.TestLex2();
			lextest.TestLex3();

			WaRateFiles.UnitTest.TokenizerTest tt = new WaRateFiles.UnitTest.TokenizerTest();
			tt._DebugTraceEntryPoint();

			string addfn, zipfn, ratefn;
			FileMaintenance.GetLocalFileBaseNames(Period.CurrentPeriod(), out addfn, out ratefn, out zipfn);
			UnitTest.RateTest rt = new UnitTest.RateTest(addfn + ".csv", ratefn + ".csv", zipfn + ".csv");
			rt._DebugTraceEntryPoint();
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());
		}
	}
}