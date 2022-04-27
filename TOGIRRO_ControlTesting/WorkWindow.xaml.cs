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

			//SubjectTree.ItemsSource = Workfield.ActiveSubjects;

			Workfield.Init();

			Workfield.CurrentSubject = Workfield.Subjects[0];
			Workfield.CurrentVariant = Workfield.CurrentSubject.Variants[0];
			Workfield.CurrentQuestion = Workfield.CurrentVariant.Questions[0];

			SubjectsList.ItemsSource = Workfield.Subjects;
			QuestionList.ItemsSource = Workfield.CurrentVariant.Questions;

			SubjectTypesCBox.ItemsSource = Workfield.SubjectTypes;
			CreateSubject_Type.ItemsSource = Workfield.SubjectTypes;
			EditSubject_Type.ItemsSource = Workfield.SubjectTypes;
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

		private void ButtonC_CreateSubject(object sender, RoutedEventArgs e)
		{
			CreateSubject_Name.Text = "";
			CreateSubject_Description.Text = "";
			CreateSubject_Type.SelectedIndex = 0;
			CreateTab.IsSelected = true;
		}

		private void ButtonC_EditSubject(object sender, RoutedEventArgs e)
		{
			Subject SelectedSubject = (Subject)SubjectsList.SelectedItem;
			if (SelectedSubject != null)
			{
				EditSubject_Name.Text = SelectedSubject.Name;
				EditSubject_Description.Text = SelectedSubject.Description;
				EditSubject_Type.SelectedIndex = GetIndexByElement(SelectedSubject.Type, Workfield.SubjectTypes);
				ICollectionView view = CollectionViewSource.GetDefaultView(Workfield.Subjects);
				view.Refresh();
				EditTab.IsSelected = true;
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
					Name = CreateSubject_Name.Text.Trim(' '),
					//Type = (SubjectType)(CreateSubject_Type.SelectedIndex + 1),
					Description = CreateSubject_Description.Text.Trim(' ')
				});
				ListTab.IsSelected = true;
			}
		}

		private void ButtonC_CreateSubject_Cancel(object sender, RoutedEventArgs e)
		{
			ListTab.IsSelected = true;
		}

		private void ButtonC_EditSubject_Edit(object sender, RoutedEventArgs e)
		{
			if (CreateSubject_Name.Text.Trim(' ') != "")
			{
				Subject SelectedSubject = (Subject)SubjectsList.SelectedItem;
				SelectedSubject.Name = EditSubject_Name.Text;
				SelectedSubject.Description = EditSubject_Description.Text;
				//SelectedSubject.Type = (SubjectType)(EditSubject_Type.SelectedIndex + 1);
				ListTab.IsSelected = true;
			}
		}

		private void ButtonC_EditSubject_Cancel(object sender, RoutedEventArgs e)
		{
			ListTab.IsSelected = true;
		}
	}
}
