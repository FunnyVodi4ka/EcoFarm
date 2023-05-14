using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.CropProduction;
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

namespace EcoFarm.FishFarming
{
    /// <summary>
    /// Логика взаимодействия для PageAlbumFish.xaml
    /// </summary>
    public partial class PageAlbumFish : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        AlbumFish newRow = new AlbumFish();
        string SaveFilename, newImageName;
        int idRecord = 0;

        public PageAlbumFish(Fish fish)
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            idRecord = fish.IdFish;

            ListAlbum.ItemsSource = SortFilterAlbum();
        }

        AlbumFish[] SortFilterAlbum()
        {
            List<AlbumFish> photos = AppConnect.ModelDB.AlbumFish.Where(x => x.IdFish == idRecord).ToList();

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
                    newRow.IdFish = idRecord;
                    LoadImageInDirectory();
                    newRow.Photo = newImageName;
                    if (tbNote.Text.Length <= 0)
                        newRow.Note = null;
                    else
                        newRow.Note = tbNote.Text;

                    AppConnect.ModelDB.AlbumFish.Add(newRow);
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
            newRow = new AlbumFish();
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
                File.Copy(SaveFilename, System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\FishImages\\AlbumFish\\" + newImageName);
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
                var currentRow = ListAlbum.SelectedItems.Cast<AlbumFish>().ToList().ElementAt(0);
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string fileNameForDelete = currentRow.Photo;
                    AppConnect.ModelDB.AlbumFish.Remove(currentRow);
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
                File.Move(System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\FishImages\\AlbumFish\\" + row, System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\FishImages\\AlbumFish\\" + "ForDelete" + row);
            }
            catch
            {
                //MessageBox.Show("Ошибка удаления изображения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
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
