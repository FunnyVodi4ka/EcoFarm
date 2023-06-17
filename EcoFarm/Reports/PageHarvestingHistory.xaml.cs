using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
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
using Excel = Microsoft.Office.Interop.Excel;

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
                    rows = rows.Where(x => x.DateOfHarvest.Day == DateTime.Now.Day).ToList();
                    break;
                case 2:
                    rows = rows.Where(x => x.DateOfHarvest >= DateTime.Now.AddDays(-7)).ToList();
                    break;
                case 3:
                    rows = rows.Where(x => x.DateOfHarvest.Month == DateTime.Now.Month).ToList();
                    break;
                case 4:
                    rows = rows.Where(x => x.DateOfHarvest.Month >= DateTime.Now.Month - 3).ToList();
                    break;
                case 5:
                    rows = rows.Where(x => x.DateOfHarvest.Month >= DateTime.Now.Month - 6).ToList();
                    break;
                case 6:
                    rows = rows.Where(x => x.DateOfHarvest.Year == DateTime.Now.Year).ToList();
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

        private void menuClickDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ListTasks.SelectedItem != null)
            {
                var currentRow = ListTasks.SelectedItems.Cast<CompletedWorkHistory>().ToList().ElementAt(0);
                try
                {
                    if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        AppConnect.ModelDB.CompletedWorkHistory.Remove(currentRow);
                        AppConnect.ModelDB.SaveChanges();
                        ListTasks.ItemsSource = SortFilterTasks();
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ListTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(ListTasks.SelectedItem != null)
            {
                HarvestingHistory row = ListTasks.SelectedItem as HarvestingHistory;
                AppFrame.frameMain.Navigate(new PageAddEditHarvestingHistory(row));
            }
        }

        private void menuClickEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ListTasks.SelectedItem != null)
            {
                HarvestingHistory row = ListTasks.SelectedItem as HarvestingHistory;
                AppFrame.frameMain.Navigate(new PageAddEditHarvestingHistory(row));
            }
        }

        private void menuClickReset_Click(object sender, RoutedEventArgs e)
        {
            if (ListTasks.SelectedItem != null)
            {
                HarvestingHistory row = ListTasks.SelectedItem as HarvestingHistory;

                try
                {
                    row.SalePrice = null;
                    AppConnect.ModelDB.SaveChanges();
                    MessageBox.Show("Продажа урожая сброшена!", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Ошибка " + Ex.Message.ToString() + "Критическая работа приложения", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void TabBarHistoryWork_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarHistoryWork.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarHistoryWork_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarHistoryWork.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }

        private void TabBarBudgetHistory_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarBudgetHistory.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarBudgetHistory_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarBudgetHistory.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var excelApp = new Excel.Application();
                excelApp.Workbooks.Add();
                Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;

                workSheet.Cells[1, "A"] = "Код записи";
                workSheet.Cells[1, "B"] = "Область работы";
                workSheet.Cells[1, "C"] = "Номер";
                workSheet.Cells[1, "D"] = "Содержимое";
                workSheet.Cells[1, "E"] = "Дата посадки";
                workSheet.Cells[1, "F"] = "Дата сбора";
                workSheet.Cells[1, "G"] = "Урожай";
                workSheet.Cells[1, "H"] = "Количество семян/мальков";
                workSheet.Cells[1, "I"] = "Затраты на семена/мальков";
                workSheet.Cells[1, "J"] = "Размер поля/аквариума";
                workSheet.Cells[1, "K"] = "Сумма продажи";
                workSheet.Cells[1, "L"] = "Фамилия";
                workSheet.Cells[1, "M"] = "Имя";
                workSheet.Cells[1, "N"] = "Отчество";

                var row = 1;
                foreach (var item in SortFilterTasks().ToArray())
                {
                    row++;
                    workSheet.Cells[row, "A"] = item.IdHistory;
                    workSheet.Cells[row, "B"] = item.PlaceForHistory.Name;
                    workSheet.Cells[row, "C"] = item.Number;
                    workSheet.Cells[row, "D"] = item.ContentName;
                    workSheet.Cells[row, "E"] = item.BoardingDate;
                    workSheet.Cells[row, "F"] = item.DateOfHarvest;
                    workSheet.Cells[row, "G"] = item.CropWeight;
                    workSheet.Cells[row, "H"] = item.Quantity;
                    workSheet.Cells[row, "I"] = item.Expenses;
                    workSheet.Cells[row, "J"] = item.Size;
                    workSheet.Cells[row, "K"] = item.SalePrice;
                    workSheet.Cells[row, "L"] = item.UserSurname;
                    workSheet.Cells[row, "M"] = item.UserName;
                    workSheet.Cells[row, "N"] = item.UserPatronymic;
                }

                workSheet.Columns[1].AutoFit();
                workSheet.Columns[2].AutoFit();
                workSheet.Columns[3].AutoFit();
                workSheet.Columns[4].AutoFit();
                workSheet.Columns[5].AutoFit();
                workSheet.Columns[6].AutoFit();
                workSheet.Columns[7].AutoFit();
                workSheet.Columns[8].AutoFit();
                workSheet.Columns[9].AutoFit();
                workSheet.Columns[10].AutoFit();
                workSheet.Columns[11].AutoFit();
                workSheet.Columns[12].AutoFit();
                workSheet.Columns[13].AutoFit();
                workSheet.Columns[14].AutoFit();

                excelApp.Visible = true;
            }
            catch { }
        }
    }
}
