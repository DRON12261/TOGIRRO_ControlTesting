using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
			CreateSubject_Type.ItemsSource = Enum.GetValues(typeof(SubjectType)).Cast<SubjectType>();
			CreateSubject_Type.SelectedIndex = 0;

			EditSubject_Type.ItemsSource = Enum.GetValues(typeof(SubjectType)).Cast<SubjectType>();
			EditSubject_Type.SelectedIndex = 0;

			Workfield.Init();
			SubjectsList.ItemsSource = Workfield.Subjects;

			QuestionList.ItemsSource = Workfield.Quastions;
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
			EditSubject_Name.Text = SelectedSubject.Name;
			EditSubject_Description.Text = SelectedSubject.Description;
			//EditSubject_Type.SelectedIndex = (int)(SelectedSubject.Type) - 1;
			ICollectionView view = CollectionViewSource.GetDefaultView(Workfield.Subjects);
			view.Refresh();
			EditTab.IsSelected = true;
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
					CreateDate = DateTime.Now,
					ChangeDate = DateTime.Now,
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

		private void ButtonC_LaunchSubject(object sender, RoutedEventArgs e)
		{
			Subject SelectedSubject = (Subject)SubjectsList.SelectedItem;
			Workfield.ActiveSubjects.Add(SelectedSubject);
		}
	}
}
