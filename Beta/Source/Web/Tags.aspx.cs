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
    public partial class Tags : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TagsVideoGrid.ConfigurationType = VideoGrid.ConfigurationTypes.Grid;
            TagsVideoGrid.ViewType = VideoGrid.ViewTypes.Tag;
            if (Request.QueryString["Tag"] != null)
            {
                string tag = ValidateTag(Request.QueryString["Tag"]);
                TagsVideoGrid.FilterValue = tag;
                TagLabel.Text = "&nbsp;&gt;&nbsp;" + tag;
                AllVideosLink.Visible = true;
                AllVideosLabel.Visible = false;
                if (!IsPostBack)
                {
                    ((MasterPage)(this.Master)).SearchText = tag;
                }
            }
        }

        private string ValidateTag(string tag)
        {
            // TODO: validate the tag
            return tag;
        }
    }
}
