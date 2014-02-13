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
    public class FavoriteWebservice : System.Web.Services.WebService
    {
        private VideoShow.Data.DataServices.FavoriteServices favoriteService = new VideoShow.Data.DataServices.FavoriteServices();
        public class FavoriteList
        {
            public int VideoId;
            public string UserId;
            public string VideoTitle;
            public string Username;
        }

        private FavoriteList[] GetSearchResult(IQueryable<Favorite> favorites)
        {
            List<FavoriteList> favoriteResults = new List<FavoriteList>();
            foreach (Favorite f in favorites)
            {
                favoriteResults.Add(
                    new FavoriteList
                    {
                        UserId = f.UserId.ToString(),
                        VideoId = f.VideoId,
                        VideoTitle = f.Video.Title,
                        Username = f.aspnet_User.UserName
                    });
            }
            return favoriteResults.ToArray();
        }

        [WebMethod]
        public FavoriteList[] GetFavoritesByVideo(int VideoId)
        {
            var favorites = favoriteService.GetFavoritesByVideo(VideoId);
            return GetSearchResult(favorites);
        }

        [WebMethod]
        public FavoriteList[] GetFavoritesByUser(string UserId)
        {
            var favorites = favoriteService.GetFavoritesByUser(UserId);
            return GetSearchResult(favorites);
        }

        [WebMethod]
        public bool GetFavoriteStatus(string UserId, int VideoId)
        {
            return favoriteService.GetFavoriteStatus(UserId, VideoId);
        }

        public Favorite GetFavorite(string UserId, int VideoId)
        {
            return favoriteService.GetFavorite(UserId, VideoId);
        }

        [WebMethod]
        public void ToggleFavoriteStatus(string UserId, int VideoId)
        {
            favoriteService.ToggleFavoriteStatus(UserId, VideoId);
        }

        [WebMethod]
        public string SetFavoriteStatus(string UserId, int VideoId, bool IsFavorite)
        {
            return favoriteService.SetFavoriteStatus(UserId, VideoId, IsFavorite);
        }
    }
}