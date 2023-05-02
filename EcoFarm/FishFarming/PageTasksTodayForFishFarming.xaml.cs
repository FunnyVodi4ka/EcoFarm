using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
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
    /// Логика взаимодействия для PageTasksTodayForFishFarming.xaml
    /// </summary>
    public partial class PageTasksTodayForFishFarming : Page
    {
        AccessVerification access = new AccessVerification();
        CurrentWorksForFishFarming currentWorks = new CurrentWorksForFishFarming();

        public PageTasksTodayForFishFarming()
        {
            access.CheckAuthorization();

            InitializeComponent();

            TabBarAccess();

            SetFilter();
            SetSort();
            ListTasks.ItemsSource = SortFilterTasks();
        }

        CurrentWorksForFishFarming[] SortFilterTasks()
        {
            List<CurrentWorksForFishFarming> tasks = AppConnect.ModelDB.CurrentWorksForFishFarming.ToList();
            var CounterALL = tasks;
            if (textBoxSearch.Text != null)
            {
                tasks = tasks.Where(x => x.Aquariums.Number.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();

                switch (comboBoxSort.SelectedIndex)
                {
                    case 0:
                        tasks = tasks.OrderBy(x => x.DateOfNextWork).ToList();
                        break;
                    case 1:
                        tasks = tasks.OrderByDescending(x => x.DateOfNextWork).ToList();
                        break;
                }
            }

            if (comboBoxFilter.SelectedIndex > 0)
            {
                tasks = tasks.Where(x => x.Aquariums.Fish.Name == comboBoxFilter.SelectedItem.ToString()).ToList();
            }

            if (tasks.Count != 0)
            {
                Counter.Text = "Показано: " + tasks.Count + " из " + CounterALL.Count;
            }
            else
            {
                Counter.Text = "Текущих работ к выполнению не найдено";
            }

            return tasks.ToArray();
        }

        private void SetSort()
        {
            comboBoxSort.Items.Add("Ближайшие работы");
            comboBoxSort.Items.Add("Дальнейшие работы");

            comboBoxSort.SelectedIndex = 0;
        }

        private void SetFilter()
        {
            comboBoxFilter.Items.Add("Все рыбы");
            foreach (var fish in AppConnect.ModelDB.Fish)
            {
                comboBoxFilter.Items.Add(fish.Name);
            }

            comboBoxFilter.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void comboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            currentWorks = ListTasks.SelectedItems.Cast<CurrentWorksForFishFarming>().ToList().ElementAt(0);

            var nextDate = AppConnect.ModelDB.FishWork.FirstOrDefault(x => x.IdFish == currentWorks.Aquariums.IdFish && x.IdWork == currentWorks.IdWork);

            currentWorks.DateOfNextWork = currentWorks.DateOfNextWork.AddDays(nextDate.PeriodInDays);
            WriteToHistory(currentWorks);
            AppConnect.ModelDB.SaveChanges();

            ListTasks.ItemsSource = SortFilterTasks();
        }

        private void WriteToHistory(CurrentWorksForFishFarming work)
        {
            CompletedWorkHistory record = new CompletedWorkHistory();

            var place = AppConnect.ModelDB.WorkPlaceForHistory.FirstOrDefault(x => x.Name == "Аквариум");
            record.IdWorkPlace = place.IdWorkPlace;
            record.Number = work.Aquariums.Number;
            record.ContentName = work.Aquariums.Fish.Name;
            record.WorkName = work.ListOfWorksForFishFarming.Name;
            record.DateOfWork = DateTime.Today;
            record.UserSurname = AuthorizedUser.user.Surname;
            record.UserName = AuthorizedUser.user.Name;
            record.UserPatronymic = AuthorizedUser.user.Patronymic;

            AppConnect.ModelDB.CompletedWorkHistory.Add(record);
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

        private void TabBarAccess()
        {
            string color;
            if (!access.CheckMenegerAccessBoolResult())
            {
                color = "#C41E3A";
            }
            else
            {
                color = "#5D5D5D";
            }

            stTabBarAquariums.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            stTabBarFish.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            stTabBarListOfWorks.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
        }
    }
}
