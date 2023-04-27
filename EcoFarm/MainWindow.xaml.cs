using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
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

            AppConnect.ModelDB = new EcoFarmDBEntities();
            AppFrame.frameMain = frmMain;

            frmMain.Navigate(new PageLogin());
        }

        private void ChangeColorMenuButtons()
        {
            stackPanelB1.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");
            stackPanelB2.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");
            stackPanelB3.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");
            stackPanelB4.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");
            stackPanelB5.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00ffffff");

            switch(SelectedMenuTab.selectedMenuTab)
            {
                case "PageTasksToday":
                    stackPanelB1.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
                    break;
                case "PageFields":
                    stackPanelB2.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
                    break;
                case "PagePlants":
                    stackPanelB3.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
                    break;
                case "PageListOfWorks":
                    stackPanelB4.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
                    break;
                case "PageUsers":
                    stackPanelB5.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
                    break;
            }
        }

        private void frmMain_Navigated(object sender, NavigationEventArgs e)
        {
            ChangeColorMenuButtons();

            if (AuthorizedUser.user == null)
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
            SelectedMenuTab.selectedMenuTab = "PageTasksToday";
            frmMain.Navigate(new PageTasksToday());
        }

        private void btnPageFields_Click(object sender, RoutedEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PageFields";
            frmMain.Navigate(new PageFields());
        }

        private void btnPagePlants_Click(object sender, RoutedEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PagePlants";
            frmMain.Navigate(new PagePlants());
        }

        private void btnPageListOfWorks_Click(object sender, RoutedEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PageListOfWorks";
            frmMain.Navigate(new PageListOfWorks());
        }

        private void btnPageUsers_Click(object sender, RoutedEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PageUsers";
            frmMain.Navigate(new PageUsers());
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            HiddenButtons();
            AuthorizedUser.user = null;
            SelectedMenuTab.selectedMenuTab = null;
            frmMain.Navigate(new PageLogin());
        }
    }
}
