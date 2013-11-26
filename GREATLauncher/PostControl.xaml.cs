using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for PostControl.xaml
    /// </summary>
    public partial class PostControl : UserControl
    {
        private ApiClient.Post post;
        public ApiClient.Post Post {
            get
            {
                return this.post;
            }
        }

        public PostControl(ApiClient.Post post)
        {
            this.post = post;

            InitializeComponent();

            this.titleLabel.Content = this.post.title;
            this.contentLabel.Content = (this.post.content.Length > 160) ? this.post.content.Substring(0, 160) + "..." : this.post.content;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE001A1C"));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = Brushes.Transparent;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://eraparadox.com/posts/" + this.post.id.ToString());
        }
    }
}
