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

namespace WaRateFiles.UnitTest
{
	[TestFixture]
	public class RateTest
	{
		private RateLookup m_lu;
		
		public RateTest()
		{
			m_lu = new RateLookup("./State.txt", "./Rates040108.csv", "./ZIP4RATESQ22008.csv", RateLookupEngine.STANDARDIZER, false);
		}

		public RateTest(string addfn, string ratefn, string zipfn)
		{
			m_lu = new RateLookup(addfn, ratefn, zipfn, RateLookupEngine.STANDARDIZER, false);
		}

		[TestFixtureSetUp]
		public void Init()
		{
			if (FileMaintenance.IsUpdateAvailable("./"))
			{
				FileMaintenance.UpdateFiles("./");
			}
		}

		private void CheckLookup(string street, string city, string zip, LocationSource src, int loccode, decimal drate)
		{
			AddressLine addr;
			Rate rate = null;
			LocationSource srcout;
			
			bool ret = m_lu.FindRate(street, city, zip, out addr, ref rate, out srcout);
			Assert.IsTrue(src == srcout);
			if (srcout == LocationSource.NONE)
			{
				return;
			}
			Assert.AreEqual(true, ret);
			Assert.AreEqual(loccode, rate.LocationCodeInt);
			Assert.AreEqual(drate, rate.TotalRate);
		}

		[Test]
		public void Aberdeen()
		{
			CheckLookup("17 LENTZ DRIVE", "Aberdeen", "98520", LocationSource.ADDRESS, 1400, (decimal).083);
			CheckLookup("1608 w. market", "aberdeen", "98520", LocationSource.ADDRESS, 1401, (decimal).083);
		}

		[Test]
		public void Algorna()
		{
			CheckLookup("320 1st Ave N", "algorna", "98001", LocationSource.ADDRESS, 1701, (decimal).09);
		}

		[Test]
		public void Allyn()
		{
			CheckLookup("18327 east state route 3", "Allyn", "98524", LocationSource.ZIP5, 2300, (decimal).084);
		}

		[Test]
		public void AmandaPark()
		{
			CheckLookup("7127 ", "Amanda Park", "98526", LocationSource.ZIP5, 1400, (decimal).083);
		}

		[Test]
		public void Anacortes()
		{
			CheckLookup("1400 4th ST", "Anacortes", "98221", LocationSource.ADDRESS, 2901, (decimal).08);
			CheckLookup("1400 4th Street", "Anacortes", "98221", LocationSource.ADDRESS, 2901, (decimal).08);
		}

		[Test]
		public void Arlington()
		{
			CheckLookup("17809 85th ave ne", "arlington", "98223", LocationSource.ADDRESS, 3101, (decimal).085);
			CheckLookup("17318 117th Pl NE","Arlington","98223", LocationSource.ADDRESS, 4200, (decimal).076);
			CheckLookup("4417 172ND ST NE", "", "98223", LocationSource.ADDRESS, 3101, (decimal).085);
			CheckLookup("16410 35th Ave NE", "Arlington", "98223", LocationSource.ADDRESS, 3101, (decimal).085);
			CheckLookup("26320 2ND AVENE", "ARLINGTON", "98223", LocationSource.ADDRESS, 4231, (decimal).085);
			CheckLookup("208 WEST AVE S", "ARLINGTON", "98223", LocationSource.ADDRESS, 3101, (decimal).085);
			CheckLookup("17725 48", "Arlington", "98223", LocationSource.ADDRESS, 3101, (decimal).085);
			CheckLookup("17725 48th", "Arlington", "98223", LocationSource.ADDRESS, 3101, (decimal).085);
			CheckLookup("139 n lenore ave ", "arlington", "98223", LocationSource.ADDRESS, 3101, (decimal).085);
			CheckLookup("20800 lake riley rd", "arlington", "98223", LocationSource.ADDRESS, 4231, (decimal).085);
			
			// Not found
			//CheckLookup("13917 Clubway","Arlington","98223", LocationSource.ADDRESS, 4200, (decimal).076);
		}

		[Test]
		public void Auburn()
		{
			CheckLookup("509 Aaby Dr", "auburn", "98001", LocationSource.ADDRESS, 1702, (decimal).09);
			CheckLookup("38911 AUBURN ENUMCLAW RD", "AUBURN", "98092", LocationSource.ADDRESS, 4000, (decimal).086);
			CheckLookup("4710 Mill Pond DR se","auburn","98092", LocationSource.ADDRESS, 1702, (decimal).09);
			CheckLookup("25 Second Street NW", "Auburn", "98001", LocationSource.ADDRESS, 1702, (decimal).09);
		}

		[Test]
		public void BainbridgeIsland()
		{
			CheckLookup("6260 NE Tara Lane", "Bainbridge island", "98110", LocationSource.ADDRESS, 1804, (decimal).086);
		}

		[Test]
		public void BattleGround()
		{
			CheckLookup("24521 NE Lewisville Hwy.", "Battle Ground", "98604", LocationSource.ADDRESS, 600, (decimal).077);
			CheckLookup("510 NE 17TH AVE ","BATTLE GROUND","98604", LocationSource.ADDRESS, 601, (decimal).082);
			CheckLookup("201 North Parkway Avenue", "Battle Ground", "98604", LocationSource.ADDRESS, 601, (decimal).082);
			CheckLookup("620 NW 18TH STREET ","BATTLE GROUND WA ","98604", LocationSource.ADDRESS, 601, (decimal).082);
		}

		[Test]
		public void Bellevue()
		{
			CheckLookup("1485 130th Ave NE", "", "98005", LocationSource.ADDRESS, 1704, (decimal).09);
			CheckLookup("5356 153rd ave SE", "bellevue", "98006", LocationSource.ADDRESS, 1700, (decimal).09);
			CheckLookup("15909 SE 1st ST.", "Bellevue", "98008", LocationSource.ADDRESS, 1704, (decimal).09);
			CheckLookup("11121 MAIN STREET","BEAUX ARTS VILLAGE","98004", LocationSource.ADDRESS, 1704, (decimal).09);
			CheckLookup("4648 177TH AVE SE","BELLEVUE","98006", LocationSource.ADDRESS, 1704, (decimal).09);
			CheckLookup("10405 se 28th st","bellevue","98004", LocationSource.ADDRESS, 1703, (decimal).09);
			CheckLookup("17615 se 46th pl", "bellevue", "98006", LocationSource.ADDRESS, 1704, (decimal).09);
			CheckLookup("3220 98th Ave NE", "Bellevue", "98004", LocationSource.ADDRESS, 1708, (decimal).09);
			
			// Dash causes problem
			//CheckLookup("3380-146th Place SE Suite 320", "Bellevue", "98007", LocationSource.ADDRESS, 1704, (decimal).09);
		}

		[Test]
		public void Bellingham()
		{
			// correct street name is '1919 Humboldt St'
			CheckLookup("1919 humbolt", "bellingham", "98225", LocationSource.ADDRESS, 3701, (decimal).084);
			CheckLookup("1616", "Bellingham", "98225", LocationSource.ZIP5, 3781, (decimal).084);
			CheckLookup("12 Bellwether way", "Bellingham", "98225", LocationSource.ADDRESS, 3781, (decimal).084);
			CheckLookup("918 38TH ST", "BELLINGHAM", "98226", LocationSource.ZIP5, 3700, (decimal).078);
			CheckLookup("1345 King St", "Bellingham", "982266223", LocationSource.ZIP5, 3700, (decimal).078);
			CheckLookup("2400 SAMISH WAY", "BELLINGHAM", "98229", LocationSource.ADDRESS, 3701, (decimal).084);
			CheckLookup("2901 Squalicum Parkway","","98225", LocationSource.ADDRESS, 3701, (decimal).084);
			CheckLookup("3693 Waldron Place", "Bellingham", "98229", LocationSource.ADDRESS, 3737, (decimal).084);
			CheckLookup("4015 ELIZA AVE", "BELLINGHAM", "98226", LocationSource.ADDRESS, 3701, (decimal).084);
			CheckLookup("2900 E. CASCADE AVE", "BELLINGHAM", "98229", LocationSource.ADDRESS, 3701, (decimal).084);
		}

		[Test]
		public void Blaine()
		{
			CheckLookup("4816 n golf course drive", "blaine", "98230", LocationSource.ADDRESS, 3737, (decimal).084);

			// Address not found
			//CheckLookup("936 Peach Portal Drive", "Blaine", "98230", LocationSource.ADDRESS, 3702, (decimal).084);
		}

		[Test]
		public void BonneyLake()
		{
			CheckLookup("20508 108TH ST E", "BONNEY LAKE", "98391", LocationSource.ADDRESS, 2727, (decimal).088);
		}

		[Test]
		public void Bothell()
		{
			CheckLookup("20611 Bothell Everett Highway", "Bothell", "98012", LocationSource.ADDRESS, 3100, (decimal).08);
			CheckLookup("18939 120th Ave. NE","Bothell","98011", LocationSource.ADDRESS, 1706, (decimal).090);			
			CheckLookup("1927 172nd pl se","bothell ","98012-6410", LocationSource.ADDRESS, 3131, (decimal).089);
			CheckLookup("106 202ND ST SE", "bothell", "98012", LocationSource.ADDRESS, 3131, (decimal).089);
		}

		[Test]
		public void Brier()
		{
			CheckLookup("3501 222 Pl SW", "Brier", "98036", LocationSource.ADDRESS, 3102, (decimal).089);
		}

		[Test]
		public void Bremerton()
		{
			CheckLookup("3560 ridgetop ct ne", "", "98310", LocationSource.ADDRESS, 1801, (decimal).086);
			CheckLookup("4104 Chrey Lane", "Bremerton", "98312", LocationSource.ADDRESS, 1800, (decimal).086);
			CheckLookup("2601 Cherry Ave #111", "Bremerton", "98310", LocationSource.ADDRESS, 1801, (decimal).086);
		}

		[Test]
		public void Brewster()
		{
			CheckLookup("908 HIGHWAY 97", "BREWSTER", "98812", LocationSource.ADDRESS, 2400, (decimal).077);
		}

		[Test]
		public void BrushPrairie()
		{
			CheckLookup("17912 NE 159th Street", "Brush Prairie", "98606", LocationSource.ADDRESS, 600, (decimal).077);
		}

		[Test]
		public void Burien()
		{
			CheckLookup("15025 1st ave south", "burien", "98148", LocationSource.ADDRESS, 1734, (decimal).09);
			CheckLookup("18010 8th Avenue S", "Burien", "98148", LocationSource.ADDRESS, 1734, (decimal).09);
			CheckLookup("153 S 160th st apt 32","Burien","98148-1452", LocationSource.ADDRESS, 1734, (decimal).09);
		}

		[Test]
		public void Buckley()
		{
			CheckLookup("12009 261ST AVE E","BUCKLEY","98321", LocationSource.ADDRESS, 4100, (decimal).078);
			CheckLookup("24416 Buckley Tapps Hwy E", "Buckley", "98321", LocationSource.ADDRESS, 2700, (decimal).082);
		}

		[Test]
		public void Burlington()
		{
			CheckLookup("145 cascade pl", "burlington", "98233", LocationSource.ADDRESS, 2902, (decimal).08);
		}

		[Test]
		public void Camas()
		{
			CheckLookup("26104 NE 36TH ST","CAMAS","98607", LocationSource.ADDRESS, 600, (decimal).077);
		}
		
		[Test]
		public void Carbonado()
		{
			// Adress not found
			//CheckLookup("110 williams st", "carbonado", "98323", LocationSource.ADDRESS, 2703, (decimal).09);
		}

		[Test]
		public void CastleRock()
		{
			CheckLookup("728 Studebaker Road","Castle Rock","98611", LocationSource.ADDRESS, 800, (decimal).076);
		}
		
		[Test]
		public void Chehalis()
		{
			CheckLookup("214 VISTA RD","chehalis","98532", LocationSource.ADDRESS, 2100, (decimal).077);
			CheckLookup("1060 SW 20TH ","CHEHALIS","98532", LocationSource.ADDRESS, 2102, (decimal).079);
		}
		
		[Test]
		public void Chattaroy()
		{
			CheckLookup("36413 n. newport hwy","chattaroy","99003", LocationSource.ADDRESS, 3200, (decimal).08);
			CheckLookup("5925 E. Grouse Rd","Chattaroy","99003", LocationSource.ADDRESS, 3200, (decimal).08);
		}

		[Test]
		public void Clarkston()
		{
			CheckLookup("1715 powe dr", "clarkston", "99403", LocationSource.ADDRESS, 200, (decimal).075);
			CheckLookup("937 Port Way","Clarkston","99403", LocationSource.ADDRESS, 200, (decimal).075);
		}

		[Test]
		public void CleElum()
		{
			CheckLookup("1753 STONE RIDGE DRIVE", "CLE ELUM, WA", "98922", LocationSource.ADDRESS, 1900, (decimal).08);
			CheckLookup("3411 Lower Peoh Point Rd", "Cle Elum", "98922", LocationSource.ADDRESS, 1900, (decimal).08);
		}

		[Test]
		public void Colbert()
		{
			CheckLookup("3207 e woolard","colbert","99005", LocationSource.ADDRESS, 3200, (decimal).08);
		}

		[Test]
		public void Covington()
		{
			CheckLookup("17425 SE 272nd St", "Covington", "98042", LocationSource.ADDRESS, 1712, (decimal).086);
		}

		[Test]
		public void Coupeville()
		{
			CheckLookup("610 Ellwood Drive", "Coupeville", "98239", LocationSource.ADDRESS, 1500, (decimal).084);
		}

		[Test]
		public void Dayton()
		{
			CheckLookup("176 E. Main", "Dayton", "99328", LocationSource.ADDRESS, 701, (decimal).079);
		}

		[Test]
		public void DeerPark()
		{
			CheckLookup("6012 w herman", "deer park", "99006", LocationSource.ADDRESS, 3200, (decimal).08);
		}

		[Test]
		public void Deming()
		{
			CheckLookup("3714 Nelson Road", "", "98244", LocationSource.ADDRESS, 3737, (decimal).084);
		}

		[Test]
		public void Eatonville()
		{
			CheckLookup("35819 84th ave E", "Eatonville ", "98328", LocationSource.ADDRESS, 4100, (decimal).078);
		}
		
		[Test]
		public void Edmonds()
		{
			CheckLookup("8957 188th St SW", "Edmonds", "98026", LocationSource.ADDRESS, 3104, (decimal).089);

			// House number is order of magnitude off, street spans location codes
			//CheckLookup("117623 72nd Ave W", "", "98026", LocationSource.ADDRESS, 3104, (decimal).089);
		}

		[Test]
		public void Enumclaw()
		{
			CheckLookup("633 Harmony Lane","Enumclaw","98022", LocationSource.ADDRESS, 1711, (decimal).086);
		}
		
		[Test]
		public void Everett()
		{
			CheckLookup("3010 51st pl sw", "everett", "98203-1364", LocationSource.ADDRESS, 3105, (decimal).086);
			CheckLookup("11322 25th ST SE", "everett", "98205", LocationSource.ADDRESS, 4200, (decimal).076);
			CheckLookup("303 91st Avenue Northeast", "everett", "98205", LocationSource.ADDRESS, 3109, (decimal).085);
			CheckLookup("9407 4th Street Northeast", "Everett", "98205", LocationSource.ADDRESS, 4231, (decimal).085);
			CheckLookup("8609 Evergreen way","everett","98208", LocationSource.ADDRESS, 3105, (decimal).086);
			CheckLookup("3528 Federal Ave","Everett","98201", LocationSource.ADDRESS, 3105, (decimal).086);			
			CheckLookup("8422 6TH AVE SE","EVERETT","98208", LocationSource.ADDRESS, 3105, (decimal).086);

			CheckLookup("1527 87TH AVE NE", "EVERETT", "98205", LocationSource.ADDRESS, 3111, (decimal).085);
			CheckLookup("430 91ST AVE NE", "EVERETT", "98205", LocationSource.ADDRESS, 3109, (decimal).085);
			CheckLookup("1027 89TH DR NE", "EVERETT", "98205", LocationSource.ADDRESS, 4231, (decimal).085);
			CheckLookup("8322 15TH PL NE", "EVERETT", "98205", LocationSource.ADDRESS, 4231, (decimal).085);
			CheckLookup("8014 4TH PL NE", "EVERETT", "98205", LocationSource.ADDRESS, 3109, (decimal).085);
			CheckLookup("704 87TH AVE NE", "EVERETT", "98205", LocationSource.ADDRESS, 4231, (decimal).085);
			CheckLookup("8801 12TH ST NE", "EVERETT", "98205", LocationSource.ADDRESS, 3105, (decimal).086);
			CheckLookup("1230 96TH AVE SE", "EVERETT", "98258", LocationSource.ADDRESS, 3109, (decimal).085);
			CheckLookup("12303 4TH ST NE", "EVERETT", "98258", LocationSource.ADDRESS, 4231, (decimal).085);
			CheckLookup("1516 88TH AVE NE", "EVERETT", "98205", LocationSource.ADDRESS, 4200, (decimal).076);
			CheckLookup("1524 90TH DR NE", "EVERETT", "98205", LocationSource.ADDRESS, 3109, (decimal).085);
			CheckLookup("2524 south lake stevens road", "everett", "98205", LocationSource.ADDRESS, 4200, (decimal).076);

			// IMS says loc code 4200
			//CheckLookup("8820 E SUNNYSIDE SCHOOL RD", "EVERETT", "98205", LocationSource.ADDRESS, 3111, (decimal)0.085);

			// This looks like the correct row, but arcgis returns 3109.
			//1401,1599,O,90TH DR SE,WA,98205,1820,Q32008,3109,N,Snohomish PTBA,
			//1400,1598,E,90TH DR SE,WA,98205,1820,Q32008,4231,N,Snohomish PTBA,
			CheckLookup("1314 90TH DR NE", "EVERETT", "98205", LocationSource.ADDRESS, 4231, (decimal).085);
		}

		[Test]
		public void FallCity()
		{
			CheckLookup("7426 Lake Alice Rd SE", "Fall City", "98024", LocationSource.ADDRESS, 4000, (decimal).086);
			CheckLookup("31419 ne 155th st ","","98019", LocationSource.ADDRESS, 4000, (decimal).086);
			CheckLookup("33623 SE 43rd Street","Fall City","98024-0535", LocationSource.ADDRESS, 4000, (decimal).086);			
		}

		[Test]
		public void FederalWay()
		{
			CheckLookup("1939 s. commons", "federal way", "98003", LocationSource.ADDRESS, 1792, (decimal).09);
			CheckLookup("32710 40th ave sw", "", "98023", LocationSource.ADDRESS, 1732, (decimal).09);
			CheckLookup("31260 pacific highway south", "", "98003", LocationSource.ADDRESS, 1792, (decimal).09); 
			CheckLookup("34913 10th pl sw","federal way","98023", LocationSource.ADDRESS, 1732, (decimal).09);			
			CheckLookup("34913 10th place","federal way","98023", LocationSource.ADDRESS, 1732, (decimal).09);			
			CheckLookup("2626 south 360th street","federal way","98003", LocationSource.ADDRESS, 1700, (decimal).09);			
			CheckLookup("3455 South 344th Way","Federal Way","98001", LocationSource.ADDRESS, 1732, (decimal).09);
		}

		[Test]
		public void FridayHarbor()
		{
			CheckLookup("360 carter ave","friday harbor","98250", LocationSource.ADDRESS, 2801, (decimal).077);
		}
		
		[Test]
		public void GigHarbor()
		{
			CheckLookup("10336 Kopachuck Drive NW", "Gig Harbor", "98335", LocationSource.ADDRESS, 4127, (decimal).084);
			CheckLookup("5299 OLYMPIC DR NW", "GIG HARBOR", "98335", LocationSource.ADDRESS, 2708, (decimal).084);
			CheckLookup("6970 Ford Drive NW", "Gig Harbor", "98335", LocationSource.ADDRESS, 4127, (decimal).084);
			CheckLookup("8714 rosedale st ","gig harbor","98335", LocationSource.ADDRESS, 4127, (decimal).084);
			CheckLookup("826 jewil dr", "gig harbor", "98333", LocationSource.ADDRESS, 4127, (decimal).084);

			// File has 2788
			//CheckLookup("1805 44th st ct nw", "gig harbor", "98332", LocationSource.ADDRESS, 4127, (decimal).084);
		}

		[Test]
		public void Graham()
		{			
			//  This address is on the border between RTA and PTBA.  IMS returns 4127, but it's puting 
			//  the point street centerline.
			CheckLookup("8808 224", "GRAHAM", "98338", LocationSource.ADDRESS, 4127, (decimal).084);
			CheckLookup("8209 243rd st ct e", "graham", "98338", LocationSource.ADDRESS, 4100, (decimal).078);
		}

		[Test]
		public void Greenbank()
		{
			CheckLookup("765 Wonn Rd", "Greenbank", "98253", LocationSource.ADDRESS, 1500, (decimal).084);
			CheckLookup("4027 Junco Rd", "Greenbank", "98253", LocationSource.ADDRESS, 1500, (decimal).084);
		}

		[Test]
		public void Hoquiam()
		{
			// This is right on the border with aberdeen.  GIS returns 1401, but the file indicates 1404
			CheckLookup("820 Myrtle ST", "Hoquiam", "98550", LocationSource.ADDRESS, 1404, (decimal).083);
		}

		[Test]
		public void Issaquah()
		{
			CheckLookup("4514 193rd Pl. S.E.", "Issaquah", "98027", LocationSource.ADDRESS, 1714, (decimal).09);
			CheckLookup("908 3RD AVENUE NE", "ISSAQUAH", "98027", LocationSource.ADDRESS, 1714, (decimal).09);
			CheckLookup("6420 E. Lake Sammamish Parkway SE", "Issaquah", "98029", LocationSource.ADDRESS, 1714, (decimal).09);
			CheckLookup("25235 SE Mirrormont Dr.","Issaquah", "98027", LocationSource.ADDRESS, 4000, (decimal).086);
			CheckLookup("1628 24th Ave NE","Issaquah","98029", LocationSource.ADDRESS, 1714, (decimal).09);
		}

		[Test]
		public void Kalama()
		{
			CheckLookup("334 kilkelly rd", "kalama", "98625", LocationSource.ADDRESS, 800, (decimal).076);
		}
		
		[Test]
		public void Kennewick()
		{
			CheckLookup("639 N Kellogg", "Kennewick", "99336", LocationSource.ADDRESS, 302, (decimal).083);
			CheckLookup("8825 W 1st Ave", "kennewick", "99336", LocationSource.ADDRESS, 302, (decimal).083);
			CheckLookup("2825 w kennewick","kennewick","99336", LocationSource.ADDRESS, 302, (decimal).083);
			
			// Not found
			//CheckLookup("37912 S Nine Canyon Rd","Kennewick","99336", LocationSource.ADDRESS, 333, (decimal).083);
		}

		[Test]
		public void Kent()
		{
			CheckLookup("10204 SE 240th ST", "Kent", "98031", LocationSource.ADDRESS, 1715, (decimal).09);
			CheckLookup("6750 SOUTH 228TH STREET", "Kent", "98032", LocationSource.ADDRESS, 1715, (decimal).09);
			CheckLookup("14716 se 263rd st", "kent", "98042-8116", LocationSource.ADDRESS, 1715, (decimal).09);
			CheckLookup("10105 SE 212th St","kent","98031", LocationSource.ADDRESS, 1700, (decimal).09);
			CheckLookup("30876 W Lake Morton Dr SE ", "kent", "98042", LocationSource.ADDRESS, 4000, (decimal).086);
			CheckLookup("23528 132nd Avenue SE", "Kent", "98042", LocationSource.ADDRESS, 1715, (decimal).09);

			// Not found, dash problem
			//CheckLookup("24523 - 116th Ave. SE","Kent","98030-4939", LocationSource.ADDRESS, 1715, (decimal).09);
		}

		[Test]
		public void Kirkland()
		{
			CheckLookup("11800 124th", "Kirkland", "98034", LocationSource.ADDRESS, 1716, (decimal).09);
			CheckLookup("14140 125TH AVE NE", "KIRKLAND", "98033", LocationSource.ADDRESS, 1716, (decimal).09);
			CheckLookup("10018 NE 127th PL Apt B107", "Kirkland", "98034", LocationSource.ADDRESS, 1716, (decimal).09);
			CheckLookup("10519 NE 119TH ","KIRKLAND ","98034", LocationSource.ADDRESS, 1716, (decimal).09);
			CheckLookup("10634 ne 46th", "kirkland", "98033", LocationSource.ADDRESS, 1716, (decimal).09);
		}

		[Test]
		public void LaCenter()
		{
			CheckLookup("34710 NE 112th Ct","LaCenter","98629", LocationSource.ADDRESS, 600, (decimal).077);
		}
		
		[Test]
		public void Lacey()
		{
			CheckLookup("4500 Ruddell Rd","Lacey","98503", LocationSource.ADDRESS, 3402, (decimal).084);
			CheckLookup("4710 park center", "lacey", "98516", LocationSource.ADDRESS, 3400, (decimal).078);
			CheckLookup("691 SLEATER KINNEY ROAD SE", "LACEY", "98503-1007", LocationSource.ADDRESS, 3402, (decimal).084);
			CheckLookup("1930 Carpenter Rd. SE", "Lacey", "98503", LocationSource.ADDRESS, 3434, (decimal).084);
		}
		
		[Test]
		public void Lakebay()
		{
			CheckLookup("16605 10th Ave Kpn", "lakebay", "98349", LocationSource.ZIP5, 4127, (decimal).084);
		}

		[Test]
		public void LakeForestPark()
		{
			CheckLookup("4530 ne 204th pl.", "lake forest park", "98155", LocationSource.ADDRESS, 1717, (decimal).09);
		}
		
		[Test]
		public void LakeStevens()
		{
			CheckLookup("2717 86th Drive NE","lake stevens","98205", LocationSource.ZIP5, 4200, (decimal).076);
			CheckLookup("623 95TH DR SE", "LAKE STEVENS", "98258", LocationSource.ZIP5, 4200, (decimal).076);
			CheckLookup("3015 90TH AVE NE", "LAKE STEVENS", "98258", LocationSource.ADDRESS, 4200, (decimal).076);
		}
		
		[Test]
		public void Lakewood()
		{
			CheckLookup("7601 Onyx Drive SW", "Lakewood", "98498", LocationSource.ADDRESS, 2721, (decimal).088);
			CheckLookup("9923 Zircon Drive SW", "Lakewood", "98498", LocationSource.ADDRESS, 2721, (decimal).088);
			CheckLookup("9315 gravelly lake drive ", "Lakewood", "98499", LocationSource.ADDRESS, 2721, (decimal).088);
			CheckLookup("7701 steilacoom blvd", "lakewood", "98498", LocationSource.ADDRESS, 2721, (decimal).088);
		}

		[Test]
		public void Langley()
		{
			CheckLookup("5939 Ariel Way", "Langley", "98260", LocationSource.ADDRESS, 1500, (decimal).084);
		}

		[Test]
		public void LibertyLake()
		{
			CheckLookup("710 N garry dr.", "liberty lake", "99019", LocationSource.ADDRESS, 3212, (decimal).086);
			CheckLookup("22710 E Country Vista Dr", "Liberty Lake ", "99019", LocationSource.ADDRESS, 3212, (decimal).086);
			CheckLookup("23728 E 1st Avenue", "Liberty Lake", "99019", LocationSource.ADDRESS, 3200, (decimal).08);
		}

		[Test]
		public void Longview()
		{
			CheckLookup("2610 Ocean Beach Hwy", "Longview", "98632", LocationSource.ADDRESS, 804, (decimal).077);
		}

		[Test]
		public void Lynden()
		{
			CheckLookup("528 Pangborn Road", "Lynden", "98264", LocationSource.ADDRESS, 3737, (decimal).084);
			CheckLookup("637 Mayberry Dr", "Lynden", "98264", LocationSource.ADDRESS, 3705, (decimal).084);
			CheckLookup("1120 Van Dyk Road", "Lynden", "98264-9447", LocationSource.ADDRESS, 3737, (decimal).084);
		}

		[Test]
		public void Lynnwood()
		{
			CheckLookup("17925 Hwy 99", "lynnwood", "98037", LocationSource.ADDRESS, 3110, (decimal).089);
			CheckLookup("3109 156th St. SW", "Lynnwood", "98087", LocationSource.ADDRESS, 3131, (decimal).089);
			CheckLookup("17515 52","ltnnwood ","98037", LocationSource.ADDRESS, 3110, (decimal).089);
			CheckLookup("17515 52","lynnwood","98037", LocationSource.ADDRESS, 3110, (decimal).089);
			CheckLookup("15001 35TH AVE W %23 20-201", "lynnwood", "98036", LocationSource.ADDRESS, 3131, (decimal).089);
			CheckLookup("13825 Ash way ", "Lynnwood", "98037", LocationSource.ADDRESS, 3131, (decimal).089);
			CheckLookup("17515 52w d", "lynnwood", "98037", LocationSource.ADDRESS, 3110, (decimal).089);
		}

		[Test]
		public void MapleValley()
		{
			CheckLookup("22837 228TH CT SE", "MAPLE VALLEY", "98038", LocationSource.ADDRESS, 4000, (decimal).086);
		}

		[Test]
		public void Marysville()
		{
			CheckLookup("7726 33rd st ne", "Marysville", "98270", LocationSource.ADDRESS, 3111, (decimal).085);
		}

		[Test]
		public void Mead()
		{
			CheckLookup("14606 n fairview","mead","99021", LocationSource.ADDRESS, 3200, (decimal).08);
			CheckLookup("e 4509 bixel ct","mead","99021", LocationSource.ADDRESS, 3200, (decimal).08);			
		}

		[Test]
		public void Medina()
		{
			CheckLookup("3401 EVERGREEN POINT RD","MEDINA","98039", LocationSource.ADDRESS, 1718, (decimal).09);
		}
		
		[Test]
		public void MercerIsland()
		{
			CheckLookup("621 86th ave se", "mercer island", "98040", LocationSource.ADDRESS, 1719, (decimal).09);
			CheckLookup("9105 SE 57TH ST","MERCER ISLAND","98040", LocationSource.ADDRESS, 1719, (decimal).09);
		}

		[Test]
		public void MillCreek()
		{
			CheckLookup("4522 132ND STREET", "MILL CREEK", "98012", LocationSource.ADDRESS, 3119, (decimal).089);
		}

		[Test]
		public void Monroe()
		{
			CheckLookup("26024 Old Owen Rd", "Monroe", "98272-9069", LocationSource.ADDRESS, 4231, (decimal).085);
			CheckLookup("17201 Beaton Rd Se", "monroe", "98272", LocationSource.ADDRESS, 3112, (decimal).085);
			CheckLookup("21406 167th Ave", "Monroe", "98272", LocationSource.ADDRESS, 4200, (decimal).076);
		}

		[Test]
		public void Morton()
		{
			// Wrong ZIP
			//CheckLookup("377 south 2nd", "MORTON", "98356", LocationSource.ADDRESS, 1600, (decimal).084);
		}

		[Test]
		public void MosesLake()
		{
			CheckLookup("1725 Kittleson RD","Moses Lake","98837", LocationSource.ADDRESS, 1309, (decimal).079);
			CheckLookup("128 E WHEELER RD", "MOSES LAKE", "98837", LocationSource.ADDRESS, 1309, (decimal).079);
		}

		[Test]
		public void MountlakeTerrace()
		{
			CheckLookup("6608 220th st SW", "Mountlake Terrace", "98033", LocationSource.ZIP5, 1724, (decimal).09);
		}

		[Test]
		public void MountVernon()
		{
			CheckLookup("16965 Britt ","Mount Vernon","98273", LocationSource.ADDRESS, 2900, (decimal).078);
		}
		
		[Test]
		public void Mukilteo()
		{
			CheckLookup("5311 85th Pl SW", "Mukilteo", "98275", LocationSource.ADDRESS, 3114, (decimal).089);
			CheckLookup("10729 47th ave west","mukilteo","98275", LocationSource.ADDRESS, 3114, (decimal).089);
			CheckLookup("4680 CAMPUS PLACE", "MUKILTEO", "98275", LocationSource.ADDRESS, 3114, (decimal).089);
		}

		[Test]
		public void Newcastle()
		{
			CheckLookup("16523 SE Cougar mountain way", "newcastle", "98053", LocationSource.ZIP5, 4024, (decimal).086);
			CheckLookup("9016 138th Ave SE", "Newcastle", "98059", LocationSource.ADDRESS, 1736, (decimal).09);
		}

		[Test]
		public void NoosackEverson()
		{
			CheckLookup("101 1st  St", "Nooksack", "98247", LocationSource.ADDRESS, 3706, (decimal).084);
			CheckLookup("101 First St", "Nooksack", "98247", LocationSource.ADDRESS, 3706, (decimal).084);
			CheckLookup("101 1st St", "Everson", "98247", LocationSource.ADDRESS, 3703, (decimal).084);
		}

		[Test]
		public void NorthBend()
		{
			CheckLookup("44028 se 142nd st","north bend","98045", LocationSource.ADDRESS, 4000, (decimal).086);			
		}
		
		[Test]
		public void OakHarbor()
		{
			CheckLookup("1517 POLNELL ROAD", "", "98277", LocationSource.ADDRESS, 1500, (decimal).084);
			CheckLookup("741 NW CATHLAMET DR","OAK HARBOR","98277", LocationSource.ADDRESS, 1503, (decimal).084);
			CheckLookup("Midway Street", "Oak Harbor", "98278", LocationSource.ADDRESS, 1500, (decimal).084);
		}

		[Test]
		public void OakVille()
		{
			CheckLookup("312 PINE STREET", "OAKVILLE", "98568", LocationSource.ADDRESS, 1407, (decimal).083);
		}

		[Test]
		public void Olympia()
		{
			CheckLookup("3000 amhurst place se", "Olympia", "98501", LocationSource.ADDRESS, 3403, (decimal).084);
			CheckLookup("5614 McLane Creek Ct. SW", "Olympia", "98512", LocationSource.ADDRESS, 3400, (decimal).078);
			CheckLookup("3909 Swayne", "Olympia", "98516", LocationSource.ZIP5, 3400, (decimal).078);
			CheckLookup("4904 hemphill dr se", "olympia", "98513", LocationSource.ADDRESS, 3434, (decimal).084);
			CheckLookup("2405 crestline dr nw", "olympia", "98502", LocationSource.ADDRESS, 3434, (decimal).084);
			CheckLookup("1707 camden pk dr sw", "olympia", "98512", LocationSource.ADDRESS, 3403, (decimal).084);
			CheckLookup("9319 Clover CT SE","Olympia","98513", LocationSource.ADDRESS, 3434, (decimal).084);
			CheckLookup("1103 93rd Ave Se", "Olympia", "98501", LocationSource.ADDRESS, 3400, (decimal).078);
			CheckLookup("3722 kinsale ln","olympia","98501", LocationSource.ADDRESS, 3434, (decimal).084);
			CheckLookup("8745 martin way","olympia","98516", LocationSource.ADDRESS, 3402, (decimal).084);
			CheckLookup("5745 etude lp se", "olympia", "98513", LocationSource.ADDRESS, 3400, (decimal).078);
			CheckLookup("105 e 8th st", "olympia", "98512", LocationSource.ZIP5, 3400, (decimal).078);
			CheckLookup("10143 brooks ln se", "olympia", "98512", LocationSource.ZIP5, 3400, (decimal).078);
			CheckLookup("1501 SUMMIT LAKE SHORE RD.", "OLYMPIA", "98502", LocationSource.ADDRESS, 3400, (decimal).078);
			CheckLookup("2415 LEACH CT SE", "olympia", "98501-3169", LocationSource.ADDRESS, 3403, (decimal).084);
			CheckLookup("1100 plum street se", "olympia", "98501", LocationSource.ADDRESS, 3403, (decimal).084);
		}

		[Test]
		public void Packwood()
		{
			CheckLookup("118 JACK FIR CT E", "PACKWOOD", "98361", LocationSource.ZIP5, 2100, (decimal).077);
		}

		[Test]
		public void Pacific()
		{
			CheckLookup("1350 thornton ave sw","pacific","98047-2112", LocationSource.ADDRESS, 2723, (decimal).088);
		}

		[Test]
		public void PoBox()
		{
			CheckLookup("PO BOX 123", "", "98507", LocationSource.ADDRESS, 3403, (decimal).084);
		}

		[Test]
		public void PortOrchard()
		{
			CheckLookup("7366 E. Fir St.", "Port Orchard", "98366-8227", LocationSource.ADDRESS, 1800, (decimal).086);
			CheckLookup("5665 Banner Road SE,", "port orchid", "98367", LocationSource.ADDRESS, 1800, (decimal).086);
			CheckLookup("4430 se scenic view lanes","port orchard","98367", LocationSource.ADDRESS, 1800, (decimal).086);

			// IMS finds this, but in the address file the ZIP is 98367
			//CheckLookup("1920 sw pine rd ","port orchard","98366", LocationSource.ADDRESS, 1800, (decimal).086);
		}

		[Test]
		public void Poulsbo()
		{
			//CheckLookup("28502 STATE HIGHWAY 3 NE", "poulsbo", "98370", LocationSource.ADDRESS, 1803, (decimal).086);
		}
	
		[Test]
		public void Prosser()
		{
			// The house number is not in the file (nearest is about 150,000).
			CheckLookup("130511 W. Old Inland Empire Hwy", "Prosser", "99350-8526", LocationSource.ZIP9, 333, (decimal).083);
		}

		[Test]
		public void Puyallup()
		{
			CheckLookup("16217 87th ave east", "puyallup", "98375", LocationSource.ADDRESS, 2727, (decimal).088);
			CheckLookup("8010 151st E", "Puyallup", "98375-8440", LocationSource.ADDRESS, 2727, (decimal).088);
			CheckLookup("349 valley ave nw","puyallup","98371", LocationSource.ADDRESS, 2711, (decimal).088);			
			CheckLookup("16120 MERIDIAN EAST","PUYALLUP","98375", LocationSource.ADDRESS, 2727, (decimal).088);
			CheckLookup("15306 134th Ave E", "Puyallup", "98374-9614", LocationSource.ADDRESS, 2727, (decimal).088);
		}

		[Test]
		public void Redmond()
		{
			CheckLookup("2207 NE Bel-Red Rd", "Redmond", "98052", LocationSource.ADDRESS, 1724, (decimal).09);
			CheckLookup("25618 NE 100th St", "Redmond", "98053", LocationSource.ADDRESS, 4000, (decimal).086);
			CheckLookup("15223 ne 72nd st", "redmond", "98052", LocationSource.ADDRESS, 1724, (decimal).09);
			CheckLookup("9225 151st ne ","redmond","98052", LocationSource.ADDRESS, 1724, (decimal).09);			
		}

		[Test]
		public void Ridgefield()
		{
			CheckLookup("1843 s 15th court", "Ridgefield", "98642", LocationSource.ADDRESS, 604, (decimal).082);
			CheckLookup("5647 NW 199th Street","Ridgefield","98642", LocationSource.ADDRESS, 600, (decimal).077);
			CheckLookup("4313 NW 187th Way","Ridgefield","98642", LocationSource.ADDRESS, 600, (decimal).077);
		}

		[Test]
		public void Renton()
		{
			CheckLookup("19800 108th Ave SE", "Renton", "98055", LocationSource.ADDRESS, 1700, (decimal).09);
			CheckLookup("400 south 43rd street", "renton", "98055", LocationSource.ADDRESS, 1725, (decimal).09);
			CheckLookup("560 Nachess","renton","98057-2219", LocationSource.ADDRESS, 1725, (decimal).09);
			CheckLookup("862 ILWACO PL NE", "RENTON", "98059", LocationSource.ADDRESS, 1725, (decimal).09);
			CheckLookup("129 Capri Ave South", "Renton", "98056", LocationSource.ADDRESS, 1725, (decimal).09);

			// Totally missing from the files
			//CheckLookup("10720 SE Carr Rd", "Renton", "98050", LocationSource.ADDRESS, 1725, (decimal).09);
		}

		[Test]
		public void Roy()
		{
			CheckLookup("6520 288th St S","Roy","98580", LocationSource.ADDRESS, 4100, (decimal).078);
		}

		[Test]
		public void Sammamish()
		{
			CheckLookup("19740 se 17th st", "sammamish", "98075", LocationSource.ADDRESS, 1739, (decimal).09);
			CheckLookup("20732 ne inglewood hill rd ","sammamish","98074", LocationSource.ADDRESS, 1739, (decimal).09);
			CheckLookup("3437 207th ave se", "sammamish", "98075", LocationSource.ADDRESS, 1739, (decimal).09);
		}

		[Test]
		public void SeaTac()
		{
			CheckLookup("17019 33rd Ave South", "SeaTac", "98188", LocationSource.ADDRESS, 1733, (decimal).09);
			CheckLookup("12513 Military Rd S ", "Seatac", "98168", LocationSource.ADDRESS, 1700, (decimal).09);

			// Wrong ZIP
			//CheckLookup("3505 S 172ND ST", "SEATAC", "98180", LocationSource.ADDRESS, 1700, (decimal).09);
		}

		[Test]
		public void Seattle()
		{
			CheckLookup("934 31st Ave", "Seattle", "98122", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("18522 Fremont Ave. North", "Seattle", "98133", LocationSource.ADDRESS, 1737, (decimal).09);
			CheckLookup("1201 amgen ct west", "Seattle", "98119", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("9322 14th Ave S", "Seattle", "98108", LocationSource.ADDRESS, 1700, (decimal).09);
			CheckLookup("18000 International Blvd", "seattle", "98188", LocationSource.ADDRESS, 1733, (decimal).09);
			CheckLookup("309 South Cloverdale", "Seattle", "98108", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("2842 SW 102nd street", "seatle", "98146", LocationSource.ADDRESS, 1700, (decimal).09);
			CheckLookup("10633 20th Ave S", "seattle", "98168", LocationSource.ADDRESS, 1700, (decimal).09);
			CheckLookup("1660 s columbian way", "seattle", "98108", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("940 s harney st", "seattle", "98108", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("1301 4th ave", "seattle", "98101", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("1705 S. 93rd Street", "Seatle", "98108", LocationSource.ADDRESS, 1700, (decimal).09);
			CheckLookup("325 9th av", "seattle", "98104", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("3924 sw arroyo ct", "", "98146", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("400 NE RAVENNA BLVD", "SEATTLE", "98115", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("1200 6th Avenue","Seattle","98101", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("403 23rd ave s","seattle","98144", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("8551 Greenwood Ave N","Seattle","98103", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("6468 Marshall Ave SW","Seattle","98136", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("403 23rd ave","seattle","98144", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("22 W Lee St Apt 201","Seattle","98119-3394", LocationSource.ADDRESS, 1726, (decimal).09);			
			CheckLookup("226 1st street ne","seattle","98002", LocationSource.ADDRESS, 1702, (decimal).09);			
			CheckLookup("4135 SW ROSE ST","SEATTLE","98136", LocationSource.ADDRESS, 1726, (decimal).09);			
			CheckLookup("719 2nd ave","seattle","98104", LocationSource.ADDRESS, 1726, (decimal).09);			
			CheckLookup("2735 California Ave SW","Seattle","98116", LocationSource.ADDRESS, 1726, (decimal).09);			
			CheckLookup("800 5th ave ste 4000","seattle","98104", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("12441 des moines memorial dr s","seattle","98168", LocationSource.ADDRESS, 1700, (decimal).09);
			CheckLookup("7272 W Marginal Way S","","98108", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("1010 Fifth Avenue", "Seattle", "98104", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("4th Pike", "Seattle", "98101", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("11000 lLAKE CITY WAY NE", "SEATTLE", "98125", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("1427 third ave", "seattle", "98101", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("3522 Burke Avenue N", "Seattle", "98103-9028", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("11000 LAKE CITY WAY NE", "SEATTLE", "98125", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("1718 13th Ave S", "Seattle", "98144", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("119 Yesler Way", "Seattle", "98104-2525", LocationSource.ADDRESS, 1726, (decimal).09);

			// ZIP not found
			//CheckLookup("3800 S OTHELLO STREET", "SEATTLE", "98111", LocationSource.ADDRESS, 1726, (decimal).09);
		}

		[Test]
		public void SedroWoolley()
		{
			CheckLookup("34531 state route 20", "sedro woolley", "98284", LocationSource.ADDRESS, 2929, (decimal).08);
			CheckLookup("948 FIDALGO ST ","","98284", LocationSource.ADDRESS, 2908, (decimal).08);
			CheckLookup("6804 butler mill rd", "sedro wooley", "98284", LocationSource.ADDRESS, 2929, (decimal).08);
		}

		[Test]
		public void Shoreline()
		{
			CheckLookup("14900 Aurora N", "shoreline", "98133", LocationSource.ADDRESS, 1737, (decimal).09);
			CheckLookup("17551 15th ave ne", "shoreline", "98155", LocationSource.ADDRESS, 1737, (decimal).09);
			CheckLookup("19909 Ballinger Way NE", "Shoreline ", "98155", LocationSource.ADDRESS, 1737, (decimal).09);
		}

		[Test]
		public void Snohomish()
		{
			CheckLookup("18518 Storm Lake Rd", "snohomish", "98290", LocationSource.ADDRESS, 4200, (decimal).076);
			CheckLookup("5318 Lake Bosworth Ln", "Snohomish ", "98290", LocationSource.ADDRESS, 4231, (decimal).085);
			CheckLookup("16207 88TH ST SE", "Snohomish", "98290", LocationSource.ADDRESS, 4231, (decimal).085);
			CheckLookup("19309 78TH AVE SE", "SNOHOMISH", "98296", LocationSource.ADDRESS, 4200, (decimal).076);
			CheckLookup("210 Cypress Way", "snohomish", "98290", LocationSource.ADDRESS, 3115, (decimal).085);
			CheckLookup("310 garden court","snohomish ","98290", LocationSource.ADDRESS, 3115, (decimal).085);
			CheckLookup("16120 51st st se","snohomish","98290", LocationSource.ADDRESS, 4200, (decimal).076);
			CheckLookup("208 WEST AVE S", "", "98223", LocationSource.ADDRESS, 3101, (decimal).085);
			CheckLookup("20319 82nd ae se", "snohomish", "98290", LocationSource.ADDRESS, 4200, (decimal).076);
			//CheckLookup("704 87th ave ne", "", "98205", LocationSource.ADDRESS, 3109, (decimal).085);
		}

		[Test]
		public void Skyway()
		{
			CheckLookup("12222 78th Ave S", "Skyway", "98178", LocationSource.ADDRESS, 1700, (decimal).09);
		}

		[Test]
		public void Snoqualmie()
		{
			CheckLookup("7428 heather ave se", "Snoqualmie", "98065", LocationSource.ADDRESS, 1728, (decimal).086);

			// ZIP is not in the gis system?
			//CheckLookup("1001 906", "snoqualmie", "98068", LocationSource.ADDRESS, 3232, (decimal).086);
		}

		[Test]
		public void Spanaway()
		{
			CheckLookup("17416 Pacific Ave. S., Suite B", "Spanaway", "98387", LocationSource.ADDRESS, 2727, (decimal).088);
		}

		[Test]
		public void Spokane()
		{
			CheckLookup("2211 west strong rd", "spokane", "99208", LocationSource.ADDRESS, 3210, (decimal).086);
			CheckLookup("11327 N Whitehouse St", "Spokane", "99218", LocationSource.ADDRESS, 3232, (decimal).086);
			CheckLookup("4330 s regal st", "Spokane", "99223-5095", LocationSource.ADDRESS, 3210, (decimal).086);
			CheckLookup("6230 W Custer", "Spokane", "99223", LocationSource.ADDRESS, 3200, (decimal).08);
			CheckLookup("611 marll ct", "fairfield", "99012", LocationSource.ADDRESS, 3204, (decimal).08);
			CheckLookup("3352 w woodside","spokane","99208", LocationSource.ADDRESS, 3210, (decimal).086);
			CheckLookup("1330 N washington", "", "99201-2456", LocationSource.ADDRESS, 3210, (decimal).086);
			CheckLookup("515 W ST THOMAS MORE WAY", "SPOKANE", "99208", LocationSource.ADDRESS, 3232, (decimal).086);
			CheckLookup("16125 E Wellesley", "Spokane", "99216", LocationSource.ADDRESS, 3213, (decimal).086);
			CheckLookup("35 W MAIN", "SPOKANE", "99201", LocationSource.ADDRESS, 3210, (decimal).086);
			CheckLookup("2400 n craig rd", "spokane", "99224", LocationSource.ADDRESS, 3200, (decimal).08);
			CheckLookup("828 W SPOFFORD AVE", "SPOKANE", "99205", LocationSource.ADDRESS, 3210, (decimal).086);
		}

		[Test]
		public void SpokaneValley()
		{
			CheckLookup("10209 E. Broadway", "Spokane Valley", "99206", LocationSource.ADDRESS, 3213, (decimal).086);
			CheckLookup("16814 E. Sprague", "Spokane Valley", "99037", LocationSource.ADDRESS, 3213, (decimal).086);
			CheckLookup("11400 E SPRAGUE AVE", "Spokane Valley", "99206", LocationSource.ADDRESS, 3213, (decimal).086);
		}

		[Test]
		public void Steilacoom()
		{
		}
		
		[Test]
		public void Sumner()
		{
			CheckLookup("1302 puyallup st","sumner","98390", LocationSource.ADDRESS, 2716, (decimal).088);
		}
		
		[Test]
		public void Tumwater()
		{
			CheckLookup("6300 linderson way sw", "tumwater", "98501", LocationSource.ADDRESS, 3406, (decimal).084);
			CheckLookup("6300 linderson way", "tumwater", "98501", LocationSource.ADDRESS, 3406, (decimal).084);
			CheckLookup("6300 linderson", "tumwater", "98501", LocationSource.ADDRESS, 3406, (decimal).084);
			CheckLookup("6300 linderson sw", "tumwater", "98501", LocationSource.ADDRESS, 3406, (decimal).084);
			
			// Miss spellings
			CheckLookup("6300 lindersan way sw", "tumwater", "98501", LocationSource.ADDRESS, 3406, (decimal).084);
			CheckLookup("6300 lindrson", "tumwater", "98501", LocationSource.ADDRESS, 3406, (decimal).084);

			CheckLookup("1205 Second Ave S", "Tumwater", "98512", LocationSource.ADDRESS, 3406, (decimal).084);
		}

		[Test]
		public void Tacoma()
		{
			CheckLookup("6230 SOUTH CHEYENNE ST", "TACOMA", "98409", LocationSource.ADDRESS, 2717, (decimal).088);
			CheckLookup("4502 south steele st", "tacoma", "98409", LocationSource.ADDRESS, 2717, (decimal).088);
			CheckLookup("1011 East E Street", "Tacoma", "98421", LocationSource.ADDRESS, 2717, (decimal).088);
			CheckLookup("100 alexander ave ","","98421", LocationSource.ADDRESS, 2717, (decimal).088);
			CheckLookup("623 N CARR ST", "TACOMA", "98433", LocationSource.ZIP5, 2700, (decimal).082);
			CheckLookup("16127 winchester dr e","tacoma","98445", LocationSource.ADDRESS, 2727, (decimal).088);			
			CheckLookup("901 alexander ave","tacoma","98421", LocationSource.ADDRESS, 2717, (decimal).088);			
			CheckLookup("6114 N 16TH","TACOMA","98406", LocationSource.ADDRESS, 2717, (decimal).088);			
			CheckLookup("6105 n 24th st","tacoma","98406", LocationSource.ADDRESS, 2717, (decimal).088);			
			CheckLookup("1675 lincoln ave bld 300","tacoma","98421", LocationSource.ADDRESS, 2717, (decimal).088);
			CheckLookup("6405 so tacoma way","","98409", LocationSource.ADDRESS, 2717, (decimal).088);
			CheckLookup("13013 8th ave ct e", "tacoma", "98445", LocationSource.ADDRESS, 2727, (decimal).088);
			CheckLookup("802 N G ST", "Tacoma", "98403", LocationSource.ADDRESS, 2717, (decimal).088);

			// Pacific isn't in this this ZIP, but 2727 is returned by IMS
			CheckLookup("14113 PACIFIC AVE", "TACOMA", "98445", LocationSource.ZIP5, 2727, (decimal).088);
		}

		[Test]
		public void Toledo()
		{
			CheckLookup("864 cedar creek rd", "toledo", "98591", LocationSource.ADDRESS, 2100, (decimal).077);
		}

		[Test]
		public void Toutle()
		{
			CheckLookup("210 toutle River Road", "Toutle ", "98611", LocationSource.ADDRESS, 800, (decimal).076);
		}

		[Test]
		public void Underwood()
		{
			CheckLookup("51 CIRLCE DRIVE", "UNDERWOOD", "98651", LocationSource.ZIP5, 3000, (decimal).07);
		}

		[Test]
		public void Union()
		{
			CheckLookup("81 e vinemaple lane", "union", "98592", LocationSource.ADDRESS, 2300, (decimal).084);
		}

		[Test]
		public void UniversityPlace()
		{
			CheckLookup("10311 CHAMBERS CREEK RD W", "UNIVERSITY PLACE", "98467", LocationSource.ADDRESS, 2727, (decimal).088);
		}

		[Test]
		public void Vancouver()
		{
			CheckLookup("13701 NE Fourth Plain", "vancouver", "98682", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("501 E 4TH PLAIN BLVD", "vancouver", "98663", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("13503 SE Mill Plain Blvd.", "vancouver", "98684", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("7814 NE 62nd Way", "vancouver", "98662", LocationSource.ADDRESS, 666, (decimal).082);
			CheckLookup("1507 se 113th ct", "vancouver", "98664", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("125 NW 111 LOOP", "VANCOUVER ", "98685", LocationSource.ADDRESS, 666, (decimal).082);
			CheckLookup("500 broadway street", "vancouver", "98660", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("525 ne 125th circle", "Vancouver", "98685", LocationSource.ADDRESS, 666, (decimal).082);
			CheckLookup("13601 NW INDIAN SPRINGS DR","Vancouver","98685", LocationSource.ADDRESS, 666, (decimal).082);
			CheckLookup("16006 NE 6th Ave","Vancouver","98684", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("16621 NE 207th Ave","Brush Prairie","98606-9702", LocationSource.ADDRESS, 600, (decimal).077);
			CheckLookup("11006 NE 37th Cir","Vancouver","98682", LocationSource.ADDRESS, 605, (decimal).082);			
			CheckLookup("6810 NE 150th Place","Vancouver","98682", LocationSource.ADDRESS, 666, (decimal).082);
			CheckLookup("14215 SE 35 Loop","Vancouver","98683", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("4313 NW 187th Way","","98642", LocationSource.ADDRESS, 600, (decimal).077);
			CheckLookup("14215 SE 35 Loop","Vancouver","98683", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("4608 NE 112TH CIR", "VANCOUVER", "98686-4436", LocationSource.ADDRESS, 666, (decimal).082);
			CheckLookup("13503 SE Mill Plain Boulevard %237","Vancouver","98684-6984", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("10303 NE Fourth Plain Rd.", "Vancouver", "98682", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("7317 E Mill Plain Blvd ", "Vancouver", "98664", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("13801 NW 10th Court", "vancouver", "98685", LocationSource.ADDRESS, 666, (decimal).082);
			CheckLookup("600 SE Maritime Ave ", "Vancouver", "98661", LocationSource.ADDRESS, 605, (decimal).082);
			CheckLookup("1800 nw 111th st", "", "98685", LocationSource.ADDRESS, 666, (decimal).082);

			// Wrong ZIP
			//CheckLookup("1111 Main Place", "Vancouver", "98661", LocationSource.ADDRESS, 605, (decimal).082);
		}

		[Test]
		public void VashonIsland()
		{
			CheckLookup("11344 SW McCormick Pl", "Vashon Island", "98070", LocationSource.ADDRESS, 4000, (decimal).086);
			CheckLookup("11413 103rd ave sw", "vashon", "98070-3091", LocationSource.ADDRESS, 4000, (decimal).086);
		}

		[Test]
		public void Veradale()
		{
			CheckLookup("15209 E. Pinnacle Lane", "Veradale", "99037", LocationSource.ADDRESS, 3232, (decimal).086);
		}

		[Test]
		public void Washougal()
		{
			CheckLookup("700 se 380th ct", "washougal", "98671", LocationSource.ADDRESS, 600, (decimal).077);
			CheckLookup("182 JENNIFER WAY","WASHOUGAL","98671", LocationSource.ADDRESS, 3000, (decimal).07);			

			CheckLookup("3720 grant st", "Washougal", "98671", LocationSource.ADDRESS, 600, (decimal).077);
		}

		[Test]
		public void Wenatchee()
		{
			CheckLookup("3919 CASCADE NW","EAST WENATCHEE","98802", LocationSource.ADDRESS, 909, (decimal).08);
		}

		[Test]
		public void Woodinville()
		{
			CheckLookup("18430 NE 194th St", "Woodinville", "98077", LocationSource.ADDRESS, 4000, (decimal).086);
		}

		[Test]
		public void Woodland()
		{
			CheckLookup("2820 NE 434th Street", "Woodland", "98674", LocationSource.ADDRESS, 600, (decimal).077);
		}

		[Test]
		public void Yakima()
		{
			CheckLookup("5010 Richey Road", "Yakima", "98908", LocationSource.ADDRESS, 3913, (decimal).082);
			CheckLookup("604 S 30TH AVE", "YAKIMA", "98902", LocationSource.ADDRESS, 3913, (decimal).082);
		}

		[Test]
		public void Yacolt()
		{
			CheckLookup("36800 NE 233rd Avenue", "Yacolt", "98675", LocationSource.ADDRESS, 600, (decimal).077);
		}

		[Test]
		public void Junk()
		{
			CheckLookup("NO. 08", "RENTON", "98058", LocationSource.ZIP5, 4000, (decimal).086);

			CheckLookup("164th Ave", "Vancouver", "98682", LocationSource.ZIP5, 600, (decimal).077);
			CheckLookup("24 West Shore CCourt", "Steilacoom", "98388", LocationSource.ZIP5, 2700, (decimal).082);
			CheckLookup("5732 S RAVENCREST", "SPOKANE", "99224", LocationSource.ZIP5, 3200, (decimal).08);
			CheckLookup("5706 S LAURELCREST", "SPOKANE", "99224", LocationSource.ZIP5, 3200, (decimal).08);
			CheckLookup("S 376 st", "auburn", "98001", LocationSource.ZIP5, 2720, (decimal).088);
			CheckLookup("491 SR 506", "Toledo", "98591", LocationSource.ZIP5, 2107, (decimal).077);
			CheckLookup("2425 NORTHEAST 17TH AVE", "VANCOUVER", "98669", LocationSource.NONE, 1500, (decimal).084);
			//CheckLookup("8623 Armstrong Road SW", "Olympia", "98504", LocationSource.ADDRESS, 3403, (decimal).084);
			CheckLookup("P.O. Box 4069", "Spanaway", "98387", LocationSource.ADDRESS, 2727, (decimal).088);
			CheckLookup("11290 SE 294th ", "Tumwater", "98512", LocationSource.ZIP5, 3400, (decimal).078);
			CheckLookup("706 S ASPEN ST", "AIRWAY HEIGHTS", "99001", LocationSource.ZIP5, 3232, (decimal).086);
			CheckLookup("1118 SOUTH AFTEN", "AIRWAY HEIGHTS", "99001", LocationSource.ZIP5, 3232, (decimal).086);
			CheckLookup("p o box 1670", "deer park", "99006", LocationSource.ADDRESS, 3203, (decimal).08);
			//CheckLookup("29416 SR 410", "ENUMCLAW", "WA", LocationSource.NONE, 4000, (decimal).086);
			CheckLookup("008w sunrise ", "", "99620-8652", LocationSource.NONE, 4000, (decimal).086);
			CheckLookup("71 E CAMPUS DRIVE", "BELFAIR", "98528", LocationSource.ZIP5, 2300, (decimal).084);
			CheckLookup("4318 Tombstone Way", "Springdale", "98173-9750", LocationSource.NONE, 4000, (decimal).086);
			CheckLookup("105 MT PARK BLVD", "ISSAQUAH", "98027", LocationSource.ZIP5, 4014, (decimal).086);
			CheckLookup("2601 SR 509 N. FRONTAGE RD", "TACOMA", "98421", LocationSource.ZIP5, 2717, (decimal).088);
			CheckLookup("15730 State Route 9 Se", "Snohomish", "98296", LocationSource.ZIP5, 4200, (decimal).076);
			CheckLookup("298 Summer Circle", "Walla Walla", "99362", LocationSource.ZIP5, 3600, (decimal).08);
			CheckLookup("5155 A Casberg Burroughs Rd", "Deer Park", "99006", LocationSource.ZIP5, 3300, (decimal).076);
			CheckLookup("po box 1914", "seattle", "98111-1914", LocationSource.ADDRESS, 1726, (decimal).09);
			CheckLookup("618 harvest rd", "bothell", "98033", LocationSource.ZIP5, 1724, (decimal).09);
			//CheckLookup("23226 30th Ave S", "Des Moines", " 9819", LocationSource.NONE, 4000, (decimal).086);
			CheckLookup("12900 Lake avenue", "lakewood", "44107", LocationSource.NONE, 4000, (decimal).086);
			CheckLookup("930 moffet", "port orchard", "98366", LocationSource.ZIP5, 1802, (decimal).086);
			CheckLookup("37500 North Bend Way", "Snoqualmie", "98065", LocationSource.ZIP5, 4000, (decimal).086);
			CheckLookup("9317 N Elm Lane", "", "99208", LocationSource.ZIP5, 3200, (decimal).08);
			CheckLookup("29712 N KONZAL LANE ", "DEERPARK", "99006", LocationSource.ZIP5, 3300, (decimal).076);
			CheckLookup("NE 109th Way ", "Kirkland, Wa. ", "98033", LocationSource.ZIP5, 1724, (decimal).09);
			CheckLookup("37500 North Bend Way", "Snoqualmie", "98065", LocationSource.ZIP5, 4000, (decimal).086);
			CheckLookup("28010 ", "Buckley", "98321-9212", LocationSource.ZIP9, 2702, (decimal).084);
			CheckLookup("28010 State Route 410 East", "Buckley", "98321", LocationSource.ZIP5, 4100, (decimal).078);

			CheckLookup("86317 sr410 east", "greenwater", "98022", LocationSource.ZIP5, 4100, (decimal).078);
			CheckLookup("29900 FERN BLUFF ROAD", "MONROE", "79272", LocationSource.NONE, 0, (decimal)0);

			// File returns 2720
			//CheckLookup("SUITE B", "AUBURN", "98001", LocationSource.ZIP5, 1702, (decimal).09);

			CheckLookup("HC80 BOX 1475", "FORKS", "98331", LocationSource.ZIP5, 1600, (decimal).084);
			CheckLookup("HC 80 BOX 1475", "FORKS", "98331", LocationSource.ZIP5, 1600, (decimal).084);
			CheckLookup("12502 n hwy 395", "spokane", "99218", LocationSource.ZIP5, 3200, (decimal).08);
			CheckLookup("P O 1265", "deer park", "99006", LocationSource.ZIP5, 3300, (decimal).076);
			CheckLookup("16312 SR 9", "Snohomish", "98296-9110", LocationSource.ZIP5, 4200, (decimal).076);
			CheckLookup("25 95 th Drive NE", "Lake Stevens", "98258", LocationSource.ZIP5, 4200, (decimal).076);
			CheckLookup("600 Mt. Olympus Dr SW", "Issaquah", "98027", LocationSource.ZIP5, 4014, (decimal).086);
			CheckLookup("721 North Landing Way %23405", "Renton", "98057", LocationSource.ZIP5, 1729, (decimal).09);
			CheckLookup("15515 juanita-wood way ne", "bothell", "98011", LocationSource.ZIP5, 1766, (decimal).09);
			CheckLookup("W Malaga Rd", "Malaga", "98828-9724", LocationSource.ZIP9, 400, (decimal).08);
			CheckLookup("6103 mt tacoma dr sw", "tacoma", "98499", LocationSource.ZIP5, 2700, (decimal).082);
			CheckLookup("781 village way", "Monroe, wa", "98292-2171", LocationSource.ADDRESS, 3116, (decimal).085);
			CheckLookup("1592 AMERICO MIRANDA", "SAN JUAN", "98002-0092", LocationSource.ZIP9, 1702, (decimal).09);

			CheckLookup("3805 s ridgeview dr", "", "99206", LocationSource.ADDRESS, 3213, (decimal).086);
			CheckLookup("112 SEARS RD", "", "98532", LocationSource.ADDRESS, 2100, (decimal).077);
			CheckLookup("6500 MERRILL CREEK PKWY", "", "98203", LocationSource.ADDRESS, 3105, (decimal).086);
			CheckLookup("827 140TH ST SW", "", "98087", LocationSource.ADDRESS, 3131, (decimal).089);
			CheckLookup("2434 lewis river rd", "", "98674", LocationSource.ADDRESS, 800, (decimal).076);
			//CheckLookup("215 BELLA VISTA RD", "KELSO", " 9862", LocationSource.ZIP5, 4100, (decimal).078);

			CheckLookup("19909 Ballinger Way NE", "Lake Forest Park", "98155", LocationSource.ADDRESS, 1737, (decimal).09);

			//CheckLookup("312 S RIEGER RD", "ROSALIA", "WA", LocationSource.ADDRESS, 4100, (decimal).078);
		}

		/// <summary>
		/// Helper for source level debugging of the unit tests
		/// </summary>
		public void _DebugTraceEntryPoint()
		{
			Junk();
			Aberdeen();
			Algorna();
			Allyn();
			AmandaPark();
			Anacortes();
			Arlington();
			Auburn();
			BainbridgeIsland();
			BattleGround();
			Bellevue();
			Bellingham();
			Blaine();
			BonneyLake();
			Bothell();
			Bremerton();
			Brewster();
			Brier();
			BrushPrairie();
			Buckley();
			Burien();
			Burlington();
			Camas();
			Carbonado();
			CastleRock();
			Chehalis();
			Chattaroy();
			Colbert();
			Covington();
			Coupeville();
			Clarkston();
			CleElum();
			Dayton();
			DeerPark();
			Deming();
			//DesMoines();
			Eatonville();
			Edmonds();
			Everett();
			FallCity();
			FederalWay();
			FridayHarbor();
			GigHarbor();
			Graham();
			Greenbank();
			Hoquiam();
			Issaquah();
			Kalama();
			Kennewick();
			Kent();
			Kirkland();
			LaCenter();
			Lacey();
			Lakebay();
			LakeForestPark();
			LakeStevens();
			Lakewood();
			Langley();
			LibertyLake();
			Longview();
			Lynden();
			Lynnwood();
			MapleValley();
			Marysville();
			Mead();
			Medina();
			MercerIsland();
			MillCreek();
			Monroe();
			MountlakeTerrace();
			NoosackEverson();
			NorthBend();
			Morton();
			MosesLake();
			MountVernon();
			Mukilteo();
			Newcastle();
			OakVille();
			Olympia();
			Packwood();
			Pacific();
			PoBox();
			PortOrchard();
			Poulsbo();
			Prosser();
			Puyallup();
			Redmond();
			Renton();
			Ridgefield();
			Roy();
			Sammamish();
			SeaTac();
			Seattle();
			SedroWoolley();
			Shoreline();
			Skyway();
			Snohomish();
			Snoqualmie();
			Spanaway();
			Spokane();
			SpokaneValley();
			Steilacoom();
			Sumner();
			Tacoma();
			Toledo();
			Toutle();
			Tumwater();
			Underwood();
			Union();
			UniversityPlace();
			Vancouver();
			VashonIsland();
			Veradale();
			Washougal();
			Wenatchee();
			Woodinville();
			Woodland();
			Yacolt();
			Yakima();
		}
	}
}
