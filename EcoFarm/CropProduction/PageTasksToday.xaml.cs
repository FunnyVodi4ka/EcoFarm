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
                    case 1:
                        tasks = tasks.OrderBy(x => x.DateOfNextWork).ToList();
                        break;
                    case 2:
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
                Counter.Text = "Текущих работ к выполнению не найдено";
            }

            return tasks.ToArray();
        }

        private void SetSort()
        {
            comboBoxSort.Items.Add("Без сортировки");
            comboBoxSort.Items.Add("Ближние работы");
            comboBoxSort.Items.Add("Дальние работы");

            comboBoxSort.SelectedIndex = 1;
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
            AppConnect.ModelDB.SaveChanges();

            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void TabBarTasksToday_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PageCropProduction";
            AppFrame.frameMain.Navigate(new PageTasksToday());
        }

        private void TabBarFields_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PageCropProduction";
            AppFrame.frameMain.Navigate(new PageFields());
        }

        private void TabBarPlants_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PageCropProduction";
            AppFrame.frameMain.Navigate(new PagePlants());
        }

        private void TabBarListOfWorks_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PageCropProduction";
            AppFrame.frameMain.Navigate(new PageListOfWorks());
        }
    }
}
