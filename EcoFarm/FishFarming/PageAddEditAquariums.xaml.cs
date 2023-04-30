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
            }
            else
            {
                comboBoxFish.SelectedIndex = 0;
                currentAquarium.BoardingDate = DateTime.Now;
            }

            DataContext = currentAquarium;
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
                    currentAquarium.Size = double.Parse(tbSize.Text.Replace('.', ','));
                    currentAquarium.Note = tbNote.Text;

                    if (currentAquarium.IdAquarium == 0)
                    {
                        EcoFarmDBEntities.GetContext().Aquariums.Add(currentAquarium);
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
            List<FishWork> works = AppConnect.ModelDB.FishWork.ToList();
            works = works.Where(x => x.IdFish == currentAquarium.IdFish).ToList();

            foreach (FishWork work in works)
            {
                CurrentWorksForFishFarming newCurrentWork = new CurrentWorksForFishFarming();

                newCurrentWork.IdAquarium = currentAquarium.IdAquarium; 
                newCurrentWork.IdWork = work.IdWork;
                newCurrentWork.DateOfNextWork = currentAquarium.BoardingDate;

                EcoFarmDBEntities.GetContext().CurrentWorksForFishFarming.Add(newCurrentWork);
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
    }
}
