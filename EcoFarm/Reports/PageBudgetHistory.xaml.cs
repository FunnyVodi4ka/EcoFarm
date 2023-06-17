using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.CropProduction;
using EcoFarm.DatabaseConnection;
using EcoFarm.FishFarming;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
using Excel = Microsoft.Office.Interop.Excel;

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
                    rows = rows.Where(x => x.Date.Day == DateTime.Now.Day).ToList();
                    break;
                case 2:
                    rows = rows.Where(x => x.Date >= DateTime.Now.AddDays(-7)).ToList();
                    break;
                case 3:
                    rows = rows.Where(x => x.Date.Month == DateTime.Now.Month).ToList();
                    break;
                case 4:
                    rows = rows.Where(x => x.Date.Month >= DateTime.Now.Month - 3).ToList();
                    break;
                case 5:
                    rows = rows.Where(x => x.Date.Month >= DateTime.Now.Month - 6).ToList();
                    break;
                case 6:
                    rows = rows.Where(x => x.Date.Year == DateTime.Now.Year).ToList();
                    break;
            }

            CalculateBudget(rows);

            if (textBoxSearch.Text != null)
            {
                rows = rows.Where(x => x.Name.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();
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

        private void CalculateBudget(List<BudgetHistory> rows)
        {
            double profit = 0, expenses = 0;
            foreach (var row in rows)
            {
                if (row.TypeOfOperation.Name == "Приход")
                {
                    profit += row.Amount;
                }
                else if (row.TypeOfOperation.Name == "Расход")
                {
                    expenses += row.Amount;
                }
            }
            double total = profit - expenses;

            profit = Math.Round(profit, 2);
            expenses = Math.Round(expenses, 2);
            total = Math.Round(total, 2);

            tblProfit.Text = profit.ToString();
            tblExpenses.Text = expenses.ToString();
            tblTotal.Text = total.ToString();

            if (total >= 0)
            {
                tblTotal.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#44944A");
            }
            else
            {
                tblTotal.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#B00000");
            }
        }

        private void SetSort()
        {
            comboBoxSort.Items.Add("От новых к старым");
            comboBoxSort.Items.Add("От старых к новым");

            comboBoxSort.SelectedIndex = 0;
        }

        private void SetFilter()
        {
            comboBoxFilter.Items.Add("За всё время");
            comboBoxFilter.Items.Add("За сегодня");
            comboBoxFilter.Items.Add("За 7 дней");
            comboBoxFilter.Items.Add("За этот месяц");
            comboBoxFilter.Items.Add("За три месяца");
            comboBoxFilter.Items.Add("За полгода");
            comboBoxFilter.Items.Add("За этот год");

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
            if (ListBudget.SelectedItem != null)
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
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditBudgetHistory(null));
        }

        private void ListBudget_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBudget.SelectedItem != null)
            {
                BudgetHistory row = ListBudget.SelectedItem as BudgetHistory;
                AppFrame.frameMain.Navigate(new PageAddEditBudgetHistory(row));
            }
        }

        private void menuClickAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditBudgetHistory(null));
        }

        private void menuClickEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ListBudget.SelectedItem != null)
            {
                BudgetHistory row = ListBudget.SelectedItem as BudgetHistory;
                AppFrame.frameMain.Navigate(new PageAddEditBudgetHistory(row));
            }
        }

        private void menuClickDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        private void TabBarHistoryWork_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarHistoryWork.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarHistoryWork_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarHistoryWork.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }

        private void TabBarHarvestingHistory_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarHarvestingHistory.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarHarvestingHistory_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarHarvestingHistory.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var excelApp = new Excel.Application();
                excelApp.Workbooks.Add();
                Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;

                workSheet.Cells[1, "A"] = "Код записи";
                workSheet.Cells[1, "B"] = "Название операции";
                workSheet.Cells[1, "C"] = "Тип операции";
                workSheet.Cells[1, "D"] = "Сумма";
                workSheet.Cells[1, "E"] = "Дата операции";

                var row = 1;
                foreach (var item in SortFilterTasks().ToArray())
                {
                    row++;
                    workSheet.Cells[row, "A"] = item.IdHistory;
                    workSheet.Cells[row, "B"] = item.Name;
                    workSheet.Cells[row, "C"] = item.TypeOfOperation.Name;
                    workSheet.Cells[row, "D"] = item.Amount;
                    workSheet.Cells[row, "E"] = item.Date;
                }

                workSheet.Cells[1, "F"] = "Приход";
                workSheet.Cells[1, "G"] = "Расход";
                workSheet.Cells[1, "H"] = "Итог";
                workSheet.Cells[2, "F"] = tblProfit.Text;
                workSheet.Cells[2, "G"] = tblExpenses.Text;
                workSheet.Cells[2, "H"] = tblTotal.Text;

                workSheet.Columns[1].AutoFit();
                workSheet.Columns[2].AutoFit();
                workSheet.Columns[3].AutoFit();
                workSheet.Columns[4].AutoFit();
                workSheet.Columns[5].AutoFit();
                workSheet.Columns[6].AutoFit();
                workSheet.Columns[7].AutoFit();
                workSheet.Columns[8].AutoFit();

                excelApp.Visible = true;
            }
            catch { }
        }
    }
}
