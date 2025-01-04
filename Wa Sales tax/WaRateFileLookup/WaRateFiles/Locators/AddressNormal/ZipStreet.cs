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
using WaRateFiles.Standardizer;

namespace WaRateFiles.Locators.AddressNormal
{
	internal class ZipStreet
	{
		private static ObjectPool<AddressLineTokenizer> m_tokenizerPool = new ObjectPool<AddressLineTokenizer>();

		private int m_zip;
		private bool m_useShortcutEval;
		private Dictionary<string, StreetName> m_streets = new Dictionary<string, StreetName>();

		/// <summary>
		/// If not null, all streets in this ZIP have the same data attributes
		/// </summary>
		private AddressLine m_representitiveAddress = null;

		public ZipStreet(int zip5, bool useShortcutEval)
		{
			m_zip = zip5;
			m_useShortcutEval = useShortcutEval;
		}

		public void Add(AddressLine addr)
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer(addr.Street);
			string street = tokenizer.Street.Lexum;
			if (!m_streets.ContainsKey(street))
			{
				m_streets.Add(street, new StreetName(street, m_useShortcutEval));
			}
			m_streets[street].Add(addr, tokenizer);

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

		public bool Locate(string street, string city, ref AddressLine addr)
		{
			if (m_useShortcutEval && null != m_representitiveAddress)
			{
				addr = new AddressLine(new ZIP(m_zip), m_representitiveAddress);
				return true;
			}

			AddressLineTokenizer tokenizer = m_tokenizerPool.Get();
			tokenizer.Init(street);
			if (null == tokenizer.Street || null == tokenizer.House)
			{
				m_tokenizerPool.Release(tokenizer);
				return false;
			}
			string streetName = tokenizer.Street.Lexum;
			if (m_streets.ContainsKey(streetName))
			{
				StreetName sn = m_streets[streetName];
				bool ret = sn.Locate(tokenizer, city, ref addr);
				m_tokenizerPool.Release(tokenizer);
				return ret;				
			}

			if (tokenizer.Street.LexToken == LexTokenType.ADDRLEX_ORDINAL)
			{
				// don't try spell check on ordinals
				m_tokenizerPool.Release(tokenizer);
				return false;
			}

			if (tokenizer.Street.LexToken == LexTokenType.ADDRLEX_NUM)
			{
				tokenizer.Street.ToOrdinal();
				streetName = tokenizer.Street.Lexum;
				if (m_streets.ContainsKey(streetName))
				{
					StreetName sn = m_streets[streetName];
					bool ret = sn.Locate(tokenizer, city, ref addr);
					m_tokenizerPool.Release(tokenizer);
					return ret;
				}
			}

			//int sim = 0;
			StreetName snLow = null;
			//string tsenc = RefinedSoundex.Encode(tokenizer.Street.Lexum);
			string tsenc = Metaphone.Encode(tokenizer.Street.Lexum);

			foreach (StreetName sn in m_streets.Values)
			{
				//int tsim = RefinedSoundex.DifferenceEncoded(tsenc, RefinedSoundex.Encode(sn.Name));
				//if (tsim > sim)
				//{
				//	sim = tsim;
				//	snLow = sn;
				//}
				string snenc = Metaphone.Encode(sn.Name);
				if (tsenc == snenc)
				{
					snLow = sn;
					break;
				}
			}
			if (null == snLow /*|| tsenc.Length - sim > 2*/)
			{
				m_tokenizerPool.Release(tokenizer);
				return false;
			}
			bool bret = snLow.Locate(tokenizer, city, ref addr);
			m_tokenizerPool.Release(tokenizer);
			return bret;
		}
	}
}
