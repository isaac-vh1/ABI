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

namespace WaRateFiles.Locators.AddressNormal
{
	/// <summary>
	/// StreetName contains information on all streets with a particular name in a FLL
	/// </summary>
	internal class StreetName
	{
		private string m_name;
		private bool m_useShortcutEval;
		private List<StreetComponent> m_streets = new List<StreetComponent>();

		/// <summary>
		/// If not null, all streets in this ZIP/FLL/Name have the same data attributes
		/// </summary>
		private AddressLine m_representitiveAddress = null;

		public string Name
		{
			get { return m_name; }
		}
		
		public AddressLine RepresentitivieAddress
		{
			get { return m_representitiveAddress; }
		}

		public StreetName(string name, bool useShortcutEval)
		{
			m_name = name;
			m_useShortcutEval = useShortcutEval;
		}

		public void Add(AddressLine addr, AddressLineTokenizer tokenizer)
		{
			StreetComponent scmp = new StreetComponent(addr, tokenizer);
			m_streets.Add(scmp);

			if (m_streets.Count == 1)
			{
				m_representitiveAddress = addr;
				return;
			}
			if (null != m_representitiveAddress && !m_representitiveAddress.HasIdenticalAttibutes(addr))
			{
				m_representitiveAddress = null;
			}
		}

		public bool Locate(AddressLineTokenizer tokenizer, string city, ref AddressLine addr)
		{
			if (null != m_representitiveAddress && m_useShortcutEval)
			{
				addr = new AddressLine(new ZIP(m_representitiveAddress.Zip5), m_representitiveAddress);
				return true;
			}

			AddressLine match = null;
			double score = -3;
			double mscore;

			foreach (StreetComponent sc in m_streets)
			{
				//Debug.WriteLine(sc.Address.ToString());
				if ((mscore = sc.Match(tokenizer)) > score)
				{
					match = sc.Address;
					score = mscore;
				}
			}
			if (null == match || score < -0.1)
			{
				return false;
			}
			addr = match;
			return true;
		}
	}
}
