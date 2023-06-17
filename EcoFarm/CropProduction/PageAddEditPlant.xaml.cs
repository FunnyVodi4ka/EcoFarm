using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
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
                btnOpenAlbum.Visibility = Visibility.Visible;
                FindWorks();
            }
            else
            {
                spListOfWork.Visibility = Visibility.Hidden;
                ListViewPlantWork.Visibility = Visibility.Hidden;
                btnOpenAlbum.Visibility = Visibility.Hidden;
            }

            DataContext = currentPlant;
        }

        private void FindWorks()
        {
            ListViewPlantWork.ItemsSource = listPlantWork();
        }

        PlantWork[] listPlantWork()
        {
            List<PlantWork> rows = AppConnect.ModelDB.PlantWork.ToList();
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
                MessageBox.Show("Ошибка: Растение с таким названием уже есть!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!validation.CheckStringData(tbName.Text, 2, 150))
            {
                tbName.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Название не может содержать меньше 2 и больше 150 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!validation.CheckStringData(tbDescription.Text, 2, 500))
            {
                tbDescription.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Описание не может содержать меньше 2 и больше 500 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (tbNote.Text.Length > 0)
            {
                if (!validation.CheckStringData(tbNote.Text, 2, 500))
                {
                    tbNote.BorderBrush = Brushes.Red;
                    MessageBox.Show("Ошибка: Заметки не могут содержать меньше 2 и больше 500 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            if (!validation.CheckIntData(tbGrowthPeriodInDays.Text))
            {
                tbGrowthPeriodInDays.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректное число!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (tbNote.Text.Length <= 0)
                        currentPlant.Note = null;
                    else
                        currentPlant.Note = tbNote.Text;
                    currentPlant.GrowthPeriodInDays = Int32.Parse(tbGrowthPeriodInDays.Text);
                    if (!string.IsNullOrEmpty(SaveFilename))
                    {
                        LoadImageInDirectory();
                        currentPlant.ImageOfThePlant = newImageName;
                    }

                    if (currentPlant.IdPlant == 0)
                    {
                        AppConnect.ModelDB.Plants.Add(currentPlant);
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

        private bool CheckPlantWorkRow()
        {
            tbPeriod.BorderBrush = Brushes.Black;

            if (!validation.CheckIntData(tbPeriod.Text))
            {
                tbPeriod.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректное число!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void FindWorkItem()
        {
            var work = AppConnect.ModelDB.ListOfWorks.FirstOrDefault(x => x.Name == comboBoxWorks.SelectedItem.ToString());
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

                    List<PlantWork> rows = AppConnect.ModelDB.PlantWork.ToList();
                    rows = rows.Where(x => x.IdPlant == currentPlant.IdPlant && x.IdWork == plantWork.IdWork).ToList();
                    if (rows.Count > 0)
                    {
                        MessageBox.Show("Такая работа уже добавлена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        AppConnect.ModelDB.PlantWork.Add(plantWork);
                        AppConnect.ModelDB.SaveChanges();

                        ListViewPlantWork.ItemsSource = listPlantWork();
                    }
                }
                    catch (Exception)
                {
                    MessageBox.Show("Ошибка: Критическая работа приложения", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentRow = ListViewPlantWork.SelectedItems.Cast<PlantWork>().ToList().ElementAt(0);
                if (MessageBox.Show("Вы уверены, что хотите удалить работу?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    AppConnect.ModelDB.PlantWork.Remove(currentRow);
                    AppConnect.ModelDB.SaveChanges();
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
                MessageBox.Show("Ошибка загрузки изображения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOpenAlbum_Click(object sender, RoutedEventArgs e)
        {
            Plants plant = currentPlant;
            AppFrame.frameMain.Navigate(new PageAlbumPlants(plant));
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

        private void tbGrowthPeriodInDays_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void tbPeriod_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void imagePlant_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnSaveImage_Click(sender, e);
        }

        private void tbName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (tbName.Text.Length >= 150)
            {
                e.Handled = true;
            }
        }

        private void tbDescription_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (tbDescription.Text.Length >= 500)
            {
                e.Handled = true;
            }
        }

        private void tbNote_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (tbNote.Text.Length >= 500)
            {
                e.Handled = true;
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
