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
using System.Text;

namespace WaRateFiles.Support
{
	/// <summary>
	/// Symplistic log file
	/// </summary>
	public class LogFile
	{
		private string m_filename;

		public LogFile(string filename)
		{
			m_filename = filename;
		}

		public void WriteLog(string msg)
		{
			try
			{
				lock (this)
				{
					File.AppendAllText(m_filename, DateTime.Now + ": " + msg + Environment.NewLine);
				}
			}
			catch (Exception)
			{
				// Ignore log file errors
			}
		}

		public void WriteLog(string caller, string msg)
		{
			try
			{
				lock (this)
				{
					File.AppendAllText(m_filename, DateTime.Now + " " + caller + ": " + msg + Environment.NewLine);
				}
			}
			catch (Exception)
			{
				// Ignore log file errors
			}
		}

		public void WriteLog(string caller, Exception ex)
		{
			try
			{
				lock (this)
				{
					File.AppendAllText(m_filename, DateTime.Now + ": " + ex.ToString() + Environment.NewLine);
				}
			}
			catch (Exception)
			{
				// Ignore log file errors
			}
		}

		public static void SysWriteLog(string msg)
		{
			try
			{
				File.AppendAllText(GetStaticFileName(), DateTime.Now + ": " + msg + Environment.NewLine);
			}
			catch (Exception)
			{
				// Ignore log file errors
			}
		}

		public static void SysWriteLog(string caller, string msg)
		{
			try
			{
				File.AppendAllText(GetStaticFileName(), DateTime.Now + " " + caller + ": " + msg + Environment.NewLine);
			}
			catch (Exception)
			{
				// Ignore log file errors
			}
		}

		public static void SysWriteLog(string caller, Exception ex)
		{
			File.AppendAllText(GetStaticFileName(), DateTime.Now + ": " + ex.ToString() + Environment.NewLine);
		}

		private static string GetStaticFileName()
		{
			string filename = ConfigurationManager.AppSettings["LOG_FILE"];
			if (filename == null)
			{
				return "_logfile.txt";
			}
			return filename;
		}
	}
}
