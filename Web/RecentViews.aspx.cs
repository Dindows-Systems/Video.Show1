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

namespace VideoShow
{
    public partial class RecentViews : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RecentViewsVideoGrid.ConfigurationType = VideoGrid.ConfigurationTypes.Grid;
            RecentViewsVideoGrid.ViewType = VideoGrid.ViewTypes.RecentViews;
            if (Request.QueryString["memberId"] != null)
            {
                if (ValidateMemberId(Request.QueryString["memberId"]))
                {
                    RecentViewsVideoGrid.FilterValue = Request.QueryString["memberId"];
                }
            }
        }

        // todo: create a helper function
        private bool ValidateMemberId(string memberId)
        {
            try
            {
                new Guid(memberId);
                return true;
            }
            catch (SystemException)
            {
                return false;
            }
        }
    }
}
