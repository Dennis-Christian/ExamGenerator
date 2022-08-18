using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ExamGenerator
{
    public class Preset : INotifyPropertyChanged, ICloneable, IValidate, IJsonConvertable
	{
		public Preset()
		{
			this.id = ExamGeneratorContext.GetNextPresetId();
			this.forbiddenCategories = new ObservableCollection<string>();
		}

		#region Equals and GetHashCode implementation
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				hashCode += 1000000007 * id.GetHashCode();
				if (description != null)
					hashCode += 1000000009 * description.GetHashCode();
				hashCode += 1000000021 * easyQuestions.GetHashCode();
				hashCode += 1000000033 * mediumQuestions.GetHashCode();
				hashCode += 1000000087 * difficultQuestions.GetHashCode();
				hashCode += 1000000093 * allowDuplicates.GetHashCode();
				hashCode += 1000000097 * maxNumQuestionsPerCategory.GetHashCode();
				if (forbiddenCategories != null)
					hashCode += 1000000103 * forbiddenCategories.GetHashCode();
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			Preset other = obj as Preset;
			if (other == null)
				return false;
			return this.id == other.id && this.description == other.description && this.easyQuestions == other.easyQuestions && this.mediumQuestions == other.mediumQuestions && this.difficultQuestions == other.difficultQuestions && this.allowDuplicates == other.allowDuplicates && this.maxNumQuestionsPerCategory == other.maxNumQuestionsPerCategory && object.Equals(this.forbiddenCategories, other.forbiddenCategories);
		}

		public static bool operator ==(Preset lhs, Preset rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (lhs is null || rhs is null)
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Preset lhs, Preset rhs)
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

				if (Total < 1)
				{
					list.Add("Gesamtzahl der Fragen unter 1");
				}

				if (string.IsNullOrWhiteSpace(Description))
				{
					list.Add("Description ist leer");
				}

				return list;
			}
		}

		#endregion

		#region ICloneable implementation

		public object Clone()
		{
            var c = new Preset
            {
                id = this.id,
                description = this.description,
                easyQuestions = this.easyQuestions,
                mediumQuestions = this.mediumQuestions,
                difficultQuestions = this.difficultQuestions,
                maxNumQuestionsPerCategory = this.maxNumQuestionsPerCategory,
                forbiddenCategories = this.forbiddenCategories
            };

            return c;
		}

		#endregion

		#region fields

		int id;
		string description;
		int easyQuestions;
		int mediumQuestions;
		int difficultQuestions;
		bool allowDuplicates;
		int maxNumQuestionsPerCategory;
		ObservableCollection<string> forbiddenCategories;

		#endregion

		#region properties

		public int Id
		{
			get { return id; }
			set { SetField(ref id, value, nameof(Id)); }
		}

		public string Description
		{
			get { return description; }
			set { SetField(ref description, value, nameof(Description)); }
		}

		public int EasyQuestions
		{
			get { return easyQuestions; }
			set
			{
				SetField(ref easyQuestions, value, nameof(EasyQuestions));
				OnPropertyChanged(nameof(Total));
			}
		}

		public int MediumQuestions
		{
			get { return mediumQuestions; }
			set
			{
				SetField(ref mediumQuestions, value, nameof(MediumQuestions));
				OnPropertyChanged(nameof(Total));
			}
		}

		public int DifficultQuestions
		{
			get { return difficultQuestions; }
			set
			{
				SetField(ref difficultQuestions, value, nameof(DifficultQuestions));
				OnPropertyChanged(nameof(Total));
			}
		}

		public int MaxNumQuestionsPerCategory
		{
			get { return maxNumQuestionsPerCategory; }
			set
			{
				SetField(ref maxNumQuestionsPerCategory, value, nameof(MaxNumQuestionsPerCategory));
			}
		}

		public bool AllowDuplicates
		{
			get
			{
				return allowDuplicates;
			}
			set
			{
				SetField(ref allowDuplicates, value, nameof(AllowDuplicates));
			}
		}

		public int Total
		{
			get { return easyQuestions + mediumQuestions + difficultQuestions; }
		}

		public ObservableCollection<string> ForbiddenCategories
		{
			get { return forbiddenCategories; }
			set
			{
				SetField(ref forbiddenCategories, value, nameof(ForbiddenCategories));
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
