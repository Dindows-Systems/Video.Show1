using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Services;
using System.Web.UI.MobileControls;
using VideoShow.Data;

namespace VideoShow
{
    [System.Web.Script.Services.ScriptService]
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class VideoWebservice : System.Web.Services.WebService
    {
        private VideoShow.Data.DataServices.VideoServices videoService = new VideoShow.Data.DataServices.VideoServices();

        [WebMethod]
        public VideoShow.Data.DataServices.VideoServices.VideoSearchResult GetVideos(int pageSize, int pageNumber, string sortType)
        {
            return videoService.GetVideos(pageSize, pageNumber, sortType);
        }

        [WebMethod]
        public VideoShow.Data.DataServices.VideoServices.VideoSearchResult GetVideosByTag(int pageSize, int pageNumber, string sortType, string tag)
        {
            return videoService.GetVideosByTag(pageSize, pageNumber, sortType, tag);
        }

        [WebMethod]
        public VideoShow.Data.DataServices.VideoServices.VideoSearchResult GetVideosByOwner(int pageSize, int pageNumber, string sortType, string memberId)
        {
            return videoService.GetVideosByOwner(pageSize, pageNumber, sortType, memberId);
        }

        [WebMethod]
        public VideoShow.Data.DataServices.VideoServices.VideoSearchResult GetVideosByRecentViews(int pageSize, int pageNumber, string sortType, string memberId)
        {
            return videoService.GetVideosByRecentViews(pageSize, pageNumber, sortType, memberId);
        }

        [WebMethod]
        public VideoShow.Data.DataServices.VideoServices.VideoSearchResult GetVideosByFavorite(int pageSize, int pageNumber, string sortType, string memberId)
        {
            return videoService.GetVideosByFavorite(pageSize, pageNumber, sortType, memberId);
        }

        [WebMethod]
        public VideoShow.Data.DataServices.VideoServices.VideoList GetVideoDetails(int videoId)
        {
            return videoService.GetVideoDetails(videoId);
        }

        public string GetOwnerAvatarUrl(int videoId)
        {
            return videoService.GetOwnerAvatarUrl(videoId);
        }

        public string[] GetTags(int videoId)
        {
            return videoService.GetTags(videoId);
        }
    }
}