using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Services;
using System.Web.UI.MobileControls;
using VideoShow.Data;
using VideoShow.Data.DataServices;

namespace VideoShow
{
    [System.Web.Script.Services.ScriptService]
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class CommentWebservice : System.Web.Services.WebService
    {
        private CommentServices commentService = new CommentServices();

        public class CommentList
        {
            public int CommentId;
            public string Username;
            public string AvatarUrl;
            public Guid UserId;
            public int VideoId;
            public string CommentText;
            public DateTime DateCreated;
            public double VideoTimeInSeconds;
        }

        private CommentList[] GetSearchResult(IQueryable<Comment> comments)
        {
            List<CommentList> commentResults = new List<CommentList>();
            UserServices userServices = new UserServices();
            foreach (Comment c in comments)
            {
                commentResults.Add(
                    new CommentList
                    {
                        CommentId = c.CommentId,
                        UserId = c.UserId,
                        VideoId = c.VideoId,
                        CommentText = c.Comment1,
                        DateCreated = c.DateCreated,
                        Username = c.aspnet_User.UserName,
                        AvatarUrl = userServices.GetAvatarUrl(c.UserId.ToString()),
                        VideoTimeInSeconds = c.VideoTimeInSeconds
                    });
            }
            return commentResults.ToArray();
        }

        [WebMethod]
        public CommentList[] GetComments(int VideoId)
        {
            IQueryable<Comment> comments = commentService.GetComments(VideoId);
            return GetSearchResult(comments);
        }

        [WebMethod]
        public CommentList[] GetCommentsByUser(string UserId)
        {
            IQueryable<Comment> comments = commentService.GetCommentsByUser(UserId);
            return GetSearchResult(comments);
        }

        [WebMethod]
        public string AddComment(string UserId, int VideoId, string CommentText, float VideoTimeInSeconds)
        {
            return commentService.AddComment(UserId, VideoId, CommentText, VideoTimeInSeconds);
        }
    }
}