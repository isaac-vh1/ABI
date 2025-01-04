namespace WaRateFiles
{
	partial class frmConfigure
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.txtZipFileName = new System.Windows.Forms.TextBox();
			this.txtRatesFile = new System.Windows.Forms.TextBox();
			this.txtStreetsFile = new System.Windows.Forms.TextBox();
			this.btnZipFile = new System.Windows.Forms.Button();
			this.btnStreetFile = new System.Windows.Forms.Button();
			this.btnRateFile = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCanx = new System.Windows.Forms.Button();
			this.chkNoShortCut = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(29, 25);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(30, 13);
			this.label7.TabIndex = 0;
			this.label7.Text = "Zips:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(11, 51);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(48, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Address:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(22, 77);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "Rates:";
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog";
			// 
			// txtZipFileName
			// 
			this.txtZipFileName.Location = new System.Drawing.Point(65, 22);
			this.txtZipFileName.MaxLength = 40;
			this.txtZipFileName.Name = "txtZipFileName";
			this.txtZipFileName.Size = new System.Drawing.Size(224, 20);
			this.txtZipFileName.TabIndex = 0;
			this.txtZipFileName.Text = "D:\\GISRawData\\ZIP4RATESQ22008.csv";
			// 
			// txtRatesFile
			// 
			this.txtRatesFile.Location = new System.Drawing.Point(65, 74);
			this.txtRatesFile.MaxLength = 40;
			this.txtRatesFile.Name = "txtRatesFile";
			this.txtRatesFile.Size = new System.Drawing.Size(224, 20);
			this.txtRatesFile.TabIndex = 4;
			this.txtRatesFile.Text = "D:\\GISRawData\\Rates.csv";
			// 
			// txtStreetsFile
			// 
			this.txtStreetsFile.Location = new System.Drawing.Point(65, 48);
			this.txtStreetsFile.MaxLength = 40;
			this.txtStreetsFile.Name = "txtStreetsFile";
			this.txtStreetsFile.Size = new System.Drawing.Size(224, 20);
			this.txtStreetsFile.TabIndex = 2;
			this.txtStreetsFile.Text = "D:\\GISRawData\\State.txt";
			// 
			// btnZipFile
			// 
			this.btnZipFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnZipFile.Location = new System.Drawing.Point(295, 22);
			this.btnZipFile.Name = "btnZipFile";
			this.btnZipFile.Size = new System.Drawing.Size(31, 20);
			this.btnZipFile.TabIndex = 1;
			this.btnZipFile.Text = "...";
			this.btnZipFile.UseVisualStyleBackColor = true;
			this.btnZipFile.Click += new System.EventHandler(this.btnZipFile_Click);
			// 
			// btnStreetFile
			// 
			this.btnStreetFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnStreetFile.Location = new System.Drawing.Point(295, 48);
			this.btnStreetFile.Name = "btnStreetFile";
			this.btnStreetFile.Size = new System.Drawing.Size(31, 20);
			this.btnStreetFile.TabIndex = 3;
			this.btnStreetFile.Text = "...";
			this.btnStreetFile.UseVisualStyleBackColor = true;
			this.btnStreetFile.Click += new System.EventHandler(this.btnStreetFile_Click);
			// 
			// btnRateFile
			// 
			this.btnRateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRateFile.Location = new System.Drawing.Point(295, 73);
			this.btnRateFile.Name = "btnRateFile";
			this.btnRateFile.Size = new System.Drawing.Size(31, 20);
			this.btnRateFile.TabIndex = 5;
			this.btnRateFile.Text = "...";
			this.btnRateFile.UseVisualStyleBackColor = true;
			this.btnRateFile.Click += new System.EventHandler(this.btnRateFile_Click);
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(56, 133);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 6;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCanx
			// 
			this.btnCanx.Location = new System.Drawing.Point(205, 133);
			this.btnCanx.Name = "btnCanx";
			this.btnCanx.Size = new System.Drawing.Size(75, 23);
			this.btnCanx.TabIndex = 7;
			this.btnCanx.Text = "Cancel";
			this.btnCanx.UseVisualStyleBackColor = true;
			this.btnCanx.Click += new System.EventHandler(this.btnCanx_Click);
			// 
			// chkNoShortCut
			// 
			this.chkNoShortCut.AutoSize = true;
			this.chkNoShortCut.Location = new System.Drawing.Point(65, 101);
			this.chkNoShortCut.Name = "chkNoShortCut";
			this.chkNoShortCut.Size = new System.Drawing.Size(141, 17);
			this.chkNoShortCut.TabIndex = 8;
			this.chkNoShortCut.Text = "Use Complete Addr Eval";
			this.chkNoShortCut.UseVisualStyleBackColor = true;
			// 
			// frmConfigure
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(338, 168);
			this.Controls.Add(this.chkNoShortCut);
			this.Controls.Add(this.btnCanx);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnRateFile);
			this.Controls.Add(this.btnStreetFile);
			this.Controls.Add(this.btnZipFile);
			this.Controls.Add(this.txtZipFileName);
			this.Controls.Add(this.txtRatesFile);
			this.Controls.Add(this.txtStreetsFile);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Name = "frmConfigure";
			this.Text = "Select Files";
			this.Load += new System.EventHandler(this.frmConfigure_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.TextBox txtZipFileName;
		private System.Windows.Forms.TextBox txtRatesFile;
		private System.Windows.Forms.TextBox txtStreetsFile;
		private System.Windows.Forms.Button btnZipFile;
		private System.Windows.Forms.Button btnStreetFile;
		private System.Windows.Forms.Button btnRateFile;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCanx;
		private System.Windows.Forms.CheckBox chkNoShortCut;
	}
}