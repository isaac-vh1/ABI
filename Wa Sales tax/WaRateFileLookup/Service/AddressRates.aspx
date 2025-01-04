<%@ Page Language="C#" EnableSessionState="false" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Net.Sockets" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="DOR.Efile" %>
<%@ Import Namespace="DOR.Web" %><script runat="server">
	protected void Page_Load(object sender, EventArgs e)
	{
		Page.Controls.Clear();
		
		string format = Request["output"];
		string addr = Request["addr"];
		string city = Request["city"];
		string zip = Request["zip"];

		if (null == format &&
			null == addr &&
			null == city &&
			null == zip)
		{
			string html = File.ReadAllText(Server.MapPath("~/AddressRates.txt"));
			html = html.Replace("dorwebgis2", "dor.wa.gov");
			html = html.Replace("8080", "80");
			Response.Write(html);
			return;
		}
		
		if (null == format)
		{
			Response.Write("LocationCode=-1 Rate=-1 ResultCode=4 debughint=output parameter missing");
			return;
		}
		if ("xml" == format)
		{
			Response.ContentType = "text/xml";
		}
		else if ("stats" == format)
		{
			CallStatsService();
			return;
		}
		
		if (null == addr ||
			null == city ||
			null == zip)
		{
			SendErrorResponse(format, "4", "missing parameter.  addr, city, and zip must be present." );
			return;
		}

		CallSourceCodeService(format, addr, city, zip);
	}

	private void SendErrorResponse(string format, string code, string msg)
	{
		if (format == "xml")
		{
			Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
				"<response loccode=\"-1\" localrate=\"-1\" rate=\"-1\" code=\"" + code + "\" debughint=\"" + msg + "\" />");
		}
		else
		{
			Response.Write("LocationCode=-1 Rate=-1 ResultCode=" + code + " debughint=" + msg);
		}
	}

	private void CallStatsService()
	{
		string urlbase = ConfigurationManager.AppSettings["RATE_SERVICE_SC_URL"];
		if (null == urlbase)
		{
			DOR.Web.Context.LogError("Set RATE_SERVICE_SC_URL in the web.config");
			SendErrorResponse("text", "5", "RATE_SERVICE_SC_URL configuration setting missing on the server.");
			return;
		}

		WebClient wc = new WebClient();
		WebRequest.DefaultWebProxy;
		string uri = urlbase + "/stats";
		StreamReader reader = new StreamReader(wc.OpenRead(uri));
		string resp = reader.ReadToEnd();
		reader.Close();
		reader.Dispose();

		Response.Write(resp);
	}
                     		
	private void CallSourceCodeService
	(
		string format,
		string addr,
		string city,
		string zip
	)
	{
		string urlbase = ConfigurationManager.AppSettings["RATE_SERVICE_SC_URL"];
		if (null == urlbase)
		{
			DOR.Web.Context.LogError("Set RATE_SERVICE_SC_URL in the web.config");
			SendErrorResponse(format, "5", "RATE_SERVICE_SC_URL configuration setting missing on the server.");
			return;
		}

		WebClient wc = new WebClient();
		WebRequest.DefaultWebProxy;
		string uri = urlbase + "/" + format + "?addr=" + UrlEncode(addr) + "&city=" + UrlEncode(city) + "&zip=" + UrlEncode(zip);
		StreamReader reader = new StreamReader(wc.OpenRead(uri));
		string resp = reader.ReadToEnd();
		reader.Close();
		reader.Dispose();

		Response.Write(resp);
	}

	/// <summary>Characters that must be URL encoded.</summary>
	private static char[] m_urlchars = new char[] { '&', '%', '#', '?' };

	/// <summary>
	/// For portability and avoid reference to System.web.  May not be 100% complete.
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	private string UrlEncode(string str)
	{
		if (str.IndexOfAny(m_urlchars) < 0)
		{
			return str;
		}
		StringBuilder buf = new StringBuilder();
		for (int x = 0; x < str.Length; x++)
		{
			switch (str[x])
			{
				case '&':
					buf.Append("%26");
					break;
				case '%':
					buf.Append("%25");
					break;
				case '#':
					buf.Append("%23");
					break;
				case '?':
					buf.Append("%3f");
					break;
				default:
					buf.Append(str[x]);
					break;
			}
		}
		return buf.ToString();
	}
</script>