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
using System.Text;

namespace WaRateFiles.Support
{
	/// <summary>
	/// Interface for both US and foriegn postal codes
	/// </summary>
	[Serializable]
	public abstract class IPostalCode
	{
		public abstract string PostalCodeBase { get; }
		public abstract string PostalCodeExtension { get; }
		public abstract bool PostalCodeHasExtension { get; }
		public abstract override string ToString();

		public static IPostalCode Parse(string code)
		{
			if (CanadianPostalCode.IsCaPostalCode(code))
			{
				return CanadianPostalCode.Parse(code);
			}
			return ZIP.Parse(code);
		}

		public static IPostalCode Parse(string code, string extension)
		{
			if ((null == extension || extension.Length == 0) && CanadianPostalCode.IsCaPostalCode(code))
			{
				return CanadianPostalCode.Parse(code, extension);
			}
			return ZIP.Parse(code, extension);
		}

		public static bool IsPostalCode(string code)
		{
			return CanadianPostalCode.IsCaPostalCode(code) || ZIP.IsZip(code);
		}

		public static bool IsPostalCode(string code, string extension)
		{
			return ((null == extension || extension.Length == 0) && CanadianPostalCode.IsCaPostalCode(code))
				|| ZIP.IsZip(code, extension);
		}
	}

	/// <summary>
	/// http://en.wikipedia.org/wiki/Canadian_postal_code
	/// </summary>
	public class CanadianPostalCode : IPostalCode
	{
		private string m_code;

		private CanadianPostalCode(string code)
		{
			m_code = code;
		}

		public override string PostalCodeBase
		{
			get { return m_code; }
		}

		public override string PostalCodeExtension
		{
			get { return ""; }
		}

		public override bool PostalCodeHasExtension
		{
			get { return false; }
		}

		public override string ToString()
		{
			return m_code;
		}

		public static bool IsCaPostalCode(string code)
		{
			int spaceCtn = StringHelper.CountOccurancesOf(code, ' ');
			if (spaceCtn > 1)
			{
				code = code.Trim();
			}
			if (code.Length < 6)
			{
				return false;
			}

			int pos = 0;
			if (!Char.IsLetter(code[pos++]))
			{
				return false;
			}
			if (!Char.IsDigit(code[pos++]))
			{
				return false;
			}
			if (!Char.IsLetter(code[pos++]))
			{
				return false;
			}
			while (pos < code.Length && code[pos] == ' ')
			{
				pos++;
			}
			if (pos + 3 > code.Length)
			{
				return false;
			}
			if (!Char.IsDigit(code[pos++]))
			{
				return false;
			}
			if (!Char.IsLetter(code[pos++]))
			{
				return false;
			}
			if (!Char.IsDigit(code[pos++]))
			{
				return false;
			}
			return true;
		}

		public static new CanadianPostalCode Parse(string code)
		{
			if (!CanadianPostalCode.IsPostalCode(code))
			{
				throw new ArgumentException("Invalid postal code " + code);
			}
			if (code.Length == 7 && code[3] == ' ')
			{
				return new CanadianPostalCode(code);
			}
			code = code.Replace(" ", "");
			if (code.Length != 6)
			{
				throw new ArgumentException("Invalid postal code " + code);
			}
			return new CanadianPostalCode(code.Substring(0, 3) + " " + code.Substring(3));
		}
	}

	/// <summary>
	/// A ZIP + 4
	/// </summary>
	[Serializable]
	public class ZIP : IPostalCode
	{
		private int m_zip;
		private int m_plus4;

		public ZIP(int zip)
		{
			if (zip > 99999)
			{
				m_zip = zip / 10000;
				m_plus4 = zip - m_zip * 10000;
				if (m_zip > 99999)
				{
					throw new ArgumentException("Invalid zip code " + zip);
				}
				if (m_plus4 > 9999)
				{
					throw new ArgumentException("Invalid zip code " + zip);
				}
			}
			else
			{
				m_zip = zip;
				m_plus4 = 0;
			}
		}

		public ZIP(int zip, int plus4)
		{
			m_zip = zip;
			m_plus4 = plus4;
			if (m_zip > 99999)
			{
				throw new ArgumentException("Invalid zip code " + zip);
			}
			if (m_plus4 > 9999)
			{
				throw new ArgumentException("Invalid zip code " + zip);
			}
		}

		public int Zip5
		{
			get { return m_zip; }
		}

		public int Plus4
		{
			get { return m_plus4; }
		}

		public bool HasPlus4
		{
			get { return 0 != m_plus4; }
		}

		public int ToInt()
		{
			return m_zip * 10000 + m_plus4;
		}

		public override string ToString()
		{
			if (0 != m_plus4)
			{
				return m_zip.ToString("00000") + "-" + m_plus4.ToString("0000");
			}
			return m_zip.ToString("00000");
		}

		public string ToShortString()
		{
			if (0 != m_plus4)
			{
				return m_zip.ToString("00000") + m_plus4.ToString("0000");
			}
			return m_zip.ToString("00000");
		}

		public override string PostalCodeBase
		{
			get { return m_zip.ToString("00000"); }
		}

		public override string PostalCodeExtension
		{
			get
			{
				if (HasPlus4)
				{
					return m_plus4.ToString("0000");
				}
				return "";
			}
		}

		public override bool PostalCodeHasExtension
		{
			get { return HasPlus4; }
		}

		public static new ZIP Parse(string zip5, string zipPlus4)
		{
			if (null == zipPlus4 || zipPlus4.Length == 0)
			{
				return Parse(zip5);
			}
			if (zip5.IndexOf(' ') > -1)
			{
				zip5 = zip5.Trim();
			}
			if (zipPlus4.IndexOf(' ') > -1)
			{
				zipPlus4 = zipPlus4.Trim();
			}
			if (zip5.Length > 5 && zipPlus4.Length == 0)
			{
				return Parse(zip5);
			}
			if (zip5.Length > 5 || zipPlus4.Length > 4)
			{
				throw new ArgumentException("Invalid ZIP code " + zip5 + "-" + zipPlus4);
			}
			if (!StringHelper.IsNumeric(zip5) || !StringHelper.IsNumeric(zipPlus4))
			{
				throw new ArgumentException("Invalid ZIP code " + zip5 + "-" + zipPlus4);
			}
			return new ZIP(Int32.Parse(zip5), Int32.Parse(zipPlus4));
		}

		public static new ZIP Parse(string zip)
		{
			int idx;
			if (zip.IndexOf(' ') > -1)
			{
				zip = zip.Replace(" ", "");
			}
			if ((idx = zip.IndexOf('-')) > -1)
			{
				return Parse(zip.Substring(0, idx), zip.Substring(idx + 1));
			}
			else if (zip.Length == 9)
			{
				return Parse(zip.Substring(0, 5), zip.Substring(5, 4));
			}
			if (zip.Length == 0)
			{
				return new ZIP(0);
			}
			if (!StringHelper.IsNumeric(zip))
			{
				throw new ArgumentException("Invalid ZIP code " + zip);
			}
			return new ZIP(Int32.Parse(zip));
		}

		public static bool IsZip(string zip, string plus4)
		{
			if (StringHelper.CountOccurancesOf(zip, '-') > 1)
			{
				return false;
			}
			if (zip.IndexOf('-') > -1)
			{
				zip = zip.Replace("-", "");
			}
			if (null == plus4 || plus4.Length == 0)
			{
				return false;
			}
			if (null == zip)
			{
				return false;
			}
			if (zip.IndexOf(' ') > -1)
			{
				zip = zip.Trim();
			}
			if (plus4.IndexOf(' ') > -1)
			{
				plus4 = plus4.Trim();
			}
			if (!IsZip(zip))
			{
				return false;
			}
			return StringHelper.IsNumeric(zip) && zip.Length < 6 && plus4 != null && plus4.Length < 5 && StringHelper.IsNumeric(plus4);
		}

		private static char[] m_dashArray = new char[] { '-' };

		public static bool IsZip(string zip)
		{
			if (StringHelper.CountOccurancesOf(zip, '-') > 1)
			{
				return false;
			}
			if (zip.IndexOf(' ') > -1)
			{
				zip = zip.Trim();
			}
			int idx;
			if ((idx = zip.IndexOf('-')) > -1)
			{
				return IsZip(zip.Substring(0, idx), zip.Substring(idx + 1));
			}
			int dashpos = zip.IndexOf('-');
			if (dashpos > -1)
			{
				string[] parts = zip.Split(m_dashArray);
				if (parts.Length != 2)
				{
					return false;
				}
				return IsZip(parts[0], parts[1]);
			}
			if (zip.Length > 5)
			{
				return IsZip(zip.Substring(0, 5), zip.Substring(5));
			}
			return zip.Length < 6 && StringHelper.IsNumeric(zip);
		}
	}
}
