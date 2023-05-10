using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using EcoFarm.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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

namespace EcoFarm.CropProduction
{
    /// <summary>
    /// Логика взаимодействия для PageAddEditField.xaml
    /// </summary>
    public partial class PageAddEditField : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        private Fields currentField = new Fields();
        bool flagShowHideHarvestGroup = false;
        int growthPeriodInDays = 0;

        public PageAddEditField(Fields field)
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            SetPlants();

            if (field != null)
            {
                currentField = field;

                DisableAttributeEditField();
                FindField();
                ShowHideHarvestGroup(true);
            }
            else
            {
                ShowHideHarvestGroup(false);
                comboBoxPlant.SelectedIndex = 0;
                currentField.BoardingDate = DateTime.Now;
                dpCollectionDate.Text = "Нет";
            }

            DataContext = currentField;
        }

        private void ShowHideHarvestGroup(bool result)
        {
            if(result)
            {
                btnShowHideHarvest.Visibility = Visibility.Visible;
            }
            else
            {
                btnShowHideHarvest.Visibility = Visibility.Hidden;
            }
            btnHarvest.Visibility = Visibility.Hidden;
            tbHarvest.Visibility = Visibility.Hidden;
            tblHarvest.Visibility = Visibility.Hidden;
        }

        private void DisableAttributeEditField()
        {
            comboBoxPlant.IsEnabled = false;
            dpDate.IsEnabled = false;
        }

        private void SetPlants()
        {
            comboBoxPlant.Items.Add("Выберите растение");
            foreach (var plants in AppConnect.ModelDB.Plants)
            {
                comboBoxPlant.Items.Add(plants.Name);
            }
            comboBoxPlant.SelectedIndex = 0;
        }

        private void FindField()
        {
            var plant = AppConnect.ModelDB.Plants.FirstOrDefault(x => x.IdPlant == currentField.IdPlant);
            comboBoxPlant.SelectedItem = plant.Name;
        }

        private bool CheckAllData()
        {
            tbNumber.BorderBrush = Brushes.Black;
            dpDate.BorderBrush = Brushes.Black;
            tbSize.BorderBrush = Brushes.Black;
            tbNote.BorderBrush = Brushes.Black;

            if (!validation.CheckUniqueFieldNumber(tbNumber.Text, currentField.IdField))
            {
                tbNumber.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Поле с таким номером уже используется!");
                return false;
            }
            if (!validation.CheckStringData(tbNumber.Text, 1, 50))
            {
                tbNumber.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Номер не может содержать меньше 1 и больше 50 символов!");
                return false;
            }
            if (!validation.CheckIntData(tbQuantity.Text))
            {
                tbQuantity.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректное кол-во семян!");
                return false;
            }
            if (!validation.CheckDoubleData(tbExpenses.Text))
            {
                tbExpenses.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректная стоимость семян!");
                return false;
            }
            if (!validation.CheckDoubleData(tbSize.Text))
            {
                tbSize.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректный размер поля!");
                return false;
            }
            if (!validation.CheckDate(dpDate.Text))
            {
                dpDate.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректная дата посадки!");
                return false;
            }
            if (tbNote.Text.Length > 0)
            {
                if (!validation.CheckStringData(tbNote.Text, 2, 500))
                {
                    tbNote.BorderBrush = Brushes.Red;
                    MessageBox.Show("Ошибка: Заметки не могут содержать меньше 2 и больше 500 символов!");
                    return false;
                }
            }
            if (!CheckPlant())
            {
                MessageBox.Show("Выберите растение!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool CheckPlant()
        {
            var plant = AppConnect.ModelDB.Plants.FirstOrDefault(x => x.Name == comboBoxPlant.SelectedItem.ToString());
            if (plant != null)
            {
                return true;
            }
            return false;
        }

        private void SetPlant()
        {
            var plant = AppConnect.ModelDB.Plants.FirstOrDefault(x => x.Name == comboBoxPlant.SelectedItem.ToString());
            if (plant != null)
            {
                currentField.IdPlant = plant.IdPlant;
                growthPeriodInDays = plant.GrowthPeriodInDays;
            }
        }

        private void tbSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAllData())
            {
                try
                {
                    currentField.Number = tbNumber.Text;
                    SetPlant();
                    currentField.BoardingDate = DateTime.Parse(dpDate.Text);
                    currentField.CollectionDate = currentField.BoardingDate.AddDays(growthPeriodInDays);
                    currentField.Quantity = Int32.Parse(tbQuantity.Text);
                    currentField.Expenses = double.Parse(tbExpenses.Text.Replace('.', ','));
                    currentField.Size = double.Parse(tbSize.Text.Replace('.', ','));
                    if (tbNote.Text.Length <= 0)
                        currentField.Note = null;
                    else
                        currentField.Note = tbNote.Text;

                    if (currentField.IdField == 0)
                    {
                        EcoFarmDBEntities.GetContext().Fields.Add(currentField);
                        SaveCurrentWork();
                    }

                    EcoFarmDBEntities.GetContext().SaveChanges();
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

        private void SaveCurrentWork()
        {
            List<PlantWork> works = AppConnect.ModelDB.PlantWork.ToList();
            works = works.Where(x => x.IdPlant == currentField.IdPlant).ToList();

            foreach (PlantWork work in works)
            {
                CurrentWorks newCurrentWork = new CurrentWorks();

                newCurrentWork.IdField = currentField.IdField;
                newCurrentWork.IdWork = work.IdWork;
                newCurrentWork.DateOfNextWork = currentField.BoardingDate;

                EcoFarmDBEntities.GetContext().CurrentWorks.Add(newCurrentWork);
            }
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

        private void tbHarvest_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9.]"))
            {
                e.Handled = true;
            }
        }

        private void btnShowHideHarvest_Click(object sender, RoutedEventArgs e)
        {
            if(!flagShowHideHarvestGroup)
            {
                flagShowHideHarvestGroup = true;
                btnShowHideHarvest.Content = "Скрыть сбор урожая";
                btnHarvest.Visibility = Visibility.Visible;
                tbHarvest.Visibility = Visibility.Visible;
                tblHarvest.Visibility = Visibility.Visible;
            }
            else
            {
                flagShowHideHarvestGroup = false;
                btnShowHideHarvest.Content = "Показать сбор урожая";
                btnHarvest.Visibility = Visibility.Hidden;
                tbHarvest.Visibility = Visibility.Hidden;
                tblHarvest.Visibility = Visibility.Hidden;
            }
        }

        private void btnHarvest_Click(object sender, RoutedEventArgs e)
        {
            if (validation.CheckDoubleData(tbHarvest.Text))
            {
                if (MessageBox.Show("Вы уверены, что хотите собрать урожай? Предупреждение: при сборе урожая поле будет считаться пустым и будет удалено из списка!", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        HarvestingHistory record = new HarvestingHistory();

                        var place = AppConnect.ModelDB.PlaceForHistory.FirstOrDefault(x => x.Name == "Поле");
                        record.IdPlace = place.IdPlace;
                        record.Number = currentField.Number;
                        record.ContentName = currentField.Plants.Name;
                        record.DateOfHarvest = DateTime.Today;
                        record.CropWeight = double.Parse(tbHarvest.Text.Replace('.', ','));
                        record.Quantity = currentField.Quantity;
                        record.Expenses = currentField.Expenses;
                        record.Size = currentField.Size;
                        record.UserSurname = AuthorizedUser.user.Surname;
                        record.UserName = AuthorizedUser.user.Name;
                        record.UserPatronymic = AuthorizedUser.user.Patronymic;

                        AppConnect.ModelDB.HarvestingHistory.Add(record);
                        AppConnect.ModelDB.Fields.Remove(currentField);
                        AppConnect.ModelDB.SaveChanges();

                        AppFrame.frameMain.GoBack();
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка, повторите попытку позже!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Ошибка: Некорректный вес урожая!");
            }
        }

        private void tbQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9]"))
            {
                e.Handled = true;
            }
        }

        private void tbExpenses_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9.]"))
            {
                e.Handled = true;
            }
        }
    }
}
