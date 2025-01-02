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

using WaRateFiles.Support;

namespace WaRateFiles
{
	/// <summary>
	/// A local tax line from the rate file.
	/// </summary>
	public class Rate
	{
		private string m_locationName;
		private string m_locationCode;
		private int m_locationCodeInt;
		private decimal m_stateRate;
		private decimal m_localRate;

		public string LocationCode
		{
			get { return m_locationCode; }
		}

		public int LocationCodeInt
		{
			get { return m_locationCodeInt; }
			set
			{
				m_locationCodeInt = value;
				m_locationCode = m_locationCodeInt.ToString("0000");
			}
		}

		public int CountyCode
		{
			get { return m_locationCodeInt / 100; }
		}

		public int LocNum
		{
			get { return m_locationCodeInt - CountyCode; }
		}

		public string LocationName
		{
			get { return m_locationName; }
		}
		
		public int LineCode
		{
			get { return 45; }
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
			get { return m_stateRate + m_localRate; }
		}

		public Rate(string locName, string locCode, decimal stateRate, decimal localRate)
		{
			m_locationName = locName;
			LocationCodeInt = Int32.Parse(locCode);
			m_stateRate = stateRate;
			m_localRate = localRate;
		}

		public Rate(string csv)
		{
			string[] cols = csv.Split();
			m_locationName = cols[0].Trim();
			m_locationCodeInt = Int32.Parse(cols[1]);
			m_locationCode = m_locationCodeInt.ToString("0000");
			m_stateRate = Decimal.Parse(cols[2]);
			m_localRate = Decimal.Parse(cols[3]);
		}

		public Rate(DelimitedFileReader csv)
		{
			m_locationName = csv.Column(0).ToString().Trim();
			m_locationCodeInt = Int32.Parse(csv.Column(1).ToString());
			m_locationCode = m_locationCodeInt.ToString("0000");
			m_stateRate = Decimal.Parse(csv.Column(2).ToString());
			m_localRate = Decimal.Parse(csv.Column(3).ToString());
		}

		public string ToXml()
		{
			return "<rate name=\"" + m_locationName +
				"\" code=\"" + m_locationCode +
				"\" staterate=\"" + m_stateRate.ToString() +
				"\" localrate=\"" + m_localRate.ToString() + "\"/>";
		}

		public static Rate ParseXml(string xml)
		{
			if (xml == "<rate/>")
			{
				return null;
			}
			int pos = xml.IndexOf("name=") + 6;
			string name = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("code=") + 6;
			string code = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("staterate=") + 11;
			string staterate = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));
			pos = xml.IndexOf("localrate=") + 11;
			string localrate = StringHelper.MidStr(xml, pos, xml.IndexOf('"', pos));

			return new Rate(name, code, Decimal.Parse(staterate), Decimal.Parse(localrate));
		}
	}
}
