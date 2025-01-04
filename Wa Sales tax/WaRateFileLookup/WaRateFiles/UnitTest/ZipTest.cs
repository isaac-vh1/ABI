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

namespace WaRateFiles.Support
{
	[TestFixture]
	public class ZipTest
	{
		public ZipTest()
		{
		}

		[Test]
		public void TestZipIsZIP()
		{		
			Assert.IsTrue(ZIP.IsZip("12345"));
			
			Assert.IsTrue(ZIP.IsZip("12345-1234"));
			Assert.IsTrue(ZIP.IsZip("123451234"));
			
			Assert.IsFalse(ZIP.IsZip("ABCDE"));
			Assert.IsFalse(ZIP.IsZip("ABCDE"));
			
			Assert.IsTrue(ZIP.IsZip("12345", "1234"));
			Assert.IsFalse(ZIP.IsZip("12345", "----"));
			Assert.IsTrue(ZIP.IsZip("123451234"));
			Assert.IsTrue(ZIP.IsZip("12345", ""));			
		}
		
		[Test]
		public void TestZipParse()
		{
			ZIP zip = ZIP.Parse("12345");
			Assert.AreEqual(zip.Zip5, 12345);
			zip = ZIP.Parse("12345");
			Assert.AreEqual(zip.Zip5, 12345);
			Assert.AreEqual(zip.ToInt() , 123450000);
		
			zip = ZIP.Parse("12345-1234");
			Assert.AreEqual(zip.Zip5, 12345);
			Assert.IsTrue(zip.HasPlus4);
			Assert.AreEqual(zip.Plus4, 1234);
			Assert.AreEqual(zip.ToInt(), 123451234);
			
			zip = ZIP.Parse("123451234");
			Assert.AreEqual(zip.Zip5, 12345);
			Assert.AreEqual(zip.Plus4, 1234);
			Assert.AreEqual(zip.ToInt(), 123451234);
			
			zip = ZIP.Parse("12345", "1234");
			Assert.AreEqual(zip.Zip5, 12345);
			Assert.IsTrue(zip.HasPlus4);
			Assert.AreEqual(zip.Plus4, 1234);
			Assert.AreEqual(zip.ToInt(), 123451234);			
		}
	}
}
