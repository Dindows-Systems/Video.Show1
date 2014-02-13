using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoShow.Data.DataServices
{
    public class TagServices
    {
        private VideoShowDataContext dataContext = DataContextFactory.DataContext();
        private VideoShowDataContext DataContext
        {
            get { return dataContext; }
        }

        /// <summary>
        /// Gets the tags for a video as a string array.
        /// </summary>
        /// <param name="VideoId">The video id.</param>
        /// <returns></returns>
        public string[] GetTagsForVideo(int VideoId)
        {
            var tagInfo =
                from video in DataContext.Videos
                join vtt in DataContext.VideosToTags on video.VideoId equals vtt.VideoId
                join tag in DataContext.Tags on vtt.TagId equals tag.TagId
                where video.VideoId == VideoId
                select tag;

            List<string> result = new List<string>();
            foreach (var t in tagInfo)
            {
                result.Add(t.Tag1);
            }

            return result.ToArray<string>();
        }

        /// <summary>
        /// Gets the most popular tags in the database as a string array.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public List<string> GetPopularTags(int pageSize)
        {
            List<string> result = new List<string>();
            var popularTags =
                (from vtt in DataContext.VideosToTags
                 join t in DataContext.Tags on vtt.TagId equals t.TagId
                 group vtt by vtt.TagId into topTags
                 orderby topTags.Count() descending
                 select new
                 {
                     tagName =
                         (from tag in DataContext.Tags
                          where tag.TagId == topTags.Key
                          select tag.Tag1).SingleOrDefault()
                 }).Take(pageSize);

            foreach(var tag in popularTags)
                result.Add(tag.tagName);

            return result;
        }
    }
}
