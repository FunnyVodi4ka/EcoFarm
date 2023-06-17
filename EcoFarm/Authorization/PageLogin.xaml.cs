using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.CropProduction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            string hashUserPassword = HashMD5.hashPassword(PasswordBoxEnter.Password);
            var userObj = AppConnect.ModelDB.Users.FirstOrDefault(x => x.Login == textBoxLogin.Text && x.Password == hashUserPassword);
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

        private void imageEye_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            textBoxPassword.Text = PasswordBoxEnter.Password;
            PasswordBoxEnter.Visibility = Visibility.Collapsed;
            textBoxPassword.Visibility = Visibility.Visible;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Resources/AppImages/eye-open.png", UriKind.Relative);
            bi3.EndInit();
            imageEye.Source = bi3;
        }

        private void imageEye_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PasswordBoxEnter.Visibility = Visibility.Visible;
            textBoxPassword.Visibility = Visibility.Collapsed;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Resources/AppImages/eye-close.png", UriKind.Relative);
            bi3.EndInit();
            imageEye.Source = bi3;
        }

        private void imageEye_MouseLeave(object sender, MouseEventArgs e)
        {
            imageEye_PreviewMouseLeftButtonUp(sender, null);
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void textBoxLogin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (textBoxLogin.Text.Length >= 150)
            {
                e.Handled = true;
            }
        }

        private void PasswordBoxEnter_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (PasswordBoxEnter.Password.Length >= 50)
            {
                e.Handled = true;
            }
        }
    }
}
