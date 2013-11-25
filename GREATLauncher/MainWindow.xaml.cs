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
using System.Windows.Threading;

namespace GREATLauncher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private ApiClient client;
        private ApiClient.User user;

		public MainWindow(ApiClient client)
		{
            this.client = client;
			
            this.InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                this.serverTimeLabel.Content = "Server Time " + DateTime.Now.ToString("HH:mm:ss");
            }, this.Dispatcher);
		}

        private void titleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void window_Loaded(object sender, RoutedEventArgs e)
        {
            this.user = await this.client.GetUser();

            this.welcomeLabel.Content = "Welcome " + this.user.username;

            this.friendsStackPanel.Children.Add(new FriendControl(new ApiClient.User() { username = "Bob" }, false));
            this.friendsStackPanel.Children.Add(new FriendControl(new ApiClient.User() { username = "Nigguh" }, true));
            this.friendsStackPanel.Children.Add(new FriendControl(new ApiClient.User() { username = "Faggit" }, false));
            this.friendsStackPanel.Children.Add(new FriendControl(new ApiClient.User() { username = "OP" }, true));

            this.loadingGrid.Visibility = System.Windows.Visibility.Hidden;
            this.mainGrid.Visibility = System.Windows.Visibility.Visible;
        }
	}
}