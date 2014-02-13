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
using VideoShow.Data;
using VideoShow.UserControls;

public partial class MemberList : System.Web.UI.Page
{
    private int gridIndex;

    enum PageMode { AllMembers, WithVideos, NotSet }
    PageMode DefaultPageMode = PageMode.AllMembers ;
    private PageMode currentMode = PageMode.NotSet;

    private VideoShowDataContext dataContext = DataContextFactory.DataContext();
    private VideoShowDataContext DataContext
    {
        get { return dataContext; }
    }

    private PageMode CurrentMode
    {
        get
        {
            if(currentMode == PageMode.NotSet)
            {
                string mode = Request.QueryString["pageMode"];
                if (!string.IsNullOrEmpty(mode))
                {
                    currentMode = (PageMode)Enum.Parse(typeof(PageMode), Request.QueryString["pageMode"]);
                }
                else
                    currentMode = DefaultPageMode;
            }
            return currentMode;
        }
        set
        {
            currentMode = value;
        }
    }

    private int membersPerPage
    {
        get
        {
            if (CurrentMode == PageMode.WithVideos)
                return 4;
            else
                return 20;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (CurrentMode == PageMode.AllMembers)
        {
            PanelAllMembers.Visible = true;
            PanelMembersWithVideos.Visible = false;
        }
        else
        {
            PanelAllMembers.Visible = false;
            PanelMembersWithVideos.Visible = true;
        }

        if (!this.IsPostBack)
        {
            GetMembers(0);
        }
    }

    protected int AllMembersCount()
    {
        int count = (from users in DataContext.aspnet_Users 
                     select users).Count();
        return count;
    }

    protected int WithVideosCount()
    {
        int count = (from users in DataContext.aspnet_Users
                    where users.Videos.Count > 0
                    select users).Count();
        return count;
    }

    protected void next_Click(object sender, EventArgs e)
    {
        GetMembers(Int32.Parse(CurrentPage.Value) + 1);
    }

    protected void previous_Click(object sender, EventArgs e)
    {
        GetMembers(Int32.Parse(CurrentPage.Value) - 1);
    }

    private void GetMembers(int currentPage)
    {
        ListView output;
        IQueryable<aspnet_User> userInfo;
        if (CurrentMode == PageMode.WithVideos)
        {
            userInfo = from user in DataContext.aspnet_Users
                       where user.Videos.Count > 0
                       orderby user.Videos.Count descending
                       select user;
            output = MemberRepeater;
        }
        else
        {
            userInfo = 
                from user in DataContext.aspnet_Users 
                orderby user.Videos.Count descending, user.UserName ascending
                select user;
            output = MemberListView;
        }

        int recordCount = userInfo.Count();
        userInfo = userInfo.Skip(currentPage * membersPerPage).Take(membersPerPage);

        CurrentPage.Value = currentPage.ToString();
        PreviousButton.Visible = currentPage > 0;
        NextButton.Visible = recordCount / membersPerPage > currentPage; 

        output.DataSource = userInfo;
        output.DataBind();
    }

    protected string GetAvatarUrl(string userId)
    {
        var query = (from uta in DataContext.UsersToAvatars
                     join avatars in DataContext.Avatars on uta.AvatarId equals avatars.AvatarId
                     where uta.UserId == new Guid(userId)
                     select avatars.ImageUrl).SingleOrDefault();
        return query;
    }

    protected void MemberRepeater_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        ListViewDataItem currentItem = (ListViewDataItem)e.Item;
        VideoGrid videoGrid = (VideoGrid)e.Item.FindControl("MemberVideoGrid");
        videoGrid.GridName = "MemberVideoGrid_" + gridIndex++;
        videoGrid.ConfigurationType = VideoGrid.ConfigurationTypes.Playlist;
        videoGrid.ViewType = VideoGrid.ViewTypes.Owner;
        videoGrid.FilterValue = ((Guid)DataBinder.Eval(currentItem.DataItem, "UserId")).ToString();
    }

    //protected string GetGravatarUrl(object dataItem)
    //{
    //    string email = (string)DataBinder.Eval(dataItem, "email");
    //    string hash = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(email.Trim(), "MD5");
    //    hash = hash.Trim().ToLower();

    //    //string defaultImage = Page.ResolveUrl("~/Images/avatar.png");
    //    string defaultImage = HttpUtility.UrlEncode("http://silverlight.net/utility/anonymous.gif");
    //    //TODO:Include a default image. Querystring parameter example: &default=http%3A%2F%2Fwww.example.com%2Fsomeimage.jpg
    //    string gravatarUrl = string.Format("http://www.gravatar.com/avatar.php?gravatar_id={0}&rating=G&size=65&default={1}",hash,defaultImage);
    //    return gravatarUrl;
    //}
}
