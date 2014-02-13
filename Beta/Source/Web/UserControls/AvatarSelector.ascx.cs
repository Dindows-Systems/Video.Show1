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

namespace VideoShow.UserControls
{
    public partial class AvatarSelector : System.Web.UI.UserControl
    {
        public int SelectedAvatarId
        {
            get 
            {
                int result = 0;
                int.TryParse(SelectedAvatar.Value, out result);
                return result;
            }
            set
            {
                SelectedAvatar.Value = value.ToString();
            }
        }

        public string GetSelectedAvatarControlId()
        {
            return SelectedAvatar.UniqueID.Replace("$","_").Replace(":","_");
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}