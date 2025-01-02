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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text;

using WaRateFiles.Support;
using WaRateFiles;

namespace RateBatchJob
{
	public class RateJob
	{
		private string m_datafile;
		private string m_emailfile;
		private string m_filename;
		private string m_email;
		private LogFile m_controlReport;

		public string EmailAddress
		{
			get { return m_email; }
			set { m_email = value; }
		}

		public string DataFile
		{
			get { return m_datafile; }
			set { m_datafile = value; }
		}

		public string FileName
		{
			get { return m_filename; }
			set { m_filename = value; }
		}

		public string EmailFile
		{
			get { return m_emailfile; }
			set { m_emailfile = value; }
		}

		public string IncomingDirectory
		{
			get { return StringHelper.EnsureTrailingChar(ConfigurationManager.AppSettings["Incoming_File_Directory"].ToString(), '\\'); }
		}

		public string OutgoingDirectory
		{
			get { return StringHelper.EnsureTrailingChar(ConfigurationManager.AppSettings["Outgoing_File_Directory"].ToString(), '\\'); }
		}

		public void Run()
		{
			m_controlReport = new LogFile("CADS_CTL.txt");

			if (File.Exists("AddressRateLoad_Running.txt"))
			{
				m_controlReport.WriteLog("RateJob.Run", "ERROR: Stop file found, aborting");
				return;
			}

			RateLookup gis;
			if ( ConfigurationManager.AppSettings["USE_NTSERVICE"] == "true" )
			{
				string server = ConfigurationManager.AppSettings["NTSERVICE_SERVER"];
				string sport = ConfigurationManager.AppSettings["NTSERIVCE_PORT"];
				if ( null == server || server == "" )
				{
					m_controlReport.WriteLog("RateJob.Run", "Set NTSERVICE_SERVER key in the app.config, or set USE_NTSERVICE to false.");
					return;
				}
				if ( sport == null || sport == "" )
				{
					m_controlReport.WriteLog("RateJob.Run", "Set NTSERIVCE_PORT key in the app.config, or set USE_NTSERVICE to false.");
					return;
				}
				if ( !StringHelper.IsInt(sport) )
				{
					m_controlReport.WriteLog("RateJob.Run", "NTSERIVCE_PORT key in the app.config must be an integer.");
					return;
				}
				gis = new RateLookup("http", server, Int32.Parse(sport));
			}
			else
			{
				string addrFileName = ConfigurationManager.AppSettings["ADDRESS_FILE"];
				if (null == addrFileName)
				{
					throw new ConfigurationErrorsException("Set ADDRESS_FILE in the app.config file");
				}
				if (!File.Exists(addrFileName))
				{
					throw new FileNotFoundException(addrFileName + " not found");
				}
	
				string rateFileName = ConfigurationManager.AppSettings["RATE_FILE"];
				if (null == rateFileName)
				{
					throw new ConfigurationErrorsException("Set RATE_FILE in the app.config file");
				}
				if (!File.Exists(rateFileName))
				{
					throw new FileNotFoundException(rateFileName + " not found");
				}
	
				string zipFileName = ConfigurationManager.AppSettings["ZIP_FILE"];
				if (null == zipFileName)
				{
					throw new ConfigurationErrorsException("Set ZIP_FILE in the app.config file");
				}
				if (!File.Exists(zipFileName))
				{
					throw new FileNotFoundException(zipFileName + " not found");
				}
				gis = new RateLookup(addrFileName, rateFileName, zipFileName, RateLookupEngine.STANDARDIZER, false);
			}

			File.WriteAllText("AddressRateLoad_Running.txt", DateTime.Now.ToString());

			DataFile = "";
			EmailFile = "";
			EmailAddress = "";
			int errorCount = 0;

			try
			{
				string[] files = Directory.GetFiles(IncomingDirectory, "*.email");
				string file = "";

				if (files.Length > 0)
				{
					for (int i = 0; i < files.Length; i++)
					{
						file = files[i].Trim();
						Debug.Assert(file.EndsWith(".email"));

						EmailFile = file;
						DataFile = file.Replace(".email", ".txt");

						if (!File.Exists(DataFile))
						{
							DataFile = file.Replace(".email", ".csv");
						}
						if (!File.Exists(DataFile))
						{
							m_controlReport.WriteLog("RateJob.Run", "ERROR: Data file not found for corresponding Email file.");
							continue;
						}
						if (EmailFile != "" && DataFile != "")
						{
							// Extract Email from flat file
							StreamReader readerEmail = new StreamReader(EmailFile);
							EmailAddress = readerEmail.ReadLine().Trim();
							FileName = readerEmail.ReadLine().Trim();
							readerEmail.Close();

							if (EmailAddress.Trim() != "" && EmailAddress != null)
							{
								if (!ProcessFiles(gis, EmailFile, DataFile))
								{
									errorCount++;
								}
							}
							else
							{
								m_controlReport.WriteLog("RateJob.Run", "ERROR: No email found for file: " + DataFile);
							}

							// Reset file variables for next files on stack
							DataFile = "";
							EmailFile = "";
						}
					}
					SendEmail("cms@dor.wa.gov", ConfigurationManager.AppSettings["TAA_CONTACT_EMAIL"], "CADS: " + files.Length.ToString() + " files processed", DateTime.Now.ToString());
				}
				else
				{
					// no files
					if (files.Length == 0)
					{
						m_controlReport.WriteLog("RateJob.Run", "INFO: No files to process for " + DateTime.Now.ToShortDateString());
					}
					else
					{
						m_controlReport.WriteLog("RateJob.Run", "ERROR: Missing corresponding file: email or data file is missing");
					}
				}
				PurgeOldFiles();
			}
			catch (Exception ex)
			{
				m_controlReport.WriteLog("RateJob.Run", ex);
			}
			finally
			{
				m_controlReport.WriteLog("RateJob.Run", "Run complete at " + DateTime.Now.ToString());
				if (errorCount != 0)
				{
					//DOR.Batch.Mailer.SendMailWithAttachment("Customer Database Load", ConfigurationManager.AppSettings["ERROR_RECIPIENTS"], "Efile_Batch@dor.wa.gov", "CADS ERRORS", "The Customer Database Load had an error. Please see exception report", controlReport.ExceptionReport.Filename);
				}
			}
			File.Delete("AddressRateLoad_Running.txt");
		}

		private static void AddErrorRow(DelimitedFile errors, List<string> row, string message)
		{
			row.Add("");
			row.Add("");
			row.Add(message);
			errors.AddRow(row);
		}

		private bool ProcessFiles(RateLookup gis, string emailfile, string datafile)
		{
			DateTime startTime = DateTime.Now;
			int counter = 0;
			int lineCount = 0;

			string baseFileName = datafile.Substring(datafile.LastIndexOf('\\') + 1);
			baseFileName = baseFileName.Substring(0, baseFileName.Length - 4);

			DelimitedFile input;
			DelimitedFile errors;

			if (datafile.EndsWith(".txt"))
			{
				input = new DelimitedFile('\t', datafile);
			}
			else
			{
				input = new DelimitedFile(',', datafile);
			}
			errors = new DelimitedFile(input.Delimiter, input.ColumnCount + 1);
			input.InsertRowAt(0);
			input.Rows[0].Insert(0, "#The location codes and tax rates are valid from  " + Period.CurrentPeriod().StartDate.ToString() + " through " + Period.CurrentPeriod().EndDate.ToString());
			counter++;

			for (int row = 1; row < input.Rows.Count; row++)
			{
				bool addressGeoCodeOk = false;

				if (input.Rows[row].Count > 0 && input.Rows[row][0].Length > 0 && input.Rows[row][0][0] == '#')
				{
					// comment line
					counter++;
					continue;
				}

				if (input.Rows[row].Count != 5 && input.Rows[row].Count != 6)
				{
					AddErrorRow(errors, input.Rows[row], "Invalid field count for row.");
					counter++;
					continue;
				}

				string street = input.Rows[row][1].Trim();
				if (street.Length == 0)
				{
					AddErrorRow(errors, input.Rows[row], "No street address.");
					counter++;
					continue;
				}
				string city = input.Rows[row][2].Trim();
				string state = input.Rows[row][3].Trim().ToUpper();
				string szip4 = null;
				string szip = input.Rows[row][4].Trim();

				// Ensure the state is above the ZIP, in case of CA address.
				if (state != "WA")
				{
					AddErrorRow(errors, input.Rows[row], "Out of state");
					counter++;
					continue;
				}

				if (input.Rows[row].Count == 6)
				{
					szip4 = input.Rows[row][5].Trim();
					if (!ZIP.IsZip(szip, szip4))
					{
						AddErrorRow(errors, input.Rows[row], "Invalid zip code");
						counter++;
						continue;
					}
				}
				else
				{
					if (!ZIP.IsZip(szip))
					{
						AddErrorRow(errors, input.Rows[row], "Invalid zip code");
						counter++;
						continue;
					}
				}

				AddressLine addr;
				Rate rate = null;
				LocationSource loctyp;

				if (null != szip4 && "" != szip4)
				{
					addressGeoCodeOk = gis.FindRate(street, city, ZIP.Parse(szip, szip4), out addr, ref rate, out loctyp);
				}
				else
				{
					addressGeoCodeOk = gis.FindRate(street, city, ZIP.Parse(szip), out addr, ref rate, out loctyp);
				}
				if (loctyp != LocationSource.ADDRESS && loctyp != LocationSource.ZIP9)
				{
					AddErrorRow(errors, input.Rows[row], "Address not found.");
					counter++;
					continue;
				}

				if (addressGeoCodeOk)
				{
					input.Rows[row].Add(rate.LocationCode);
					input.Rows[row].Add(rate.TotalRate.ToString());
					counter++;
				}
				else
				{
					AddErrorRow(errors, input.Rows[row], "Unable to locate address.");
					counter++;
				}
			}

			string filenoext = datafile.Remove(datafile.Length - 4, 4);
			string errorfile = datafile.Remove(datafile.Length - 4, 4);
			string outputdatafilename = "Results" + filenoext.Substring(filenoext.LastIndexOf("\\") + 1);
			string outputerrorfilename = "Errors" + errorfile.Substring(errorfile.LastIndexOf("\\") + 1);

			if (datafile.EndsWith(".txt"))
			{
				filenoext = OutgoingDirectory + outputdatafilename + ".txt";
				errorfile = OutgoingDirectory + outputerrorfilename + ".txt";
			}
			else
			{
				filenoext = OutgoingDirectory + outputdatafilename + ".csv";
				errorfile = OutgoingDirectory + outputerrorfilename + ".csv";
			}

			// Check to see if we missed any records
			lineCount = input.Rows.Count;

			if (!lineCount.Equals(counter))
			{
				m_controlReport.WriteLog("RateJob.Run", "Record count is off, please review file:" + datafile);
				return false;
			}
			File.Delete(filenoext);
			input.Write(filenoext);
			if (errors.Count > 0)
			{
				File.Delete(errorfile);
				errors.Write(errorfile);
			}

			if (!SendEmailNotice(baseFileName))
			{
				File.Delete(filenoext);
				File.Delete(errorfile);
				return false;
			}

			File.Delete(datafile);
			File.Move(emailfile, OutgoingDirectory + emailfile.Substring(emailfile.LastIndexOf("\\") + 1) + ".txt");

			m_controlReport.WriteLog("RateJob.Run", baseFileName + " processed " + input.Rows.Count + " rows, " + errors.Rows.Count + " error rows start=" + startTime + " end=" + DateTime.Now.ToString());
			return true;
		}

		private bool SendEmailNotice(string baseFileName)
		{
			// If we have a valid email address continue, 
			if (EmailAddress == null || EmailAddress.Trim() == "")
			{
				throw new Exception("Email address missing");
			}

			StringBuilder body = new StringBuilder();
			body.Append("<font face='verdana'>Your customer address database is ready!  The location codes and tax rates provided are valid between " + Period.CurrentPeriod().StartDate.ToString() + " through " + Period.CurrentPeriod().EndDate.ToString() + ". </p>")
				.Append("<strong>Location codes and tax rates change quarterly</strong></p>")
				.Append("Tax rate and location codes can change on a quarterly basis. It is your responsibility to make sure your database has the current tax rates and location codes.</p>")
				.Append("To ensure that your database has the most up-to-date information: ")
				.Append("<ol type='square'><li>Refer to the <a href='http://dor.wa.gov/Content/GetAFormOrPublication/FormBySubject/forms_sale.aspx'>Local Sales & Use Tax Rates & Changes publication</a> to update your database with the current location codes and tax rates, or</li>")
				.Append("<li>Submit your database at the beginning of every quarter. The quarters begin on the following dates: </li>")
				.Append("<ol type='circle'>")
				.Append("<li>Quarter 1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;January 1</li>")
				.Append("<li>Quarter 2&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Apri1 1</li>")
				.Append("<li>Quarter 3&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;July 1</li>")
				.Append("<li>Quarter 4&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;October 1</li>")
				.Append("</ol></ol></p>")
				.Append("<strong>What you'll get</strong></p>")
				.Append("There are two files for you to download")
				.Append("<ol type='disc'><li>The results file contains the original data with results for each record. Location code and tax rate will be listed if found.</li>")
				.Append("<li>To help you identify the addresses not found, the error file contains only those addresses not found, along with an explanation of why. Here are few tips to correct your address data: </li>")
				.Append("<ol type='disc'><li>Verify that the address is correct including the zip code. The <a href='http://zip4.usps.com/zip4/welcome.jsp'>USPS</a> can help you verify the address.</li>")
				.Append("<li>Remove any % or # characters from the address.</li>")
				.Append("<li>Use the Department's <a href='http://gis.dor.wa.gov/content/findtaxesandrates/salesandusetaxrates/lookupataxrate/'>Tax Rate Lookup Tool</a>.</li></ol></ol></p>")
				.Append("<a href='")
				.Append(ConfigurationManager.AppSettings["WEB_APP_SERVER"])
				.Append("/SecureForms/Content/FindTaxesAndRates/RetailSalesTax/DestinationBased/AddressFileDownload.aspx?id=")
				.Append(baseFileName)
				.Append("'><strong>Retrieve your customer address files</strong></a></p>")
				.Append("<strong>Questions?</strong></p>")
				.Append("Contact ")
				.Append(ConfigurationManager.AppSettings["TAA_CONTACT"])
				.Append(" at ")
				.Append(ConfigurationManager.AppSettings["TAA_CONTACT_PHONE"])
				.Append(", Monday - Friday 9:00 a.m. - 4:00 p.m. PST.</font></p>")
				.Append("<font face='verdana' size='-1'>This message contains information that may be confidential and privileged. Unless you are the addressee (or authorized to receive for the addressee), you may not use, copy or disclose to anyone the message or any information contained in the message.  If you received the message in error, please e-mail <a href=mailto:" + ConfigurationManager.AppSettings["TAA_CONTACT_EMAIL"] + ">" + ConfigurationManager.AppSettings["TAA_CONTACT"] + "</a> and delete this message.  Thank you.</font>");

			return SendEmail("cms@dor.wa.gov", EmailAddress, "Your Address Rates File Is Ready", body.ToString());
		}

		private bool SendEmail(string from, string to, string subj, string body)
		{
			MailMessage email = new MailMessage(new MailAddress(from), new MailAddress(to));
			email.IsBodyHtml = true;
			email.Priority = MailPriority.Normal;

			email.Subject = subj;
			email.Body = body;

			SmtpClient smtp = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["PATH_SMTP_SERVER"]);

			try
			{
				smtp.Send(email);
				return true;
			}
			catch (Exception exp)
			{
				m_controlReport.WriteLog("SendEmail", exp);
			}
			return false;
		}

		/// <summary>
		/// Delete stored files that are 5 days past the Quarter's end date pertaining to the file
		/// </summary>
		private void PurgeOldFiles()
		{
			//object[] m_files = Directory.GetFiles(OutgoingDirectory);
			//for (int i = 0; i < m_files.Length; i++)
			//{
			//    FileInfo m_info = new FileInfo((string)m_files[i]);
			//    if (m_info.CreationTime.CompareTo(Period.CurrentPeriod().EndDate.ToDateTime().AddDays(5)) > 0)
			//    {
			//        File.Delete(m_files[i].ToString());
			//    }
			//}
		}
	}
}
