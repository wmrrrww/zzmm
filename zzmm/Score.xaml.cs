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
using System.Windows.Shapes;

namespace zzmm
{
    /// <summary>
    /// Логика взаимодействия для Name.xaml
    /// </summary>
    public partial class InputNameDialog : Window
    {
        public InputNameDialog()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Устанавливаем DialogResult в true
            Close(); // Закрываем окно
        }

        // Обработчик события нажатия кнопки "Cancel"
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Устанавливаем DialogResult в false
            Close(); // Закрываем окно
        }

        // Свойство для получения введенного имени игрока
        public string PlayerName
        {
            get { return playerNameTextBox.Text; }
        }

       

    }
}
