using System.Collections.Generic;
using System.Windows;

namespace ExamGenerator
{
    /// <summary>
    /// Contains uservisible Labels
    /// </summary>
    public class LabelsContainer
    {
        string welcomeTextHeader = "Welcome to the Exam Generator";
        string welcomeTextParagraph = "This tool is designed to help you generate exams for your students. See the manual for a how-to and contact information.";
        string statisticTextHeader = "Statistical Information";
        string problematicHeader = "Problematic Entries";
        string exam = "Exam";
        string exams = "Exams";
        string category = "Category";
        string categories = "Categories";
        string preset = "Preset";
        string presets = "Presets";
        string question = "Question";
        string questions = "Questions";

        public string Exams { get => exams; }
        public string Categories { get => categories; }
        public string Presets { get => presets; }
        public string Questions { get => questions; }

        public string WelcomeTextHeader { get => welcomeTextHeader; }
        public string WelcomeTextParagraph { get => welcomeTextParagraph; }
        public string StatisticTextHeader { get => statisticTextHeader; }
        public List<string> StatisticEntries { get => GetStatisticEntries(); }

        public Visibility ShowProblematicEntries { get { return HasProblematicEntries() ? Visibility.Visible : Visibility.Collapsed; } }
        public string ProblematicHeader { get => problematicHeader; }
        public List<string> ProblematicEntries { get => GetProblematicEntries(); }

        private List<string> GetStatisticEntries()
        {
            var list = new List<string>();

            string template = "{0}: {1}";

            list.Add(string.Format(template, Exams, ExamGeneratorContext.ExamCatalogue.Count));
            list.Add(string.Format(template, Questions, ExamGeneratorContext.QuestionCatalogue.Count));
            list.Add(string.Format(template, Categories, ExamGeneratorContext.CategoryCatalogue.Count));
            list.Add(string.Format(template, Presets, ExamGeneratorContext.PresetCatalogue.Count));

            return list;
        }

        private bool HasProblematicEntries()
        {
            //TODO HasProblematicEntries()
            //todo return true on first problematic entry
            return false;
        }

        private List<string> GetProblematicEntries()
        {
            //TODO GetProblematicEntries()

            var list = new List<string>();

            if (HasProblematicEntries())
            {
                //TODO add problematic entries
            }

            return list;
        }
    }
}
