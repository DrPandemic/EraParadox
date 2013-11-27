using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private ApiClient.Game game;

        private DispatcherTimer gameUpdater;

		public MainWindow(ApiClient client)
		{
            this.client = client;
			
            this.InitializeComponent();

            this.gameUpdater = new DispatcherTimer(new TimeSpan(0, 0, 3), DispatcherPriority.Normal, async delegate {
                ApiClient.Game g = await this.client.GetGame(this.game.id);
                if (g != null) this.game = g;

                if (this.game.status == "matchmaking" && this.statusLabel.Content.ToString() != "Waiting for players...") {
                    this.statusLabel.Content = "Waiting for players...";
                } else if (this.game.status == "waiting" && this.statusLabel.Content.ToString() != "Waiting for the server...") {
                    this.statusLabel.Content = "Waiting for the server...";
                    this.forceStartButton.IsEnabled = false;
                } else if (this.game.status == "started" && this.statusLabel.Content.ToString() != "Game is starting...") {
                    this.statusLabel.Content = "Game is starting...";
                    this.forceStartButton.IsEnabled = false;
                    this.manmegaChampionControl.IsEnabled = false;
                    this.zoroChampionControl.IsEnabled = false;
                    this.gameUpdater.Stop();

                    Process p = Process.Start(@"GREATClient\bin\Release\GREATClient.exe", this.game.server.host + " " + this.game.server.port + " " + ((this.zoroChampionControl.IsSelected) ? 1 : 0));

                    this.preGameGrid.Visibility = Visibility.Hidden;
                    this.mainGrid.Visibility = Visibility.Visible;
                }

                if (this.game.users.Count != this.playersStackPanel.Children.Count) {
                    this.playersStackPanel.Children.Clear();
                    foreach (ApiClient.User user in this.game.users) {
                        this.playersStackPanel.Children.Add(new PlayerControl(user, true));
                    }
                }
            }, this.Dispatcher);
            this.gameUpdater.Stop();
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
                this.friendsStackPanel.Children.Add(new PlayerControl(user, false));
            }

            foreach (ApiClient.Post post in await this.client.GetPosts()) {
                this.blogStackPanel.Children.Add(new PostControl(post));
            }

            this.loadingGrid.Visibility = Visibility.Hidden;
            this.mainGrid.Visibility = Visibility.Visible;
        }

        private async void quickMatchButton_Click(object sender, RoutedEventArgs e)
        {
            this.mainGrid.Visibility = Visibility.Hidden;
            this.loadingGrid.Visibility = Visibility.Visible;

            this.statusLabel.Content = "Waiting for players...";
            this.manmegaChampionControl.IsEnabled = true;
            this.zoroChampionControl.IsEnabled = true;
            this.forceStartButton.IsEnabled = true;

            this.game = await this.client.GetGame();

            this.playersStackPanel.Children.Clear();
            foreach (ApiClient.User user in this.game.users) {
                this.playersStackPanel.Children.Add(new PlayerControl(user, true));
            }

            this.gameUpdater.Start();

            this.loadingGrid.Visibility = Visibility.Hidden;
            this.preGameGrid.Visibility = Visibility.Visible;
        }

        private async void addFriendButton_Click(object sender, RoutedEventArgs e)
        {
            this.friendTextBox.Visibility = Visibility.Hidden;
            this.addFriendButton.Visibility = Visibility.Hidden;
            this.friendMarqueeControl.Visibility = Visibility.Visible;

            if (await this.client.CreateFriend(this.friendTextBox.Text)) {
                this.friendsStackPanel.Children.Clear();
                foreach (ApiClient.User user in await this.client.GetFriends()) {
                    this.friendsStackPanel.Children.Add(new PlayerControl(user, false));
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

        private void manmegaChampionControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.manmegaChampionControl.IsSelected = true;
            this.zoroChampionControl.IsSelected = false;
        }

        private void zoroChampionControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.manmegaChampionControl.IsSelected = false;
            this.zoroChampionControl.IsSelected = true;
        }

        private async void forceStartButton_Click(object sender, RoutedEventArgs e)
        {
            this.forceStartButton.IsEnabled = false;
            if (await this.client.UpdateGame(this.game.id, 1) == null) this.forceStartButton.IsEnabled = true;
        }
	}
}