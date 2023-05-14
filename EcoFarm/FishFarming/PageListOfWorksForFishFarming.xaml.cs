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
    /// Логика взаимодействия для PageListOfWorksForFishFarming.xaml
    /// </summary>
    public partial class PageListOfWorksForFishFarming : Page
    {
        AccessVerification access = new AccessVerification();

        private ListOfWorksForFishFarming currentWork = new ListOfWorksForFishFarming();

        public PageListOfWorksForFishFarming()
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            ListViewListOfWorks.ItemsSource = SortFilterListOfWorks();
        }

        ListOfWorksForFishFarming[] SortFilterListOfWorks()
        {
            List<ListOfWorksForFishFarming> rows = AppConnect.ModelDB.ListOfWorksForFishFarming.ToList();
            var CounterALL = rows;
            if (textBoxSearch.Text != null)
            {
                rows = rows.Where(x => x.Name.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();
                rows = rows.OrderBy(x => x.Name).ToList();
            }

            if (rows.Count != 0)
            {
                tbCounter.Text = "Показано: " + rows.Count + " из " + CounterALL.Count;
            }
            else
            {
                tbCounter.Text = "Не найдено";
            }

            return rows.ToArray();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentRow = ListViewListOfWorks.SelectedItems.Cast<ListOfWorksForFishFarming>().ToList().ElementAt(0);
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    AppConnect.ModelDB.ListOfWorksForFishFarming.Remove(currentRow);
                    AppConnect.ModelDB.SaveChanges();
                    ListViewListOfWorks.ItemsSource = SortFilterListOfWorks();

                    ResetEditRow();
                }
            }
            catch
            {
                MessageBox.Show("Для удаления записи её необходимо выбрать!");
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var uniqueName = AppConnect.ModelDB.ListOfWorksForFishFarming.FirstOrDefault(x => x.Name == tbAdd.Text);
            if (tbAdd.Text.Length >= 2 && tbAdd.Text.Length <= 100)
            {
                if (uniqueName == null || currentWork.IdWork == uniqueName.IdWork)
                {
                    try
                    {
                        currentWork.Name = tbAdd.Text;
                        if (currentWork.IdWork == 0)
                        {
                            AppConnect.ModelDB.ListOfWorksForFishFarming.Add(currentWork);
                        }
                        AppConnect.ModelDB.SaveChanges();
                        AppConnect.ModelDB.SaveChanges();
                        MessageBox.Show("Данные успешно сохранены!", "Уведомление",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                        ListViewListOfWorks.ItemsSource = SortFilterListOfWorks();
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show("Ошибка " + Ex.Message.ToString() + "Критическая работа приложения", "Уведомление",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    ResetEditRow();
                }
                else
                {
                    MessageBox.Show("Ошибка: Такой цвет уже существует");
                }
            }
            else
            {
                MessageBox.Show("Ошибка: Название должно содержать от 2 до 100 символов!");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListViewListOfWorks.ItemsSource = SortFilterListOfWorks();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetEditRow();
        }

        private void ResetEditRow()
        {
            currentWork = new ListOfWorksForFishFarming();
            tblAdd.Text = "Новый вид работ";
            tbAdd.Text = "";
            btnAdd.Content = "Добавить";
            btnReset.Visibility = Visibility.Hidden;

            menuClickAdd.Header = "Создать";
            menuClickEdit.Header = "Редактировать";
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListViewListOfWorks.ItemsSource = SortFilterListOfWorks();
        }

        private void LbListOfWorks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            currentWork = ListViewListOfWorks.SelectedItem as ListOfWorksForFishFarming;
            tblAdd.Text = "Изменение работы";
            tbAdd.Text = currentWork.Name;
            btnAdd.Content = "Изменить";
            btnReset.Visibility = Visibility.Visible;
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
            btnAdd_Click(sender, e);

            menuClickAdd.Header = "Создать";
        }

        private void menuClickEdit_Click(object sender, RoutedEventArgs e)
        {
            if (menuClickEdit.Header.ToString() == "Редактировать")
            {
                currentWork = ListViewListOfWorks.SelectedItem as ListOfWorksForFishFarming;
                tblAdd.Text = "Изменение работы";
                tbAdd.Text = currentWork.Name;
                btnAdd.Content = "Изменить";
                btnReset.Visibility = Visibility.Visible;

                menuClickAdd.Header = "Сохранить";
                menuClickEdit.Header = "Отменить";
            }
            else
            {
                ResetEditRow();
            }
        }

        private void menuClickDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete_Click(sender, e);
        }
    }
}
