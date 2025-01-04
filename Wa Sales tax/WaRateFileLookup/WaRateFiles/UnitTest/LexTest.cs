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

using NUnit.Framework;
using WaRateFiles.Support;

namespace WaRateFiles.UnitTest
{
	[TestFixture]
	public class LexTest
	{
		public LexTest()
		{
		}
		
		[Test]
		public void TestLex1()
		{			
			Lex lex = new Lex("123 ABC ST SW");
			
			LexTokenType token;
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_NUM, "Token 123");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"123"), "Lexum 123");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ALPHA, "Token ABC");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"ABC"), "Lexum ABC");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_TWOCHAR, "Token ST");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"ST"), "Lexum ST");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_TWOCHAR, "Token SW");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"SW"), "Lexum SW");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_EOF, "EOF");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),""), "Lexum EOF");
		}
		
		[Test]
		public void TestLex2()
		{
			Lex lex = new Lex("123rd ABC Street west");

			LexTokenType token;
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ORDINAL, "Token 123rd");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"123RD"), "Lexum 123rd");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ALPHA, "Token ABC");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"ABC"), "Lexum ABC");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ALPHA, "Token Street");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"STREET"), "Lexum Street");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ALPHA, "Token west");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"WEST"), "Lexum west");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_EOF, "EOF");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),""), "Lexum EOF");
		}
		
		[Test]
		public void TestLex3()
		{
			Lex lex = new Lex("S 100rk 123-1 1/1 3rd&4th #3");

			LexTokenType token;
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ONECHAR, "Token S");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"S"), "Lexum S");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ALPHANUM, "Token 100rk");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"100RK"), "Lexum 100rk");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ALPHANUM, "Token 123-1");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"123-1"), "Lexum 123-1");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_FRACTION, "Token 1/1");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"1/1"), "Lexum 1/1");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ORDINAL, "Token 3rd");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"3RD"), "Lexum 3rd");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_AMP, "Token &");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"&"), "Lexum &");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ORDINAL, "Token 4th");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"4TH"), "Lexum 4th");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_ALPHANUM, "Token #3");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),"#3"), "Lexum #3");
			Assert.IsTrue(lex.NextToken(out token) && token == LexTokenType.ADDRLEX_EOF, "EOF");
			Assert.IsTrue(StringHelper.AreEqual(lex.Lexum(),""), "Lexum EOF");
		}
	}
}
