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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using WaRateFiles.Support;

namespace WaRateFiles
{
	public partial class frmMain : Form
	{
		private string m_ratefileName = "";
		private string m_addrfileName = "";
		private string m_zipfileName = "";
		private RateLookup m_lookup = null;
		private bool m_useShortcutEval = false;

		public string RateFileName
		{
			set { m_ratefileName = value; }
		}

		public string AddressFileName
		{
			set { m_addrfileName = value; }
		}

		public string ZipFileName
		{
			set { m_zipfileName = value; }
		}

		public bool UseShortCutEval
		{
			set { m_useShortcutEval = value; }
		}

		public frmMain()
		{
			InitializeComponent();

			FileMaintenance.GetLocalFileBaseNames(Period.CurrentPeriod(), out m_addrfileName, out m_ratefileName, out m_zipfileName);
			m_addrfileName = "./" + m_addrfileName + ".csv";
			m_ratefileName = "./" + m_ratefileName + ".csv";
			m_zipfileName = "./" + m_zipfileName + ".csv";
		}

		private void btnScan_Click(object sender, EventArgs e)
		{
			lblLocCode.Text = "";
			lblRate.Text = "";
			lblMatch.Text = "Searching";

			AddressLine addr;
			Rate rate = null;
			LocationSource locsrc;
			if (m_lookup.FindRate(txtAddr.Text, "", txtZIP.Text, out addr, ref rate, out locsrc))
			{
				lblRate.Text = rate.TotalRate.ToString();
				lblLocCode.Text = rate.LocationCode;
				if (locsrc == LocationSource.ADDRESS)
				{
					lblMatch.Text = "Address";
				}
				else if (locsrc == LocationSource.ZIP9)
				{
					lblMatch.Text = "ZIP+4";
				}
				else if (locsrc == LocationSource.ZIP5)
				{
					lblMatch.Text = "ZIP5";
				}
				else
				{
					lblMatch.Text = "";
					Debug.Assert(false, "Shouldn't happen");
				}
			}
			else
			{
				lblLocCode.Text = "";
				lblRate.Text = "";
				lblMatch.Text = "Not found";
			}
		}

		private void frmMain_Shown(object sender, EventArgs e)
		{
			if (!File.Exists(m_ratefileName))
			{
				MessageBox.Show("Unable to find " + m_ratefileName);
				return;
			}
			if (!File.Exists(m_addrfileName))
			{
				MessageBox.Show("Unable to find " + m_addrfileName);
				return;
			}
			if (!File.Exists(m_zipfileName))
			{
				MessageBox.Show("Unable to find " + m_zipfileName);
				return;
			}
			m_lookup = new RateLookup(m_addrfileName, m_ratefileName, m_zipfileName, RateLookupEngine.STANDARDIZER, m_useShortcutEval);
		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			Application.Exit();
		}
	}
}