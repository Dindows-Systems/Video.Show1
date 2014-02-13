using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoShow.Data.DataServices
{
    public class FavoriteServices
    {
        private VideoShowDataContext dataContext = DataContextFactory.DataContext();
        private VideoShowDataContext DataContext
        {
            get { return dataContext; }
        }

        /// <summary>
        /// Gets the favorites by video.
        /// </summary>
        /// <param name="VideoId">The video id.</param>
        /// <returns></returns>
        public IQueryable<Favorite> GetFavoritesByVideo(int VideoId)
        {
            var favorites =
                from favorite in DataContext.Favorites
                where favorite.VideoId == VideoId
                select favorite;
            return favorites;
        }

        /// <summary>
        /// Gets favorites for a user.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        public IQueryable<Favorite> GetFavoritesByUser(string UserId)
        {
            var favorites =
                from favorite in DataContext.Favorites
                where favorite.UserId == new Guid(UserId)
                select favorite;
            return favorites;
        }

        /// <summary>
        /// Determines if a user has favorited a specific video.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <param name="VideoId">The video id.</param>
        /// <returns></returns>
        public bool GetFavoriteStatus(string UserId, int VideoId)
        {
            Favorite favorite = GetFavorite(UserId, VideoId);
            return (favorite != null);
        }

        /// <summary>
        /// Gets the favorite record for a video / user combination.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <param name="VideoId">The video id.</param>
        /// <returns></returns>
        public Favorite GetFavorite(string UserId, int VideoId)
        {
            Favorite favorite = (from favorites in DataContext.Favorites
                            where favorites.UserId == new Guid(UserId)
                            where favorites.VideoId == VideoId
                            select favorites).SingleOrDefault();
            return favorite;
        }

        /// <summary>
        /// Toggles the favorite status for a user / video combination.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <param name="VideoId">The video id.</param>
        public void ToggleFavoriteStatus(string UserId, int VideoId)
        {
            //TODO:This is inefficient - we call GetFavorite on Get and Set of Favorite status
            if (GetFavoriteStatus(UserId, VideoId) == true)
            {
                SetFavoriteStatus(UserId, VideoId, false);
            }
            else
            {
                SetFavoriteStatus(UserId, VideoId, true);
            }
        }

        /// <summary>
        /// Sets the favorite status.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <param name="VideoId">The video id.</param>
        /// <param name="IsFavorite">if set to <c>true</c> [is favorite].</param>
        /// <returns></returns>
        public string SetFavoriteStatus(string UserId, int VideoId, bool IsFavorite)
        {
            Favorite favorite = GetFavorite(UserId, VideoId);

            if (favorite != null && IsFavorite)
                return string.Empty;

            if (favorite == null && !IsFavorite)
                return string.Empty;

            string result = string.Empty;
            try
            {
                if (IsFavorite)
                {
                    Favorite f = new Favorite();
                    f.VideoId = VideoId;
                    f.UserId = new Guid(UserId);
                    DataContext.Favorites.Add(f);
                    DataContext.SubmitChanges();
                }
                else
                {
                    DataContext.Favorites.Remove(favorite);
                    DataContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

    }
}