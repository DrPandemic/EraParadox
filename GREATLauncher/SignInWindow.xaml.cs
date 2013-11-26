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

            this.emailTextBox.Text = Properties.Settings.Default.email;
        }

        private void titleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void signInButton_Click(object sender, RoutedEventArgs e)
        {
            this.errorLabel.Content = "";

            this.emailTextBox.IsEnabled = false;
            this.passwordTextBox.IsEnabled = false;
            this.rememberCheckBox.IsEnabled = false;

            this.signInButton.Visibility = Visibility.Hidden;
            this.signInMarqueeControl.Visibility = Visibility.Visible;

            ApiClient client = new ApiClient();
            if (await client.SignIn(this.emailTextBox.Text, this.passwordTextBox.Password)) {
                Properties.Settings.Default.email = (this.rememberCheckBox.IsChecked.HasValue && (bool)this.rememberCheckBox.IsChecked) ? this.emailTextBox.Text : null;
                Properties.Settings.Default.Save();
                
                new MainWindow(client).Show();
                this.Close();
            } else {
                this.passwordTextBox.Password = null;
                this.errorLabel.Content = "Invalid username and/or password";
            }

            this.emailTextBox.IsEnabled = true;
            this.passwordTextBox.IsEnabled = true;
            this.rememberCheckBox.IsEnabled = true;

            this.signInMarqueeControl.Visibility = Visibility.Hidden;
            this.signInButton.Visibility = Visibility.Visible;
        }

        private void emailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                signInButton_Click(sender, e);
            }
        }
    }
}
