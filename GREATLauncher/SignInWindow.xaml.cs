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

namespace GREATLauncher
{
    /// <summary>
    /// Interaction logic for SignInWindow.xaml
    /// </summary>
    public partial class SignInWindow : Window
    {
        public SignInWindow()
        {
            InitializeComponent();
        }

        private void titleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void signInButton_Click(object sender, RoutedEventArgs e)
        {
            this.signInButton.Visibility = Visibility.Hidden;
            this.signInMarqueeControl.Visibility = Visibility.Visible;

            ApiClient client = new ApiClient();
            if (await client.SignIn(this.emailTextBox.Text, this.passwordTextBox.Password)) {
                new MainWindow(client, await client.GetUser()).Show();
                this.Close();
            }

            this.signInMarqueeControl.Visibility = Visibility.Hidden;
            this.signInButton.Visibility = Visibility.Visible;
        }
    }
}
