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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApiClient client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        	DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.client = new ApiClient();
            if (this.client.SignIn(this.emailTextBox.Text, this.passwordTextBox.Password)) {
                this.client.SignOut();
            }
        }
    }
}
