namespace WaRateFiles
{
	partial class frmMain
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
			this.btnScan = new System.Windows.Forms.Button();
			this.txtAddr = new System.Windows.Forms.TextBox();
			this.txtZIP = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblLocCode = new System.Windows.Forms.Label();
			this.lblRate = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lblMatch = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnScan
			// 
			this.btnScan.Location = new System.Drawing.Point(99, 73);
			this.btnScan.Name = "btnScan";
			this.btnScan.Size = new System.Drawing.Size(75, 23);
			this.btnScan.TabIndex = 0;
			this.btnScan.Text = "Lookup";
			this.btnScan.UseVisualStyleBackColor = true;
			this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
			// 
			// txtAddr
			// 
			this.txtAddr.Location = new System.Drawing.Point(56, 12);
			this.txtAddr.MaxLength = 40;
			this.txtAddr.Name = "txtAddr";
			this.txtAddr.Size = new System.Drawing.Size(217, 20);
			this.txtAddr.TabIndex = 1;
			// 
			// txtZIP
			// 
			this.txtZIP.Location = new System.Drawing.Point(56, 38);
			this.txtZIP.MaxLength = 10;
			this.txtZIP.Name = "txtZIP";
			this.txtZIP.Size = new System.Drawing.Size(80, 20);
			this.txtZIP.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(81, 114);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Location Code:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(127, 135);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(33, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Rate:";
			// 
			// lblLocCode
			// 
			this.lblLocCode.AutoSize = true;
			this.lblLocCode.Location = new System.Drawing.Point(167, 114);
			this.lblLocCode.Name = "lblLocCode";
			this.lblLocCode.Size = new System.Drawing.Size(31, 13);
			this.lblLocCode.TabIndex = 5;
			this.lblLocCode.Text = "0000";
			// 
			// lblRate
			// 
			this.lblRate.AutoSize = true;
			this.lblRate.Location = new System.Drawing.Point(166, 135);
			this.lblRate.Name = "lblRate";
			this.lblRate.Size = new System.Drawing.Size(31, 13);
			this.lblRate.TabIndex = 6;
			this.lblRate.Text = "X.XX";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Street:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(23, 42);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(27, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "ZIP:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(93, 156);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(67, 13);
			this.label8.TabIndex = 15;
			this.label8.Text = "Match Type:";
			// 
			// lblMatch
			// 
			this.lblMatch.AutoSize = true;
			this.lblMatch.Location = new System.Drawing.Point(167, 156);
			this.lblMatch.Name = "lblMatch";
			this.lblMatch.Size = new System.Drawing.Size(56, 13);
			this.lblMatch.TabIndex = 16;
			this.lblMatch.Text = "XXXXXXX";
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(285, 184);
			this.Controls.Add(this.lblMatch);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblRate);
			this.Controls.Add(this.lblLocCode);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtZIP);
			this.Controls.Add(this.txtAddr);
			this.Controls.Add(this.btnScan);
			this.Name = "frmMain";
			this.Text = "Lookup Rates";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
			this.Shown += new System.EventHandler(this.frmMain_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnScan;
		private System.Windows.Forms.TextBox txtAddr;
		private System.Windows.Forms.TextBox txtZIP;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblLocCode;
		private System.Windows.Forms.Label lblRate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lblMatch;
	}
}

