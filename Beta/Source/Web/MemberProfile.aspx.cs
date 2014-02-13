using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using VideoShow.Data;
using VideoShow.Data.DataServices;

namespace VideoShow
{
    public partial class MemberProfile : System.Web.UI.Page
    {
        MembershipUser currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            currentUser = Membership.GetUser();
            if(!Page.IsPostBack)
            {
                if (currentUser == null)
                {
                    Response.Redirect("~/");
                }
                Username.Text = currentUser.UserName;
                Email.Text = currentUser.Email;

                UserProfile profile = UserProfile.GetUserProfile(currentUser.UserName);
                Description.Text = profile.Description;
                FullName.Text = profile.FullName;
                Location.Text = profile.Location;
                FavoriteMovie.Text = profile.FavoriteMovie;

                UserServices userServices = new UserServices();
                int AvatarId = userServices.GetAvatarId(currentUser.ProviderUserKey.ToString());
                AvatarSelector.SelectedAvatarId = AvatarId;
            }
        }

        protected void UpdateSubmit_Click(object sender, EventArgs e)
        {
            currentUser.Email = Email.Text;

            UserServices userServices = new UserServices();
            string userId = currentUser.ProviderUserKey.ToString();
            if (AvatarSelector.SelectedAvatarId != userServices.GetAvatarId(userId))
                ChangeUserAvatar(userId, AvatarSelector.SelectedAvatarId);

            UserProfile profile = UserProfile.GetUserProfile(currentUser.UserName);
            profile.Description = Description.Text;
            profile.FullName = FullName.Text;
            profile.Location = Location.Text;
            profile.FavoriteMovie = FavoriteMovie.Text;
            profile.Save();
        }

        protected static void ChangeUserAvatar(string userId, int newAvatarId)
        {
            using (VideoShowDataContext DataContext = DataContextFactory.DataContext())
            {
                UsersToAvatar uta =
                    (from userToAvatar in DataContext.UsersToAvatars
                     where userToAvatar.UserId == new Guid(userId)
                     select userToAvatar).SingleOrDefault();

                uta.AvatarId = newAvatarId;
                DataContext.SubmitChanges();
            }
        }
    }
}
