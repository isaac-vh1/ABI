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

using NUnit.Framework;
using WaRateFiles.Standardizer;

namespace WaRateFiles.UnitTest
{
	[TestFixture]
	public class TokenizerTest
	{
		public TokenizerTest()
		{
		}

		[Test]
		public void OneWord()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);
			Assert.AreEqual(tokenizer.Street.Lexum, "123");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
		}

		[Test]
		public void NumberStreetNameBasic()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 MAIN");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "MAIN");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
		}

		[Test]
		public void NumberStreetNameTypeBasic()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 MAIN ST");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "MAIN");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
		}

		[Test]
		public void NumberStreetNameTypeSufdirBasic()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 MAIN ST SW");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "MAIN");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.SuffixDir.ResultToken, StreetToken.SUFDIR);

			tokenizer = new AddressLineTokenizer("123 MAIN ST S W");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "MAIN");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.SuffixDir.ResultToken, StreetToken.SUFDIR);
		}

		[Test]
		public void NumberStreetNameTypePredirBasic()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("SW 123 MAIN ST");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "MAIN");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);

			tokenizer = new AddressLineTokenizer("S W 123 MAIN ST");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "MAIN");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
		}

		[Test]
		public void Junk1()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123YABA MAIN ST");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.Street.Lexum, "MAIN");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);

			tokenizer = new AddressLineTokenizer("123Y&ABA - 1/3 $100 #104");

			tokenizer = new AddressLineTokenizer("NO. 08");
		}

		[Test]
		public void NumberStreetNameBasicOrd()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 1ST");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
		}

		[Test]
		public void NumberStreetNameTypeBasicOrd()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 1ST ST");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
		}

		[Test]
		public void NumberStreetNameTypeSufdirBasicOrd()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 1ST ST SW");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.SuffixDir.ResultToken, StreetToken.SUFDIR);

			tokenizer = new AddressLineTokenizer("123 1ST ST S W");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.SuffixDir.ResultToken, StreetToken.SUFDIR);
		}

		[Test]
		public void NumberStreetNameTypePredirBasicOrd()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("SW 123 1ST ST");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);

			tokenizer = new AddressLineTokenizer("S W 123 1ST ST");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
		}

		[Test]
		public void NumberStreetNameTypePredirBasicOrd2()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 SW 1ST ST");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);

			tokenizer = new AddressLineTokenizer("123 S W 1ST ST");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
		}

		[Test]
		public void NumberStreetPref()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 MAIN HILL RD");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNotNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.StreetPrefix.Lexum, "HILL");
			Assert.AreEqual(tokenizer.StreetPrefix.ResultToken, StreetToken.STREETPRE);
			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "MAIN");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);

			tokenizer = new AddressLineTokenizer("123 MAIN HILL RD SW");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNotNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.SuffixDir.ResultToken, StreetToken.SUFDIR);
			Assert.AreEqual(tokenizer.StreetPrefix.Lexum, "HILL");
			Assert.AreEqual(tokenizer.StreetPrefix.ResultToken, StreetToken.STREETPRE);
			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "MAIN");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
		}
		
		[Test]
		public void NumberQualPreStreet()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 OLD HIGHWAY 99");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNotNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.StreetQualifier.ResultToken, StreetToken.STREETQUALIF);
			Assert.AreEqual(tokenizer.StreetPrefix.Lexum, "HIGHWAY");
			Assert.AreEqual(tokenizer.StreetPrefix.ResultToken, StreetToken.STREETPRE);
			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "99");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
		}

		[Test]
		public void NumPredirRtStSufdir()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("616 S Road 40 E");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "616");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "S");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
			Assert.AreEqual(tokenizer.Street.Lexum, "40");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "E");
			Assert.AreEqual(tokenizer.SuffixDir.ResultToken, StreetToken.SUFDIR);
		}

		[Test]
		public void S_340TH_E()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("117 S 340TH E");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "117");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "S");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.Street.Lexum, "340TH");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "E");
			Assert.AreEqual(tokenizer.SuffixDir.ResultToken, StreetToken.SUFDIR);
		}

		[Test]
		public void EZ_ST()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 EZ ST");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "EZ");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
		}

		[Test]
		public void MY_DR_N()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("123 MY DR N");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "123");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "MY");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "DR");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "N");
			Assert.AreEqual(tokenizer.SuffixDir.ResultToken, StreetToken.SUFDIR);
		}

		[Test]
		public void SE_E_ST()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 SE E ST");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "SE");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "E");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.StreetType.ResultToken, StreetToken.STREETTYPE);
		}

		[Test]
		public void S_1ST()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 S 1ST");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "S");
			Assert.AreEqual(tokenizer.PrefixDir.ResultToken, StreetToken.PREDIR);
			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.House.ResultToken, StreetToken.HOUSE);
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.Street.ResultToken, StreetToken.STREET);
		}

		[Test]
		public void _82ND_AVE_W()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 82ND AVE W");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "82ND");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "AVE");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "W");
		}

		[Test]
		public void W_NORTH_RIVER_DR()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 W NORTH RIVER DR");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "W");
			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "NORTH RIVER");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "DR");
		}

		[Test]
		public void GRAVELLY_BEACH_LOOP_NW()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 GRAVELLY BEACH LOOP NW");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNotNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "GRAVELLY");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "LOOP");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "NW");
		}

		[Test]
		public void E_MILL_PLAIN_BLVD()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 E MILL PLAIN BLVD");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "E");
			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "MILL PLAIN");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "BLVD");
		}

		[Test]
		public void HWY_112()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 HWY 112");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "112");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "HWY");

			tokenizer = new AddressLineTokenizer("HWY 112");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.Street.Lexum, "112");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "HWY");
		}

		[Test]
		public void POINT_LAWRENCE_RD()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 POINT LAWRENCE RD");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "POINT LAWRENCE");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
		}

		[Test]
		public void MERIDIAN_E()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 MERIDIAN E");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "MERIDIAN");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "E");
		}

		[Test]
		public void CAMELOT_PARK_SW()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 CAMELOT PARK SW");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "CAMELOT");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "PARK");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
		}

		[Test]
		public void ST_158TH_KP_N()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 ST 158TH KP N");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "158TH");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "N");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "KP");
		}

		[Test]
		public void OLD_HIGHWAY_99_SW()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 OLD HIGHWAY 99 SW");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNotNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "99");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.StreetPrefix.Lexum, "HIGHWAY");

			tokenizer = new AddressLineTokenizer("OLD HIGHWAY 99 SW");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNotNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.Street.Lexum, "99");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.StreetPrefix.Lexum, "HIGHWAY");
		}

		[Test]
		public void OLD_HIGHWAY_99_NORTH_RD()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 OLD HIGHWAY 99 NORTH RD");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNotNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "99");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "N");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.StreetPrefix.Lexum, "HIGHWAY");

			tokenizer = new AddressLineTokenizer("OLD HIGHWAY 99 NORTH RD");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNotNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.Street.Lexum, "99");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "N");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.StreetPrefix.Lexum, "HIGHWAY");
		}

		[Test]
		public void FLYING_L_LN()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 FLYING L LN");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "FLYING L");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "LN");
		}

		[Test]
		public void LA_PUSH_RD()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 LA PUSH RD");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.Street.Lexum, "LA PUSH");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
		}

		[Test]
		public void W_CORD_12()
		{
			//AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 W CORD 12");
			//Assert.IsNotNull(tokenizer.PrefixDir);
			//Assert.IsNotNull(tokenizer.House);
			//Assert.IsNull(tokenizer.StreetQualifier);
			//Assert.IsNull(tokenizer.StreetPrefix);
			//Assert.IsNotNull(tokenizer.Street);
			//Assert.IsNull(tokenizer.StreetType);
			//Assert.IsNull(tokenizer.SuffixDir);

			//Assert.AreEqual(tokenizer.PrefixDir.Lexum, "W");
			//Assert.AreEqual(tokenizer.House.Lexum, "100");
			//Assert.AreEqual(tokenizer.Street.Lexum, "CORD 12");
		}

		[Test]
		public void OLD_99_N()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 OLD 99 N");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "N");
			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.Street.Lexum, "99");

			tokenizer = new AddressLineTokenizer("OLD 99 N");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.Street.Lexum, "99");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "N");
		}

		[Test]
		public void _79TH_ST_NE()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 79TH ST NE");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "ST");
			Assert.AreEqual(tokenizer.Street.Lexum, "79TH");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "NE");
		}

		[Test]
		public void SE_TJ_LN()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 SE TJ LN");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "LN");
			Assert.AreEqual(tokenizer.Street.Lexum, "TJ");
			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "SE");

			tokenizer = new AddressLineTokenizer("SE TJ LN");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.StreetType.Lexum, "LN");
			Assert.AreEqual(tokenizer.Street.Lexum, "TJ");
			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "SE");
		}

		[Test]
		public void RD_I6_NE()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 RD I6 NE");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.Street.Lexum, "I6");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "NE");

			tokenizer = new AddressLineTokenizer("RD I6 NE");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.Street.Lexum, "I6");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "NE");
		}

		[Test]
		public void R_W_JOHNSON_RD_SW()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 R W JOHNSON RD SW");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.Street.Lexum, "R W JOHNSON");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
		}

		[Test]
		public void MARK_E_REED_WAY()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("100 MARK E REED WAY");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "100");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "WAY");
			Assert.AreEqual(tokenizer.Street.Lexum, "MARK E REED");
		}

		[Test]
		public void NE_Fourth_Plain()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("13701 NE Fourth Plain");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "13701");
			Assert.AreEqual(tokenizer.Street.Lexum, "4TH PLAIN");
			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "NE");
		}

		[Test]
		public void E_4TH_PLAIN_BLVD()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("501 E 4TH PLAIN BLVD");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "501");
			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "E");
			Assert.AreEqual(tokenizer.Street.Lexum, "4TH PLAIN");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "BLVD");

			tokenizer = new AddressLineTokenizer("E 4TH PLAIN BLVD");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "E");
			Assert.AreEqual(tokenizer.Street.Lexum, "4TH PLAIN");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "BLVD");
		}

		[Test]
		public void Old_Owen_Rd()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("26024 Old Owen Rd");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "26024");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.Street.Lexum, "OWEN");
		}

		[Test]
		public void _1st_ave_south()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("15025 1st ave south");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "15025");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "AVE");
			Assert.AreEqual(tokenizer.Street.Lexum, "1ST");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "S");
		}

		[Test]
		public void _934_31_Ave()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("934 31 Ave");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "934");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "AVE");
			Assert.AreEqual(tokenizer.Street.Lexum, "31");
		}

		[Test]
		public void _3380_146th_Place_SE_Suite_320()
		{
			//AddressLineTokenizer tokenizer = new AddressLineTokenizer("3380-146th Place SE Suite 320");
			//Assert.IsNull(tokenizer.PrefixDir);
			//Assert.IsNotNull(tokenizer.House);
			//Assert.IsNull(tokenizer.StreetQualifier);
			//Assert.IsNull(tokenizer.StreetPrefix);
			//Assert.IsNotNull(tokenizer.Street);
			//Assert.IsNotNull(tokenizer.StreetType);
			//Assert.IsNotNull(tokenizer.SuffixDir);

			//Assert.AreEqual(tokenizer.House.Lexum, "3380");
			//Assert.AreEqual(tokenizer.StreetType.Lexum, "PL");
			//Assert.AreEqual(tokenizer.Street.Lexum, "146TH");
			//Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SE");
		}

		[Test]
		public void _15001_35TH_AVE_W_PCT23()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("15001 35TH AVE W %23 20-201");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "15001");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "AVE");
			Assert.AreEqual(tokenizer.Street.Lexum, "35TH");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "W");
		}

		[Test]
		public void _515_W_ST_THOMAS_MORE_WAY()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("515 W ST THOMAS MORE WAY");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "W");
			Assert.AreEqual(tokenizer.House.Lexum, "515");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "WAY");
			Assert.AreEqual(tokenizer.Street.Lexum, "SAINT THOMAS MORE");
		}

		[Test]
		public void _4418_Ne_St_James_Rd()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("4418 Ne St James Rd");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "NE");
			Assert.AreEqual(tokenizer.House.Lexum, "4418");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.Street.Lexum, "SAINT JAMES");
		}

		[Test]
		public void _34913_10th_place()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("34913 10th place");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "34913");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "PL");
			Assert.AreEqual(tokenizer.Street.Lexum, "10TH");
		}

		[Test]
		public void _28502_STATE_HIGHWAY_3_NE()
		{
			//AddressLineTokenizer tokenizer = new AddressLineTokenizer("28502 STATE HIGHWAY 3 NE");
			//Assert.IsNull(tokenizer.PrefixDir);
			//Assert.IsNotNull(tokenizer.House);
			//Assert.IsNull(tokenizer.StreetQualifier);
			//Assert.IsNull(tokenizer.StreetPrefix);
			//Assert.IsNotNull(tokenizer.Street);
			//Assert.IsNull(tokenizer.StreetType);
			//Assert.IsNotNull(tokenizer.SuffixDir);

			//Assert.AreEqual(tokenizer.House.Lexum, "28502");
			//Assert.AreEqual(tokenizer.Street.Lexum, "STHY 3");
			//Assert.AreEqual(tokenizer.SuffixDir.Lexum, "NE");
		}

		[Test]
		public void _8115_NE_St_Johns()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("8115 NE St. Johns");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.PrefixDir.Lexum, "NE");
			Assert.AreEqual(tokenizer.House.Lexum, "8115");
			Assert.AreEqual(tokenizer.Street.Lexum, "SAINT JOHNS");
		}

		[Test]
		public void _LA_VISTA_DR_SW()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("LA VISTA DR SW");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNotNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.Street.Lexum, "LA VISTA");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "DR");
			Assert.AreEqual(tokenizer.SuffixDir.Lexum, "SW");
		}

		[Test]
		public void OLD_STATE_9_RD()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("OLD STATE 9 RD");
			Assert.IsNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNotNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.Street.Lexum, "9");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "RD");
			Assert.AreEqual(tokenizer.StreetPrefix.Lexum, "STATE");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
		}

		[Test]
		public void W_OLD_INLAND_EMPIRE_HWY()
		{
			AddressLineTokenizer tokenizer = new AddressLineTokenizer("130511 W. OLD INLAND EMPIRE HWY");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNotNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.House.Lexum, "130511");
			Assert.AreEqual(tokenizer.StreetType.Lexum, "HWY");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.Street.Lexum, "INLAND EMPIRE");

			tokenizer = new AddressLineTokenizer("W OLD INLAND EMPIRE HWY");
			Assert.IsNotNull(tokenizer.PrefixDir);
			Assert.IsNull(tokenizer.House);
			Assert.IsNotNull(tokenizer.StreetQualifier);
			Assert.IsNull(tokenizer.StreetPrefix);
			Assert.IsNotNull(tokenizer.Street);
			Assert.IsNotNull(tokenizer.StreetType);
			Assert.IsNull(tokenizer.SuffixDir);

			Assert.AreEqual(tokenizer.StreetType.Lexum, "HWY");
			Assert.AreEqual(tokenizer.StreetQualifier.Lexum, "OLD");
			Assert.AreEqual(tokenizer.Street.Lexum, "INLAND EMPIRE");
		}

		/// <summary>
		/// Helper for source level debugging of the unit tests
		/// </summary>
		public void _DebugTraceEntryPoint()
		{
			OneWord();
			NumberStreetNameBasic();
			NumberStreetNameTypeBasic();
			NumberStreetNameTypeSufdirBasic();
			NumberStreetNameTypePredirBasic();
			Junk1();
			NumberStreetNameBasicOrd();
			NumberStreetNameTypeBasicOrd();
			NumberStreetNameTypeSufdirBasicOrd();
			NumberStreetNameTypePredirBasicOrd();
			NumberStreetNameTypePredirBasicOrd2();
			NumberStreetPref();
			NumberQualPreStreet();
			NumPredirRtStSufdir();
			S_340TH_E();
			EZ_ST();
			MY_DR_N();
			SE_E_ST();
			S_1ST();
			_82ND_AVE_W();
			W_NORTH_RIVER_DR();
			GRAVELLY_BEACH_LOOP_NW();
			E_MILL_PLAIN_BLVD();
			HWY_112();
			POINT_LAWRENCE_RD();
			MERIDIAN_E();
			CAMELOT_PARK_SW();
			ST_158TH_KP_N();
			OLD_HIGHWAY_99_SW();
			OLD_HIGHWAY_99_NORTH_RD();
			FLYING_L_LN();
			LA_PUSH_RD();
			W_CORD_12();
			OLD_99_N();
			_79TH_ST_NE();
			SE_TJ_LN();
			RD_I6_NE();
			R_W_JOHNSON_RD_SW();
			MARK_E_REED_WAY();
			NE_Fourth_Plain();
			E_4TH_PLAIN_BLVD();
			Old_Owen_Rd();
			_1st_ave_south();
			_934_31_Ave();
			_3380_146th_Place_SE_Suite_320();
			_15001_35TH_AVE_W_PCT23();
			_515_W_ST_THOMAS_MORE_WAY();
			_34913_10th_place();
			_28502_STATE_HIGHWAY_3_NE();
			_4418_Ne_St_James_Rd();
			_8115_NE_St_Johns();
			_LA_VISTA_DR_SW();
			OLD_STATE_9_RD();
			W_OLD_INLAND_EMPIRE_HWY();
		}
	}
}
