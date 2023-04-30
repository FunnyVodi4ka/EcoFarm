using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using EcoFarm.Validation;
using System;
using System.Collections.Generic;
using System.Data;
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
            }
            else
            {
                comboBoxPlant.SelectedIndex = 0;
                currentField.BoardingDate = DateTime.Now;
            }

            DataContext = currentField;
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
    }
}
