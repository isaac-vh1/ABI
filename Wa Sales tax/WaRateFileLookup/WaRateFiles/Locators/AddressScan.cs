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
	/// An address lookup that opens and scans the file for each call.
	/// </summary>
	public class AddressScan : IAddressLocator
	{
		private string m_filename;

		public AddressScan()
		{
			m_filename = "";
		}

		public AddressScan(string addressFileName)
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
			get { return File.Exists(m_filename); }
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

			string shouseNum;
			string linefragment = street.ToUpper();
			ExtractHouseNum(out shouseNum, ref linefragment);
			linefragment = "," + linefragment + ",WA," + zip.Zip5.ToString("00000") + ",";
			int houseNum = Int32.Parse(shouseNum);

			string line = null;
			StreamReader reader = new StreamReader(m_filename);
			while ((line = reader.ReadLine()) != null)
			{
				if (line.IndexOf(linefragment) > -1)
				{
					int pos = line.IndexOf(',');
					string shouseLow = line.Substring(0, pos);
					pos = line.IndexOf(',', pos + 1);
					string shouseHi = line.Substring(shouseLow.Length + 1, (pos - shouseLow.Length) - 1);

					string type = line.Substring(pos + 1, 1);
					Debug.Assert(type == "E" || type == "O");
					if ((houseNum % 2) == 0 && type == "O")
					{
						continue;
					}
					else if ((houseNum % 2) != 0 && type == "E")
					{
						continue;
					}
					if (houseNum < Int32.Parse(shouseLow) || houseNum > Int32.Parse(shouseHi))
					{
						continue;
					}

					// match
					reader.Close();

					addr = new AddressLine(line);
					return true;
				}
			}
			reader.Close();
			return false;
		}

		public static void ExtractHouseNum(out string houseNum, ref string addr)
		{
			string[] parts = addr.Split(new char[] { ' ' });
			int housepos = -1;
			for (int x = 0; x < parts.Length; x++)
			{
				if (StringHelper.IsInt(parts[x]))
				{
					housepos = x;
					break;
				}
			}
			if (-1 == housepos)
			{
				houseNum = "0";
				return;
			}
			houseNum = parts[housepos];
			addr = "";
			for (int x = 0; x < parts.Length; x++)
			{
				if (x != housepos)
				{
					if (addr.Length > 0)
					{
						addr += " ";
					}
					addr += parts[x];
				}
			}
		}
	}
}
