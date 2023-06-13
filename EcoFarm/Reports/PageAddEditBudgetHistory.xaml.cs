using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using EcoFarm.Validation;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
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
    /// Логика взаимодействия для PageAddEditBudgetHistory.xaml
    /// </summary>
    public partial class PageAddEditBudgetHistory : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        private BudgetHistory currentBudgetHistory = new BudgetHistory();

        public PageAddEditBudgetHistory(BudgetHistory budgetHistory)
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            SetTypes();

            if (budgetHistory != null)
            {
                currentBudgetHistory = budgetHistory;
                FindType();
            }

            DataContext = currentBudgetHistory;
        }

        private void FindType()
        {
            var row = AppConnect.ModelDB.TypeOfOperation.FirstOrDefault(x => x.IdOperation == currentBudgetHistory.IdOperation);
            comboBoxOperation.SelectedItem = row.Name;
        }

        private void SetTypes()
        {
            comboBoxOperation.Items.Add("Выберите тип");
            foreach (var row in AppConnect.ModelDB.TypeOfOperation)
            {
                comboBoxOperation.Items.Add(row.Name);
            }
            comboBoxOperation.SelectedIndex = 0;
        }

        private void tbExpenses_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
            tbName.BorderBrush = Brushes.Black;
            tbAmount.BorderBrush = Brushes.Black;

            if (!validation.CheckStringData(tbName.Text, 2, 250))
            {
                tbName.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Наименование не может содержать меньше 2 и больше 250 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!validation.CheckDoubleData(tbAmount.Text))
            {
                tbAmount.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректная сумма операции!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!CheckType())
            {
                MessageBox.Show("Выберите тип операции!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool CheckType()
        {
            var row = AppConnect.ModelDB.TypeOfOperation.FirstOrDefault(x => x.Name == comboBoxOperation.SelectedItem.ToString());
            if (row != null)
            {
                return true;
            }
            return false;
        }

        private void SetType()
        {
            var row = AppConnect.ModelDB.TypeOfOperation.FirstOrDefault(x => x.Name == comboBoxOperation.SelectedItem.ToString());
            if (row != null)
            {
                currentBudgetHistory.IdOperation = row.IdOperation;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAllData())
            {
                try
                {
                    currentBudgetHistory.Name = tbName.Text;
                    SetType();
                    currentBudgetHistory.Amount = double.Parse(tbAmount.Text.Replace('.', ','));
                    currentBudgetHistory.Date = DateTime.Now;

                    if (currentBudgetHistory.IdHistory == 0)
                    {
                        AppConnect.ModelDB.BudgetHistory.Add(currentBudgetHistory);
                    }
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
    }
}
