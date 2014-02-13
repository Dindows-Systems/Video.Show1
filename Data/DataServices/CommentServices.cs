using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoShow.Data.DataServices
{
    public class CommentServices
    {
        /// <summary>
        /// Gets the data context.
        /// </summary>
        /// <value>The data context.</value>
        private VideoShowDataContext DataContext
        {
            get { return DataContextFactory.DataContext(); }
        }

        /// <summary>
        /// Gets all comments for a video.
        /// </summary>
        /// <param name="VideoId">The video id.</param>
        /// <returns></returns>
        public IQueryable<Comment> GetComments(int VideoId)
        {
            var comments =
                from comment in DataContext.Comments
                where comment.VideoId == VideoId
                orderby comment.VideoTimeInSeconds
                select comment;
            return comments;
        }


        /// <summary>
        /// Gets all comments by user.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        public IQueryable<Comment> GetCommentsByUser(string UserId)
        {
            var comments =
                from comment in DataContext.Comments
                where comment.UserId == new Guid(UserId)
                orderby comment.VideoTimeInSeconds
                select comment;
            return comments;
        }

        /// <summary>
        /// Adds a comment to a video.
        /// </summary>
        /// <param name="UserId">The user id of the user adding the comment.</param>
        /// <param name="VideoId">The video to which the comment is being added.</param>
        /// <param name="CommentText">The comment text.</param>
        /// <param name="VideoTimeInSeconds">The video time in seconds at which the video is being added.</param>
        /// <returns></returns>
        public string AddComment(string UserId, int VideoId, string CommentText, float VideoTimeInSeconds)
        {
            string result = string.Empty;
            try
            {
                Comment c = new Comment();
                c.UserId = new Guid(UserId);
                c.VideoId = VideoId;
                c.Comment1 = CommentText;
                c.DateCreated = DateTime.Now;
                c.VideoTimeInSeconds = Math.Round(VideoTimeInSeconds, 1); //Make sure we don't get repeating decimals
                DataContext.Comments.Add(c);
                DataContext.SubmitChanges();
                result = c.CommentId.ToString();
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }
    }
}