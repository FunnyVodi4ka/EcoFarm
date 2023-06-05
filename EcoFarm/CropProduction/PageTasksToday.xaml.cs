using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Логика взаимодействия для PageTasksToday.xaml
    /// </summary>
    public partial class PageTasksToday : Page
    {
        AccessVerification access = new AccessVerification();
        CurrentWorks currentWorks = new CurrentWorks();

        public PageTasksToday()
        {
            access.CheckAuthorization();

            InitializeComponent();

            TabBarAccess();

            SetFilter();
            SetSort();
            ListTasks.ItemsSource = SortFilterTasks();
        }

        CurrentWorks[] SortFilterTasks()
        {
            List<CurrentWorks> tasks = AppConnect.ModelDB.CurrentWorks.ToList();
            var CounterALL = tasks;
            if (textBoxSearch.Text != null)
            {
                tasks = tasks.Where(x => x.Fields.Number.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();

                switch (comboBoxSort.SelectedIndex)
                {
                    case 0:
                        tasks = tasks.OrderBy(x => x.DateOfNextWork).ToList();
                        break;
                    case 1:
                        tasks = tasks.OrderByDescending(x => x.DateOfNextWork).ToList();
                        break;
                }
            }

            if (comboBoxFilter.SelectedIndex > 0)
            {
                tasks = tasks.Where(x => x.Fields.Plants.Name == comboBoxFilter.SelectedItem.ToString()).ToList();
            }

            if (tasks.Count != 0)
            {
                Counter.Text = "Показано: " + tasks.Count + " из " + CounterALL.Count;
            }
            else
            {
                Counter.Text = "Не найдено";
            }

            return tasks.ToArray();
        }

        private void SetSort()
        {
            comboBoxSort.Items.Add("Ближайшие работы");
            comboBoxSort.Items.Add("Дальнейшие работы");

            comboBoxSort.SelectedIndex = 0;
        }

        private void SetFilter()
        {
            comboBoxFilter.Items.Add("Все растения");
            foreach (var plants in AppConnect.ModelDB.Plants)
            {
                comboBoxFilter.Items.Add(plants.Name);
            }

            comboBoxFilter.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void comboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            currentWorks = ListTasks.SelectedItems.Cast<CurrentWorks>().ToList().ElementAt(0);

            var nextDate = AppConnect.ModelDB.PlantWork.FirstOrDefault(x => x.IdPlant == currentWorks.Fields.IdPlant && x.IdWork == currentWorks.IdWork);

            currentWorks.DateOfNextWork = currentWorks.DateOfNextWork.AddDays(nextDate.PeriodInDays);
            WriteToHistory(currentWorks);
            AppConnect.ModelDB.SaveChanges();

            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void WriteToHistory(CurrentWorks work)
        {
            CompletedWorkHistory record = new CompletedWorkHistory();

            var place = AppConnect.ModelDB.PlaceForHistory.FirstOrDefault(x => x.Name == "Поле");
            record.IdPlace = place.IdPlace;
            record.Number = work.Fields.Number;
            record.ContentName = work.Fields.Plants.Name;
            record.WorkName = work.ListOfWorks.Name;
            record.DateOfWork = DateTime.Today;
            record.UserSurname = AuthorizedUser.user.Surname;
            record.UserName = AuthorizedUser.user.Name;
            if (AuthorizedUser.user.Patronymic.Length > 0)
            {
                record.UserPatronymic = AuthorizedUser.user.Patronymic;
            }

            AppConnect.ModelDB.CompletedWorkHistory.Add(record);
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

        private void TabBarAccess()
        {
            string color;
            if (!access.CheckMenegerAccessBoolResult())
            {
                color = "#C41E3A";
            }
            else
            {
                color = "#5D5D5D";
            }

            stTabBarFields.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            stTabBarPlants.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            stTabBarListOfWorks.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
        }
    }
}
