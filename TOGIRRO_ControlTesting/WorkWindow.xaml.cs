using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Reflection;

namespace TOGIRRO_ControlTesting
{
	/// <summary>
	/// Логика взаимодействия для WorkWindow.xaml
	/// </summary>
	public partial class WorkWindow : Window
	{
		public WorkWindow()
		{
			InitializeComponent();

			Workfield.Init();

			Workfield.CurrentSubject = Workfield.Subjects[0];
			Workfield.CurrentVariant = Workfield.CurrentSubject.Variants[0];
			Workfield.CurrentQuestion = Workfield.CurrentSubject.Questions[0];

			SubjectsList.ItemsSource = Workfield.Subjects;
			QuestionList.ItemsSource = Workfield.CurrentSubject.Questions;

			SubjectTypesCBox.ItemsSource = Workfield.SubjectTypeEnumValues;
			CreateSubject_Type.ItemsSource = Workfield.QuestionTypeEnumValues;
			EditSubject_Type.ItemsSource = Workfield.CheckTypeEnumValues;
			SubjectTypesCBox.SelectedIndex = 0;
			CreateSubject_Type.SelectedIndex = 0;
			EditSubject_Type.SelectedIndex = 0;
		}

		public int GetIndexByElement(string value, string[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (value == values[i])
				{
					return i;
				}
			}

			return -1;
		}

		private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void ButtonC_CreateSubject(object sender, RoutedEventArgs e)
		{
			CreateSubject_EventCode.Text = "";
			CreateSubject_SubjectCode.Text = "";
			CreateSubject_Name.Text = "";
			CreateSubject_Description.Text = "";
			CreateSubject_Type.SelectedIndex = 0;
			CreateSubject_MinScore.Text = "";
			CreateSubject_ProjectFolderPath.Text = "";
			CreateSubject_RegistrationForm.Text = "";
			CreateSubject_AnswersForm1.Text = "";
			CreateSubject_AnswersForm2.Text = "";
			CreateSubject_LogFile.Text = "";
			CreateSubject_ScoreSystem.IsChecked = true;
			Subjects_CreateTab.IsSelected = true;
		}

		private void ButtonC_EditSubject(object sender, RoutedEventArgs e)
		{
			Subject SelectedSubject = (Subject)SubjectsList.SelectedItem;
			if (SelectedSubject != null)
			{
				EditSubject_EventCode.Text = SelectedSubject.EventCode.ToString();
				EditSubject_SubjectCode.Text = SelectedSubject.SubjectCode.ToString();
				EditSubject_Name.Text = SelectedSubject.Name;
				EditSubject_Description.Text = SelectedSubject.Description;
				EditSubject_Type.SelectedIndex = (int)SelectedSubject.Type;
				EditSubject_MinScore.Text = SelectedSubject.MinScore.ToString();
				EditSubject_ProjectFolderPath.Text = SelectedSubject.ProjectFolderPath;
				EditSubject_RegistrationForm.Text = SelectedSubject.RegistrationForm;
				EditSubject_AnswersForm1.Text = SelectedSubject.AnswersForm1;
				EditSubject_AnswersForm2.Text = SelectedSubject.AnswersForm2;
				EditSubject_LogFile.Text = SelectedSubject.LogFile;
				if (SelectedSubject.IsMark) EditSubject_MarkSystem.IsChecked = true; else EditSubject_ScoreSystem.IsChecked = true;
				ICollectionView view = CollectionViewSource.GetDefaultView(Workfield.Subjects);
				view.Refresh();
				Subjects_EditTab.IsSelected = true;
			}
		}

		private void ButtonC_DeleteSubject(object sender, RoutedEventArgs e)
		{
			Subject SelectedSubject = (Subject)SubjectsList.SelectedItem;
			Workfield.Subjects.Remove(SelectedSubject);
		}

		private void ButtonC_CreateSubject_Create(object sender, RoutedEventArgs e)
		{
			if (CreateSubject_Name.Text.Trim(' ') != "")
			{
				Workfield.Subjects.Add(new Subject()
				{
					SubjectCode = short.Parse(CreateSubject_SubjectCode.Text.Trim(' ')),
					EventCode = short.Parse(CreateSubject_EventCode.Text.Trim(' ')),
					Name = CreateSubject_Name.Text.Trim(' '),
					Description = CreateSubject_Description.Text.Trim(' '),
					Type = (SubjectTypeEnum)CreateSubject_Type.SelectedIndex,
					MinScore = short.Parse(CreateSubject_MinScore.Text.Trim(' ')),
					ProjectFolderPath = CreateSubject_ProjectFolderPath.Text.Trim(' '),
					RegistrationForm = CreateSubject_RegistrationForm.Text.Trim(' '),
					AnswersForm1 = CreateSubject_AnswersForm1.Text.Trim(' '),
					AnswersForm2 = CreateSubject_AnswersForm2.Text.Trim(' '),
					LogFile = CreateSubject_LogFile.Text.Trim(' '),
					IsMark = CreateSubject_MarkSystem.IsChecked.Value
				});
				Subjects_ListTab.IsSelected = true;
			}
		}

		private void ButtonC_CreateSubject_Cancel(object sender, RoutedEventArgs e)
		{
			Subjects_ListTab.IsSelected = true;
		}

		private void ButtonC_EditSubject_Edit(object sender, RoutedEventArgs e)
		{
			if (EditSubject_Name.Text.Trim(' ') != "")
			{
				Subject SelectedSubject = (Subject)SubjectsList.SelectedItem;
				SelectedSubject.SubjectCode = short.Parse(EditSubject_SubjectCode.Text.Trim(' '));
				SelectedSubject.EventCode = short.Parse(EditSubject_EventCode.Text.Trim(' '));
				SelectedSubject.Name = EditSubject_Name.Text.Trim(' ');
				SelectedSubject.Description = EditSubject_Description.Text.Trim(' ');
				SelectedSubject.Type = (SubjectTypeEnum)EditSubject_Type.SelectedIndex;
				SelectedSubject.MinScore = short.Parse(EditSubject_MinScore.Text.Trim(' '));
				SelectedSubject.ProjectFolderPath = EditSubject_ProjectFolderPath.Text.Trim(' ');
				SelectedSubject.RegistrationForm = EditSubject_RegistrationForm.Text.Trim(' ');
				SelectedSubject.AnswersForm1 = EditSubject_AnswersForm1.Text.Trim(' ');
				SelectedSubject.AnswersForm2 = EditSubject_AnswersForm2.Text.Trim(' ');
				SelectedSubject.LogFile = EditSubject_LogFile.Text.Trim(' ');
				SelectedSubject.IsMark = EditSubject_MarkSystem.IsChecked.Value;
				Subjects_ListTab.IsSelected = true;
			}
		}

		private void ButtonC_EditSubject_Cancel(object sender, RoutedEventArgs e)
		{
			Subjects_ListTab.IsSelected = true;
		}

		private void ButtonC_EditQuestionsMode(object sender, RoutedEventArgs e)
		{
			EditQuestionsMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = true;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = false;
		}

		private void ButtonC_EditScaleMode(object sender, RoutedEventArgs e)
		{
			EditScaleMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = true;
			EditAnswersMode_Button.IsChecked = false;
		}

		private void ButtonC_EditAnswersMode(object sender, RoutedEventArgs e)
		{
			EditAnswersMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = true;
		}

		private void ButtonC_EditVariantNameMode(object sender, RoutedEventArgs e)
		{
			EditVariantMode.IsSelected = true;
		}

		private void ButtonC_CreateVariant(object sender, RoutedEventArgs e)
		{

		}

		private void ButtonC_DeleteVariant(object sender, RoutedEventArgs e)
		{

		}

		private void ButtonC_EditVariantSuccess(object sender, RoutedEventArgs e)
		{
			SelectVariantMode.IsSelected = true;
		}

		private void ButtonC_EditVariantCancel(object sender, RoutedEventArgs e)
		{
			SelectVariantMode.IsSelected = true;
		}
	}



	public class DescriptionConverter : EnumConverter
	{
		public DescriptionConverter(Type type) : base(type) { }
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value != null)
				{
					FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
					if (fieldInfo != null)
					{
						var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

						return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
					}
				}
				return string.Empty;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
