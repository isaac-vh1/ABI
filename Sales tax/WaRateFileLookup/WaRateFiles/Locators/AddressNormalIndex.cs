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
using WaRateFiles.Locators.AddressNormal;

namespace WaRateFiles
{
	/// <summary>
	/// An address lookup that loads all data into memory. It can handle missing or 
	/// incorrect postal components and some amount of misspelling.  For real user
	/// input, the match rate is about 80%.
	/// </summary>
	public class AddressNormalIndex : IAddressLocator
	{
		private string m_filename;
		private Dictionary<int, ZipStreet> m_idx = new Dictionary<int, ZipStreet>();

		public AddressNormalIndex()
		{
			m_filename = "";
		}

		public AddressNormalIndex(string addressFileName, bool useShortcutEval)
		{
			ReLoad(addressFileName, useShortcutEval);
		}

		public void ReLoad(string addressFileName, bool useShortcutEval)
		{
			if (!File.Exists(addressFileName))
			{
				throw new FileNotFoundException(addressFileName);
			}
			m_filename = addressFileName;
			m_idx.Clear();

			Period period = Period.CurrentPeriod();
			StreamReader reader = new StreamReader(m_filename);
			DelimitedFileReader csv = new DelimitedFileReader(',', 12, reader, null);

			// skip the header
			csv.Next();

			while (csv.Next())
			{
				if (!csv.RowHadData)
				{
					continue;
				}
				AddressLine addrline = new AddressLine(csv);
				if (addrline.Period != period.PeriodLit)
				{
					continue;
				}

				if (!m_idx.ContainsKey(addrline.Zip5))
				{
					m_idx.Add(addrline.Zip5, new ZipStreet(addrline.Zip5, useShortcutEval));
				}
				ZipStreet zstr = m_idx[addrline.Zip5];
				zstr.Add(addrline);
			}
			if (m_idx.Count == 0)
			{
				throw new Exception("No address records found for period " + period.PeriodLit);
			}
		}

		public Period Period
		{
			get
			{
				AddressLine addr = null;

				if (Locate("700 KNIGHT HILL RD", "", "98953", ref addr))
				{
					return Period.Parse(addr.Period);
				}
				if (Locate("6500 LINDERSON WAY SW", "TUMWATER", "98501", ref addr))
				{
					return Period.Parse(addr.Period);
				}
				throw new Exception("Can't find period in file");
			}
		}

		public bool HasData
		{
			get { return m_idx.Count > 0; }
		}

		public bool Locate(string street, string city, string zip, string plus4, ref AddressLine addr)
		{
			return Locate(street, city, zip + "-" + plus4, ref addr);
		}

		public bool Locate(string street, string city, string szip, ref AddressLine addr)
		{
			ZIP zip = ZIP.Parse(szip);
			return Locate(street, city, zip, ref addr);
		}

		public bool Locate(string street, string city, ZIP zip, ref AddressLine addr)
		{
			if (!m_idx.ContainsKey(zip.Zip5))
			{
				return false;
			}
			ZipStreet zsf = m_idx[zip.Zip5];
			if (zsf.Locate(street, city, ref addr))
			{
				return true;
			}
			
			return false;
		}
	}
}
