using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Services;
using System.Web.UI.MobileControls;
using VideoShow.Data;

namespace VideoShow
{
    /// <summary>
    /// Webservice which provides interaction with Tags
    /// </summary>
    [System.Web.Script.Services.ScriptService]
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class TagWebservice : System.Web.Services.WebService
    {
        private VideoShow.Data.DataServices.TagServices tagService = new VideoShow.Data.DataServices.TagServices();

        [WebMethod]
        public string[] GetTagsForVideo(int VideoId)
        {
            return tagService.GetTagsForVideo(VideoId);
        }
    }
}