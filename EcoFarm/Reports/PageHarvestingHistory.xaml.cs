using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
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
    /// Логика взаимодействия для PageHarvestingHistory.xaml
    /// </summary>
    public partial class PageHarvestingHistory : Page
    {
        AccessVerification access = new AccessVerification();

        public PageHarvestingHistory()
        {
            access.CheckAuthorization();

            InitializeComponent();

            SetFilter();
            SetSort();
            ListTasks.ItemsSource = SortFilterTasks();
        }

        HarvestingHistory[] SortFilterTasks()
        {
            List<HarvestingHistory> rows = AppConnect.ModelDB.HarvestingHistory.ToList();
            var CounterALL = rows;

            if (textBoxSearch.Text != null)
            {
                rows = rows.Where(x => x.UserSurname.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();
            }

            switch (comboBoxSort.SelectedIndex)
            {
                case 0:
                    rows = rows.OrderByDescending(x => x.DateOfHarvest).ToList();
                    break;
                case 1:
                    rows = rows.OrderBy(x => x.DateOfHarvest).ToList();
                    break;
            }

            switch (comboBoxFilter.SelectedIndex)
            {
                case 1:
                    rows = rows.Where(x => x.PlaceForHistory.Name == "Поле").ToList();
                    break;
                case 2:
                    rows = rows.Where(x => x.PlaceForHistory.Name == "Аквариум").ToList();
                    break;
            }

            if (rows.Count != 0)
            {
                Counter.Text = "Показано: " + rows.Count + " из " + CounterALL.Count;
            }
            else
            {
                Counter.Text = "Не найдено";
            }

            return rows.ToArray();
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

        private void TabBarHarvestingHistory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (access.CheckMenegerAccessBoolResult())
            {
                SelectedMenuTab.selectedMenuTab = "PageReports";
                AppFrame.frameMain.Navigate(new PageHarvestingHistory());
            }
        }

        private void TabBarBudgetHistory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (access.CheckMenegerAccessBoolResult())
            {
                SelectedMenuTab.selectedMenuTab = "PageReports";
                AppFrame.frameMain.Navigate(new PageBudgetHistory());
            }
        }
    }
}
