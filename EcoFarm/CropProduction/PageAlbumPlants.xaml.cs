using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using EcoFarm.Validation;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using System.Xml.Linq;

namespace EcoFarm.CropProduction
{
    /// <summary>
    /// Логика взаимодействия для PageAlbumPlants.xaml
    /// </summary>
    public partial class PageAlbumPlants : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        AlbumPlants newRow = new AlbumPlants();
        string SaveFilename, newImageName;
        int idRecord = 0;

        public PageAlbumPlants(Plants plant)
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            idRecord = plant.IdPlant;

            ListAlbum.ItemsSource = SortFilterAlbum();
        }

        AlbumPlants[] SortFilterAlbum()
        {
            List <AlbumPlants> photos = AppConnect.ModelDB.AlbumPlants.Where(x => x.IdPlant == idRecord).ToList();

            if (photos.Count != 0)
            {
                Counter.Text = "Найдено: " + photos.Count + " фото";
            }
            else
            {
                Counter.Text = "Не найдено";
            }

            return photos.ToArray();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListAlbum.ItemsSource = SortFilterAlbum();
        }

        private void btnLoadPhoto_Click(object sender, RoutedEventArgs e)
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
                imagePhoto.Source = bitmap;
            }
            catch { }
        }

        private bool CheckAllData()
        {
            tbNote.BorderBrush = Brushes.Black;

            if (tbNote.Text.Length > 0)
            {
                if (!validation.CheckStringData(tbNote.Text, 2, 500))
                {
                    tbNote.BorderBrush = Brushes.Red;
                    MessageBox.Show("Ошибка: Заметки не могут содержать меньше 2 и больше 500 символов!");
                    return false;
                }
            }
            if (string.IsNullOrEmpty(SaveFilename))
            {
                MessageBox.Show("Ошибка: Некорректное изображение!");
                return false;
            }

            return true;
        }

        private void btnSavePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAllData())
            {
                try
                {
                    newRow.IdPlant = idRecord;
                    LoadImageInDirectory();
                    newRow.Photo = newImageName;
                    if (tbNote.Text.Length <= 0)
                        newRow.Note = null;
                    else
                        newRow.Note = tbNote.Text;

                    AppConnect.ModelDB.AlbumPlants.Add(newRow);
                    AppConnect.ModelDB.SaveChanges();

                    ListAlbum.ItemsSource = SortFilterAlbum();

                    SetDefaultvalue();

                    //MessageBox.Show("Фото добавлено в альбом!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка: Критическая работа приложения", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void SetDefaultvalue()
        {
            newRow = new AlbumPlants();
            SaveFilename = null;
            tbNote.Text = null;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Resources/AppImages/DefaultPicture.png", UriKind.Relative);
            bi3.EndInit();
            imagePhoto.Source = bi3;
        }

        private void LoadImageInDirectory()
        {
            try
            {
                File.Copy(SaveFilename, System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\PlantsImages\\AlbumPlants\\" + newImageName);
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки изображения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentRow = ListAlbum.SelectedItems.Cast<AlbumPlants>().ToList().ElementAt(0);
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string fileNameForDelete = currentRow.Photo;

                    AppConnect.ModelDB.AlbumPlants.Remove(currentRow);
                    AppConnect.ModelDB.SaveChanges();

                    ListAlbum.ItemsSource = SortFilterAlbum();

                    DeletePhotoFromDirectory(fileNameForDelete);
                }
            }
            catch
            {
                MessageBox.Show("Для удаления работы её необходимо выбрать!");
            }
        }

        private void DeletePhotoFromDirectory(string row)
        {
            try
            {
                File.Move(System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\PlantsImages\\AlbumPlants\\" + row, System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\FilesToDelete\\" + "ForDelete" + row);
            }
            catch
            {
                //MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
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
