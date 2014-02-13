using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace VideoShow.UserControls
{
    public partial class Login : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Login from homepage should send user to their member page.
            string pagename = System.IO.Path.GetFileName(Request.ServerVariables["SCRIPT_NAME"]);
            if(pagename == "Default.aspx")
            {
                this.LoginControl.DestinationPageUrl = "~/Member.aspx";
            }
        }

        //Since the login control is hidden by default, we need to expressly show it in event of a login failure.
        protected void Login_LoginError(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(
                this,
                this.GetType(), 
                "showLoginCss",
                "<style> DIV#sign-in-panel {filter: Alpha(opacity=100); opacity: 1; top: 30px;</style>",
                false);
        }
    }
}