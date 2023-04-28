using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.CropProduction;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EcoFarm.Authorization
{
    /// <summary>
    /// Логика взаимодействия для PageLogin.xaml
    /// </summary>
    public partial class PageLogin : Page
    {
        int attemptCounter = 0;

        public PageLogin()
        {
            InitializeComponent();
        }

        private void buttonEnter_Click(object sender, RoutedEventArgs e)
        {
            var userObj = AppConnect.ModelDB.Users.FirstOrDefault(x => x.Login == textBoxLogin.Text && x.Password == PasswordBoxEnter.Password);
            if(userObj != null)
            {
                attemptCounter = 0;
                AuthorizedUser.user = userObj;
                SelectedMenuTab.selectedMenuTab = "PageCropProduction";
                AppFrame.frameMain.Navigate(new PageTasksToday());
            }
            else
            {
                attemptCounter++;
                MessageBox.Show("Такого пользователя нет!", "Ошибка при авторизации!", MessageBoxButton.OK, MessageBoxImage.Error);
                //textBoxLogin.Clear();
                //PasswordBoxEnter.Clear();
                if(attemptCounter >= 3)
                {
                    WindowCaptcha captcha = new WindowCaptcha();
                    captcha.ShowDialog();
                }
            }
        }

        private void buttonRegistration_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new PageRegistration());
        }
    }
}
