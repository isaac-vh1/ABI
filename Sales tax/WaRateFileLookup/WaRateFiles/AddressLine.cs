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
using System.Text;

using WaRateFiles.Support;
using WaRateFiles.Standardizer;

namespace WaRateFiles
{
	/// <summary>
	/// A line from the address file.  Sometimes only the attribute data is 
	/// returned from RateLookup, but the can be overridden by setting
	/// USE_SHORTCUT_ADDRESS_EVALUATION to false in the app.config.
	/// </summary>
	public class AddressLine
	{
		private int m_houseLow;
		private int m_houseHigh;
		private bool m_isEven;
		private string m_street;
		private string m_state;
		private int m_zip;
		private int m_zipP4;
		private string m_period;
		private string m_code;
		private bool m_isRta;
		private string m_ptba;
		private string m_cez;

		private static char[] m_commaArray = new char[] { ',' };

		public int HouseLow
		{
			get { return m_houseLow; }
		}

		public int HouseHigh
		{
			get { return m_houseHigh; }
		}

		public bool IsEven
		{
			get { return m_isEven; }
		}

		public string Street
		{
			get { return m_street; }
		}

		public int Zip5
		{
			get { return m_zip; }
		}

		public int ZipPlus4
		{
			get { return m_zipP4; }
		}

		public string ZipLit
		{
			get 
			{
				if (m_zipP4 != 0)
				{
					return m_zip.ToString("00000") + "-" + m_zipP4.ToString("0000");
				}
				return m_zip.ToString("00000");
			}
		}

		public string Period
		{
			get { return m_period; }
		}

		public string LocationCode
		{
			get { return m_code; }
		}
		
		public bool IsRta
		{
			get { return m_isRta; }
		}
		
		public string Ptba
		{
			get { return m_ptba; }
		}
		
		public string Cez
		{
			get { return m_cez; }
		}

		public AddressLine(string csv)
		{
			string[] cols = csv.Split(m_commaArray);
			m_houseLow = Int32.Parse(cols[0]);
			m_houseHigh = Int32.Parse(cols[1]);
			m_isEven = cols[2] == "E";
			m_street = cols[3];
			m_state = cols[4];
			m_zip = Int32.Parse(cols[5]);
			m_zipP4 = Int32.Parse(cols[6]);
			m_period = cols[7];
			m_code = cols[8];
			m_isRta = cols[9] == "Y";
			m_ptba = cols[10];
			m_cez = cols[11];
		}

		public AddressLine(DelimitedFileReader csv)
		{
			m_houseLow = Int32.Parse(csv.Column(0).ToString());
			m_houseHigh = Int32.Parse(csv.Column(1).ToString());
			m_isEven = StringHelper.AreEqual(csv.Column(2), "E");
			m_street = csv.Column(3).ToString();
			m_state = AddressLineTokenizer.StringTable.Get(csv.Column(4));
			m_zip = Int32.Parse(AddressLineTokenizer.StringTable.Get(csv.Column(5)));
			m_zipP4 = Int32.Parse(AddressLineTokenizer.StringTable.Get(csv.Column(6)));
			m_period = AddressLineTokenizer.StringTable.Get(csv.Column(7));
			m_code = AddressLineTokenizer.StringTable.Get(csv.Column(8));
			m_isRta = StringHelper.AreEqual(csv.Column(9), "Y");
			m_ptba = AddressLineTokenizer.StringTable.Get(csv.Column(10));
			m_cez = AddressLineTokenizer.StringTable.Get(csv.Column(11));
		}

		public AddressLine(ZIP zip, AddressLine addr)
		{
			m_street = "";
			m_zip = zip.Zip5;
			m_zipP4 = zip.Plus4;
			m_period = addr.m_period;
			m_code = addr.m_code;
			m_isRta = addr.m_isRta;
			m_ptba = addr.m_ptba;
			m_cez = addr.m_cez;
			m_state = addr.m_state;
		}

		public AddressLine(int houselow, int househigh, bool iseven, string street, string state, int zip, int plus4, string period,
			string code, bool isrta, string ptba, string cez)
		{
			m_houseHigh = househigh;
			m_houseLow = houselow;
			m_street = street.Trim();
			m_zip = zip;
			m_zipP4 = plus4;
			m_period = period;
			m_code = code;
			m_isRta = isrta;
			m_ptba = ptba;
			m_cez = cez;
			m_state = state.Trim();
		}

		public bool HasIdenticalAttibutes(AddressLine addr)
		{
			return addr.LocationCode == LocationCode &&
				addr.Cez == Cez;
		}

		public string ToAddressKey()
		{
			return "," +
					((m_isEven) ? "E" : "O") + "," +
					m_street + "," +
					m_state + "," +
					m_zip.ToString("00000") + ",";
		}

		public override string ToString()
		{
			return m_houseLow + "," +
				m_houseHigh + "," +
				((m_isEven) ? "E" : "O") + "," +
				m_street + "," +
				m_state + "," +
				m_zip.ToString("00000") + "," +
				m_zipP4.ToString("0000") + "," +
				m_period + "," +
				m_code + "," +
				(m_isRta ? "Y" : "N") + "," +
				m_ptba + "," +
				m_cez;
		}

		public string ToXml()
		{
			return "<addressline houselow=\"" + m_houseLow + "\" househigh=\"" +
				m_houseHigh + "\" evenodd=\"" +
				((m_isEven) ? "E" : "O") + "\" street=\"" +
				m_street + "\" state=\"" +
				m_state + "\" zip=\"" +
				m_zip.ToString("00000") + "\" plus4=\"" +
				m_zipP4.ToString("0000") + "\" period=\"" +
				m_period + "\" code=\"" +
				m_code + "\" rta=\"" +
				(m_isRta ? "Y" : "N") + "\" ptba=\"" +
				m_ptba + "\" cez=\"" +
				m_cez + "\"/>";
		}

		public static AddressLine ParseXml(string xml)
		{
			if (xml == "<addressline/>")
			{
				return null;
			}
			int pos = xml.IndexOf("houselow=") + 10;
			string houselow = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("househigh=") + 11;
			string househigh = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("evenodd=") + 9;
			string evenodd = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("street=") + 8;
			string street = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("state=") + 7;
			string state = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("zip=") + 5;
			string zip = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("plus4=") + 7;
			string plus4 = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("period=") + 8;
			string period = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("code=") + 6;
			string code = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("rta=") + 5;
			string rta = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("ptba=") + 6;
			string ptba = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("cez=") + 5;
			string cez = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));

			return new AddressLine(Int32.Parse(houselow), Int32.Parse(househigh), evenodd == "E", street, state, Int32.Parse(zip), Int32.Parse(plus4), period, code, rta == "Y", ptba, cez);
		}

		public static string CreateAddressKey(bool isEven, string street, string state, int zip5)
		{
			return "," + ((isEven) ? "E" : "O") + "," + street + "," + state + "," + zip5.ToString("00000") + ",";
		}

		public static void CreateAddressKey(StringBuilder sb, bool isEven, string street, string state, int zip5)
		{
			sb.Length = 0;
			sb.Append(",");
			sb.Append((isEven) ? "E" : "O");
			sb.Append(",");
			sb.Append(street);
			sb.Append(",");
			sb.Append(state);
			sb.Append(",");
			sb.Append(zip5.ToString("00000"));
			sb.Append(",");
		}
	}
}
