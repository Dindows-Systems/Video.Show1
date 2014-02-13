using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace VideoShow.Data.DataServices
{
    public class UserServices
    {
        private VideoShowDataContext dataContext = DataContextFactory.DataContext();
        private VideoShowDataContext DataContext
        {
            get { return dataContext; }
        }

        /// <summary>
        /// Data transfer object for user related queries
        /// </summary>
        public class UserList
        {
            public string UserName;
            public string AvatarUrl;
            public Guid UserId;
        }

        /// <summary>
        /// Converts a query result (holding aspnet_User objects) to a list of user DTO's.
        /// </summary>
        /// <param name="users">The user list.</param>
        /// <returns></returns>
        private UserList[] GetSearchResult(IQueryable<aspnet_User> users)
        {
            List<UserList> userResults =
                new List<UserList>();
            foreach (aspnet_User u in users)
            {
                userResults.Add(
                    new UserList
                    {
                        UserId = u.UserId,
                        UserName = u.UserName,
                        AvatarUrl = GetAvatarUrl(u.UserId.ToString())
                    });
            }
            return userResults.ToArray();
        }

        /// <summary>
        /// Gets the avatar URL for a user.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        public string GetAvatarUrl(string UserId)
        {
            //Default image if none found
            if (string.IsNullOrEmpty(UserId))
                return "/App_Themes/Default/Images/geezer.png"; //TODO: Replace this

            string avatarUrl =
                (from users in DataContext.aspnet_Users
                 join uta in DataContext.UsersToAvatars on users.UserId equals uta.UserId
                 join avatar in DataContext.Avatars on uta.AvatarId equals avatar.AvatarId
                 where users.UserId == new Guid(UserId)
                 select avatar.ImageUrl).SingleOrDefault();
            return avatarUrl;
        }

        /// <summary>
        /// Gets the avatar id for a user.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        public int GetAvatarId(string UserId)
        {
            //Default image if none found
            if (string.IsNullOrEmpty(UserId))
                return 0;

            int avatarId =
                (from users in DataContext.aspnet_Users
                 join uta in DataContext.UsersToAvatars on users.UserId equals uta.UserId
                 join avatar in DataContext.Avatars on uta.AvatarId equals avatar.AvatarId
                 where users.UserId == new Guid(UserId)
                 select avatar.AvatarId).SingleOrDefault();
            return avatarId;
        }

        /// <summary>
        /// Gets the user (as a UserList DTO) by id.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        public UserList GetUserById(string UserId)
        {
            UserList user =
                (from users in DataContext.aspnet_Users
                 join uta in DataContext.UsersToAvatars on users.UserId equals uta.UserId
                 join avatar in DataContext.Avatars on uta.AvatarId equals avatar.AvatarId
                 where users.UserId == new Guid(UserId)
                 select new UserList { AvatarUrl = avatar.ImageUrl, UserId = users.UserId, UserName = users.UserName }).SingleOrDefault();
            return user;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <param name="Password">The password.</param>
        /// <param name="Email">The email.</param>
        /// <param name="AvatarId">The avatar id.</param>
        /// <returns></returns>
        public string CreateUser(string UserName, string Password, string Email, int AvatarId)
        {
            string result = string.Empty;
            try
            {
                MembershipUser user = Membership.CreateUser(UserName, Email, Password);
                result = user.ProviderUserKey.ToString();

                //Assign a random Avatar until UI supports selection
                UsersToAvatar uta = new UsersToAvatar();
                uta.UserId = new Guid(user.ProviderUserKey.ToString());
                uta.AvatarId = AvatarId;
                DataContext.UsersToAvatars.Add(uta);
                DataContext.SubmitChanges();
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }
    }
}
