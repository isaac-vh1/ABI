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
using System.Diagnostics;
using System.IO;
using System.Text;

using WaRateFiles.Support;

namespace WaRateFiles
{
	/// <summary>
	/// 9-digit ZIP rate lookup using an in-memory hashtable.
	/// </summary>
	public class ZipPlus4MemoryIndex : IZipPlus4Lookup
	{
		private Dictionary<int, List<ZipPlus4Line>> m_idx = new Dictionary<int,List<ZipPlus4Line>>();

		public ZipPlus4MemoryIndex()
		{
		}

		public ZipPlus4MemoryIndex(string filename)
		{
			ReLoad(filename);
		}

		public void ReLoad(string filename)
		{
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException(filename);
			}
			m_idx.Clear();

			Period period = Period.CurrentPeriod();
			string line = null;

			StreamReader reader = new StreamReader(filename);
			while ((line = reader.ReadLine()) != null)
			{
				ZipPlus4Line zpline = new ZipPlus4Line(line);
				if (period.StartDateRevInt > zpline.ExpiresRevInt ||
					period.StartDateRevInt < zpline.EffectiveRevInt)
				{
					continue;
				}

				if (!m_idx.ContainsKey(zpline.ZIP))
				{
					m_idx.Add(zpline.ZIP, new List<ZipPlus4Line>());
				}
				List<ZipPlus4Line> lst = m_idx[zpline.ZIP];
				lst.Add(zpline);
			}
			reader.Close();
		}

		public bool Locate(ZIP zip, ref string locCode, out bool isZip5Rate)
		{
			int lowLocCode = 9999;
			decimal lowRate = 1;
			isZip5Rate = false;
			
			if ( !m_idx.ContainsKey(zip.Zip5) )
			{
				return false;
			}
			List<ZipPlus4Line> lst = m_idx[zip.Zip5];
			foreach ( ZipPlus4Line zpline in lst )
			{
				if (zip.HasPlus4 && zip.Plus4 >= zpline.Plus4Low && zip.Plus4 <= zpline.Plus4High)
				{
					// found matching record
					locCode = zpline.LocationCode;
					return true;
				}
				
				// update the ZIP5 rate
				if (zpline.TotalRate < lowRate)
				{
					lowLocCode = zpline.LocationCodeInt;
					lowRate = zpline.TotalRate;
				}
				else if (zpline.TotalRate == lowRate && lowLocCode < zpline.LocationCodeInt)
				{
					lowLocCode = zpline.LocationCodeInt;
					lowRate = zpline.TotalRate;
				}
			}
			
			if (lowLocCode < 9999)
			{
				// return the rate for the ZIP5
				locCode = lowLocCode.ToString("0000");
				isZip5Rate = true;
				return true;
			}
			isZip5Rate = false;
			return false;
		}
	}
}
