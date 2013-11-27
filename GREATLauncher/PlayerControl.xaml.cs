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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GREATLauncher
{
	/// <summary>
	/// Interaction logic for PlayerControl.xaml
	/// </summary>
	public partial class PlayerControl : UserControl
	{
        private ApiClient.User user;
        public ApiClient.User User {
            get
            {
                return this.user;
            }
        }
        
        private bool online;
        public bool Online {
            get
            {
                return this.online;
            }
            set
            {
                this.online = value;
                if (this.online) {
                    this.statusEllipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1CEBFF"));
                } else {
                    this.statusEllipse.Fill = Brushes.Transparent;
                }
            }
        }

		public PlayerControl(ApiClient.User user, bool online = false)
		{
            this.user = user;

			this.InitializeComponent();

            this.usernameLabel.Content = this.user.username;
            this.Online = online;
		}

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE001A1C"));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = Brushes.Transparent;
        }
	}
}