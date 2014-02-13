using System;
using System.Configuration;

public partial class InstallHelp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AccountId"]) && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["AccountKey"]))
        {
            Response.Redirect("~/");
        }
    }
}