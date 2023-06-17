using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.CropProduction;
using EcoFarm.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
            List<CompletedWorkHistory> rows = AppConnect.ModelDB.CompletedWorkHistory.ToList();
            var CounterALL = rows;
            if (textBoxSearch.Text != null)
            {
                rows = rows.Where(x => x.UserSurname.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();
            }
            switch (comboBoxSort.SelectedIndex)
            {
                case 0:
                    rows = rows.OrderByDescending(x => x.DateOfWork).ToList();
                    break;
                case 1:
                    rows = rows.OrderBy(x => x.DateOfWork).ToList();
                    break;
            }

            switch (comboBoxFilter.SelectedIndex)
            {
                case 1:
                    rows = rows.Where(x => x.DateOfWork.Day == DateTime.Now.Day).ToList();
                    break;
                case 2:
                    rows = rows.Where(x => x.DateOfWork >= DateTime.Now.AddDays(-7)).ToList();
                    break;
                case 3:
                    rows = rows.Where(x => x.DateOfWork.Month == DateTime.Now.Month).ToList();
                    break;
                case 4:
                    rows = rows.Where(x => x.DateOfWork.Month >= DateTime.Now.Month - 3).ToList();
                    break;
                case 5:
                    rows = rows.Where(x => x.DateOfWork.Month >= DateTime.Now.Month - 6).ToList();
                    break;
                case 6:
                    rows = rows.Where(x => x.DateOfWork.Year == DateTime.Now.Year).ToList();
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

        private void TabBarHarvestingHistory_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarHarvestingHistory.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarHarvestingHistory_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarHarvestingHistory.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
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
                workSheet.Cells[1, "E"] = "Название работы";
                workSheet.Cells[1, "F"] = "Дата работы";
                workSheet.Cells[1, "G"] = "Фамилия";
                workSheet.Cells[1, "H"] = "Имя";
                workSheet.Cells[1, "I"] = "Отчество";

                var row = 1;
                foreach (var item in SortFilterTasks().ToArray())
                {
                    row++;
                    workSheet.Cells[row, "A"] = item.IdHistory;
                    workSheet.Cells[row, "B"] = item.PlaceForHistory.Name;
                    workSheet.Cells[row, "C"] = item.Number;
                    workSheet.Cells[row, "D"] = item.ContentName;
                    workSheet.Cells[row, "E"] = item.WorkName;
                    workSheet.Cells[row, "F"] = item.DateOfWork;
                    workSheet.Cells[row, "G"] = item.UserSurname;
                    workSheet.Cells[row, "H"] = item.UserName;
                    workSheet.Cells[row, "I"] = item.UserPatronymic;
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

                excelApp.Visible = true;
            }
            catch { }
        }
    }
}
