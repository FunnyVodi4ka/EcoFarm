using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.CropProduction;
using EcoFarm.DatabaseConnection;
using EcoFarm.FishFarming;
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
    /// Логика взаимодействия для PageBudgetHistory.xaml
    /// </summary>
    public partial class PageBudgetHistory : Page
    {
        AccessVerification access = new AccessVerification();

        public PageBudgetHistory()
        {
            access.CheckAuthorization();

            InitializeComponent();

            SetFilter();
            SetSort();
            ListBudget.ItemsSource = SortFilterTasks();
        }

        BudgetHistory[] SortFilterTasks()
        {
            List<BudgetHistory> rows = AppConnect.ModelDB.BudgetHistory.ToList();
            var CounterALL = rows;

            double profit = 0, expenses = 0, total = 0;
            foreach (var row in rows)
            {
                if (row.TypeOfOperation.Name == "Приход")
                {
                    profit += row.Amount;
                }
                else if(row.TypeOfOperation.Name == "Расход")
                {
                    expenses += row.Amount;
                }
            }
            total = profit - expenses;
            tblProfit.Text = profit.ToString();
            tblExpenses.Text = expenses.ToString();
            tblTotal.Text = total.ToString();
            if(total >= 0)
            {
                tblTotal.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#44944A");
            }
            else
            {
                tblTotal.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#B00000");
            }

            if (textBoxSearch.Text != null)
            {
                rows = rows.Where(x => x.Name.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();
            }
            switch (comboBoxSort.SelectedIndex)
            {
                case 0:
                    rows = rows.OrderByDescending(x => x.Date).ToList();
                    break;
                case 1:
                    rows = rows.OrderBy(x => x.Date).ToList();
                    break;
            }

            switch (comboBoxFilter.SelectedIndex)
            {
                case 1:
                    rows = rows.Where(x => x.TypeOfOperation.Name == "Приход").ToList();
                    break;
                case 2:
                    rows = rows.Where(x => x.TypeOfOperation.Name == "Расход").ToList();
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
            comboBoxFilter.Items.Add("Все операции");
            comboBoxFilter.Items.Add("Приход");
            comboBoxFilter.Items.Add("Расход");

            comboBoxFilter.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListBudget.ItemsSource = SortFilterTasks();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListBudget.ItemsSource = SortFilterTasks();
        }

        private void comboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBudget.ItemsSource = SortFilterTasks();
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBudget.ItemsSource = SortFilterTasks();
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentRow = ListBudget.SelectedItems.Cast<BudgetHistory>().ToList().ElementAt(0);
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    AppConnect.ModelDB.BudgetHistory.Remove(currentRow);
                    AppConnect.ModelDB.SaveChanges();
                    ListBudget.ItemsSource = SortFilterTasks();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditBudgetHistory((sender as Button).DataContext as BudgetHistory));
        }

        private void ListBudget_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BudgetHistory row = ListBudget.SelectedItem as BudgetHistory;
            AppFrame.frameMain.Navigate(new PageAddEditBudgetHistory(row));
        }

        private void menuClickAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditBudgetHistory(null));
        }

        private void menuClickEdit_Click(object sender, RoutedEventArgs e)
        {
            BudgetHistory row = ListBudget.SelectedItem as BudgetHistory;
            AppFrame.frameMain.Navigate(new PageAddEditBudgetHistory(row));
        }

        private void menuClickDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete_Click(sender, e);
        }
    }
}
