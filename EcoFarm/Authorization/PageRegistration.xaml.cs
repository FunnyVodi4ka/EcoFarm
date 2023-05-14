using EcoFarm.AppConnection;
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
using System.Xml.Linq;

namespace EcoFarm.Authorization
{
    /// <summary>
    /// Логика взаимодействия для PageRegistration.xaml
    /// </summary>
    public partial class PageRegistration : Page
    {
        Users newUser = new Users();
        ValidationClass validation = new ValidationClass();

        public PageRegistration()
        {
            InitializeComponent();
        }

        private bool CheckAllData()
        {
            tbSurname.BorderBrush = Brushes.Black;
            tbName.BorderBrush = Brushes.Black;
            tbPatronymic.BorderBrush = Brushes.Black;
            tbEmail.BorderBrush = Brushes.Black;
            tbPhone.BorderBrush = Brushes.Black;
            tbLogin.BorderBrush = Brushes.Black;
            pbPassword.BorderBrush = Brushes.Black;

            if (!validation.CheckStringData(tbSurname.Text, 2, 50))
            {
                tbSurname.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Фамилия не может содержать меньше 2 и больше 50 символов!");
                return false;
            }
            if (!validation.CheckStringData(tbName.Text, 2, 50))
            {
                tbName.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Имя не может содержать меньше 2 и больше 50 символов!");
                return false;
            }
            if (!validation.CheckStringData(tbPatronymic.Text, 2, 50))
            {
                tbPatronymic.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Отчество не может содержать меньше 2 и больше 50 символов!");
                return false;
            }
            if (!validation.CheckUniqueEmail(tbEmail.Text, newUser.IdUser))
            {
                tbEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Пользователь с такой почтой уже зарегистрирован!");
                return false;
            }
            if (!validation.CheckEmail(tbEmail.Text))
            {
                tbEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректная почта!");
                return false;
            }
            if (!validation.CheckUniqueEmail(tbEmail.Text, newUser.IdUser))
            {
                tbEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Пользователь с такой почтой уже зарегистрирован!");
                return false;
            }
            if (!validation.CheckEmail(tbEmail.Text))
            {
                tbEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректная почта!");
                return false;
            }
            if (!validation.CheckUniquePhone(tbPhone.Text, newUser.IdUser))
            {
                tbPhone.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Пользователь с таким телефоном уже зарегистрирован!");
                return false;
            }
            if (!validation.CheckPhone(tbPhone.Text))
            {
                tbPhone.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Некорректный телефон (Пример: 89998887766)!");
                return false;
            }
            if (!validation.CheckUniqueLogin(tbLogin.Text, newUser.IdUser))
            {
                tbLogin.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Такой логин уже занят!");
                return false;
            }
            if (!validation.CheckStringData(tbLogin.Text, 2, 150))
            {
                tbLogin.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Логин должен содержать от 2 до 150 символов!");
                return false;
            }
            if (!validation.CheckPassword(pbPassword.Password))
            {
                pbPassword.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Пароль должен содержать от 12 до 50 символов (Латинские прописные и строчные буквы, специальные символы)!");
                return false;
            }
            return true;
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

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }

        private void buttonRegCreate_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAllData())
            {
                try
                {
                    newUser.Login = tbLogin.Text;
                    newUser.Password = pbPassword.Password;
                    newUser.Surname = tbSurname.Text;
                    newUser.Name = tbName.Text;
                    newUser.Patronymic = tbPatronymic.Text;
                    var role = AppConnect.ModelDB.Roles.FirstOrDefault(x => x.Name == "Сотрудник");
                    newUser.IdRole = role.IdRole;
                    newUser.Email = tbEmail.Text;
                    newUser.Phone = tbPhone.Text;

                    AppConnect.ModelDB.Users.Add(newUser);

                    AppConnect.ModelDB.SaveChanges();
                    AppConnect.ModelDB.SaveChanges();

                    MessageBox.Show("Регистрация прошла успешно!", "Регистрация успешна", MessageBoxButton.OK, MessageBoxImage.Information);
                    AppFrame.frameMain.Navigate(new PageLogin());
                }
                catch
                {
                    MessageBox.Show("Ошибка, попробуйте зарегистрироваться позже!", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void tbPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9]") || tbPhone.Text.Length >= 11)
            {
                e.Handled = true;
            }
        }
    }
}
