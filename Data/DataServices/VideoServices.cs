using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoShow.Data.DataServices
{
    public class VideoServices
    {
        /// <summary>
        /// Data transfer object to hold query results
        /// </summary>
        public class VideoSearchResult
        {
            public int totalPages;
            public int totalVideos;
            public string ownerName;  // populated only if searching for a member's videos.
            public VideoList[] videos;
        }

        public enum VideoStatus
        {
            New = 1,
            Encoding = 2,
            Encoded = 3,
            Uploading = 4,
            Uploaded = 5,
            Complete = 6,
            Failed = 7
        }

        /// <summary>
        /// Data Transfer Object which holds information for a specific video
        /// </summary>
        public class VideoList
        {
            public string thumbnailUrl;
            public string previewUrl;
            public string videoUrl;
            public string title;
            public int videoId;
            public int views;
            public int favorites;
            public string ownerName;
            public string avatarUrl;
            public string description;
            public string[] tags;
            public DateTime datePublished;
            public int durationInSeconds;
        }

        private VideoShowDataContext dataContext = DataContextFactory.DataContext();
        private VideoShowDataContext DataContext
        {
            get { return dataContext; }
        }

        /// <summary>
        /// Gets a paged list of videos.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="sortType">Type of the sort.</param>
        /// <returns></returns>
        public VideoSearchResult GetVideos(int pageSize, int pageNumber, string sortType)
        {
            var videos =
                from video in DataContext.Videos
                where video.StatusId == (int)VideoServices.VideoStatus.Complete
                orderby video.DatePublished descending
                select video;
            return GetSearchResult(videos, pageSize, pageNumber);
        }

        /// <summary>
        /// Gets the a list of videos by tag.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="sortType">Type of the sort.</param>
        /// <param name="tag">The tag we're filtering on.</param>
        /// <returns></returns>
        public VideoSearchResult GetVideosByTag(int pageSize, int pageNumber, string sortType, string tag)
        {
            var videos =
                from video in DataContext.Videos
                join vtt in DataContext.VideosToTags on video.VideoId equals vtt.VideoId
                join tags in DataContext.Tags on vtt.TagId equals tags.TagId
                where tags.Tag1 == tag
                where video.StatusId == (int)VideoServices.VideoStatus.Complete
                orderby video.DatePublished descending
                select video;
            return GetSearchResult(videos, pageSize, pageNumber);
        }

        /// <summary>
        /// Gets videos by owner.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="sortType">Type of the sort.</param>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        public VideoSearchResult GetVideosByOwner(int pageSize, int pageNumber, string sortType, string memberId)
        {
            var videos =
                from video in DataContext.Videos
                where video.OwnerUserId == new Guid(memberId)
                where video.StatusId == (int)VideoServices.VideoStatus.Complete
                orderby video.DatePublished descending
                select video;
            return GetSearchResultByOwner(videos, pageSize, pageNumber, new Guid(memberId));
        }

        /// <summary>
        /// Gets all recently vidwed videos for a user.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="sortType">Type of the sort.</param>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        public VideoSearchResult GetVideosByRecentViews(int pageSize, int pageNumber, string sortType, string memberId)
        {
            var videos =
            (from video in DataContext.Videos
             join recentView in DataContext.Views on video.VideoId equals recentView.VideoId
             where recentView.UserId == new Guid(memberId)
             where video.StatusId == (int)VideoServices.VideoStatus.Complete
             orderby recentView.DateViewed descending
             select video);
            return GetSearchResultByOwner(videos, pageSize, pageNumber, new Guid(memberId));
        }

        /// <summary>
        /// Gets all videos a user has favorited.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="sortType">Type of the sort.</param>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        public VideoSearchResult GetVideosByFavorite(int pageSize, int pageNumber, string sortType, string memberId)
        {
            var videos =
                from video in DataContext.Videos
                join favorite in DataContext.Favorites on video.VideoId equals favorite.VideoId
                where favorite.UserId == new Guid(memberId)
                where video.StatusId == (int)VideoServices.VideoStatus.Complete
                orderby video.DatePublished descending
                select video;
            return GetSearchResultByOwner(videos, pageSize, pageNumber, new Guid(memberId));
        }

        /// <summary>
        /// Gets details for a specific video.
        /// </summary>
        /// <param name="videoId">The video id.</param>
        /// <returns></returns>
        public VideoList GetVideoDetails(int videoId)
        {
            var videoResult =
                (from video in DataContext.Videos
                 where video.VideoId == videoId
                 select video).SingleOrDefault();
            return GetVideoListFromVideo(videoResult);
        }

        /// <summary>
        /// Converts a search result to a VideoSearchResult Data Transfer Object.
        /// </summary>
        /// <param name="videos">The videos.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <returns></returns>
        private VideoSearchResult GetSearchResult(IQueryable<Video> videos, int pageSize, int pageNumber)
        {
            VideoSearchResult result = new VideoSearchResult();

            result.totalVideos = videos.Count();
            result.totalPages = Convert.ToInt32(System.Math.Ceiling((double)(videos.Count() / (double)pageSize)));

            int skipCount = pageSize * (pageNumber - 1);
            videos = videos.Skip(skipCount).Take(pageSize);

            List<VideoList> videoResults = new List<VideoList>();
            foreach (Video v in videos)
            {
                videoResults.Add(GetVideoListFromVideo(v));
            }
            result.videos = videoResults.ToArray();
            return result;
        }

        /// <summary>
        /// Gets a search result, filtered by owner.
        /// </summary>
        /// <param name="videos">The videos.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        private VideoSearchResult GetSearchResultByOwner(IQueryable<Video> videos, int pageSize, int pageNumber, Guid memberId)
        {
            VideoSearchResult result = GetSearchResult(videos, pageSize, pageNumber);
            result.ownerName =
                (from user in DataContext.aspnet_Users
                 where user.UserId == memberId
                 select user.UserName).SingleOrDefault();
            return result;
        }


        /// <summary>
        /// Gets the owner avatar URL for a video.
        /// </summary>
        /// <param name="videoId">The video id.</param>
        /// <returns></returns>
        public string GetOwnerAvatarUrl(int videoId)
        {
            string avatarUrl = (from uta in DataContext.UsersToAvatars
                                join avatars in DataContext.Avatars on uta.AvatarId equals avatars.AvatarId
                                join videos in DataContext.Videos on uta.UserId equals videos.OwnerUserId
                                where videos.VideoId == videoId
                                select avatars.ImageUrl).SingleOrDefault();
            return avatarUrl;
        }

        /// <summary>
        /// Gets all tags for a video.
        /// </summary>
        /// <param name="videoId">The video id.</param>
        /// <returns></returns>
        public string[] GetTags(int videoId)
        {
            string[] videoTags;
            var tags = from t in DataContext.Tags
                       join vtt in DataContext.VideosToTags on t.TagId equals vtt.TagId
                       where vtt.VideoId == videoId
                       select t;
            List<Tag> tag = tags.ToList<Tag>();
            videoTags = new string[tag.Count()];
            for (int i = 0; i < tag.Count(); i++)
            {
                videoTags[i] += tag[i].Tag1;
            }
            return videoTags;
        }

        /// <summary>
        /// Converts a single video object to a VideoList Data Transfer Object.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        private VideoList GetVideoListFromVideo(Video v)
        {
            return new VideoList
            {
                previewUrl = v.PreviewUrl,
                thumbnailUrl = v.ThumbnailUrl,
                videoUrl = v.VideoUrl,
                title = v.Title,
                videoId = v.VideoId,
                views = v.Views.Count,
                favorites = v.Favorites.Count,
                description = v.Description,
                datePublished = v.DatePublished,
                durationInSeconds = v.DurationInSeconds,
                ownerName = v.aspnet_User.UserName,
                avatarUrl = GetOwnerAvatarUrl(v.VideoId),
                tags = GetTags(v.VideoId)
            };
        }

    }
}