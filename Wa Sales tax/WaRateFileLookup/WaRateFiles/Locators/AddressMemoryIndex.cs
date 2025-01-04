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
	/// An address file lookup that loads the entire file into memory and uses a hashtable lookup.
	/// </summary>
	public class AddressMemoryIndex : IAddressLocator
	{
		private string m_filename;
		private Dictionary<int, Dictionary<string, AddressLine>> m_zipIdx = new Dictionary<int, Dictionary<string, AddressLine>>();

		public AddressMemoryIndex()
		{
			m_filename = "";
		}

		public AddressMemoryIndex(string addressFileName)
		{
			ReLoad(addressFileName, false);
		}

		public void ReLoad(string addressFileName, bool useShortcutEval)
		{
			if (!File.Exists(addressFileName))
			{
				throw new FileNotFoundException(addressFileName);
			}
			m_filename = addressFileName;

			m_zipIdx.Clear();

			string line = null;
			StreamReader reader = new StreamReader(m_filename);

			// skip the header
			line = reader.ReadLine();
			Debug.Assert(line.StartsWith("ADDR_LOW,ADDR_HIGH"));

			while ((line = reader.ReadLine()) != null)
			{
				AddressLine addrline = new AddressLine(line);

				if (!m_zipIdx.ContainsKey(addrline.Zip5))
				{
					m_zipIdx.Add(addrline.Zip5, new Dictionary<string, AddressLine>());
				}
				Dictionary<string, AddressLine> addrIdx = m_zipIdx[addrline.Zip5];
				string addrkey = addrline.ToAddressKey();
				if (addrIdx.ContainsKey(addrkey))
				{
					continue;
				}
				addrIdx.Add(addrkey, addrline);
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
				throw new Exception("Can't locate period in file");
			}
		}

		public bool HasData
		{
			get { return m_zipIdx.Count > 0; }
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
			if (null == street)
			{
				return false;
			}
			if (!m_zipIdx.ContainsKey(zip.Zip5))
			{
				return false;
			}
			Dictionary<string, AddressLine> addrIdx = m_zipIdx[zip.Zip5];
			
			string shouseNum;
			street = street.ToUpper();
			AddressScan.ExtractHouseNum(out shouseNum, ref street);
			int houseNum = Int32.Parse(shouseNum);

			string addrkey = AddressLine.CreateAddressKey( (houseNum %2) == 0, street, "WA", zip.Zip5 );
			if (!addrIdx.ContainsKey(addrkey))
			{
				return false;
			}
			addr = addrIdx[addrkey];
			return true;
		}
	}
}
