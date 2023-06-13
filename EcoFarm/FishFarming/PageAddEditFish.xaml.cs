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

namespace EcoFarm.FishFarming
{
    /// <summary>
    /// Логика взаимодействия для PageAddEditFish.xaml
    /// </summary>
    public partial class PageAddEditFish : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        private Fish currentFish = new Fish();
        private FishWork fishWork = new FishWork();
        string SaveFilename, newImageName;

        public PageAddEditFish(Fish fish)
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            SetWorks();
            if (fish != null)
            {
                currentFish = fish;

                spListOfWork.Visibility = Visibility.Visible;
                ListViewFishWork.Visibility = Visibility.Visible;
                FindWorks();
            }
            else
            {
                spListOfWork.Visibility = Visibility.Hidden;
                ListViewFishWork.Visibility = Visibility.Hidden;
            }

            DataContext = currentFish;
        }

        private void FindWorks()
        {
            ListViewFishWork.ItemsSource = listPlantWork();
        }

        FishWork[] listPlantWork()
        {
            List<FishWork> rows = AppConnect.ModelDB.FishWork.ToList();
            rows = rows.Where(x => x.IdFish == currentFish.IdFish).ToList();

            return rows.ToArray();
        }

        private void SetWorks()
        {
            comboBoxWorks.Items.Add("Выберите");
            foreach (var works in AppConnect.ModelDB.ListOfWorksForFishFarming)
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

            if (!validation.CheckUniquePlantName(tbName.Text, currentFish.IdFish))
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
                    currentFish.Name = tbName.Text;
                    currentFish.Description = tbDescription.Text;
                    if (tbNote.Text.Length <= 0)
                        currentFish.Note = null;
                    else
                        currentFish.Note = tbNote.Text;
                    currentFish.GrowthPeriodInDays = Int32.Parse(tbGrowthPeriodInDays.Text);
                    if (!string.IsNullOrEmpty(SaveFilename))
                    {
                        LoadImageInDirectory();
                        currentFish.ImageOfTheFish = newImageName;
                    }

                    if (currentFish.IdFish == 0)
                    {
                        AppConnect.ModelDB.Fish.Add(currentFish);
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
                MessageBox.Show("Ошибка: Некорректное число!");
                return false;
            }

            return true;
        }

        private void FindWorkItem()
        {
            var work = AppConnect.ModelDB.ListOfWorksForFishFarming.FirstOrDefault(x => x.Name == comboBoxWorks.SelectedItem.ToString());
            if (work == null)
            {
                MessageBox.Show("Такого элемента нет!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                fishWork.IdWork = work.IdWork;
            }
        }

        private void btnOpenAlbum_Click(object sender, RoutedEventArgs e)
        {
            Fish fish = currentFish;
            AppFrame.frameMain.Navigate(new PageAlbumFish(fish));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CheckPlantWorkRow())
            {
                try
                {
                    fishWork = new FishWork();
                    fishWork.IdFish = currentFish.IdFish;
                    FindWorkItem();
                    fishWork.PeriodInDays = Int32.Parse(tbPeriod.Text);

                    List<FishWork> rows = AppConnect.ModelDB.FishWork.ToList();
                    rows = rows.Where(x => x.IdFish == currentFish.IdFish && x.IdWork == fishWork.IdWork).ToList();
                    if (rows.Count > 0)
                    {
                        MessageBox.Show("Такая работа уже добавлена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        AppConnect.ModelDB.FishWork.Add(fishWork);
                        AppConnect.ModelDB.SaveChanges();

                        ListViewFishWork.ItemsSource = listPlantWork();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка: Критическая работа приложения", "Уведомление",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            //ListViewFishWork.Items.Add(fishWork);
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentRow = ListViewFishWork.SelectedItems.Cast<FishWork>().ToList().ElementAt(0);
                if (MessageBox.Show("Вы уверены, что хотите удалить работу?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    AppConnect.ModelDB.FishWork.Remove(currentRow);
                    AppConnect.ModelDB.SaveChanges();
                    //ListViewFishWork.Items.Remove(currentRow);
                    ListViewFishWork.ItemsSource = listPlantWork();
                }
            }
            catch
            {
                MessageBox.Show("Для удаления работы её необходимо выбрать!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                imageFish.Source = bitmap;
                //tblCurrentImage.Text = newImageName;
            }
            catch { }
        }

        private void LoadImageInDirectory()
        {
            try
            {
                File.Copy(SaveFilename, System.AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\FishImages\\" + newImageName);
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки изображения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
