using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EcoFarm.DatabaseConnection
{
    public partial class Plants
    {
        public string CorrectImage
        {
            get
            {
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\PlantsImages\\" + ImageOfThePlant))
                {
                    return System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\PlantsImages\\" + ImageOfThePlant;
                }
                else
                {
                    return "/Resources/AppImages/DefaultPicture.png";
                }
            }
        }
    }
}
