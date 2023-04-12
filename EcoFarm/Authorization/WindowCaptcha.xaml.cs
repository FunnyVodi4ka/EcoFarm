using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EcoFarm.Authorization
{
    /// <summary>
    /// Логика взаимодействия для WindowCaptcha.xaml
    /// </summary>
    public partial class WindowCaptcha : Window
    {
        string text = String.Empty;
        DispatcherTimer _timer;
        TimeSpan _time;

        public WindowCaptcha()
        {
            InitializeComponent();
            ReloadCATPCHA();
        }

        private void ReloadCATPCHA()
        {
            text = "";
            Random rnd = new Random();
            string ALF = "1234567890QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
            for (int i = 0; i < 5; ++i)
                text += ALF[rnd.Next(ALF.Length)];
            LabelCAPTCHA.Content = text;
        }

        private void Timer()
        {
            btnCAPTCHA.IsEnabled = false;
            tbCAPTCHA.IsEnabled = false;
            _time = TimeSpan.FromSeconds(10);
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                LabelCAPTCHA.Content = _time.ToString("c");
                if (_time == TimeSpan.Zero)
                {
                    _timer.Stop();
                    ReloadCATPCHA();
                    btnCAPTCHA.IsEnabled = true;
                    tbCAPTCHA.IsEnabled = true;
                }
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            _timer.Start();
        }

        private void btnCAPTCHA_Click(object sender, RoutedEventArgs e)
        {
            if (tbCAPTCHA.Text == text)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка, повторите попытку через 10 секунд");
                tbCAPTCHA.Text = "";
                Timer();
            }
        }
    }
}
