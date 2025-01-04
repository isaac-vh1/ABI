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
using System.ServiceProcess;
using System.Text;

using WaRateFiles.Support;

namespace RateLookupService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
            try
            {
                RateLookupService service = new RateLookupService();
                if (Environment.UserInteractive || Environment.OSVersion.ToString().ToLower().StartsWith("unix"))
                {
                    service.OnStartDebug(new string[0]);
                    Console.WriteLine("Press any key to stop program, as long as it's enter");
                    Console.Read();
                    service.OnStopDebug();
                }
                else
                {
                    ServiceBase[] servicesToRun = new ServiceBase[] { service };
                    ServiceBase.Run(servicesToRun);
                }
            }
            catch (Exception ex)
            {
                LogFile.SysWriteLog("Program.Main", ex);
                throw ex;
            }
		}
	}
}