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
	/// Interface for AddressScan, AddressMemoryIndex, and AddressNormalizer
	/// </summary>
	public interface IAddressLocator
	{
		void ReLoad(string filename, bool useShortcutEval);
		Period Period { get; }
		bool HasData { get; }
		bool Locate(string street, string city, string zip, string plus4, ref AddressLine addr);
		bool Locate(string street, string city, string szip, ref AddressLine addr);
		bool Locate(string street, string city, ZIP zip, ref AddressLine addr);
	}
}
