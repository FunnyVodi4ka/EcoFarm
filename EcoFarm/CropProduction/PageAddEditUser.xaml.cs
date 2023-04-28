using EcoFarm.AppConnection;
using EcoFarm.Authentication;
using EcoFarm.DatabaseConnection;
using EcoFarm.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для PageAddEditUser.xaml
    /// </summary>
    public partial class PageAddEditUser : Page
    {
        AccessVerification access = new AccessVerification();
        ValidationClass validation = new ValidationClass();
        private Users currentUser = new Users();

        public PageAddEditUser(Users user)
        {
            access.CheckAdminAccess();

            InitializeComponent();

            SetRoles();
            if (user != null)
            {
                currentUser = user;

                FindRole();
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
            if (!validation.CheckUniqueEmail(tbEmail.Text, currentUser.IdUser))
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
            if (!validation.CheckUniquePhone(tbPhone.Text, currentUser.IdUser))
            {
                tbPhone.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Пользователь с таким телефоном уже зарегистрирован!");
                return false;
            }
            if (!validation.CheckEmail(tbPhone.Text))
            {
                tbPhone.BorderBrush = Brushes.Red;
                MessageBox.Show("Ошибка: Ошибка: Некорректный телефон (Пример: 89998887766)!");
                return false;
            }
            if (!validation.CheckUniqueLogin(tbLogin.Text, currentUser.IdUser))
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
                    MessageBox.Show("Ошибка: Пароль должен содержать от 6 до 50 символов (Латинские прописные и строчные буквы, специальные символы)!");
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
                        currentUser.Password = pbPassword.Password;
                    currentUser.Surname = tbSurname.Text;
                    currentUser.Name = tbName.Text;
                    currentUser.Patronymic = tbPatronymic.Text;
                    currentUser.Email = tbEmail.Text;
                    currentUser.Phone = tbPhone.Text;
                    SetRole();

                    if (currentUser.IdUser == 0)
                    {
                        EcoFarmDBEntities.GetContext().Users.Add(currentUser);
                    }
                    EcoFarmDBEntities.GetContext().SaveChanges();
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
