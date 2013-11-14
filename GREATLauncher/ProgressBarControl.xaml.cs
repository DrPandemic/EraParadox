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
	/// Interaction logic for ProgressBarControl.xaml
	/// </summary>
	public partial class ProgressBarControl : UserControl
	{
        private double min = 0;
        public double Min
        {
            get
            {
                return this.min;
            }
            set
            {
                this.min = value;
                this.Value = this.value;
            }
        }

		private double max = 1;
        public double Max
        {
            get
            {
                return this.max;
            }
            set
            {
                this.max = value;
                this.Value = this.value;
            }
        }

        private double value = 1;
        public double Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (value > this.max) this.value = this.max;
                else if (value < this.min) this.value = this.min;
                else this.value = value;
                this.progressRectangle.Width = (this.progressBorder.ActualWidth - 4) / (this.max - this.min) * (this.value - this.min);
                this.progressLabel.Content = Math.Floor((this.value - this.min) / (this.max - this.min) * 100).ToString() + "%";
            }
        }
		
		public ProgressBarControl()
		{
			this.InitializeComponent();
		}
	}
}