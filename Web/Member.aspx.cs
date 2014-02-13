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
using VideoShow.UserControls;
using VideoShow.Data;
using System.Collections.Specialized;
using VideoShow.Data.DataServices;

namespace VideoShow
{
    public partial class Member : System.Web.UI.Page
    {
        protected Guid? memberId = null;
        protected VideoGrid.ViewTypes viewType = VideoGrid.ViewTypes.NotSet;

        private VideoShowDataContext DataContext
        {
            get { return DataContextFactory.DataContext(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (MemberId == null)
                    Response.Redirect("~/");

                UserProfile profile = UserProfile.GetUserProfile(GetUserName());
                AddUserInformationItem(profile.FullName, "Full Name");
                AddUserInformationItem(profile.Location, "Location");
                if (!string.IsNullOrEmpty(profile.FavoriteMovie))
                {
                    //Note - can link to IMDB with this: http://www.imdb.com/find?q=
                    AddUserInformationItem(
                        string.Format(
                            "<a target='_blank' href='http://movies.msn.com/search/movie/default.aspx?ss={0}'>{0}</a>",
                            profile.FavoriteMovie), 
                        "Favorite Movie");
                }
            }
            MemberVideoGrid.ConfigurationType = VideoGrid.ConfigurationTypes.Grid;
            MemberVideoGrid.ViewType = ViewType;
            MemberVideoGrid.FilterValue = MemberId.ToString();
            if (MemberId != LoggedInMemberId)
            {
                ShowUpload.Visible = false;
                EditAccountLinkContainer.Visible = false;
            }
            else
                MemberVideoGrid.IsMyPage = true;
        }

        private void AddUserInformationItem(string value, string title)
        {
            if (!string.IsNullOrEmpty(value))
                UserInformationList.InnerHtml += string.Format("<li>{0}: {1}</li>", title, value);
        }

        protected Guid? MemberId
        {
            get
            {
                if (memberId == null)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["memberid"]))
                        memberId = new Guid(Request.QueryString["memberid"]);
                    else
                        memberId = LoggedInMemberId;
                }
                return memberId;
            }
        }

        protected VideoGrid.ViewTypes ViewType
        {
            get
            {
                if (viewType == VideoGrid.ViewTypes.NotSet)
                {
                    string view = Request.QueryString["view"];
                    if (!string.IsNullOrEmpty(view))
                        try
                        {
                            viewType = (VideoGrid.ViewTypes)Enum.Parse(typeof(VideoGrid.ViewTypes), view, true);
                        }
                        catch
                        {
                            viewType = VideoGrid.ViewTypes.Owner;
                        }
                    else
                        viewType = VideoGrid.ViewTypes.Owner;
                }
                return viewType;
            }
        }
        
        protected Guid? LoggedInMemberId
        {
            get
            {
                if (Membership.GetUser() != null && Membership.GetUser().ProviderUserKey != null)
                    return new Guid(Membership.GetUser().ProviderUserKey.ToString());
                else
                    return null;
            }
        }

        protected string GetUserName()
        {
            string memberName = string.Empty;
            if (MemberId != null)
                memberName = (from user in DataContext.aspnet_Users
                              where user.UserId == MemberId
                              select user.UserName).Single();

            return memberName;
        }

        protected string GetAvatar()
        {
            UserServices userServices = new UserServices();
            return userServices.GetAvatarUrl(MemberId.ToString());
        }

        protected int GetVideoCount()
        {
            int count = (from video in DataContext.Videos
                         where video.OwnerUserId == MemberId
                         where video.StatusId == 6
                         select video).Count();

            return count;
        }

        protected int GetFavoriteCount()
        {
            int count = (from favorite in DataContext.Favorites
                         where favorite.UserId == MemberId
                         select favorite).Count();

            return count;
        }

    }
}
