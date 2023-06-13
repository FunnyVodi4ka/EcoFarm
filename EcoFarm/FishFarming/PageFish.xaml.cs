using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.CropProduction;
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
    /// Логика взаимодействия для PageFish.xaml
    /// </summary>
    public partial class PageFish : Page
    {
        AccessVerification access = new AccessVerification();

        public PageFish()
        {
            access.CheckMenegerAccess();

            InitializeComponent();
            SetSort();
            ListFish.ItemsSource = SortFilterFish();
        }

        Fish[] SortFilterFish()
        {
            List<Fish> fish = AppConnect.ModelDB.Fish.ToList();
            var CounterALL = fish;
            if (textBoxSearch.Text != null)
            {
                fish = fish.Where(x => x.Name.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();

                switch (comboBoxSort.SelectedIndex)
                {
                    case 0:
                        fish = fish.OrderBy(x => x.Name).ToList();
                        break;
                    case 1:
                        fish = fish.OrderByDescending(x => x.Name).ToList();
                        break;
                }
            }

            if (fish.Count != 0)
            {
                Counter.Text = "Показано: " + fish.Count + " из " + CounterALL.Count;
            }
            else
            {
                Counter.Text = "Не найдено";
            }

            return fish.ToArray();
        }

        private void SetSort()
        {
            comboBoxSort.Items.Add("По алфавиту А-Я");
            comboBoxSort.Items.Add("По алфавиту Я-А");

            comboBoxSort.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListFish.ItemsSource = SortFilterFish();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListFish.ItemsSource = SortFilterFish();
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListFish.ItemsSource = SortFilterFish();
        }

        private void ListFish_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListFish.SelectedItem != null)
            {
                Fish fish = ListFish.SelectedItem as Fish;
                AppFrame.frameMain.Navigate(new PageAddEditFish(fish));
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditFish(null));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ListFish.SelectedItem != null)
            {
                var currentRow = ListFish.SelectedItems.Cast<Fish>().ToList().ElementAt(0);
                try
                {
                    if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        AppConnect.ModelDB.Fish.Remove(currentRow);
                        AppConnect.ModelDB.SaveChanges();
                        ListFish.ItemsSource = SortFilterFish();
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

        private void menuClickAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditFish(null));
        }

        private void menuClickEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ListFish.SelectedItem != null)
            {
                Fish fish = ListFish.SelectedItem as Fish;
                AppFrame.frameMain.Navigate(new PageAddEditFish(fish));
            }
        }

        private void menuClickDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete_Click(sender, e);
        }
    }
}
