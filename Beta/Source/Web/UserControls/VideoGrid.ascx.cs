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
using System.Text;

namespace VideoShow.UserControls
{
    public partial class VideoGrid : System.Web.UI.UserControl
    {
        /// <summary>
        /// ViewType
        /// </summary>
        public enum ViewTypes
        {
            NotSet,
            Tag,
            Owner,
            Favorite,
            RecentViews
        }
        public ViewTypes ViewType
        {
            get { return viewType; }
            set { viewType = value; }
        }
        private ViewTypes viewType = ViewTypes.NotSet;

        public bool IsMyPage
        {
            get { return isMyPage; }
            set { isMyPage = value; }
        }
        private bool isMyPage;

        /// <summary>
        /// ConfigurationType
        /// </summary>
        public enum ConfigurationTypes
        {
            Grid,
            Playlist,
            Home
        }
        public ConfigurationTypes ConfigurationType
        {
            get { return configurationType; }
            set { configurationType = value; }
        }
        private ConfigurationTypes configurationType = ConfigurationTypes.Grid;

        /// <summary>
        /// FilterValue
        /// </summary>
        public string FilterValue
        {
            get { return filterValue; }
            set { filterValue = value; }
        }
        private string filterValue = string.Empty;

        public string GridName
        {
            get { return gridName; }
            set { gridName = value; }
        }
        private string gridName = "VideoGrid";

        /// <summary>
        /// SelectedIndex
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; }
        }
        private int selectedIndex;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string GetStartupScript()
        {
            Initialize();

            StringBuilder sb = new StringBuilder();
            sb.Append("<script type=\"text/javascript\">");
            sb.Append("");
            sb.Append("Silverlight.installAndCreateSilverlight('1.0', '");
            sb.Append(gridName);
            sb.Append("', '<div style=\"position:auto; text-align:left; background-repeat:no-repeat; height:412px; background-image:url(/App_Themes/Default/Images/noSilverlightPreview.png);\">");
            sb.Append("<div style=\"padding-left:349px; padding-top:110px;\">");
            sb.Append("<div id=\"InstallPromptDiv\"> </div> </div> <div id=\"PostInstallGuidance\" ");
            sb.Append("style=\"width:auto; font-family:sans-serif; height:auto; padding-top:20px;text-align:center; color:#3366ff; ");
            sb.Append("font-weight:normal; font-size:11pt\">&nbsp;</div> </div>', ");
            sb.Append("'InstallPromptDiv', Silverlight.createSilverlight, '");
            sb.Append(viewType.ToString());
            sb.Append("', '");
            sb.Append(filterValue);
            sb.Append("', '");
            sb.Append(ConfigurationType.ToString());
            sb.Append("', ");
            sb.Append(selectedIndex.ToString());
            sb.Append(", ");
            sb.Append(IsMyPage.ToString().ToLower());
            sb.AppendLine(");</script>");

            return sb.ToString();
        }

        private void Initialize()
        {
            if (viewType == ViewTypes.NotSet)
            {
                if (Request.QueryString["viewType"] != null)
                {
                    try
                    {
                        ViewType = (VideoGrid.ViewTypes)Enum.Parse(typeof(VideoGrid.ViewTypes), Request.QueryString["viewType"], true);
                    }
                    catch (ArgumentException)
                    {
                        // todo: do something else here?
                        viewType = ViewTypes.Tag;
                    }
                }
                else
                    viewType = ViewTypes.Tag;
            }

            // todo: validate??
            if (string.IsNullOrEmpty(filterValue) && Request.QueryString["filterValue"] != null)
                filterValue = Request.QueryString["filterValue"];
        }

    }
}
