using Nancy.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExamGenerator
{
    public class Exam : INotifyPropertyChanged, ICloneable, IValidate, IJsonConvertable
	{
		public Exam(Preset Preset)
		{
			id = ExamGeneratorContext.GetNextExamId();

			preset = Preset;

			questions = new Question[preset.Total];

			FillQuestions();
		}

		#region Equals and GetHashCode implementation
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				hashCode += 1000000007 * id.GetHashCode();
				if (courseNumber != null)
					hashCode += 1000000009 * courseNumber.GetHashCode();
				hashCode += 1000000021 * creationDate.GetHashCode();
				if (annotation != null)
					hashCode += 1000000033 * annotation.GetHashCode();
				if (questions != null)
					hashCode += 1000000087 * questions.GetHashCode();
				if (preset != null)
					hashCode += 1000000093 * preset.GetHashCode();
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			Exam other = obj as Exam;
			if (other == null)
				return false;
			return this.id == other.id && this.courseNumber == other.courseNumber && this.creationDate == other.creationDate && this.annotation == other.annotation && object.Equals(this.questions, other.questions) && object.Equals(this.preset, other.preset);
		}

		public static bool operator ==(Exam lhs, Exam rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (lhs is null || rhs is null)
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Exam lhs, Exam rhs)
		{
			return !(lhs == rhs);
		}

		#endregion

		void FillQuestions()
		{
			//Fill with categories
			var kategorieListe = new List<Category>();
			if (ExamGeneratorContext.CategoryCatalogue.Count <= questions.Length)
			{
				//we want each category to be present at least once
				foreach (var element in ExamGeneratorContext.CategoryCatalogue)
				{
					kategorieListe.Add(element);
				}

				//random fill the rest
				for (int i = 0; i < questions.Length - kategorieListe.Count; i++)
				{
					kategorieListe.Add(ExamGeneratorContext.GetRandomCategory());
				}
			}
			else
			{
				//random fill
				for (int i = 0; i < questions.Length; i++)
				{
					if (preset.AllowDuplicates)
					{
						kategorieListe.Add(ExamGeneratorContext.GetRandomCategory());
					}
					else
					{
						Category k;
						do
						{
							k = ExamGeneratorContext.GetRandomCategory();
						}
						while (kategorieListe.Any(x => x.Id == k.Id));
						kategorieListe.Add(k);
					}
				}
			}
			var KategorieListe = kategorieListe.Shuffle().ToList();

			//fill with difficulties
			var schwierigkeitsliste = new List<DifficultyLevel>();

			for (int i = 0; i < preset.EasyQuestions; i++)
				schwierigkeitsliste.Add(DifficultyLevel.Easy);
			for (int i = 0; i < preset.MediumQuestions; i++)
				schwierigkeitsliste.Add(DifficultyLevel.Medium);
			for (int i = 0; i < preset.DifficultQuestions; i++)
				schwierigkeitsliste.Add(DifficultyLevel.Difficult);

			var Schwierigkeitsliste = schwierigkeitsliste.Shuffle().ToList();

			//get a question for each category-difficulty combination present
			for (int i = 0; i < questions.Length; i++)
			{
				Question f;
				do
				{
					f = ExamGeneratorContext.GetRandomQuestion(KategorieListe[i], Schwierigkeitsliste[i], questions.Select(x => x.Id).ToList());
				}
				while (questions.Any(x => x.Id == f.Id));
				questions[i] = f;
			}
		}

		#region IValidate implementation

		public List<string> ValidationErrors
		{
			get
			{
				var list = new List<string>();

				foreach (var element in this.Questions)
				{
					if (Questions.Count(x => x.Id == element.Id) != 1)
					{
						list.Add("Die Question mit der ID " + element.Id + "ist nicht nur genau einmal vorhanden");
					}
				}

				return list;
			}
		}

		#endregion

		#region ICloneable implementation

		public object Clone()
		{
            var c = new Exam(preset)
            {
                creationDate = creationDate,
                questions = new Question[preset.Total]
            };
            for (int i = 0; i < questions.Length; i++)
			{
				c.questions[i] = (Question)Questions[i].Clone();
			}
			c.id = id;
			c.courseNumber = courseNumber;
			c.annotation = annotation;

			return c;
		}

		#endregion

		#region fields

		int id;
		string courseNumber;
		DateTime creationDate;
		string annotation;
		Question[] questions;
		Preset preset;

		#endregion

		#region Properties

		public int Id { get { return id; } set { SetField(ref id, value, nameof(Id)); } }
		public string CourseNumber { get { return courseNumber; } set { SetField(ref courseNumber, value, nameof(CourseNumber)); } }
		public DateTime CreationDate { get { return creationDate; } set { SetField(ref creationDate, value, nameof(CreationDate)); } }
		public string Annotation { get { return annotation; } set { SetField(ref annotation, value, nameof(Annotation)); } }
		public Question[] Questions { get { return questions; } }
		public Preset Preset { get { return preset; } private set { SetField(ref preset, value, nameof(Preset)); } }

		#endregion

		#region Export

		public void GenerateExamPDF()
		{
			//TODO GenerateExamPDF()
			//libhairuku
		}

		public void GenerateSolutionCheatsheetPDF()
		{
			//TODO GenerateSolutionCheatsheetPDF()
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

		#region save/load

		public static Exam KlausurFromJSON(string s)
		{
			var JSS = new JavaScriptSerializer();
			var k = JSS.Deserialize<Exam>(s);
			return k;
		}

		public static string KlausurToJSON(Exam k)
		{
			var JSS = new JavaScriptSerializer();
			var s = JSS.Serialize(k);
			return s;
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
