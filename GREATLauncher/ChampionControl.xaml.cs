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
    /// Interaction logic for ChampionControl.xaml
    /// </summary>
    public partial class ChampionControl : UserControl
    {
        public ImageSource Source {
            get
            {
                return this.championImage.Source;
            }
            set
            {
                this.championImage.Source = value;
            }
        }

        public string Champion
        {
            get
            {
                return this.championLabel.Content.ToString();
            }
            set
            {
                this.championLabel.Content = value;
            }
        }

        private bool selected = false;
        public bool IsSelected
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                if (this.selected) {
                    this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE001A1C"));
                } else {
                    this.Background = Brushes.Transparent;
                }
            }
        }

        public ChampionControl()
        {
            InitializeComponent();
        }
    }
}
