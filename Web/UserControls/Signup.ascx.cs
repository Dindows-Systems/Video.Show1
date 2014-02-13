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
using VideoShow.Data;

namespace VideoShow.UserControls
{
    public partial class Signup : System.Web.UI.UserControl
    {
        public string LinkText
        {
            get { return this.ShowSignupPopup.Text; }
            set { this.ShowSignupPopup.Text = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}