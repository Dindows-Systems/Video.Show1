using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Configuration;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Check for new install
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AccountId"]) || string.IsNullOrEmpty(ConfigurationManager.AppSettings["AccountKey"]))
            {
                string pagename = System.IO.Path.GetFileName(Request.ServerVariables["SCRIPT_NAME"]);
                if (pagename != "InstallHelp.aspx")
                {
                    Response.Redirect("~/InstallHelp.aspx");
                }
            }

            MembershipUser user = Membership.GetUser();
            if (user != null)
            {
                MainMenu.Items.AddAt(3, new MenuItem("My Page", null, null, "~/Member.aspx?memberid=" + user.ProviderUserKey.ToString()));
                MainMenu.Items.AddAt(4, new MenuItem("My Recent Views", null, null, "~/RecentViews.aspx?memberid=" + user.ProviderUserKey.ToString()));
                MainMenu.Items.AddAt(5, new MenuItem("Upload a Video", null, null, "~/Upload.aspx"));
            }
        }
    }
    protected void Search_Click(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(SearchTags.Text))
            Response.Redirect(string.Format("~/Tags.aspx?Tag={0}", SearchTags.Text));
    }
    public string SearchText
    {
        get { return SearchTags.Text; }
        set { SearchTags.Text = value; }
    }

}
