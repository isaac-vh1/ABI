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
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WaRateFiles
{
	public partial class frmConfigure : Form
	{
		private const string m_configpath = "_filepaths.txt";

		public frmConfigure()
		{
			InitializeComponent();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (!File.Exists(txtRatesFile.Text))
			{
				MessageBox.Show("Unable to find " + txtRatesFile.Text);
				return;
			}
			if (!File.Exists(txtStreetsFile.Text))
			{
				MessageBox.Show("Unable to find " + txtStreetsFile.Text);
				return;
			}
			if (!File.Exists(txtZipFileName.Text))
			{
				MessageBox.Show("Unable to find " + txtZipFileName.Text);
				return;
			}
			btnOk.Enabled = false;

			if (File.Exists(m_configpath))
			{
				File.Delete(m_configpath);
			}
			File.WriteAllText(m_configpath, txtRatesFile.Text + "\n" + txtStreetsFile.Text + "\n" + txtZipFileName.Text + "\n");

			frmMain main = new frmMain();
			main.RateFileName = txtRatesFile.Text;
			main.AddressFileName = txtStreetsFile.Text;
			main.ZipFileName = txtZipFileName.Text;
			main.UseShortCutEval = !chkNoShortCut.Checked;
			main.Show();
			this.Hide();
		}

		private void btnCanx_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void btnZipFile_Click(object sender, EventArgs e)
		{
			openFileDialog.DefaultExt = "csv";
			openFileDialog.Filter = "CSV|*.csv";

			if (Path.DirectorySeparatorChar != '\\' || txtZipFileName.Text.IndexOf('/') < 0)
			{
				openFileDialog.FileName = txtZipFileName.Text;
			}
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txtZipFileName.Text = openFileDialog.FileName;
			}
		}

		private void btnStreetFile_Click(object sender, EventArgs e)
		{
			openFileDialog.DefaultExt = "txt";
			openFileDialog.Filter = "TXT|*.txt";
			if (Path.DirectorySeparatorChar != '\\' || txtStreetsFile.Text.IndexOf('/') < 0)
			{
				openFileDialog.FileName = txtStreetsFile.Text;
			}
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txtStreetsFile.Text = openFileDialog.FileName;
			}
		}

		private void btnRateFile_Click(object sender, EventArgs e)
		{
			openFileDialog.DefaultExt = "csv";
			openFileDialog.Filter = "CSV|*.csv";
			if (Path.DirectorySeparatorChar != '\\' || txtRatesFile.Text.IndexOf('/') < 0)
			{
				openFileDialog.FileName = txtRatesFile.Text;
			}
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txtRatesFile.Text = openFileDialog.FileName;
			}
		}

		private void frmConfigure_Load(object sender, EventArgs e)
		{
			if (File.Exists(m_configpath))
			{
				string[] lines = File.ReadAllLines(m_configpath);
				if (lines.Length == 3)
				{
					txtRatesFile.Text = lines[0];
					txtStreetsFile.Text = lines[1];
					txtZipFileName.Text = lines[2];
				}
			}
		}
	}
}