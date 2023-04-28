using EcoFarm.AppConnection;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using EcoFarm.Validation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using static System.Net.Mime.MediaTypeNames;

namespace EcoFarm.CropProduction
{
    /// <summary>
    /// Логика взаимодействия для PageAddEditPlant.xaml
    /// </summary>
    public partial class PageAddEditPlant : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        private Plants currentPlant = new Plants();
        private PlantWork plantWork = new PlantWork();
        private string namePicture = String.Empty;
        string SaveFilename, newImageName;

        public PageAddEditPlant(Plants plant)
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            SetWorks();
            if (plant != null)
            {
                currentPlant = plant;

                spListOfWork.Visibility = Visibility.Visible;
                ListViewPlantWork.Visibility = Visibility.Visible;
                FindWorks();
            }
            else
            {
                spListOfWork.Visibility = Visibility.Hidden;
                ListViewPlantWork.Visibility = Visibility.Hidden;
            }

            DataContext = currentPlant;
        }

        private void FindWorks()
        {
            ListViewPlantWork.ItemsSource = listPlantWork();
        }

        PlantWork[] listPlantWork()
        {
            List<PlantWork> rows = EcoFarmDBEntities.GetContext().PlantWork.ToList();
            rows = rows.Where(x => x.IdPlant == currentPlant.IdPlant).ToList();

            return rows.ToArray();
        }

        private void SetWorks()
        {
            comboBoxWorks.Items.Add("Выберите");
            foreach (var works in AppConnect.ModelDB.ListOfWorks)
            {
                comboBoxWorks.Items.Add(works.Name);
            }
            comboBoxWorks.SelectedIndex = 0;
        }

        private void tbGrowthPeriodInDays_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9]"))
            {
                e.Handled = true;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }

        private bool CheckAllData()
        {
            tbName.BorderBrush = Brushes.Black;
            tbDescription.BorderBrush = Brushes.Black;
            tbGrowthPeriodInDays.BorderBrush = Brushes.Black;
            tbNote.BorderBrush = Brushes.Black;

            if (!validation.CheckUniquePlantName(tbName.Text, currentPlant.IdPlant))
            {
                tbName.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Растение с таким названием уже есть!");
                return false;
            }
            if (!validation.CheckStringData(tbName.Text, 2, 150))
            {
                tbName.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Название не может содержать меньше 2 и больше 150 символов!");
                return false;
            }
            if (!validation.CheckStringData(tbDescription.Text, 2, 500))
            {
                tbDescription.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Описание не может содержать меньше 2 и больше 500 символов!");
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
            if (!validation.CheckIntData(tbGrowthPeriodInDays.Text))
            {
                tbGrowthPeriodInDays.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректное число!");
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
                    currentPlant.Name = tbName.Text;
                    currentPlant.Description = tbDescription.Text;
                    currentPlant.Note = tbNote.Text;
                    currentPlant.GrowthPeriodInDays = Int32.Parse(tbGrowthPeriodInDays.Text);
                    if (SaveFilename != null)
                    {
                        LoadImageInDirectory();
                        currentPlant.ImageOfThePlant = newImageName;
                    }

                    if (currentPlant.IdPlant == 0)
                    {
                        EcoFarmDBEntities.GetContext().Plants.Add(currentPlant);
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

        private bool CheckPlantWorkRow()
        {
            tbPeriod.BorderBrush = Brushes.Black;

            if (!validation.CheckIntData(tbPeriod.Text))
            {
                tbPeriod.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректное число!");
                return false;
            }

            return true;
        }

        private void FindWorkItem()
        {
            var work = EcoFarmDBEntities.GetContext().ListOfWorks.FirstOrDefault(x => x.Name == comboBoxWorks.SelectedItem.ToString());
            if (work == null)
            {
                MessageBox.Show("Такого элемента нет!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                plantWork.IdWork = work.IdWork;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(CheckPlantWorkRow())
            {
                try
                {
                    plantWork = new PlantWork();
                    plantWork.IdPlant = currentPlant.IdPlant;
                    FindWorkItem();
                    plantWork.PeriodInDays = Int32.Parse(tbPeriod.Text);

                    List<PlantWork> rows = EcoFarmDBEntities.GetContext().PlantWork.ToList();
                    rows = rows.Where(x => x.IdPlant == currentPlant.IdPlant && x.IdWork == plantWork.IdWork).ToList();
                    if (rows.Count > 0)
                    {
                        MessageBox.Show("Такая работа уже добавлена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        EcoFarmDBEntities.GetContext().PlantWork.Add(plantWork);
                        EcoFarmDBEntities.GetContext().SaveChanges();

                        ListViewPlantWork.ItemsSource = listPlantWork();
                    }
                }
                    catch (Exception)
                {
                    MessageBox.Show("Ошибка: Критическая работа приложения", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            //ListViewPlantWork.Items.Add(plantWork);
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentRow = ListViewPlantWork.SelectedItems.Cast<PlantWork>().ToList().ElementAt(0);
                if (MessageBox.Show("Вы уверены, что хотите удалить работу?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    EcoFarmDBEntities.GetContext().PlantWork.Remove(currentRow);
                    EcoFarmDBEntities.GetContext().SaveChanges();
                    //ListViewPlantWork.Items.Remove(currentRow);
                    ListViewPlantWork.ItemsSource = listPlantWork();
                }
            }
            catch
            {
                MessageBox.Show("Для удаления работы её необходимо выбрать!");
            }
        }

        private void btnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.ShowDialog();

                Random rnd = new Random();
                newImageName = rnd.Next(10000, 100000).ToString() + rnd.Next(10000, 100000).ToString() + ".png";

                SaveFilename = dialog.FileName;
                
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(SaveFilename);
                bitmap.EndInit();
                imagePlant.Source = bitmap;
                //tblCurrentImage.Text = newImageName;
            }
            catch { }
        }

        private void LoadImageInDirectory()
        {
            try
            {
                File.Copy(SaveFilename, System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\PlantsImages\\" + newImageName);
            }
            catch
            {
                MessageBox.Show("Load image error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
