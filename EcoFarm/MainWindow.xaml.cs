using EcoFarm.AppConnection;
using EcoFarm.Authentication;
using EcoFarm.Authorization;
using EcoFarm.CropProduction;
using EcoFarm.DatabaseConnection;
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

namespace EcoFarm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            HiddenButtons();

            AppConnect.ModelDB = new DatabaseConnection.EcoFarmDBEntities();
            AppFrame.frameMain = frmMain;

            frmMain.Navigate(new PageLogin());
        }

        private void frmMain_Navigated(object sender, NavigationEventArgs e)
        {
            if(AuthorizedUser.user == null)
            {
                HiddenButtons();
            }
            else
            {
                if (AuthorizedUser.user.Roles.Name == "Администратор")
                {
                    ShowButtonsForAdministrators();
                }
                else if(AuthorizedUser.user.Roles.Name == "Менеджер")
                {
                    ShowButtonsForMenegers();
                }
                else if (AuthorizedUser.user.Roles.Name == "Сотрудник")
                {
                    ShowButtonsForEmployees();
                }
            }
        }

        private void ShowButtonsForAdministrators()
        {
            btnPageUsers.Visibility = Visibility.Visible;
            ShowButtonsForMenegers();
        }

        private void ShowButtonsForMenegers()
        {
            btnPageFields.Visibility = Visibility.Visible;
            btnPagePlants.Visibility = Visibility.Visible;
            btnPageListOfWorks.Visibility = Visibility.Visible;
            ShowButtonsForEmployees();
        }

        private void ShowButtonsForEmployees()
        {
            btnPageTasksForDay.Visibility = Visibility.Visible;
            btnExit.Visibility = Visibility.Visible;
        }

        private void HiddenButtons()
        {
            btnPageTasksForDay.Visibility = Visibility.Hidden;
            btnPageFields.Visibility = Visibility.Hidden;
            btnPagePlants.Visibility = Visibility.Hidden;
            btnPageListOfWorks.Visibility = Visibility.Hidden;
            btnPageUsers.Visibility = Visibility.Hidden;
            btnExit.Visibility = Visibility.Hidden;
        }

        private void btnPageTasksForDay_Click(object sender, RoutedEventArgs e)
        {
            defaultButtonColor();
            stackPanelB1.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
            frmMain.Navigate(new PageTasksToday());
        }

        private void btnPageFields_Click(object sender, RoutedEventArgs e)
        {
            defaultButtonColor();
            stackPanelB2.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
            frmMain.Navigate(new PageFields());
        }

        private void btnPagePlants_Click(object sender, RoutedEventArgs e)
        {
            defaultButtonColor();
            stackPanelB3.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
            frmMain.Navigate(new PagePlants());
        }

        private void btnPageListOfWorks_Click(object sender, RoutedEventArgs e)
        {
            defaultButtonColor();
            stackPanelB4.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
            frmMain.Navigate(new PageListOfWorks());
        }

        private void btnPageUsers_Click(object sender, RoutedEventArgs e)
        {
            defaultButtonColor();
            stackPanelB5.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
            frmMain.Navigate(new PageUsers());
        }

        private void defaultButtonColor()
        {
            stackPanelB1.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");
            stackPanelB2.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");
            stackPanelB3.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");
            stackPanelB4.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");
            stackPanelB5.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            HiddenButtons();
            AuthorizedUser.user = null;
            frmMain.Navigate(new PageLogin());
        }
    }
}
