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

namespace ExamGenerator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.SizeChanged += MainWindow_SizeChanged;

            //TODO Unhandled Exception Handling

            ExamGeneratorContext.Load();

            this.DataContext = ExamGeneratorContext.Instance;

            this.Closing += (sender, args) => ExamGeneratorContext.Save();
        }

        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var basesize = new Size(920, 550);
            var ratio_X = e.NewSize.Width / basesize.Width;
            var ratio_Y = e.NewSize.Height / basesize.Height;
            var z = (ratio_X + ratio_Y) / 2;
            if (z < 0.6) this.FontSize = 6;
            else if (z < 0.8) this.FontSize = 8;
            else if (z < 1.0) this.FontSize = 10;
            else if (z < 1.2) this.FontSize = 12;
            else if (z < 1.4) this.FontSize = 14;
            else if (z < 1.6) this.FontSize = 16;
            else if (z < 1.8) this.FontSize = 18;
            else this.FontSize = 20;
        }
    }
}
