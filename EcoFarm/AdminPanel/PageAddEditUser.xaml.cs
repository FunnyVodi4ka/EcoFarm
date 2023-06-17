using EcoFarm.AppConnection;
using EcoFarm.AppSupportClass;
using EcoFarm.Authentication;
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

namespace EcoFarm.AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для PageAddEditUser.xaml
    /// </summary>
    public partial class PageAddEditUser : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        private Users currentUser = new Users();

        public PageAddEditUser(Users user)
        {
            access.CheckAuthorization();

            InitializeComponent();

            SetRoles();
            if (user != null)
            {
                currentUser = user;

                FindRole();

                if(AuthorizedUser.user.IdRole == currentUser.IdRole)
                {
                    comboBoxRole.Visibility = Visibility.Collapsed;
                    tblRole.Text = currentUser.Roles.Name;
                    tblRole.Visibility = Visibility.Visible;
                }
            }
            else
            {
                comboBoxRole.SelectedIndex = 0;
            }

            DataContext = currentUser;
        }

        private void SetRoles()
        {
            comboBoxRole.Items.Add("Выберите роль");
            foreach (var role in AppConnect.ModelDB.Roles)
            {
                comboBoxRole.Items.Add(role.Name);
            }
            comboBoxRole.SelectedIndex = 0;
        }

        private void FindRole()
        {
            var role = AppConnect.ModelDB.Roles.FirstOrDefault(x => x.IdRole == currentUser.IdRole);
            comboBoxRole.SelectedItem = role.Name;
        }

        private bool CheckAllData()
        {
            tbSurname.BorderBrush = Brushes.Black;
            tbName.BorderBrush = Brushes.Black;
            tbPatronymic.BorderBrush = Brushes.Black;
            tbLogin.BorderBrush = Brushes.Black;
            tbEmail.BorderBrush = Brushes.Black;
            tbPhone.BorderBrush = Brushes.Black;
            pbPassword.BorderBrush = Brushes.Black;

            if (!validation.CheckStringData(tbSurname.Text, 2, 50))
            {
                tbSurname.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Фамилия не может содержать меньше 2 и больше 50 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!validation.CheckStringData(tbName.Text, 2, 50))
            {
                tbName.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Имя не может содержать меньше 2 и больше 50 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (tbPatronymic.Text.Length > 0)
            {
                if (!validation.CheckStringData(tbPatronymic.Text, 2, 50))
                {
                    tbPatronymic.BorderBrush = Brushes.Red;
                    MessageBox.Show("Ошибка: Отчество не может содержать меньше 2 и больше 50 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            if (!validation.CheckUniqueEmail(tbEmail.Text, currentUser.IdUser))
            {
                tbEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Пользователь с такой почтой уже зарегистрирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!validation.CheckEmail(tbEmail.Text))
            {
                tbEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректная почта!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!validation.CheckUniquePhone(tbPhone.Text, currentUser.IdUser))
            {
                tbPhone.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Пользователь с таким телефоном уже зарегистрирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!validation.CheckPhone(tbPhone.Text))
            {
                tbPhone.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Ошибка: Некорректный телефон (Пример: 89998887766)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!validation.CheckUniqueLogin(tbLogin.Text, currentUser.IdUser))
            {
                tbLogin.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Такой логин уже занят!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!validation.CheckStringData(tbLogin.Text, 2, 150))
            {
                tbLogin.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Логин должен содержать от 2 до 150 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!CheckRole())
            {
                MessageBox.Show("Выберите роль пользователя!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (currentUser.IdUser != 0 && pbPassword.Password.Length == 0)
            { }
            else
            {
                if (!validation.CheckPassword(pbPassword.Password))
                {
                    pbPassword.BorderBrush = Brushes.Red;
                    MessageBox.Show("Ошибка: Пароль должен содержать от 8 до 50 символов (Латинские прописные и строчные буквы, специальные символы)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }

        private bool CheckRole()
        {
            var role = AppConnect.ModelDB.Roles.FirstOrDefault(x => x.Name == comboBoxRole.SelectedItem.ToString());
            if (role != null)
            {
                return true;
            }
            return false;
        }

        private void SetRole()
        {
            var role = AppConnect.ModelDB.Roles.FirstOrDefault(x => x.Name == comboBoxRole.SelectedItem.ToString());
            if (role != null)
            {
                currentUser.IdRole = role.IdRole;
            }
        }

        private void imageEye_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            textBoxPassword.Text = pbPassword.Password;
            pbPassword.Visibility = Visibility.Collapsed;
            textBoxPassword.Visibility = Visibility.Visible;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Resources/AppImages/eye-open.png", UriKind.Relative);
            bi3.EndInit();
            imageEye.Source = bi3;
        }

        private void imageEye_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            pbPassword.Visibility = Visibility.Visible;
            textBoxPassword.Visibility = Visibility.Collapsed;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Resources/AppImages/eye-close.png", UriKind.Relative);
            bi3.EndInit();
            imageEye.Source = bi3;
        }

        private void imageEye_MouseLeave(object sender, MouseEventArgs e)
        {
            imageEye_PreviewMouseLeftButtonUp(sender, null);
        }

        private void tbSurname_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[а-яА-Я]"))
            {
                e.Handled = true;
            }
        }

        private void tbName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[а-яА-Я]"))
            {
                e.Handled = true;
            }
        }

        private void tbPatronymic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[а-яА-Я]"))
            {
                e.Handled = true;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAllData())
            {
                try
                {
                    currentUser.Login = tbLogin.Text;
                    if (pbPassword.Password != null)
                    {
                        string hashUserPassword = HashMD5.hashPassword(pbPassword.Password);
                        currentUser.Password = hashUserPassword;
                    }
                    currentUser.Surname = tbSurname.Text;
                    currentUser.Name = tbName.Text;
                    if (tbPatronymic.Text.Length > 0)
                    {
                        currentUser.Patronymic = tbPatronymic.Text;
                    }
                    currentUser.Email = tbEmail.Text;
                    currentUser.Phone = tbPhone.Text;
                    SetRole();

                    if (currentUser.IdUser == 0)
                    {
                        AppConnect.ModelDB.Users.Add(currentUser);
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

        private void tbPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9]") || tbPhone.Text.Length >= 11)
            {
                e.Handled = true;
            }
        }
    }
}
