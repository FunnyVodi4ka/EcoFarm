using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using EcoFarm.Validation;
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
using System.Xml.Linq;

namespace EcoFarm.Reports
{
    /// <summary>
    /// Логика взаимодействия для PageAddEditHarvestingHistory.xaml
    /// </summary>
    public partial class PageAddEditHarvestingHistory : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        private HarvestingHistory currentHarvestingHistory = new HarvestingHistory();

        public PageAddEditHarvestingHistory(HarvestingHistory harvestingHistory)
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            if (harvestingHistory != null)
            {
                currentHarvestingHistory = harvestingHistory;
            }

            DataContext = currentHarvestingHistory;
        }

        private void tbSalePrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9.]"))
            {
                e.Handled = true;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
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

        private bool CheckAllData()
        {
            tbSalePrice.BorderBrush = Brushes.Black;

            if (!validation.CheckDoubleData(tbSalePrice.Text))
            {
                tbSalePrice.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректная сумма операции!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAllData())
            {
                try
                {
                    currentHarvestingHistory.SalePrice = double.Parse(tbSalePrice.Text.Replace('.', ','));
                    SaveInBudgetHistory(currentHarvestingHistory);
                    AppConnect.ModelDB.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                    AppFrame.frameMain.GoBack();
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Ошибка " + Ex.Message.ToString() + "Критическая работа приложения", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void SaveInBudgetHistory(HarvestingHistory row)
        {
            BudgetHistory budgetHistory = new BudgetHistory();
            budgetHistory.Name = "Продажа урожая";
            var type = AppConnect.ModelDB.TypeOfOperation.FirstOrDefault(x => x.Name == "Приход");
            budgetHistory.IdOperation = type.IdOperation;
            budgetHistory.Amount = (double)row.SalePrice;
            budgetHistory.Date = DateTime.Today;

            AppConnect.ModelDB.BudgetHistory.Add(budgetHistory);
            AppConnect.ModelDB.SaveChanges();
        }

        private void tbSalePrice_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
