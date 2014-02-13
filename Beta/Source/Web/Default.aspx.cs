using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using VideoShow.Data;
using VideoShow.UserControls;

public partial class _Default : System.Web.UI.Page
{
    List<string> popularTags;

    protected void Page_Load(object sender, EventArgs e)
    {
        MembershipUser user = Membership.GetUser();
        if (!IsPostBack)
        {
            //Check for new install
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AccountId"]) || string.IsNullOrEmpty(ConfigurationManager.AppSettings["AccountKey"]))
            {
                Response.Redirect("~/InstallHelp.aspx");
            }

            if (user != null)
            {
                MainMenu.Items.AddAt(3, new MenuItem("My Page", null, null, "~/Member.aspx?memberid=" + user.ProviderUserKey.ToString()));
                MainMenu.Items.AddAt(4, new MenuItem("My Recent Views", null, null, "~/RecentViews.aspx?memberid=" + user.ProviderUserKey.ToString()));
                MainMenu.Items.AddAt(5, new MenuItem("Upload a Video", null, null, "~/Upload.aspx"));
            }
        }
        using (VideoShowDataContext DataContext = DataContextFactory.DataContext())
        {
            VideoShow.Data.DataServices.TagServices tagServices = new VideoShow.Data.DataServices.TagServices();
            popularTags = tagServices.GetPopularTags(20);
            if (popularTags.Count == 0)
            {
                TagCloud.Visible = false;
                PageHeading.Visible = false;
            }
        }
        HomeVideoGrid.ConfigurationType = VideoGrid.ConfigurationTypes.Home;
    }

    protected void Search_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(SearchTags.Text))
            Response.Redirect(string.Format("~/Tags.aspx?Tag={0}", SearchTags.Text));
    }

    protected string GetTagCloud()
    {
        string tagCloud = string.Empty;
        string tagPage = Page.ResolveUrl("~/Tags.aspx");
        foreach (string tag in popularTags)
        {
            tagCloud += string.Format("<a href='{0}?Tag={1}'>{2}</a> ", tagPage, tag, tag);
        }
        return tagCloud;
    }
}

