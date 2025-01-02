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
	/// Loads the rate file into memory and performs hashtable lookups.
	/// </summary>
	public class RateIndex
	{
		private Dictionary<int, Rate> m_rateIdx = new Dictionary<int, Rate>();

		public RateIndex(string rateFileName)
		{
			ReLoad(rateFileName);
		}

		public RateIndex()
		{
		}

		public void ReLoad(string rateFileName)
		{
			if (!File.Exists(rateFileName))
			{
				throw new FileNotFoundException(rateFileName);
			}
			m_rateIdx.Clear();

			StreamReader reader = new StreamReader(rateFileName);
			DelimitedFileReader csv = new DelimitedFileReader(',', 6, reader, null);

			// skip the headerline
			csv.Next();
			Debug.Assert(csv.Column(0).ToString() == "Name" || csv.Column(0).ToString() == "#Name");

			while (csv.Next())
			{
				if (IsEmptyLine(csv))
				{
					continue;
				}
				Rate rate = new Rate(csv);

				m_rateIdx.Add(rate.LocationCodeInt, rate);
			}
		}

		private bool IsEmptyLine(DelimitedFileReader csv)
		{
			for (int x = 0; x < csv.ColumnCount; x++)
			{
				if (csv.Column(x).Length != 0)
				{
					return false;
				}
			}
			return true;
		}

		public Rate FindRate(int locCode)
		{
			Debug.Assert(m_rateIdx.ContainsKey(locCode));
			return m_rateIdx[locCode];
		}
	}
}
