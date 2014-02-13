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

namespace VideoShow
{
    public partial class LoginRedirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string username = Request.QueryString["username"];
            string password = Request.QueryString["password"];

            if (Membership.ValidateUser(username, password))
            {
                if (Request.QueryString["redirect"] != null)
                {
                    FormsAuthentication.RedirectFromLoginPage(username, false);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(username, false);
                }
            }
        }
    }
}
