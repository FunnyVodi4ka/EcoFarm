using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.CropProduction;
using EcoFarm.DatabaseConnection;
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
    /// Логика взаимодействия для PageAquariums.xaml
    /// </summary>
    public partial class PageAquariums : Page
    {
        AccessVerification access = new AccessVerification();

        public PageAquariums()
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            SetFilter();
            SetSort();
            ListAquariums.ItemsSource = SortFilterFields();
        }

        Aquariums[] SortFilterFields()
        {
            List<Aquariums> aquariums = AppConnect.ModelDB.Aquariums.ToList();
            var CounterALL = aquariums;
            if (textBoxSearch.Text != null)
            {
                aquariums = aquariums.Where(x => x.Number.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();

                switch (comboBoxSort.SelectedIndex)
                {
                    case 0:
                        aquariums = aquariums.OrderBy(x => x.Number).ToList();
                        break;
                    case 1:
                        aquariums = aquariums.OrderByDescending(x => x.Number).ToList();
                        break;
                }
            }

            if (comboBoxFilter.SelectedIndex > 0)
            {
                aquariums = aquariums.Where(x => x.Fish.Name == comboBoxFilter.SelectedItem.ToString()).ToList();
            }

            if (aquariums.Count != 0)
            {
                Counter.Text = "Показано: " + aquariums.Count + " из " + CounterALL.Count;
            }
            else
            {
                Counter.Text = "Не найдено";
            }

            return aquariums.ToArray();
        }

        private void SetSort()
        {
            comboBoxSort.Items.Add("По алфавиту А-Я");
            comboBoxSort.Items.Add("По алфавиту Я-А");

            comboBoxSort.SelectedIndex = 0;
        }

        private void SetFilter()
        {
            comboBoxFilter.Items.Add("Все виды рыб");
            foreach (var plants in AppConnect.ModelDB.Fish)
            {
                comboBoxFilter.Items.Add(plants.Name);
            }

            comboBoxFilter.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListAquariums.ItemsSource = SortFilterFields();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListAquariums.ItemsSource = SortFilterFields();
        }

        private void comboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListAquariums.ItemsSource = SortFilterFields();
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListAquariums.ItemsSource = SortFilterFields();
        }

        private void ListAquariums_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Aquariums row = ListAquariums.SelectedItem as Aquariums;
            AppFrame.frameMain.Navigate(new PageAddEditAquariums(row));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditAquariums((sender as Button).DataContext as Aquariums));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentRow = ListAquariums.SelectedItems.Cast<Aquariums>().ToList().ElementAt(0);
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    AppConnect.ModelDB.Aquariums.Remove(currentRow);
                    AppConnect.ModelDB.SaveChanges();
                    ListAquariums.ItemsSource = SortFilterFields();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            NavigationService.Navigate(new PageAddEditAquariums(null));
        }

        private void menuClickEdit_Click(object sender, RoutedEventArgs e)
        {
            Aquariums row = ListAquariums.SelectedItem as Aquariums;
            AppFrame.frameMain.Navigate(new PageAddEditAquariums(row));
        }

        private void menuClickDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete_Click(sender, e);
        }
    }
}
