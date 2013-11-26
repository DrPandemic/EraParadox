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
		}

        private void titleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void window_Loaded(object sender, RoutedEventArgs e)
        {
            this.user = await this.client.GetUser();
            this.welcomeLabel.Content = "Welcome " + this.user.username;

            foreach (ApiClient.User user in await this.client.GetFriends()) {
                this.friendsStackPanel.Children.Add(new FriendControl(user, false));
            }

            foreach (ApiClient.Post post in await this.client.GetPosts()) {
                this.blogStackPanel.Children.Add(new PostControl(post));
            }

            this.loadingGrid.Visibility = Visibility.Hidden;
            this.mainGrid.Visibility = Visibility.Visible;
        }

        private void quickMatchButton_Click(object sender, RoutedEventArgs e)
        {
            this.mainGrid.Visibility = Visibility.Hidden;
            this.preGameGrid.Visibility = Visibility.Visible;
        }

        private async void addFriendButton_Click(object sender, RoutedEventArgs e)
        {
            this.friendTextBox.Visibility = Visibility.Hidden;
            this.addFriendButton.Visibility = Visibility.Hidden;
            this.friendMarqueeControl.Visibility = Visibility.Visible;

            if (await this.client.AddFriend(this.friendTextBox.Text)) {
                ApiClient.User[] users = await this.client.GetFriends();
                this.friendsStackPanel.Children.Clear();
                foreach (ApiClient.User user in users) {
                    this.friendsStackPanel.Children.Add(new FriendControl(user, false));
                }
            }

            this.friendTextBox.Text = null;
            this.friendMarqueeControl.Visibility = Visibility.Hidden;
            this.friendTextBox.Visibility = Visibility.Visible;
            this.addFriendButton.Visibility = Visibility.Visible;
        }

        private void friendTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                addFriendButton_Click(sender, e);
            }
        }
	}
}