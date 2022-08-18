using Nancy.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExamGenerator
{
    [Serializable]
	public class Category : INotifyPropertyChanged, ICloneable, IValidate, IJsonConvertable
	{
		public Category()
		{
			this.id = ExamGeneratorContext.GetNextCategoryId();
		}

		[ScriptIgnore]
		public List<string> ValidationErrors
		{
			get
			{
				var list = new List<string>();

				if (string.IsNullOrWhiteSpace(this.description))
					list.Add("Description darf nicht leer sein");

				return list;
			}
		}

		public Category(string description) : this()
		{
			this.description = description;
		}

		public object Clone()
		{
            var c = new Category
            {
                id = Id,
                description = Description
            };

            return c;
		}

		#region properties

		int id;
		public int Id
		{
			get
			{
				return id;
			}
		}

		string description;
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				SetField(ref description, value, nameof(Description));
				OnPropertyChanged(nameof(Easy));
				OnPropertyChanged(nameof(Medium));
				OnPropertyChanged(nameof(Difficult));
				OnPropertyChanged(nameof(LinkedQuestions));
			}
		}

		[ScriptIgnore]
		public int Easy { get { return ExamGeneratorContext.QuestionCatalogue.Count(c => c.Category == this && c.Difficulty == DifficultyLevel.Easy); } }

		[ScriptIgnore]
		public int Medium { get { return ExamGeneratorContext.QuestionCatalogue.Count(c => c.Category == this && c.Difficulty == DifficultyLevel.Medium); } }

		[ScriptIgnore]
		public int Difficult { get { return ExamGeneratorContext.QuestionCatalogue.Count(c => c.Category == this && c.Difficulty == DifficultyLevel.Difficult); } }

		[ScriptIgnore]
		public List<Question> LinkedQuestions { get { return ExamGeneratorContext.QuestionCatalogue.Where(x => x.Category == this).ToList(); } }

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
