using System.Windows.Controls;


namespace ExamGenerator.Pages
{
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            DataContext = ExamGeneratorContext.Instance;
        }
    }
}
