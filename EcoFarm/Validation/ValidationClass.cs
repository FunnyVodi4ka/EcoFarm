using EcoFarm.AppConnection;
using EcoFarm.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EcoFarm.Validation
{
    class ValidationClass
    {
        public bool CheckStringData(string str, int minLength = 2, int maxLength = 150)
        {
            if (str.Length >= minLength && str.Length <= maxLength)
            {
                return true;
            }
            return false;
        }

        public bool CheckUniqueLogin(string str, int idRow)
        {
            var uniqueLogin = AppConnect.ModelDB.Users.FirstOrDefault(x => x.Login == str);
            if (uniqueLogin == null || uniqueLogin.IdUser != idRow)
            {
                return true;
            }
            return false;
        }

        public bool CheckUniquePlantName(string str, int idRow)
        {
            var unique = AppConnect.ModelDB.Plants.FirstOrDefault(x => x.Name == str);
            if (unique == null || unique.IdPlant == idRow)
            {
                return true;
            }
            return false;
        }

        public bool CheckUniqueFieldNumber(string str, int idRow)
        {
            var unique = AppConnect.ModelDB.Fields.FirstOrDefault(x => x.Number == str);
            if (unique == null || unique.IdField == idRow)
            {
                return true;
            }
            return false;
        }

        public bool CheckPassword(string password)
        {
            string pattern = @"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*])[0-9a-zA-Z!@#$%^&*]{6,50}";
            Match isMatch = Regex.Match(password, pattern);
            return isMatch.Success;
        }

        public bool CheckUniqueEmail(string str, int idRow)
        {
            var uniqueEmail = AppConnect.ModelDB.Users.FirstOrDefault(x => x.Email == str);
            if (uniqueEmail == null || uniqueEmail.IdUser == idRow)
            {
                return true;
            }
            return false;
        }

        public bool CheckEmail(string email)
        {
            if (email.Length >= 5 && email.Length <= 250)
            {
                string pattern = "(^[0-9a-z._-]+@[a-z]+\\.[a-z]+)";
                Match isMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
                return isMatch.Success;
            }
            return false;
        }

        public bool CheckUniquePhone(string str, int idRow)
        {
            var uniquePhone = AppConnect.ModelDB.Users.FirstOrDefault(x => x.Phone == str);
            if (uniquePhone == null || uniquePhone.IdUser == idRow)
            {
                return true;
            }
            return false;
        }

        public bool CheckPhone(string phone)
        {
            string pattern = "^8\\d{3}\\d{3}\\d{2}\\d{2}";
            Match isMatch = Regex.Match(phone, pattern, RegexOptions.IgnoreCase);
            return isMatch.Success;
        }

        public bool CheckIntData(string number, int minValue = 0)
        {
            try
            {
                int correctData = Int32.Parse(number);
                if (correctData >= minValue)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckDoubleData(string number, double minValue = 0)
        {
            try
            {
                double correctData = double.Parse(number.Replace('.', ','));
                if (correctData >= minValue)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckDateOfBirth(string date)
        {
            try
            {
                DateTime correctDate = DateTime.Parse(date);
                if (correctDate != null && correctDate >= DateTime.Now.AddDays(-365 * 110) && correctDate <= DateTime.Now.AddDays(-365 * 14))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckDate(string date)
        {
            try
            {
                DateTime correctDate = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
