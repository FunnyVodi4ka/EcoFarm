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

namespace EcoFarm.FishFarming
{
    /// <summary>
    /// Логика взаимодействия для PageAddEditAquariums.xaml
    /// </summary>
    public partial class PageAddEditAquariums : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        private Aquariums currentAquarium = new Aquariums();
        bool flagShowHideHarvestGroup = false;
        int growthPeriodInDays = 0;

        public PageAddEditAquariums(Aquariums aquarium)
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            SetFish();

            if (aquarium != null)
            {
                currentAquarium = aquarium;

                DisableAttributeEditField();
                FindAquarium();
                ShowHideHarvestGroup(true);
            }
            else
            {
                ShowHideHarvestGroup(false);
                comboBoxFish.SelectedIndex = 0;
                currentAquarium.BoardingDate = DateTime.Now;
                dpCollectionDate.Text = "Нет";
            }

            DataContext = currentAquarium;
        }

        private void ShowHideHarvestGroup(bool result)
        {
            if (result)
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
            comboBoxFish.IsEnabled = false;
            dpDate.IsEnabled = false;
        }

        private void SetFish()
        {
            comboBoxFish.Items.Add("Выберите вид рыбы");
            foreach (var fish in AppConnect.ModelDB.Fish)
            {
                comboBoxFish.Items.Add(fish.Name);
            }
            comboBoxFish.SelectedIndex = 0;
        }

        private void FindAquarium()
        {
            var fish = AppConnect.ModelDB.Fish.FirstOrDefault(x => x.IdFish == currentAquarium.IdFish);
            comboBoxFish.SelectedItem = fish.Name;
        }

        private bool CheckAllData()
        {
            tbNumber.BorderBrush = Brushes.Black;
            dpDate.BorderBrush = Brushes.Black;
            tbSize.BorderBrush = Brushes.Black;
            tbNote.BorderBrush = Brushes.Black;

            if (!validation.CheckUniqueFieldNumber(tbNumber.Text, currentAquarium.IdAquarium))
            {
                tbNumber.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Аквариум с таким номером уже используется!");
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
                MessageBox.Show("Ошибка: Некорректное кол-во мальков!");
                return false;
            }
            if (!validation.CheckDoubleData(tbExpenses.Text))
            {
                tbExpenses.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректная стоимость мальков!");
                return false;
            }
            if (!validation.CheckDoubleData(tbSize.Text))
            {
                tbSize.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректный объём аквариума!");
                return false;
            }
            if (!validation.CheckDate(dpDate.Text))
            {
                dpDate.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректная дата выпуска рыбы!");
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
            if (!CheckFish())
            {
                MessageBox.Show("Выберите вид рыбы!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool CheckFish()
        {
            var fish = AppConnect.ModelDB.Fish.FirstOrDefault(x => x.Name == comboBoxFish.SelectedItem.ToString());
            if (fish != null)
            {
                return true;
            }
            return false;
        }

        private void SetIdFish()
        {
            var fish = AppConnect.ModelDB.Fish.FirstOrDefault(x => x.Name == comboBoxFish.SelectedItem.ToString());
            if (fish != null)
            {
                currentAquarium.IdFish = fish.IdFish;
                growthPeriodInDays = fish.GrowthPeriodInDays;
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
                    currentAquarium.Number = tbNumber.Text;
                    SetIdFish();
                    currentAquarium.BoardingDate = DateTime.Parse(dpDate.Text);
                    currentAquarium.CollectionDate = currentAquarium.BoardingDate.AddDays(growthPeriodInDays);
                    currentAquarium.Quantity = Int32.Parse(tbQuantity.Text);
                    currentAquarium.Expenses = double.Parse(tbExpenses.Text.Replace('.', ','));
                    currentAquarium.Size = double.Parse(tbSize.Text.Replace('.', ','));
                    if (tbNote.Text.Length <= 0)
                        currentAquarium.Note = null;
                    else
                        currentAquarium.Note = tbNote.Text;

                    if (currentAquarium.IdAquarium == 0)
                    {
                        AppConnect.ModelDB.Aquariums.Add(currentAquarium);
                        SaveInBudgetHistory(currentAquarium);
                        SaveCurrentWork();
                    }

                    AppConnect.ModelDB.SaveChanges();
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

        private void SaveInBudgetHistory(Aquariums aquarium)
        {
            BudgetHistory budgetHistory = new BudgetHistory();
            budgetHistory.Name = "Затраты на мальков";
            var type = AppConnect.ModelDB.TypeOfOperation.FirstOrDefault(x => x.Name == "Расход");
            budgetHistory.IdOperation = type.IdOperation;
            budgetHistory.Amount = aquarium.Expenses;
            budgetHistory.Date = DateTime.Today;

            AppConnect.ModelDB.BudgetHistory.Add(budgetHistory);
            AppConnect.ModelDB.SaveChanges();
        }

        private void tbHarvest_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9]"))
            {
                e.Handled = true;
            }
        }

        private void btnShowHideHarvest_Click(object sender, RoutedEventArgs e)
        {
            if (!flagShowHideHarvestGroup)
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
            if (validation.CheckIntData(tbHarvest.Text, 1))
            {
                if (MessageBox.Show("Вы уверены, что хотите собрать урожай? Предупреждение: при сборе урожая аквариум будет считаться пустым и будет удален из списка!", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        HarvestingHistory record = new HarvestingHistory();

                        var place = AppConnect.ModelDB.PlaceForHistory.FirstOrDefault(x => x.Name == "Аквариум");
                        record.IdPlace = place.IdPlace;
                        record.Number = currentAquarium.Number;
                        record.ContentName = currentAquarium.Fish.Name;
                        record.BoardingDate = currentAquarium.BoardingDate;
                        record.DateOfHarvest = DateTime.Today;
                        record.CropWeight = double.Parse(tbHarvest.Text.Replace('.', ','));
                        record.Quantity = currentAquarium.Quantity;
                        record.Expenses = currentAquarium.Expenses;
                        record.Size = currentAquarium.Size;
                        record.SalePrice = null;
                        record.UserSurname = AuthorizedUser.user.Surname;
                        record.UserName = AuthorizedUser.user.Name;
                        record.UserPatronymic = AuthorizedUser.user.Patronymic;

                        AppConnect.ModelDB.HarvestingHistory.Add(record);
                        AppConnect.ModelDB.Aquariums.Remove(currentAquarium);
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
                MessageBox.Show("Ошибка: Некорректное количество рыб!");
            }
        }

        private void SaveCurrentWork()
        {
            List<FishWork> works = AppConnect.ModelDB.FishWork.ToList();
            works = works.Where(x => x.IdFish == currentAquarium.IdFish).ToList();

            foreach (FishWork work in works)
            {
                CurrentWorksForFishFarming newCurrentWork = new CurrentWorksForFishFarming();

                newCurrentWork.IdAquarium = currentAquarium.IdAquarium; 
                newCurrentWork.IdWork = work.IdWork;
                newCurrentWork.DateOfNextWork = currentAquarium.BoardingDate;

                AppConnect.ModelDB.CurrentWorksForFishFarming.Add(newCurrentWork);
            }
        }

        private void TabBarTasksToday_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedMenuTab.selectedMenuTab = "PageFishFarming";
            AppFrame.frameMain.Navigate(new PageTasksTodayForFishFarming());
        }

        private void TabBarAquariums_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (access.CheckMenegerAccessBoolResult())
            {
                SelectedMenuTab.selectedMenuTab = "PageFishFarming";
                AppFrame.frameMain.Navigate(new PageAquariums());
            }
        }

        private void TabBarFish_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (access.CheckMenegerAccessBoolResult())
            {
                SelectedMenuTab.selectedMenuTab = "PageFishFarming";
                AppFrame.frameMain.Navigate(new PageFish());
            }
        }

        private void TabBarListOfWorks_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (access.CheckMenegerAccessBoolResult())
            {
                SelectedMenuTab.selectedMenuTab = "PageFishFarming";
                AppFrame.frameMain.Navigate(new PageListOfWorksForFishFarming());
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
