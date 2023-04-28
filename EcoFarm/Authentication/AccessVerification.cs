using EcoFarm.AppConnection;
using EcoFarm.Authorization;
using EcoFarm.CropProduction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoFarm.Authentication
{
    class AccessVerification
    {
        public void CheckAuthorization()
        {
            if(AuthorizedUser.user == null)
            {
                AppFrame.frameMain.Navigate(new PageLogin());
            }
        }

        public void CheckAdminAccess()
        {
            CheckAuthorization();

            string role = AuthorizedUser.user.Roles.Name;
            if(role != "Администратор")
            {
                AppFrame.frameMain.Navigate(new PageTasksToday());
            }
        }

        public void CheckMenegerAccess()
        {
            CheckAuthorization();

            string role = AuthorizedUser.user.Roles.Name;
            if (role != "Администратор" || role != "Менеджер")
            {
                AppFrame.frameMain.Navigate(new PageTasksToday());
            }
        }

        public bool CheckAdminAccessBoolResult()
        {
            string role = AuthorizedUser.user.Roles.Name;
            if (role == "Администратор")
            {
                return true;
            }
            return false;
        }

        public bool CheckMenegerAccessBoolResult()
        {
            string role = AuthorizedUser.user.Roles.Name;
            if (role == "Администратор" || role == "Менеджер")
            {
                return true;
            }
            return false;
        }
    }
}
