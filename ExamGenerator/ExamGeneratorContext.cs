using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExamGenerator
{
    partial class ExamGeneratorContext : INotifyPropertyChanged
    {
        #region singleton implementation

        static readonly ExamGeneratorContext instance = new();

        public static ExamGeneratorContext Instance { get { return instance; } }

        #endregion

        private ExamGeneratorContext()
        {
            rng = new();
            SelectedView = new();
            SelectedView.SelectedViewChanged += (sender, args) => OnPropertyChanged(nameof(Breadcrumb));

            labels = new();

            CategoryCatalogue = new();
            PresetCatalogue = new ObservableCollection<Preset>();
            QuestionCatalogue = new ObservableCollection<Question>();
            ExamCatalogue = new ObservableCollection<Exam>();

            CategoryCatalogue.CollectionChanged += (sender, e) => CategoryCatalogueHasChanges = true;
            PresetCatalogue.CollectionChanged += (sender, e) => PresetCatalogueHasChanges = true;
            QuestionCatalogue.CollectionChanged += (sender, e) => QuestionCatalogueHasChanges = true;
            ExamCatalogue.CollectionChanged += (sender, e) => ExamCatalogueHasChanges = true;
        }

        #region fields

        static LabelsContainer labels;
        static Random rng;

        #endregion

        #region properties

        public LabelsContainer Labels { get { return labels; } }

        public static Question SelectedQuestion
        {
            get;
            set;
        }

        public static Category SelectedCategory
        {
            get;
            set;
        }

        public static Exam SelectedExam
        {
            get;
            set;
        }

        public static Preset SelectedPreset
        {
            get;
            set;
        }

        public static ObservableCollection<Question> QuestionCatalogue
        {
            get;
            private set;
        }

        public static ObservableCollection<Exam> ExamCatalogue
        {
            get;
            private set;
        }

        public static ObservableCollection<Category> CategoryCatalogue
        {
            get;
            private set;
        }

        public static ObservableCollection<Preset> PresetCatalogue
        {
            get;
            private set;
        }

        #endregion

        #region Commands

        ICommand _createKlausurCommand;
        ICommand _editKlausurCommand;

        ICommand _createFrageCommand;
        ICommand _editFrageCommand;
        ICommand _deleteFrageCommand;

        ICommand _createPresetCommand;
        ICommand _editPresetCommand;
        ICommand _deletePresetCommand;

        ICommand _createKategorieCommand;
        ICommand _editKategorieCommand;
        ICommand _deleteKategorieCommand;

        public ICommand CreateKlausurCommand { get { return _createKlausurCommand ??= new CommandHandler(CreateKlausurCommandExecute, () => CreateKlausurCommandCanExecute); } }
        public ICommand EditKlausurCommand { get { return _editKlausurCommand ??= new CommandHandler(EditKlausurCommandExecute, () => EditKlausurCommandCanExecute); } }

        public ICommand CreateFrageCommand { get { return _createFrageCommand ??= new CommandHandler(CreateFrageCommandExecute, () => CreateFrageCommandCanExecute); } }
        public ICommand EditFrageCommand { get { return _editFrageCommand ??= new CommandHandler(EditFrageCommandExecute, () => EditFrageCommandCanExecute); } }
        public ICommand DeleteFrageCommand { get { return _deleteFrageCommand ??= new CommandHandler(DeleteFrageCommandExecute, () => DeleteFrageCommandCanExecute); } }

        public ICommand CreatePresetCommand { get { return _createPresetCommand ??= new CommandHandler(CreatePresetCommandExecute, () => CreatePresetCommandCanExecute); } }
        public ICommand EditPresetCommand { get { return _editPresetCommand ??= new CommandHandler(EditPresetCommandExecute, () => EditPresetCommandCanExecute); } }
        public ICommand DeletePresetCommand { get { return _deletePresetCommand ??= new CommandHandler(DeletePresetCommandExecute, () => DeletePresetCommandCanExecute); } }

        public ICommand CreateKategorieCommand { get { return _createKategorieCommand ??= new CommandHandler(CreateKategorieCommandExecute, () => CreateKategorieCommandCanExecute); } }
        public ICommand EditKategorieCommand { get { return _editKategorieCommand ??= new CommandHandler(EditKategorieCommandExecute, () => EditKategorieCommandCanExecute); } }
        public ICommand DeleteKategorieCommand { get { return _deleteKategorieCommand ??= new CommandHandler(DeleteKategorieCommandExecute, () => DeleteKategorieCommandCanExecute); } }

        public bool CreateKlausurCommandCanExecute
        {
            get
            {
                if (ExamGeneratorContext.PresetCatalogue.Count == 0) return false;
                if (ExamGeneratorContext.CategoryCatalogue.Count == 0) return false;
                if (ExamGeneratorContext.QuestionCatalogue.Count == 0) return false;

                foreach (var element in ExamGeneratorContext.CategoryCatalogue)
                {
                    if (ExamGeneratorContext.QuestionCatalogue.Count(x => x.Category.Description == element.Description) < 5)
                        return false;
                }

                return true;
            }
        }

        public bool EditKlausurCommandCanExecute
        {
            get
            {
                return SelectedExam != null;
            }
        }

        public bool CreateFrageCommandCanExecute
        {
            get
            {
                if (CategoryCatalogue.Count == 0) return false;

                return true;
            }
        }

        public bool EditFrageCommandCanExecute
        {
            get
            {
                return SelectedQuestion != null;
            }
        }

        public bool DeleteFrageCommandCanExecute
        {
            get
            {
                return SelectedQuestion != null;
            }
        }

        public bool CreatePresetCommandCanExecute
        {
            get
            {
                return true;
            }
        }

        public bool EditPresetCommandCanExecute
        {
            get
            {
                return SelectedPreset != null;
            }
        }

        public bool DeletePresetCommandCanExecute
        {
            get
            {
                return SelectedPreset != null;
            }
        }

        public bool CreateKategorieCommandCanExecute
        {
            get
            {
                return true;
            }
        }

        public bool EditKategorieCommandCanExecute
        {
            get
            {
                return SelectedCategory != null;
            }
        }

        public bool DeleteKategorieCommandCanExecute
        {
            get
            {
                return SelectedCategory != null;
            }
        }

        public void CreateKlausurCommandExecute()
        {
            SelectedExam = null;
            SelectedView.SelectedView = Views.ExamCreate;
        }

        public void EditKlausurCommandExecute()
        {
            if (SelectedExam != null)
                SelectedView.SelectedView = Views.ExamDetail;
        }

        public void CreateFrageCommandExecute()
        {
            SelectedQuestion = null;
            SelectedView.SelectedView = Views.QuestionDetail;
        }

        public void EditFrageCommandExecute()
        {
            if (SelectedQuestion != null)
                SelectedView.SelectedView = Views.QuestionDetail;
        }

        public void DeleteFrageCommandExecute()
        {
            if (SelectedQuestion != null)
            {
                QuestionCatalogue.Remove(SelectedQuestion);
                SelectedQuestion = null;
            }
        }

        public void CreatePresetCommandExecute()
        {
            SelectedPreset = null;
            SelectedView.SelectedView = Views.PresetDetail;
        }

        public void EditPresetCommandExecute()
        {
            if (SelectedPreset != null)
                SelectedView.SelectedView = Views.PresetDetail;
        }

        public void DeletePresetCommandExecute()
        {
            if (SelectedPreset != null)
            {
                PresetCatalogue.Remove(SelectedPreset);
                SelectedPreset = null;
            }
        }

        public void CreateKategorieCommandExecute()
        {
            SelectedCategory = null;
            SelectedView.SelectedView = Views.CategoryDetail;
        }

        public void EditKategorieCommandExecute()
        {
            if (SelectedCategory != null)
                SelectedView.SelectedView = Views.CategoryDetail;
        }

        public void DeleteKategorieCommandExecute()
        {
            if (SelectedCategory != null)
            {
                CategoryCatalogue.Remove(SelectedCategory);
                SelectedCategory = null;
            }
        }

        #endregion

        #region methods

        public void Navigate(Frame frame, Page page)
        {
            frame.NavigationService.Navigate(page);

            if (page is Pages.Dashboard && !SelectedView.Dashboard)
                SelectedView.Dashboard = true;

            if (page is Pages.Categories && !SelectedView.Categories)
                SelectedView.Categories = true;

            if (page is Pages.CategoryDetail && !SelectedView.CategoryDetails)
                SelectedView.CategoryDetails = true;

            if (page is Pages.ExamCreate && !SelectedView.ExamCreate)
                SelectedView.ExamCreate = true;

            if (page is Pages.ExamDetail && !SelectedView.ExamDetails)
                SelectedView.ExamDetails = true;

            if (page is Pages.Exams && !SelectedView.Exams)
                SelectedView.Exams = true;

            if (page is Pages.Manual && !SelectedView.Manual)
                SelectedView.Manual = true;

            if (page is Pages.Options && !SelectedView.Options)
                SelectedView.Options = true;

            if (page is Pages.PresetDetail && !SelectedView.PresetDetails)
                SelectedView.PresetDetails = true;

            if (page is Pages.Presets && !SelectedView.Presets)
                SelectedView.Presets = true;

            if (page is Pages.QuestionDetail && !SelectedView.QuestionDetails)
                SelectedView.QuestionDetails = true;

            if (page is Pages.Questions && !SelectedView.Questions)
                SelectedView.Questions = true;
        }

        public static int GetNextCategoryId()
        {
            if (CategoryCatalogue == null)
            {
                return 0;
            }

            if (CategoryCatalogue.Count == 0)
            {
                return 0;
            }

            return 1 + CategoryCatalogue.Max(x => x.Id);
        }

        public static int GetNextQuestionId()
        {
            if (QuestionCatalogue == null)
            {
                return 0;
            }

            if (QuestionCatalogue.Count == 0)
            {
                return 0;
            }

            return 1 + QuestionCatalogue.Max(x => x.Id);
        }

        public static int GetNextExamId()
        {
            if (ExamCatalogue == null)
            {
                return 0;
            }

            if (ExamCatalogue.Count == 0)
            {
                return 0;
            }

            return 1 + ExamCatalogue.Max(x => x.Id);
        }

        public static int GetNextPresetId()
        {
            if (PresetCatalogue == null)
            {
                return 0;
            }

            if (PresetCatalogue.Count == 0)
            {
                return 0;
            }

            return 1 + PresetCatalogue.Max(x => x.Id);
        }

        public static Category GetRandomCategory()
        {
            var i = rng.Next(0, CategoryCatalogue.Count);
            return CategoryCatalogue[i];
        }

        public static Question GetRandomQuestion(Category k, DifficultyLevel s, List<int> ExclusionIds)
        {
            var possibleQuestions = ExamGeneratorContext.QuestionCatalogue.Where(x => x.Category == k && x.Difficulty == s).ToList();

            if (possibleQuestions.Count == 0)
                throw new DataException("Keine Treffer möglich, bitte mehr Fragen der Kategorie \"" + k.Description + "\" und Schwierigkeit \"" + s.ToString() + "\" eingeben");

            foreach (var element in ExclusionIds)
                possibleQuestions.RemoveAll(x => x.Id == element);

            if (possibleQuestions.Count == 0)
                throw new DataException("Keine Treffer nach Dublettenkontrolle möglich, bitte mehr Fragen der Kategorie \"" + k.Description + "\" und Schwierigkeit \"" + s.ToString() + "\" eingeben");

            return possibleQuestions[rng.Next(0, possibleQuestions.Count)];
        }

        static bool FileExist(string filename)
        {
            if (!File.Exists(filename))
                return false;
            return true;
        }

        static void BackupFile(string filename)
        {
            File.Copy(filename, filename + ".bak", true);
        }

        #endregion

        #region save/load

        static string CategoriesFileName = "Categories.json";
        static string QuestionsFileName = "Questions.json";
        static string PresetsFileName = "Presets.json";
        static string ExamsFileName = "Exams.json";

        static bool CategoryCatalogueHasChanges = false;
        static bool PresetCatalogueHasChanges = false;
        static bool QuestionCatalogueHasChanges = false;
        static bool ExamCatalogueHasChanges = false;

        public static void Load()
        {
            Load(CategoriesFileName, CategoryCatalogue);
            Load(QuestionsFileName, QuestionCatalogue);
            Load(PresetsFileName, PresetCatalogue);
            Load(ExamsFileName, ExamCatalogue);

            //TODO Load Options
        }

        static void Load<T>(string FileName, ObservableCollection<T> Collection)
        {
            if (FileExist(FileName))
                using (var w = new StreamReader(FileName))
                {
                    while (true)
                    {
                        string line = w.ReadLine();
                        if (line == null)
                        {
                            break;
                        }

                        var o = ExtensionMethods.FromJSON<T>(line);
                        Collection.Add((T)o);
                    }
                }
        }

        public static void Save()
        {
            Save(CategoryCatalogueHasChanges, CategoriesFileName, CategoryCatalogue);
            Save(PresetCatalogueHasChanges, PresetsFileName, PresetCatalogue);
            Save(QuestionCatalogueHasChanges, QuestionsFileName, QuestionCatalogue);
            Save(ExamCatalogueHasChanges, ExamsFileName, ExamCatalogue);

            //TODO Save Options
        }

        static void Save(bool HasChanges, string FileName, IEnumerable<IJsonConvertable> Collection)
        {
            if (HasChanges)
            {
                if (FileExist(FileName))
                    BackupFile(FileName);

                using (StreamWriter w = new(FileName))
                    foreach (IJsonConvertable element in Collection)
                        w.WriteLine(element.ToJSON());
            }
        }

        #endregion

        #region SelectedViewSubcontext implementation

        public static SelectedViewSubcontext SelectedView { get; set; }

        public enum Views
        {
            Dashboard,
            Exams,
            ExamDetail,
            ExamCreate,
            Questions,
            QuestionDetail,
            Categories,
            CategoryDetail,
            Presets,
            PresetDetail,
            Manual,
            Options
        }

        public class SelectedViewSubcontext : INotifyPropertyChanged
        {
            public SelectedViewSubcontext()
            {
                Dashboard = true;
            }

            public bool Dashboard
            {
                get { return dashboard; }
                set
                {
                    SetField(ref dashboard, value, nameof(Dashboard));
                    this.SelectedView = Views.Dashboard;
                }
            }
            bool dashboard;

            public bool Exams
            {
                get { return exams; }
                set
                {
                    SetField(ref exams, value, nameof(Exams));
                    this.SelectedView = Views.Exams;
                }
            }
            bool exams;

            public bool ExamDetails
            {
                get { return examDetails; }
                set
                {
                    SetField(ref examDetails, value, nameof(ExamDetails));
                    this.SelectedView = Views.ExamDetail;
                }
            }
            bool examDetails;

            public bool ExamCreate
            {
                get { return examCreate; }
                set
                {
                    SetField(ref examCreate, value, nameof(ExamCreate));
                    this.SelectedView = Views.ExamCreate;
                }
            }
            bool examCreate;

            public bool Questions
            {
                get { return questions; }
                set
                {
                    SetField(ref questions, value, nameof(Questions));
                    this.SelectedView = Views.Questions;
                }
            }
            bool questions;

            public bool QuestionDetails
            {
                get { return questionDetails; }
                set
                {
                    SetField(ref questionDetails, value, nameof(QuestionDetails));
                    this.SelectedView = Views.QuestionDetail;
                }
            }
            bool questionDetails;

            public bool Categories
            {
                get { return categories; }
                set
                {
                    SetField(ref categories, value, nameof(Categories));
                    this.SelectedView = Views.Categories;
                }
            }
            bool categories;

            public bool CategoryDetails
            {
                get { return categoryDetails; }
                set
                {
                    SetField(ref categoryDetails, value, nameof(CategoryDetails));
                    this.SelectedView = Views.CategoryDetail;
                }
            }
            bool categoryDetails;

            public bool Presets
            {
                get { return presets; }
                set
                {
                    SetField(ref presets, value, nameof(Presets));
                    this.SelectedView = Views.Presets;
                }
            }
            bool presets;

            public bool PresetDetails
            {
                get { return presetDetails; }
                set
                {
                    SetField(ref presetDetails, value, nameof(PresetDetails));
                    this.SelectedView = Views.PresetDetail;
                }
            }
            bool presetDetails;

            public bool Manual
            {
                get { return manual; }
                set
                {
                    SetField(ref manual, value, nameof(Manual));
                    this.SelectedView = Views.Manual;
                }
            }
            bool manual;

            public bool Options
            {
                get { return options; }
                set
                {
                    SetField(ref options, value, nameof(Options));
                    this.SelectedView = Views.Options;
                }
            }
            bool options;

            public Views SelectedView
            {
                get { return selectedView; }
                set
                {
                    dashboard = false;
                    exams = false;
                    examCreate = false;
                    examDetails = false;
                    questions = false;
                    questionDetails = false;
                    categories = false;
                    categoryDetails = false;
                    presets = false;
                    presetDetails = false;
                    manual = false;
                    options = false;
                    selectedView = value;

                    switch (selectedView)
                    {
                        case Views.Dashboard:
                            dashboard = true;
                            break;

                        case Views.Exams:
                            exams = true;
                            break;

                        case Views.ExamDetail:
                            exams = true;
                            examDetails = true;
                            break;

                        case Views.ExamCreate:
                            exams = true;
                            examCreate = true;
                            break;

                        case Views.Questions:
                            questions = true;
                            break;

                        case Views.QuestionDetail:
                            questions = true;
                            questionDetails = true;
                            break;

                        case Views.Categories:
                            categories = true;
                            break;

                        case Views.CategoryDetail:
                            categories = true;
                            categoryDetails = true;
                            break;

                        case Views.Presets:
                            presets = true;
                            break;

                        case Views.PresetDetail:
                            presets = true;
                            presetDetails = true;
                            break;

                        case Views.Manual:
                            manual = true;
                            break;

                        case Views.Options:
                            options = true;
                            break;

                        default:
                            break;
                    }

                    OnPropertyChanged(nameof(Dashboard));
                    OnPropertyChanged(nameof(Exams));
                    OnPropertyChanged(nameof(ExamDetails));
                    OnPropertyChanged(nameof(ExamCreate));
                    OnPropertyChanged(nameof(Questions));
                    OnPropertyChanged(nameof(QuestionDetails));
                    OnPropertyChanged(nameof(Categories));
                    OnPropertyChanged(nameof(CategoryDetails));
                    OnPropertyChanged(nameof(Presets));
                    OnPropertyChanged(nameof(PresetDetails));
                    OnPropertyChanged(nameof(Manual));
                    OnPropertyChanged(nameof(Options));
                    OnPropertyChanged(nameof(SelectedView));
                    OnSelectedViewChanged();
                }

            }
            Views selectedView;

            #region INotifyPropertyChanged implementation

            public event PropertyChangedEventHandler PropertyChanged;

            public event EventHandler SelectedViewChanged;

            void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }

            void OnSelectedViewChanged()
            {
                EventHandler handler = SelectedViewChanged;
                if (handler != null)
                    handler(this, new EventArgs());
            }

            bool SetField<T>(ref T field, T value, string propertyName)
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                    return false;
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            #endregion
        }

        #endregion

        #region Headertext/Breadcrumb

        public string Breadcrumb
        {
            get
            {
                string Template = "{0} - #{1} \"{2}\"";

                switch (SelectedView.SelectedView)
                {
                    case Views.Dashboard:
                        return "Dashboard";

                    case Views.Manual:
                        return "Manual";

                    case Views.Questions:
                    case Views.QuestionDetail:
                        if (SelectedQuestion != null)
                        {
                            return string.Format(Template, "Questions", SelectedQuestion.Id, SelectedQuestion.Problem);
                        }
                        return "Questions";

                    case Views.Categories:
                    case Views.CategoryDetail:
                        if (SelectedCategory != null)
                        {
                            return string.Format(Template, "Categories", SelectedCategory.Id, SelectedCategory.Description);
                        }
                        return "Categories";

                    case Views.Exams:
                    case Views.ExamDetail:
                    case Views.ExamCreate:
                        if (SelectedExam != null)
                        {
                            return string.Format(Template, "Exams", SelectedExam.Id, SelectedExam.CourseNumber);
                        }
                        return "Exams";

                    case Views.Presets:
                    case Views.PresetDetail:
                        if (SelectedPreset != null)
                        {
                            return string.Format(Template, "Presets", SelectedPreset.Id, SelectedPreset.Description);
                        }
                        return "Presets";

                    case Views.Options:
                        return "Options";

                    default:
                        return string.Empty;
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
