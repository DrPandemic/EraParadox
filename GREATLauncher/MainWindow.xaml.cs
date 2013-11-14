using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GREATLauncher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private ApiClient client;
        private ApiClient.User user;

		public MainWindow(ApiClient client, ApiClient.User user)
		{
            this.client = client;
            this.user = user;
			
            this.InitializeComponent();

            this.Title = "EraParadox - " + this.user.username;
		}

        private void titleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
	}
}