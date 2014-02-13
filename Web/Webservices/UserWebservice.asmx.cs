using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.MobileControls;
using VideoShow.Data;

namespace VideoShow
{
    [System.Web.Script.Services.ScriptService]
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class UserWebservice : System.Web.Services.WebService
    {
        private VideoShow.Data.DataServices.UserServices userService = new VideoShow.Data.DataServices.UserServices();

        [WebMethod]
        public string GetAvatarUrl(string UserId)
        {
            return userService.GetAvatarUrl(UserId);
        }

        [WebMethod]
        public int GetAvatarId(string UserId)
        {
            return userService.GetAvatarId(UserId);
        }

        [WebMethod]
        public VideoShow.Data.DataServices.UserServices.UserList GetUserById(string UserId)
        {
            return userService.GetUserById(UserId);
        }

        [WebMethod]
        public string CreateUser(string UserName, string Password, string Email, int AvatarId)
        {
            return userService.CreateUser(UserName, Password, Email, AvatarId);
        }
    }
}