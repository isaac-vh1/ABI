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

using WaRateFiles.Standardizer;

namespace WaRateFiles.Locators.AddressNormal
{
	internal class StreetComponent
	{
		private AddressLine m_addr;
		private AddressLineTokenizer m_tokens;

		public AddressLine Address
		{
			get { return m_addr; }
		}

		public StreetComponent(AddressLine addr, AddressLineTokenizer tokenizer)
		{
			m_addr = addr;
			m_tokens = tokenizer;
		}

		public double Match(AddressLineTokenizer tokenizer)
		{
			double score = 0;

			if (null != tokenizer.House)
			{
				int houseNum = Int32.Parse(tokenizer.House.Lexum);
				if ((houseNum % 2) != 0 && m_addr.IsEven)
				{
					// tie breaker
					score = -0.0001;
				}
				if (houseNum < m_addr.HouseLow || houseNum > m_addr.HouseHigh)
				{
					score -= 0.1;
					//score -= Math.Abs(houseNum / 100.0 - ((m_addr.HouseHigh + m_addr.HouseLow)/2) / 100.0) / 100.0;

					int diff = Math.Abs(houseNum - (m_addr.HouseHigh + m_addr.HouseLow) / 2);
					if (m_addr.HouseLow > 10000)
					{
						score -= diff / 1000000.0;
					}
					else
					{
						score -= diff / 10000.0;
					}
				}
			}

			// Predirectional
			if (m_tokens.PrefixDir == null && tokenizer.PrefixDir == null)
			{
				score += .1;
			}
			else if (m_tokens.PrefixDir != null && tokenizer.PrefixDir != null)
			{
				if (m_tokens.PrefixDir.Lexum == tokenizer.PrefixDir.Lexum)
				{
					score += .25;
				}
				else if (m_tokens.PrefixDir.Lexum.IndexOf(tokenizer.PrefixDir.Lexum) > -1)
				{
					score += .05;
				}
				else
				{
					// User input doesn't match
					score -= .2;
				}
			}
			else if (m_tokens.PrefixDir == null)
			{
				// input address has predir, but not this address
				score -= .3;
			}

			// Road type
			if (m_tokens.StreetType == null && tokenizer.StreetType == null)
			{
				score += .1;
			}
			else if (m_tokens.StreetType != null && tokenizer.StreetType != null)
			{
				if (m_tokens.StreetType.Lexum == tokenizer.StreetType.Lexum)
				{
					score += .2;
				}
				else
				{
					// User input doesn't match
					score -= .05;
				}
			}
			else if (m_tokens.StreetType == null)
			{
				// input address has type, but not this address
				score -= .1;
			}

			// Postdirectional
			if (m_tokens.SuffixDir == null && tokenizer.SuffixDir == null)
			{
				score += .1;
			}
			else if (m_tokens.SuffixDir != null && tokenizer.SuffixDir != null)
			{
				if (m_tokens.SuffixDir.Lexum == tokenizer.SuffixDir.Lexum)
				{
					score += .25;
				}
				else if (m_tokens.SuffixDir.Lexum.IndexOf(tokenizer.SuffixDir.Lexum) > -1)
				{
					score += .05;
				}
				else
				{
					score -= .2;
				}
			}
			else if (m_tokens.SuffixDir == null)
			{
				// input address has sufdir, but not this address
				score -= .1;
			}

			return score;
		}

		public override string ToString()
		{
			return m_addr.ToString();
		}
	}
}
