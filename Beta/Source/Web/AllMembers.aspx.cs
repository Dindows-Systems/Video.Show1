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

namespace VideoShow
{
    public partial class AllMembers : System.Web.UI.Page
    {
        private int membersPerPage = 20;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                GetMembers(0);
        }

        protected void next_Click(object sender, EventArgs e)
        {
            GetMembers(Int32.Parse(CurrentPage.Value) + 1);
        }

        protected void previous_Click(object sender, EventArgs e)
        {
            GetMembers(Int32.Parse(CurrentPage.Value) - 1);
        }

        private void GetMembers(int currentPage)
        {
            using (VideoShowDataContext DataContext = DataContextFactory.DataContext())
            {
                var userInfo = from user in DataContext.aspnet_Users select user;
                int recordCount = userInfo.Count();

                userInfo = userInfo.Skip(currentPage * membersPerPage).Take(membersPerPage);

                CurrentPage.Value = currentPage.ToString();
                PreviousButton.Visible = currentPage > 0;
                NextButton.Visible = recordCount / membersPerPage > currentPage;

                MemberListView.DataSource = userInfo;
                MemberListView.DataBind();
            }
        }
    }
}
