using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ExamGenerator
{
    public class Question : INotifyPropertyChanged, ICloneable, IValidate, IJsonConvertable
	{
		public Question()
		{
			this.id = ExamGeneratorContext.GetNextQuestionId();

			this.AMC = new SolutionModeSubContext();
			this.SSC = new DifficultySubContext();
			this.KSC = new CategorySubContext();
			this.RASC = new CorrectSolutionSubContext();

			this.Mode = AnswerMode.MultipleChoice;
			AMC.Mode = this.Mode;

			AMC.PropertyChanged += (sender, e) =>
			{
				this.Mode = AMC.Mode;
			};

			this.Difficulty = DifficultyLevel.Medium;
			SSC.Difficulty = Difficulty.HasValue ? Difficulty.Value : DifficultyLevel.Medium;

			SSC.PropertyChanged += (sender, e) =>
			{
				this.Difficulty = SSC.Difficulty;
			};

			RASC.CorrectSolution = CorrectSolution.HasValue ? CorrectSolution.Value : -1;

			RASC.PropertyChanged += (sender, e) =>
			{
				this.CorrectSolution = RASC.CorrectSolution;
			};
		}

		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			Question other = obj as Question;
			if (other == null)
				return false;
			return this.id == other.id && this.problem == other.problem && this.answer1 == other.answer1 && this.answer2 == other.answer2 && this.answer3 == other.answer3 && this.answer4 == other.answer4 && this.answer5 == other.answer5 && this.correctSolution == other.correctSolution && object.Equals(this.category, other.category) && this.difficulty == other.difficulty && this.mode == other.mode && this.freetextSolution == other.freetextSolution && object.Equals(this.AMC, other.AMC) && object.Equals(this.KSC, other.KSC) && object.Equals(this.RASC, other.RASC);
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				hashCode += 1000000007 * id.GetHashCode();
				if (problem != null)
					hashCode += 1000000009 * problem.GetHashCode();
				if (answer1 != null)
					hashCode += 1000000021 * answer1.GetHashCode();
				if (answer2 != null)
					hashCode += 1000000033 * answer2.GetHashCode();
				if (answer3 != null)
					hashCode += 1000000087 * answer3.GetHashCode();
				if (answer4 != null)
					hashCode += 1000000093 * answer4.GetHashCode();
				if (answer5 != null)
					hashCode += 1000000097 * answer5.GetHashCode();
				hashCode += 1000000103 * correctSolution.GetHashCode();
				if (category != null)
					hashCode += 1000000123 * category.GetHashCode();
				hashCode += 1000000181 * difficulty.GetHashCode();
				hashCode += 1000000207 * mode.GetHashCode();
				if (freetextSolution != null)
					hashCode += 1000000223 * freetextSolution.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(Question lhs, Question rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (lhs is null || rhs is null)
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Question lhs, Question rhs)
		{
			return !(lhs == rhs);
		}

		#endregion

		#region IValidate implementation

		public List<string> ValidationErrors
		{
			get
			{
				var list = new List<string>();

				if (string.IsNullOrWhiteSpace(Id.ToString()))
					list.Add("Id ist nicht gesetzt");

				if (Id < 0)
					list.Add("Id ist negativ");

				if (string.IsNullOrWhiteSpace(Problem))
					list.Add("Fragestellung ist leer");

				if (Category == null)
					list.Add("Kategorie ist leer");

				if (Difficulty == null)
					list.Add("Schwierigkeit ist leer");

				if (Mode == AnswerMode.MultipleChoice)
				{
					if (string.IsNullOrWhiteSpace(Answer1))
						list.Add("Antwort A ist leer");

					if (string.IsNullOrWhiteSpace(Answer2))
						list.Add("Antwort B ist leer");

					if (string.IsNullOrWhiteSpace(Answer3))
						list.Add("Antwort C ist leer");

					if (string.IsNullOrWhiteSpace(Answer4))
						list.Add("Antwort D ist leer");

					if (string.IsNullOrWhiteSpace(Answer5))
						list.Add("Antwort E ist leer");

					if (CorrectSolution < 1 || CorrectSolution > 5)
						list.Add("Es wurde keine korrekte Antwort ausgewählt");
				}
				else if (Mode == AnswerMode.Freetext)
				{
					if (string.IsNullOrWhiteSpace(FreetextSolution))
						list.Add("Die Freitextantwort ist leer");
				}

				return list;
			}
		}

		#endregion

		#region ICloneable implementation

		public object Clone()
		{
            var c = new Question
            {
                answer1 = this.answer1,
                answer2 = this.answer2,
                answer3 = this.answer3,
                answer4 = this.answer4,
                answer5 = this.answer5,
                problem = this.problem,
                freetextSolution = this.freetextSolution,
                id = this.id,
                category = this.category,
                mode = this.mode,
                correctSolution = this.correctSolution,
                difficulty = this.difficulty
            };

            c.UpdateSubContextProperties();

			return c;
		}

		#endregion

		#region fields

		int id;
		string problem;
		string answer1;
		string answer2;
		string answer3;
		string answer4;
		string answer5;
		int? correctSolution;
		Category category;
		DifficultyLevel? difficulty;
		AnswerMode mode;
		string freetextSolution;

		#endregion

		#region Properties

		public int Id { get { return id; } set { SetField(ref id, value, nameof(Id)); } }
		public string Problem { get { return problem; } set { SetField(ref problem, value, nameof(Problem)); } }
		public string Answer1 { get { return answer1; } set { SetField(ref answer1, value, nameof(Answer1)); } }
		public string Answer2 { get { return answer2; } set { SetField(ref answer2, value, nameof(Answer2)); } }
		public string Answer3 { get { return answer3; } set { SetField(ref answer3, value, nameof(Answer3)); } }
		public string Answer4 { get { return answer4; } set { SetField(ref answer4, value, nameof(Answer4)); } }
		public string Answer5 { get { return answer5; } set { SetField(ref answer5, value, nameof(Answer5)); } }
		public int? CorrectSolution { get { return correctSolution.HasValue ? correctSolution.Value : -1; } set { SetField(ref correctSolution, value, nameof(CorrectSolution)); } }

		public Category Category
		{
			get
			{
				return category;
			}
			set
			{
				SetField(ref category, value, nameof(Category));
			}
		}

		public DifficultyLevel? Difficulty { get { return difficulty; } set { SetField(ref difficulty, value, nameof(Difficulty)); } }
		public AnswerMode Mode
		{
			get { return mode; }
			set
			{
				SetField(ref mode, value, nameof(Mode));
				OnPropertyChanged(nameof(FreetextVisibility));
				OnPropertyChanged(nameof(MultipleChoiceVisibility));
				// disable once RedundantCheckBeforeAssignment
				if (AMC.Mode != value)
					AMC.Mode = value;
			}
		}
		public string FreetextSolution { get { return freetextSolution; } set { SetField(ref freetextSolution, value, nameof(FreetextSolution)); } }

		public Visibility FreetextVisibility { get { return Mode == AnswerMode.Freetext ? Visibility.Visible : Visibility.Collapsed; } }
		public Visibility MultipleChoiceVisibility { get { return Mode == AnswerMode.MultipleChoice ? Visibility.Visible : Visibility.Collapsed; } }

		#endregion

		#region subcontext implementation

		public void UpdateSubContextProperties()
		{
			AMC.Mode = this.Mode;
			SSC.Difficulty = this.Difficulty.HasValue ? this.Difficulty.Value : DifficultyLevel.Medium;
			RASC.CorrectSolution = this.CorrectSolution.HasValue ? this.CorrectSolution.Value : -1;
		}

		public DifficultySubContext SSC { get; set; }
		public SolutionModeSubContext AMC { get; set; }
		public CategorySubContext KSC { get; set; }
		public CorrectSolutionSubContext RASC { get; set; }

		public class DifficultySubContext : INotifyPropertyChanged
		{
			public bool Easy
			{
				get { return easy; }
				set
				{
					SetField(ref easy, value, nameof(Easy));
					this.Difficulty = DifficultyLevel.Easy;
				}
			}
			bool easy;
			
			public bool Medium
			{
				get { return medium; }
				set
				{
					SetField(ref medium, value, nameof(Medium));
					this.Difficulty = DifficultyLevel.Medium;
				}
			}
			bool medium;
			
			public bool Difficult
			{
				get { return difficult; }
				set
				{
					SetField(ref difficult, value, nameof(Difficult));
					this.Difficulty = DifficultyLevel.Difficult;
				}
			}
			bool difficult;

			public DifficultyLevel Difficulty
			{
				get { return difficulty; }
				set
				{
					SetField(ref difficulty, value, nameof(Difficulty));
					switch (difficulty)
					{
						case DifficultyLevel.Easy:
							easy = !false;
							medium = false;
							difficult = false;
							break;

						case DifficultyLevel.Medium:
							easy = false;
							medium = !false;
							difficult = false;
							break;

						case DifficultyLevel.Difficult:
							easy = false;
							medium = false;
							difficult = !false;
							break;

						default:
							break;
					}

					OnPropertyChanged(nameof(Easy));
					OnPropertyChanged(nameof(Medium));
					OnPropertyChanged(nameof(Difficult));
				}

			}
			DifficultyLevel difficulty;

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

		public class SolutionModeSubContext : INotifyPropertyChanged
		{
			public bool MultipleChoice
			{
				get { return multiplechoice; }
				set
				{
					SetField(ref multiplechoice, value, nameof(MultipleChoice));
					Mode = AnswerMode.MultipleChoice;
				}
			}
			bool multiplechoice;

			public bool Freetext
			{
				get { return freetext; }
				set
				{
					SetField(ref freetext, value, nameof(Freetext));
					Mode = AnswerMode.Freetext;
				}
			}
			bool freetext;

			public AnswerMode Mode
			{
				get { return mode; }
				set
				{
					SetField(ref mode, value, nameof(Mode));
					if (mode == AnswerMode.MultipleChoice)
					{
						freetext = false;
						multiplechoice = true;
					}
					else
					{
						freetext = true;
						multiplechoice = false;
					}
					OnPropertyChanged(nameof(Freetext));
					OnPropertyChanged(nameof(MultipleChoice));
				}

			}
			AnswerMode mode;

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

		public class CategorySubContext
		{
			public ObservableCollection<Category> Categories { get { return ExamGeneratorContext.CategoryCatalogue; } }
		}

		public class CorrectSolutionSubContext : INotifyPropertyChanged
		{
			public bool A
			{
				get { return a; }
				set
				{
					SetField(ref a, value, nameof(A));
					this.CorrectSolution = 1;
				}
			}
			bool a;

			public bool B
			{
				get { return b; }
				set
				{
					SetField(ref b, value, nameof(B));
					this.CorrectSolution = 2;
				}
			}
			bool b;

			public bool C
			{
				get { return c; }
				set
				{
					SetField(ref c, value, nameof(C));
					this.CorrectSolution = 3;
				}
			}
			bool c;

			public bool D
			{
				get { return d; }
				set
				{
					SetField(ref d, value, nameof(D));
					this.CorrectSolution = 4;
				}
			}
			bool d;

			public bool E
			{
				get { return e; }
				set
				{
					SetField(ref e, value, nameof(E));
					this.CorrectSolution = 5;
				}
			}
			bool e;

			public int CorrectSolution
			{
				get
				{
					return correctSolution;
				}
				set
				{
					SetField(ref correctSolution, value, nameof(CorrectSolution));
					switch (correctSolution)
					{
						case 1:
							a = !false;
							b = false;
							c = false;
							d = false;
							e = false;
							break;

						case 2:
							a = false;
							b = !false;
							c = false;
							d = false;
							e = false;
							break;

						case 3:
							a = false;
							b = false;
							c = !false;
							d = false;
							e = false;
							break;

						case 4:
							a = false;
							b = false;
							c = false;
							d = !false;
							e = false;
							break;

						case 5:
							a = false;
							b = false;
							c = false;
							d = false;
							e = !false;
							break;

						default:
							break;
					}
					OnPropertyChanged(nameof(A));
					OnPropertyChanged(nameof(B));
					OnPropertyChanged(nameof(C));
					OnPropertyChanged(nameof(D));
					OnPropertyChanged(nameof(E));
				}
			}
			int correctSolution;

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
			OnPropertyChanged("IsValid");
			return true;
		}

		#endregion

		#region IJsonConvertable implementation

		public string ToJSON()
		{
			var JSS = new JavaScriptSerializer();
			var s = JSS.Serialize(this);
			return s;
		}

		#endregion
	}
}
