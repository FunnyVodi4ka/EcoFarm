using EcoFarm.AppConnection;
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

namespace EcoFarm.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для PageUsers.xaml
    /// </summary>
    public partial class PageUsers : Page
    {
        AccessVerification access = new AccessVerification();

        public PageUsers()
        {
            access.CheckAdminAccess();

            InitializeComponent();

            SetFilter();
            SetSort();
            ListUsers.ItemsSource = SortFilterUsers();
        }

        Users[] SortFilterUsers()
        {
            List<Users> users = AppConnect.ModelDB.Users.ToList();
            var CounterALL = users;
            if (textBoxSearch.Text != null)
            {
                users = users.Where(x => x.Login.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();

                switch (comboBoxSort.SelectedIndex)
                {
                    case 0:
                        users = users.OrderBy(x => x.Surname).ToList();
                        break;
                    case 1:
                        users = users.OrderByDescending(x => x.Surname).ToList();
                        break;
                }
            }

            if (comboBoxFilter.SelectedIndex > 0)
            {
                users = users.Where(x => x.Roles.Name == comboBoxFilter.SelectedItem.ToString()).ToList();
            }

            if (users.Count != 0)
            {
                Counter.Text = "Показано пользователей: " + users.Count + " из " + CounterALL.Count;
            }
            else
            {
                Counter.Text = "Не найдено";
            }

            return users.ToArray();
        }

        private void SetSort()
        {
            comboBoxSort.Items.Add("По фамилии А-Я");
            comboBoxSort.Items.Add("По фамилии Я-А");

            comboBoxSort.SelectedIndex = 0;
        }

        private void SetFilter()
        {
            comboBoxFilter.Items.Add("Все роли");
            foreach (var roles in AppConnect.ModelDB.Roles)
            {
                comboBoxFilter.Items.Add(roles.Name);
            }

            comboBoxFilter.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AppConnect.ModelDB.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            ListUsers.ItemsSource = SortFilterUsers();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListUsers.ItemsSource = SortFilterUsers();
        }

        private void comboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListUsers.ItemsSource = SortFilterUsers();
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListUsers.ItemsSource = SortFilterUsers();
        }

        private void ListUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Users user = ListUsers.SelectedItem as Users;
            AppFrame.frameMain.Navigate(new PageAddEditUser(user));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditUser((sender as Button).DataContext as Users));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentRow = ListUsers.SelectedItems.Cast<Users>().ToList().ElementAt(0);
            if (currentRow.IdUser != AuthorizedUser.user.IdUser)
            {
                try
                {
                    if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        AppConnect.ModelDB.Users.Remove(currentRow);
                        AppConnect.ModelDB.SaveChanges();
                        ListUsers.ItemsSource = SortFilterUsers();
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Вы не можете удалить себя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuClickAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAddEditUser(null));
        }

        private void menuClickEdit_Click(object sender, RoutedEventArgs e)
        {
            Users user = ListUsers.SelectedItem as Users;
            AppFrame.frameMain.Navigate(new PageAddEditUser(user));
        }

        private void menuClickDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete_Click(sender, e);
        }
    }
}
