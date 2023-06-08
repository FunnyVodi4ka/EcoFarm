using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace EcoFarm.FishFarming
{
    public class DateTimeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            if (dateTime.Date < DateTime.Now.Date)
                return "/Resources/AppImages/task-skipped.png";
            if (dateTime.Date == DateTime.Now.Date)
                return "/Resources/AppImages/task-wait.png";
            if (dateTime.Date > DateTime.Now.Date)
                return "/Resources/AppImages/task-longwait.png";

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
