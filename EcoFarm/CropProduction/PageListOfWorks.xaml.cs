using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace EcoFarm.CropProduction
{
    /// <summary>
    /// Логика взаимодействия для PageListOfWorks.xaml
    /// </summary>
    public partial class PageListOfWorks : Page
    {
        AccessVerification access = new AccessVerification();

        private ListOfWorks currentWork = new ListOfWorks();

        public PageListOfWorks()
        {
            access.CheckMenegerAccess();

            InitializeComponent();

            ListViewListOfWorks.ItemsSource = SortFilterListOfWorks();
        }

        ListOfWorks[] SortFilterListOfWorks()
        {
            List<ListOfWorks> rows = AppConnect.ModelDB.ListOfWorks.ToList();
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
            if (ListViewListOfWorks.SelectedItem != null)
            {
                try
                {
                    var currentRow = ListViewListOfWorks.SelectedItems.Cast<ListOfWorks>().ToList().ElementAt(0);
                    if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        AppConnect.ModelDB.ListOfWorks.Remove(currentRow);
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
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var uniqueName = AppConnect.ModelDB.ListOfWorks.FirstOrDefault(x => x.Name == tbAdd.Text);
            if (tbAdd.Text.Length >= 2 && tbAdd.Text.Length <= 100)
            {
                if (uniqueName == null || currentWork.IdWork == uniqueName.IdWork)
                {
                    try
                    {
                        currentWork.Name = tbAdd.Text;
                        if (currentWork.IdWork == 0)
                        {
                            AppConnect.ModelDB.ListOfWorks.Add(currentWork);
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
            currentWork = new ListOfWorks();
            tblAdd.Text = "Новый вид работ";
            tbAdd.Text = "";
            btnAdd.Content = "Добавить";
            btnReset.Visibility = Visibility.Collapsed;

            menuClickAdd.Header = "Создать";
            menuClickEdit.Header = "Редактировать";
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListViewListOfWorks.ItemsSource = SortFilterListOfWorks();
        }

        private void LbListOfWorks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewListOfWorks.SelectedItem != null)
            {
                currentWork = ListViewListOfWorks.SelectedItem as ListOfWorks;
                tblAdd.Text = "Изменение работы";
                tbAdd.Text = currentWork.Name;
                btnAdd.Content = "Изменить";
                btnReset.Visibility = Visibility.Visible;
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

        private void menuClickAdd_Click(object sender, RoutedEventArgs e)
        {
            btnAdd_Click(sender, e);

            menuClickAdd.Header = "Создать";
        }

        private void menuClickEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewListOfWorks.SelectedItem != null)
            {
                if (menuClickEdit.Header.ToString() == "Редактировать")
                {
                    currentWork = ListViewListOfWorks.SelectedItem as ListOfWorks;
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
        }

        private void menuClickDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        private void TabBarTasksToday_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarTasksToday.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarTasksToday_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarTasksToday.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }

        private void TabBarFields_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarFields.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarFields_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarFields.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }

        private void TabBarPlants_MouseEnter(object sender, MouseEventArgs e)
        {
            stTabBarPlants.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#649138");
        }

        private void TabBarPlants_MouseLeave(object sender, MouseEventArgs e)
        {
            stTabBarPlants.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5D5D5D");
        }
    }
}
