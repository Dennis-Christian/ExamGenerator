using System.Windows.Controls;

namespace ExamGenerator.Pages
{
    public partial class Exams : Page
    {
        public Exams()
        {
            InitializeComponent();
            DataContext = ExamGeneratorContext.Instance;
        }
    }
}
