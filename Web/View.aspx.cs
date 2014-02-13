using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using VideoShow.UserControls;
using VideoShow.Data;
using VideoShow;
using VideoShow.Data.DataServices;

public partial class View : System.Web.UI.Page
{

    private VideoShowDataContext DataContext = DataContextFactory.DataContext();

    //This is done in pre-render so it sees updates from other controls
    //and page envents during load.
    protected void Page_PreRender(object sender, EventArgs e)
    {
        //For use by other usercontrols
        Page.ClientScript.RegisterHiddenField("videoId", VideoId.ToString());

        string videoUrl = string.Format(
            "<script type='text/javascript'>" +
                "var videoUrl='{0}';" +
                "var videoId='{1}';" +
            "</script>",
            SelectedVideo.VideoUrl,
            SelectedVideo.VideoId
            );

        Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "VideoUrl", videoUrl);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        int videoId;
        if (int.TryParse(Request.QueryString["video"], out videoId) && videoId != 0)
        {
            this.VideoId = videoId;
        }
        else
        {
            videoId = this.VideoId;
        }

        if (!Page.IsPostBack)
        {
            if (Membership.GetUser() != null)
            {
                Guid loggedInUser = new Guid(Membership.GetUser().ProviderUserKey.ToString());

                AddViewEntry(videoId, loggedInUser);

                RegisterUserScript(loggedInUser);

                if (loggedInUser == this.SelectedVideo.OwnerUserId)
                {
                    DeleteVideo.Visible = true;
                }
            }
            else
            {
                AddViewEntry(videoId, null);
            }
        }
        PlaylistVideoGrid.ConfigurationType = VideoGrid.ConfigurationTypes.Playlist;
        PlaylistVideoGrid.ViewType = VideoGrid.ViewTypes.Owner;
        PlaylistVideoGrid.FilterValue = this.SelectedVideo.aspnet_User.UserId.ToString();
    }

    private void AddViewEntry(int videoId, Guid? userId)
    {
        VideoShow.Data.View view = new VideoShow.Data.View { 
            VideoId = videoId, UserId = userId, DateViewed = DateTime.Now
        };
        DataContext.Views.Add(view);
        DataContext.SubmitChanges();
    }

    protected void RegisterUserScript(Guid? loggedInUserId)
    {
        UserServices userServices = new UserServices();
        string avatarUrl = userServices.GetAvatarUrl(loggedInUserId.ToString());
        string userScript = string.Format(
            "<script type='text/javascript'>" +
                "var userId='{0}';" +
                "var userName='{1}';" +
                "var userAvatarUrl='{2}';" +
            "</script>",
            loggedInUserId.ToString(),
            Membership.GetUser().UserName,
            avatarUrl
            );
        Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "LoggedInUser", userScript);
    }

    protected string GetOwnerAvatar(int videoId)
    {
        VideoServices videoServices = new VideoServices();
        return videoServices.GetOwnerAvatarUrl(videoId);
    }
    /*
    private string GetFavoritesList(int videoId)
    {
        var favoritedBy =
            from favorite in DataContext.Favorites
            where favorite.VideoId == videoId
            select favorite.aspnet_User;

        string favoritesListText = "<ul>";
        foreach (aspnet_User user in favoritedBy)
            favoritesListText += string.Format(
                "<li><a href='Default.aspx?viewType=Owner&filterValue={0}'>{1}</a></li>",
                user.UserId,
                user.UserName);
        favoritesListText += "</ul>";
        return favoritesListText;
    }
    */
    protected void DeleteVideo_Click(object sender, EventArgs e)
    {
        Guid userId = this.SelectedVideo.OwnerUserId;
        int videoId = this.VideoId;
        DataContext.Views.RemoveAll(DataContext.Views.Where(v => v.VideoId == videoId));
        DataContext.Favorites.RemoveAll(DataContext.Favorites.Where(v => v.VideoId == videoId));
        DataContext.Comments.RemoveAll(DataContext.Comments.Where(v => v.VideoId == videoId));
        DataContext.VideosToTags.RemoveAll(DataContext.VideosToTags.Where(v => v.VideoId == videoId));
        DataContext.Videos.RemoveAll(DataContext.Videos.Where(v => v.VideoId == videoId));
        DataContext.SubmitChanges();

        Response.Redirect(string.Format("~/Member.aspx?memberid={0}", userId.ToString()));
    }

    private Video selectedVideo;
    public int VideoId
    {
        get
        {
            return SelectedVideo.VideoId;
        }
        set
        {
            Video queriedVideo = (from video in DataContext.Videos
                                  where video.VideoId == value
                                  select video).Single();
            if (queriedVideo != null)
                selectedVideo = queriedVideo;
        }
    }

    public Video SelectedVideo
    {
        get
        {
            if (selectedVideo == null)
            {
                Video latestVideo = (from video in DataContext.Videos
                                     orderby video.VideoId descending
                                     select video).First();
                if (latestVideo != null)
                    selectedVideo = latestVideo;
                else
                    selectedVideo = new Video();
            }
            return selectedVideo;
        }
        set
        {
            selectedVideo = value;
        }
    }

    public string GetVideoUsername()
    {
        return SelectedVideo.aspnet_User.UserName;
    }

    public string GetVideoTitle()
    {
        return SelectedVideo.Title;
    }

    //TODO: Refactor this
    //private string GetUserId()
    //{
    //    if (Membership.GetUser() != null)
    //        return Membership.GetUser().ProviderUserKey.ToString();
    //    else
    //        return string.Empty;
    //}

}