using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
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

namespace EcoFarm.Reports
{
    /// <summary>
    /// Логика взаимодействия для PageCompletedWorks.xaml
    /// </summary>
    public partial class PageCompletedWorks : Page
    {
        AccessVerification access = new AccessVerification();

        public PageCompletedWorks()
        {
            access.CheckAuthorization();

            InitializeComponent();

            SetFilter();
            SetSort();
            ListTasks.ItemsSource = SortFilterTasks();
        }

        CompletedWorkHistory[] SortFilterTasks()
        {
            List<CompletedWorkHistory> tasks = AppConnect.ModelDB.CompletedWorkHistory.ToList();
            var CounterALL = tasks;
            if (textBoxSearch.Text != null)
            {
                tasks = tasks.Where(x => x.UserSurname.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();
            }
            switch (comboBoxSort.SelectedIndex)
            {
                case 0:
                    tasks = tasks.OrderByDescending(x => x.DateOfWork).ToList();
                    break;
                case 1:
                    tasks = tasks.OrderBy(x => x.DateOfWork).ToList();
                    break;
            }

            switch (comboBoxFilter.SelectedIndex)
            {
                case 1:
                    tasks = tasks.Where(x => x.WorkPlaceForHistory.Name == "Поле").ToList();
                    break;
                case 2:
                    tasks = tasks.Where(x => x.WorkPlaceForHistory.Name == "Аквариум").ToList();
                    break;
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
            comboBoxSort.Items.Add("От новых к старым");
            comboBoxSort.Items.Add("От старых к новым");

            comboBoxSort.SelectedIndex = 0;
        }

        private void SetFilter()
        {
            comboBoxFilter.Items.Add("Все работы");
            comboBoxFilter.Items.Add("Растениеводство");
            comboBoxFilter.Items.Add("Рыбоводство");

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

        private void TabBarHistoryWork_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (access.CheckMenegerAccessBoolResult())
            {
                SelectedMenuTab.selectedMenuTab = "PageReports";
                AppFrame.frameMain.Navigate(new PageCompletedWorks());
            }
        }
    }
}
