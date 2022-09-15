using System.Windows.Controls;

namespace ExamGenerator.Pages
{
    public partial class Questions : Page
    {
        public Questions()
        {
            InitializeComponent();
            DataContext = ExamGeneratorContext.Instance;
        }
    }
}
