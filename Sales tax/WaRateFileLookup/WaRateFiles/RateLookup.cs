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
using System.Net;
using System.Net.Sockets;
using System.Text;

using WaRateFiles.Support;

namespace WaRateFiles
{
	/// <summary>
	/// Type of lookup performed by RateLookup.FindRate()
	/// </summary>
	public enum LocationSource
	{
		/// <summary>Address not found</summary>
		NONE = 3,
		/// <summary>Address found</summary>
		ADDRESS = 0,
		/// <summary>Address not found, ZIP+4 found</summary>
		ZIP9 = 1,
		/// <summary>Address not found, ZIP5 found</summary>
		ZIP5 = 2,
		INVALID_ARGS = 4,
		INTERNAL_ERROR = 5
	}

	/// <summary>
	/// Type of backend lookup engine for RateLookup to use.
	/// </summary>
	public enum RateLookupEngine
	{
		/// <summary>Each call to FindRate performs a linear file scan.  Not thread safe 
		/// and input addresses must be standardized.</summary>
		SCAN,
		/// <summary>The files are loaded into memory and lookups are performed using
		/// hashtables.  Thread safe, but input address must be standardized.</summary>
		INDEX,
		/// <summary>The files are loaded into memory and the system attempts to 
		/// standardize input addresses.  Thread safe.</summary>
		STANDARDIZER
	}

	/// <summary>
	/// Used to abstract the address lookup backend used by RateLookup.  Either 
	/// FileRateLookup or RemoteRateLookup is used.
	/// </summary>
	internal interface IRateLookup
	{
		void ReLoad(string addrFileName, string rateFileName, string zipFileName);
		Period Period { get; }
		bool HasData { get; }
		bool FindRate(string street, string city, string zip, string plus4, out AddressLine addr, ref Rate rate, out LocationSource loctype);
		bool FindRate(string street, string city, string zip, out AddressLine addr, ref Rate rate, out LocationSource loctype);
		bool FindRate(string street, string city, ZIP zip, out AddressLine addr, ref Rate rate, out LocationSource loctype);
	}
	
	/// <summary>
	/// Used to abstract the ZIP+4 lookup backend used by RateLookup.
	/// </summary>
	internal interface IZipPlus4Lookup
	{
		void ReLoad(string filename);
		bool Locate(ZIP zip, ref string locCode, out bool isZip5Rate);
	}

	/// <summary>
	/// RateLookup is the top-level interface for the rate file lookup system.  It is
	/// used by the desktop client, batch job, and NT service.
	/// </summary>
	public class RateLookup
	{
		private IRateLookup m_lookup;

		/// <summary>
		/// Create an address file lookup
		/// </summary>
		/// <param name="addrFileName">Address file path and name.  Usually State.txt.</param>
		/// <param name="rateFileName">Rate file path and name.</param>
		/// <param name="zipFileName">ZIP+4 rate file path and name</param>
		/// <param name="engine">SCAN, INDEX, or STANDARIZER</param>
		/// <param name="useShortcutEval">
		///		If true, the address lookups will check to see if all of the streets in a ZIP code,
		///		or variations of a steet in a ZIP code, have the same location code.
		/// </param>
		public RateLookup(string addrFileName, string rateFileName, string zipFileName, RateLookupEngine engine, bool useShortcutEval)
		{
			m_lookup = new FileRateLookup(addrFileName, rateFileName, zipFileName, engine, useShortcutEval);
		}

		/// <summary>
		/// This constructor is used when the data is to be loaded after initilization.
		/// </summary>
		/// <param name="engine">SCAN, INDEX, or STANDARIZER</param>
		/// <param name="useShortcutEval">See above.</param>
		public RateLookup(RateLookupEngine engine, bool useShortcutEval)
		{
			m_lookup = new FileRateLookup(engine, useShortcutEval);
		}

		/// <summary>
		/// Create an interface to the NT service running on a remote machine.
		/// </summary>
		/// <param name="protocol">http or https</param>
		/// <param name="server">The remote host name</param>
		/// <param name="port">The remote port number</param>
		public RateLookup(string protocol, string server, int port)
		{
			m_lookup = new RemoteRateLookup(protocol, server, port);
		}

		public void ReLoad(string addrFileName, string rateFileName, string zipFileName)
		{
			m_lookup.ReLoad(addrFileName, rateFileName, zipFileName);
		}

		/// <summary>
		/// Return the tax quarter for the currently loaded data
		/// </summary>
		public Period Period
		{
			get { return m_lookup.Period; }
		}

		/// <summary>
		/// True if data files are loaded.
		/// </summary>
		public bool HadData
		{
			get { return m_lookup.HasData; }
		}

		/// <summary>
		/// Find a tax rate.
		/// </summary>
		/// <param name="street">House number and street.</param>
		/// <param name="city">City name, not really required (except for Everson)</param>
		/// <param name="zip">5-digit zip</param>
		/// <param name="plus4">4-digit ZIP+4</param>
		/// <param name="addr">Address information located.  The attributes can always be used, 
		/// but the address itself will be blank if short-cut-evaluation was enabled and possible</param>
		/// <param name="rate">Rate data found, if any</param>
		/// <param name="loctype">How was the rate located, NONE, ADDRESSS, ZIP9, ZIP5</param>
		/// <returns>Returns false on error or not found.</returns>
		public bool FindRate(string street, string city, string zip, string plus4, out AddressLine addr, ref Rate rate, out LocationSource loctype)
		{
			return m_lookup.FindRate(street, city, ZIP.Parse(zip, plus4), out addr, ref rate, out loctype);
		}

		/// <summary>
		/// Find a tax rate.
		/// </summary>
		/// <param name="street">House number and street.</param>
		/// <param name="city">City name, not really required (except for Everson)</param>
		/// <param name="zip">5 or 9 digit zip</param>
		/// <param name="addr">Address information located.  The attributes can always be used, 
		/// but the address itself will be blank if short-cut-evaluation was enabled and possible</param>
		/// <param name="rate">Rate data found, if any</param>
		/// <param name="loctype">How was the rate located, NONE, ADDRESSS, ZIP9, ZIP5</param>
		/// <returns>Returns false on error or not found.</returns>
		public bool FindRate(string street, string city, string zip, out AddressLine addr, ref Rate rate, out LocationSource loctype)
		{
			if (!ZIP.IsPostalCode(zip))
			{
				addr = null;
				loctype = LocationSource.INVALID_ARGS;
				return false;
			}
			return m_lookup.FindRate(street, city, ZIP.Parse(zip), out addr, ref rate, out loctype);
		}

		/// <summary>
		/// Find a tax rate.
		/// </summary>
		/// <param name="street">House number and street.</param>
		/// <param name="city">City name, not really required (except for Everson)</param>
		/// <param name="zip">5 or 9 digit zip</param>
		/// <param name="addr">Address information located.  The attributes can always be used, 
		/// but the address itself will be blank if short-cut-evaluation was enabled and possible</param>
		/// <param name="rate">Rate data found, if any</param>
		/// <param name="loctype">How was the rate located, NONE, ADDRESSS, ZIP9, ZIP5</param>
		/// <returns>Returns false on error or not found.</returns>
		public bool FindRate(string street, string city, ZIP zip, out AddressLine addr, ref Rate rate, out LocationSource loctype)
		{
			return m_lookup.FindRate(street, city, zip, out addr, ref rate, out loctype);
		}
	}

	/// <summary>
	/// FileRateLookup is used internally by RateLookup to do lookups with local files.
	/// </summary>
	internal class FileRateLookup : IRateLookup
	{
		private bool m_useShortcutEval = false;
		private IAddressLocator m_locator = null;
		private IZipPlus4Lookup m_zipLocator = null;
		private RateIndex m_rates = null;

		public Period Period
		{
			get { return m_locator.Period; }
		}

		public bool HasData
		{
			get { return m_locator.HasData; }
		}

		public FileRateLookup(string addrFileName, string rateFileName, string zipFileName, RateLookupEngine engine, bool useShortcutEval)
		{
			m_useShortcutEval = useShortcutEval;
			if (engine == RateLookupEngine.SCAN)
			{
				m_locator = new AddressScan(addrFileName);
				m_zipLocator = new ZipPlus4Locator(zipFileName);
			}
			else if (engine == RateLookupEngine.INDEX)
			{
				m_locator = new AddressMemoryIndex(addrFileName);
				m_zipLocator = new ZipPlus4MemoryIndex(zipFileName);
			}
			else
			{
				m_locator = new AddressNormalIndex(addrFileName, useShortcutEval);
				m_zipLocator = new ZipPlus4MemoryIndex(zipFileName);
			}
			m_rates = new RateIndex(rateFileName);
		}

		/// <summary>
		/// This constructor is used when the data is to be loaded after initialization.
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="useShortcutEval"></param>
		public FileRateLookup(RateLookupEngine engine, bool useShortcutEval)
		{
			m_useShortcutEval = useShortcutEval;
			if (engine == RateLookupEngine.SCAN)
			{
				m_locator = new AddressScan();
				m_zipLocator = new ZipPlus4Locator();
			}
			else if (engine == RateLookupEngine.INDEX)
			{
				m_locator = new AddressMemoryIndex();
				m_zipLocator = new ZipPlus4MemoryIndex();
			}
			else
			{
				m_locator = new AddressNormalIndex();
				m_zipLocator = new ZipPlus4MemoryIndex();
			}
			m_rates = new RateIndex();
		}

		public void ReLoad(string addrFileName, string rateFileName, string zipFileName)
		{
			m_locator.ReLoad(addrFileName, m_useShortcutEval);
			m_zipLocator.ReLoad(zipFileName);
			m_rates.ReLoad(rateFileName);
		}

		public bool FindRate(string street, string city, string zip, string plus4, out AddressLine addr, ref Rate rate, out LocationSource loctype)
		{
			return FindRate(street, city, ZIP.Parse(zip, plus4), out addr, ref rate, out loctype);
		}

		public bool FindRate(string street, string city, string zip, out AddressLine addr, ref Rate rate, out LocationSource loctype)
		{
			return FindRate(street, city, ZIP.Parse(zip), out addr, ref rate, out loctype);
		}

		public bool FindRate(string street, string city, ZIP zip, out AddressLine addr, ref Rate rate, out LocationSource loctype)
		{
			loctype = LocationSource.NONE;

			// Address lookup
			addr = null;
			if (m_locator.Locate(street, city, zip, ref addr))
			{
				rate = m_rates.FindRate(Int32.Parse(addr.LocationCode));
				if (rate.LocationCodeInt == 3703 && city != null && city.ToLower() == "nooksack")
				{
					// Nooksack is getting coded to everson, for the loc code to nooksack
					rate.LocationCodeInt = 3706;
				}
				else if (rate.LocationCodeInt == 3706 && city != null && city.ToLower() == "everson")
				{
					// the files seem to have the opposite problem as the web site
					rate.LocationCodeInt = 3703;
				}
				loctype = LocationSource.ADDRESS;
				return true;
			}

			// ZIP+4 lookup
			string loccode = String.Empty;
			bool isdefault;
			if (zip.HasPlus4)
			{
				if (m_zipLocator.Locate(zip, ref loccode, out isdefault))
				{
					rate = m_rates.FindRate(Int32.Parse(loccode));
					if (isdefault)
					{
						loctype = LocationSource.ZIP5;
					}
					else
					{
						loctype = LocationSource.ZIP9;
					}
					return true;
				}
			}

			// ZIP5 lookup
			zip = new ZIP(zip.Zip5);
			if (m_zipLocator.Locate(zip, ref loccode, out isdefault))
			{
				rate = m_rates.FindRate(Int32.Parse(loccode));
				loctype = LocationSource.ZIP5;
				return true;
			}
			return false;
		}
	}

	/// <summary>
	/// RemoteRateLookup is used by RateLookup for remote lookups to the NT service.
	/// </summary>
	internal class RemoteRateLookup : IRateLookup
	{
		private string m_urlPrefix;
		private WebClient m_wc = new WebClient();

		public RemoteRateLookup
		(
			string protocol, 
			string server, 
			int port
		)
		{
			Debug.Assert(protocol == "http" || protocol == "https");
			m_urlPrefix = protocol + "://" + server + ":" + port.ToString() + "/";
		}

		public void ReLoad(string addrfn, string ratefn, string zipfn)
		{
		}

		public Period Period
		{
			get { return Period.CurrentPeriod(); }
		}

		public bool HasData
		{
			get { return true; }
		}

		public bool FindRate
		(
			string street, 
			string city, 
			string zip, 
			string plus4, 
			out AddressLine addr, 
			ref Rate rate, 
			out LocationSource loctype
		)
		{
			return FindRate(street, city, ZIP.Parse(zip, plus4), out addr, ref rate, out loctype);
		}

		public bool FindRate
		(
			string street, 
			string city, 
			string zip, 
			out AddressLine addr, 
			ref Rate rate, 
			out LocationSource loctype
		)
		{
			return FindRate(street, city, ZIP.Parse(zip), out addr, ref rate, out loctype);
		}

		public bool FindRate
		(
			string street, 
			string city, 
			ZIP zip, 
			out AddressLine addr, 
			ref Rate rate, 
			out LocationSource loctype
		)
		{
			addr = null;
			string uri = null;

			try
			{
				uri = m_urlPrefix + "xml?addr=" + UrlEncode(street) + "&city=" + UrlEncode(city) + "&zip=" + zip.ToString();
				StreamReader reader = new StreamReader(m_wc.OpenRead(uri));
				string xml = reader.ReadToEnd();
				reader.Close();

				int pos = xml.IndexOf("<response");
				string responseLine = StringHelper.MidStr(xml, pos, xml.IndexOf('>', pos));
				pos = responseLine.IndexOf(" code=") + 7;
				string responseCode = StringHelper.MidStr(responseLine, pos, responseLine.IndexOf('"', pos));
				loctype = (LocationSource)Int32.Parse(responseCode);
				if (responseCode == "-1" || responseCode == "3")
				{
					return false;
				}

				pos = xml.IndexOf("<addressline");
				addr = AddressLine.ParseXml(StringHelper.MidStr(xml, pos, xml.IndexOf(">", pos) + 1));
				pos = xml.IndexOf("<rate");
				rate = Rate.ParseXml(StringHelper.MidStr(xml, pos, xml.IndexOf(">", pos) + 1));
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>Characters that must be URL encoded.</summary>
		private static char[] m_urlchars = new char[] { '&', '%', '#', '?' };

		/// <summary>
		/// For portability and avoid reference to System.web.  May not be 100% complete.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private string UrlEncode(string str)
		{
			if (str.IndexOfAny(m_urlchars) < 0)
			{
				return str;
			}
			StringBuilder buf = new StringBuilder();
			for (int x = 0; x < str.Length; x++)
			{
				switch (str[x])
				{
					case '&':
						buf.Append("%26");
						break;
					case '%':
						buf.Append("%25");
						break;
					case '#':
						buf.Append("%23");
						break;
					case '?':
						buf.Append("%3f");
						break;
					default:
						buf.Append(str[x]);
						break;
				}
			}
			return buf.ToString();
		}
	}
}
