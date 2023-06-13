using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using EcoFarm.Validation;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace EcoFarm.CropProduction
{
    /// <summary>
    /// Логика взаимодействия для PagePlants.xaml
    /// </summary>
    public partial class PagePlants : Page
    {
        AccessVerification access = new AccessVerification();

        public PagePlants()
        {
            access.CheckMenegerAccess();

            InitializeComponent();
            SetSort();
            ListPlants.ItemsSource = SortFilterPlants();
        }

        Plants[] SortFilterPlants()
        {
            List<Plants> plants = AppConnect.ModelDB.Plants.ToList();
            var CounterALL = plants;
            if (textBoxSearch.Text != null)
            {
                plants = plants.Where(x => x.Name.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();

                switch (comboBoxSort.SelectedIndex)
                {
                    case 0:
                        plants = plants.OrderBy(x => x.Name).ToList();
                        break;
                    case 1:
                        plants = plants.OrderByDescending(x => x.Name).ToList();
                        break;
                }
            }

            if (plants.Count != 0)
            {
                Counter.Text = "Показано: " + plants.Count + " из " + CounterALL.Count;
            }
            else
            {
                Counter.Text = "Не найдено";
            }

            return plants.ToArray();
        }

        private void SetSort()
        {
            comboBoxSort.Items.Add("По алфавиту А-Я");
            comboBoxSort.Items.Add("По алфавиту Я-А");

            comboBoxSort.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListPlants.ItemsSource = SortFilterPlants();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListPlants.ItemsSource = SortFilterPlants();
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPlants.ItemsSource = SortFilterPlants();
        }

        private void ListPlants_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListPlants.SelectedItem != null)
            {
                Plants plant = ListPlants.SelectedItem as Plants;
                AppFrame.frameMain.Navigate(new PageAddEditPlant(plant));
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditPlant(null));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ListPlants.SelectedItem != null)
            {
                var currentRow = ListPlants.SelectedItems.Cast<Plants>().ToList().ElementAt(0);
                try
                {
                    if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        string fileNameForDelete = currentRow.ImageOfThePlant;
                        AppConnect.ModelDB.Plants.Remove(currentRow);
                        AppConnect.ModelDB.SaveChanges();
                        ListPlants.ItemsSource = SortFilterPlants();
                        //DeletePhotoFromDirectory(fileNameForDelete);
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TabBarTasksToday_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PageCropProduction";
            AppFrame.frameMain.Navigate(new PageTasksToday());
        }

        private void TabBarFields_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (access.CheckMenegerAccessBoolResult())
            {
                SelectedMenuTab.selectedMenuTab = "PageCropProduction";
                AppFrame.frameMain.Navigate(new PageFields());
            }
        }

        private void TabBarPlants_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (access.CheckMenegerAccessBoolResult())
            {
                SelectedMenuTab.selectedMenuTab = "PageCropProduction";
                AppFrame.frameMain.Navigate(new PagePlants());
            }
        }

        private void TabBarListOfWorks_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (access.CheckMenegerAccessBoolResult())
            {
                SelectedMenuTab.selectedMenuTab = "PageCropProduction";
                AppFrame.frameMain.Navigate(new PageListOfWorks());
            }
        }

        private void menuClickAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditPlant(null));
        }

        private void menuClickEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ListPlants.SelectedItem != null)
            {
                Plants plant = ListPlants.SelectedItem as Plants;
                AppFrame.frameMain.Navigate(new PageAddEditPlant(plant));
            }
        }

        private void menuClickDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        private void TabBarTasksToday_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarTasksToday.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarTasksToday_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarTasksToday.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }

        private void TabBarFields_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarFields.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarFields_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarFields.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }

        private void TabBarListOfWorks_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarListOfWorks.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarListOfWorks_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarListOfWorks.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }
    }
}
