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
	/// A line in the ZIP+4 file.
	/// </summary>
	internal class ZipPlus4Line
	{
		private int m_zip;
		private int m_p4Low;
		private int m_p4High;
		private string m_locCode;
		private int m_locCodeInt;
		private decimal m_stateRate;
		private decimal m_localRate;
		private int m_effective;
		private int m_expires;

		private static char[] m_commaArray = new char[] { ',' };

		public int ZIP
		{
			get { return m_zip; }
		}

		public int Plus4Low
		{
			get { return m_p4Low; }
		}

		public int Plus4High
		{
			get { return m_p4High; }
		}

		public string LocationCode
		{
			get { return m_locCode; }
		}

		public int LocationCodeInt
		{
			get { return m_locCodeInt; }
		}

		public decimal StateRate
		{
			get { return m_stateRate; }
		}

		public decimal LocalRate
		{
			get { return m_localRate; }
		}

		public decimal TotalRate
		{
			get { return m_localRate + m_stateRate; }
		}

		public int EffectiveRevInt
		{
			get { return m_effective; }
		}

		public int ExpiresRevInt
		{
			get { return m_expires; }
		}

		internal ZipPlus4Line(string csv)
		{
			string[] cols = csv.Split(m_commaArray);
			m_zip = Int32.Parse(cols[0]);
			m_p4Low = Int32.Parse(cols[1]);
			m_p4High = Int32.Parse(cols[2]);
			m_locCode = cols[3];
			m_locCodeInt = Int32.Parse(m_locCode);
			m_stateRate = Decimal.Parse(cols[4]);
			m_localRate = Decimal.Parse(cols[5]);
			m_effective = Int32.Parse(cols[7]);
			m_expires = Int32.Parse(cols[8]);
		}
	}

	/// <summary>
	/// File scan lookup for 9-digit ZIP codes.
	/// </summary>
	public class ZipPlus4Locator : IZipPlus4Lookup
	{
		private string m_filename;

		public ZipPlus4Locator()
		{
			m_filename = "";
		}

		public ZipPlus4Locator(string filename)
		{
			ReLoad(filename);
		}

		public void ReLoad(string filename)
		{
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException(filename);
			}
			m_filename = filename;
		}

		public bool Locate(ZIP zip, ref string locCode, out bool isZip5Rate)
		{
			int lowLocCode = 9999;
			decimal lowRate = 1;
			Period period = Period.CurrentPeriod();

			string linefrag = zip.Zip5.ToString("00000") + ",";
			string line = null;
			StreamReader reader = new StreamReader(m_filename);
			while ((line = reader.ReadLine()) != null)
			{
				if (line.StartsWith(linefrag))
				{
					// ZIP code located
					ZipPlus4Line zpline = new ZipPlus4Line(line);
					if ( period.StartDateRevInt > zpline.ExpiresRevInt ||
					    period.StartDateRevInt < zpline.EffectiveRevInt )
					{
						continue;
					}

					if (zip.HasPlus4 && zip.Plus4 >= zpline.Plus4Low && zip.Plus4 <= zpline.Plus4High)
					{
						isZip5Rate = false;
						locCode = zpline.LocationCode;
						reader.Close();
						return true;
					}
					// update default rate
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
			}
			reader.Close();

			if (lowLocCode < 9999)
			{
				locCode = lowLocCode.ToString("0000");
				isZip5Rate = true;
				return true;
			}
			isZip5Rate = false;
			return false;
		}
	}
}
