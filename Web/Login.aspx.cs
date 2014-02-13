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

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack && Request.UrlReferrer != null)
            ViewState["Referrer"] = Request.UrlReferrer.ToString();
    }
    //test
    protected void Login_LoggedIn(object sender, EventArgs e)
    {
        if (ViewState["Referrer"] != null)
            Response.Redirect(ViewState["Referrer"].ToString());
    }
}
