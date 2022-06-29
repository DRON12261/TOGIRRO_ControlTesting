using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using Microsoft.Data.SqlClient;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using Microsoft.Win32;
using System.IO;

namespace TOGIRRO_ControlTesting
{
	//========================================================================================================================================
	//===Главное окно WorkWindow==============================================================================================================
	//========================================================================================================================================
	public partial class WorkWindow : Window
	{
        //------------------------------------------------------------------------------------------------------------------------------------
        //---Конструктор и вспомогательные методы---------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Конструктор, инициализация параметров
		*/
        #region
        public WorkWindow()
		{
			InitializeComponent();
			Workfield.WorkWindow = this;
			Workfield.Init();
			Workfield.PreLoad();
			Workfield.LoadFromDB();

			SearchTypes.ItemsSource = Workfield.SubjectTypes;
			SearchTypes.SelectedValuePath = "Key";
			SearchTypes.DisplayMemberPath = "Value";
			CreateSubject_Type.ItemsSource = Workfield.SubjectTypes;
			CreateSubject_Type.SelectedValuePath = "Key";
			CreateSubject_Type.DisplayMemberPath = "Value";
			EditSubject_Type.ItemsSource = Workfield.SubjectTypes;
			EditSubject_Type.SelectedValuePath = "Key";
			EditSubject_Type.DisplayMemberPath = "Value";
			SearchTypes.SelectedIndex = 0;
			CreateSubject_Type.SelectedIndex = 0;
			EditSubject_Type.SelectedIndex = 0;

			Workfield.CurrentSubject = null;
			CreateSubjectMenu_Button.IsChecked = false;
			EditSubjectMenu_Button.IsChecked = false;
			EditSubjectMenu_Button.IsEnabled = false;
			DeleteSubject_Button.IsEnabled = false;
			MainField.IsEnabled = false;
			SubjectsList.ItemsSource = Workfield.ActualSubjects;

			EditQuestionsMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = true;

			if (AnswerList.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
			{
				AnswerList_VisibilityButton.Source = new BitmapImage(new Uri(@"/Icons/Hide.png", UriKind.Relative));
			}
			else
			{
				AnswerList_VisibilityButton.Source = new BitmapImage(new Uri(@"/Icons/Show.png", UriKind.Relative));
			}

			if (QuestionList.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
			{
				QuestionList_VisibilityButton.Source = new BitmapImage(new Uri(@"/Icons/Hide.png", UriKind.Relative));
			}
			else
			{
				QuestionList_VisibilityButton.Source = new BitmapImage(new Uri(@"/Icons/Show.png", UriKind.Relative));
			}

			CSVList.ItemsSource = Workfield.blankCSVs;
			CheckResultsList.ItemsSource = Workfield.kits;
			PrintReportsList.ItemsSource = Workfield.reports;
		}
		#endregion

		/*
			Ограничение ввода для числового TextBox
		*/
		#region
		private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}
        #endregion

        /*
			Событие закрытия окна WorkWindow
		*/
        #region
        private void WorkWindow_Closing(object sender, CancelEventArgs e)
		{
			Application.Current.Shutdown();
		}
		#endregion

		/*
			Переключение видимости вложенных таблиц в таблице вопросов
		*/
		#region
		private void AnswerList_ChangeTableVisibilityButton_Click(object sender, RoutedEventArgs e)
		{
			if (AnswerList.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
			{
				AnswerList.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
				AnswerList_VisibilityButton.Source = new BitmapImage(new Uri(@"/Icons/Show.png", UriKind.Relative));
			}
			else
			{
				AnswerList.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
				AnswerList_VisibilityButton.Source = new BitmapImage(new Uri(@"/Icons/Hide.png", UriKind.Relative));
			}
		}
		#endregion

		/*
			Переключение видимости вложенных таблиц в таблице шаблонов вопросов
		*/
		#region
		private void QuestionList_ChangeTableVisibilityButton_Click(object sender, RoutedEventArgs e)
		{
			if (QuestionList.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
			{
				QuestionList.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
				QuestionList_VisibilityButton.Source = new BitmapImage(new Uri(@"/Icons/Show.png", UriKind.Relative));
			}
			else
			{
				QuestionList.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
				QuestionList_VisibilityButton.Source = new BitmapImage(new Uri(@"/Icons/Hide.png", UriKind.Relative));
			}
		}
		#endregion

		#endregion
		//------------------------------------------------------------------------------------------------------------------------------------



		//------------------------------------------------------------------------------------------------------------------------------------
		//---Создание, редактирование и удаление предметов------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------------------------------------------
		#region

		/*
			Кнопка включения окна создания предмета
		*/
		#region
		private void ButtonC_CreateSubject(object sender, RoutedEventArgs e)
		{
			if (Subjects_CreateTab.IsSelected) 
			{
				CreateSubjectMenu_Button.IsChecked = false;
				EditSubjectMenu_Button.IsChecked = false;
				Subjects_ListTab.IsSelected = true; 
				return; 
			}
			CreateSubjectMenu_Button.IsChecked = true;
			EditSubjectMenu_Button.IsChecked = false;
			CreateSubject_EventCode.Text = "";
			CreateSubject_SubjectCode.Text = "";
			CreateSubject_Name.Text = "";
			CreateSubject_Description.Text = "";
			CreateSubject_Type.SelectedIndex = 0;
			CreateSubject_MinScore.Text = "";
			//CreateSubject_ProjectFolderPath.Text = "";
			//CreateSubject_RegistrationForm.Text = "";
			//CreateSubject_AnswersForm1.Text = "";
			//CreateSubject_AnswersForm2.Text = "";
			//CreateSubject_LogFile.Text = "";
			CreateSubject_ScoreSystem.IsChecked = true;
			Subjects_CreateTab.IsSelected = true;
		}
		#endregion

		/*
			Кнопка включения окна редактирования предмета
		*/
		#region
		private void ButtonC_EditSubject(object sender, RoutedEventArgs e)
		{
			if (Subjects_EditTab.IsSelected) 
			{
				CreateSubjectMenu_Button.IsChecked = false;
				EditSubjectMenu_Button.IsChecked = false;
				Subjects_ListTab.IsSelected = true; 
				return; 
			}
			Subject SelectedSubject = (Subject)SubjectsList.SelectedItem;
			if (SelectedSubject != null)
			{
				CreateSubjectMenu_Button.IsChecked = false;
				EditSubjectMenu_Button.IsChecked = true;
				EditSubject_EventCode.Text = SelectedSubject.EventCode.ToString();
				EditSubject_SubjectCode.Text = SelectedSubject.SubjectCode.ToString();
				EditSubject_Name.Text = SelectedSubject.Name;
				EditSubject_Description.Text = SelectedSubject.Description;
				EditSubject_Type.SelectedIndex = Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, SelectedSubject.Type)-1;
				EditSubject_MinScore.Text = SelectedSubject.MinScore.ToString();
				//EditSubject_ProjectFolderPath.Text = SelectedSubject.ProjectFolderPath;
				//EditSubject_RegistrationForm.Text = SelectedSubject.RegistrationForm;
				//EditSubject_AnswersForm1.Text = SelectedSubject.AnswersForm1;
				//EditSubject_AnswersForm2.Text = SelectedSubject.AnswersForm2;
				//EditSubject_LogFile.Text = SelectedSubject.LogFile;
				if (SelectedSubject.IsMark) EditSubject_MarkSystem.IsChecked = true; else EditSubject_ScoreSystem.IsChecked = true;
				ICollectionView view = CollectionViewSource.GetDefaultView(Workfield.Subjects);
				view.Refresh();
				Subjects_EditTab.IsSelected = true;
			}
		}
		#endregion

		/*
			Удаление предмета
		*/
		#region
		private void ButtonC_DeleteSubject(object sender, RoutedEventArgs e)
		{
			try
			{
				string command =
					"DELETE FROM Subject WHERE Subject_ID=" + Workfield.CurrentSubject.SubjectID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			Workfield.Subjects.Remove(Workfield.CurrentSubject);
			Workfield.CurrentSubject = null;
			MainField.IsEnabled = false;
			AlertList.ItemsSource = null;
			AlertList.Items.Refresh();
			QuestionList.ItemsSource = null;
			QuestionList.Items.Refresh();
			AnswerList.ItemsSource = null;
			AnswerList.Items.Refresh();
			ScaleList.ItemsSource = null;
			ScaleList.Items.Refresh();
			CurrentVariantCBox.ItemsSource = null;
			CreateSubjectMenu_Button.IsChecked = false;
			EditSubjectMenu_Button.IsChecked = false;
			EditSubjectMenu_Button.IsEnabled = false;
			DeleteSubject_Button.IsEnabled = false;
			CurrentVariantCBox.SelectedIndex = -1;
			CurrentVariantCBox.Items.Refresh();
			CurrentSubjectName1.Content = "";
			CurrentSubjectType1.Content = "";
			CurrentSubjectName2.Content = "";
			CurrentSubjectType2.Content = "";
			CurrentSubjectName3.Content = "";
			CurrentSubjectType3.Content = "";
			CurrentSubjectName4.Content = "";
			CurrentSubjectType4.Content = "";
			CurrentSubjectName5.Content = "";
			CurrentSubjectType5.Content = "";
			CurrentSubjectName6.Content = "";
			CurrentSubjectType6.Content = "";
			CurrentSubjectName7.Content = "";
			CurrentSubjectType7.Content = "";
			CurrentSubjectName8.Content = "";
			CurrentSubjectType8.Content = "";
			MaxScoreTextBlock.Text = "-";
			UpdateSubjectList();
		}
        #endregion

        /*
			Создание предмета
		*/
        #region
        private void ButtonC_CreateSubject_Create(object sender, RoutedEventArgs e)
		{
			if (CreateSubject_Name.Text.Trim(' ') != "" && CreateSubject_Type.SelectedIndex != 0)
			{
                short subjectCode;
                if (CreateSubject_SubjectCode.Text.Trim(' ') == "") subjectCode = -1;
                else subjectCode = short.Parse(CreateSubject_SubjectCode.Text.Trim(' '));
                short eventCode;
                if (CreateSubject_EventCode.Text.Trim(' ') == "") eventCode = -1;
                else eventCode = short.Parse(CreateSubject_EventCode.Text.Trim(' '));
                short minScore;
                if (CreateSubject_MinScore.Text.Trim(' ') == "") minScore = -1;
                else minScore = short.Parse(CreateSubject_MinScore.Text.Trim(' '));

                Subject newSubject = new Subject()
				{
					SubjectCode = subjectCode,
					EventCode = eventCode,
					Name = CreateSubject_Name.Text.Trim(' '),
					Description = CreateSubject_Description.Text.Trim(' '),
					Type = Workfield.SubjectTypes[CreateSubject_Type.SelectedIndex+1],
					MinScore = minScore,
					//ProjectFolderPath = CreateSubject_ProjectFolderPath.Text.Trim(' '),
					//RegistrationForm = CreateSubject_RegistrationForm.Text.Trim(' '),
					//AnswersForm1 = CreateSubject_AnswersForm1.Text.Trim(' '),
					//AnswersForm2 = CreateSubject_AnswersForm2.Text.Trim(' '),
					//LogFile = CreateSubject_LogFile.Text.Trim(' '),
					IsMark = CreateSubject_MarkSystem.IsChecked.Value
				};
				

				try
				{
					string command = 
						"INSERT INTO Subject (SubjectCode, EventCode, MinScore, SubjectName, Description, ControlEvents_FK, " +
                        "ProjectFolderPath, RegistrationFormName, AnswersForm1Name, AnswersForm2Name, LogFileName, isMark) VALUES("
						+newSubject.SubjectCode.ToString()+", "+newSubject.EventCode.ToString()+", "+newSubject.MinScore.ToString()
						+", '"+newSubject.Name+"', '"+newSubject.Description+"', "+(Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, newSubject.Type)).ToString()+", '"+
						newSubject.ProjectFolderPath+"', '"+newSubject.RegistrationForm+"', '"+newSubject.AnswersForm1+"', '"+
						newSubject.AnswersForm2+"', '"+newSubject.LogFile+"', "+Convert.ToInt32(newSubject.IsMark).ToString()+")";
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				try
				{
					string command =
						"SELECT MAX(Subject_ID) FROM Subject";
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						using (SqlDataReader reader = com.ExecuteReader())
                        {
							reader.Read();
							newSubject.SubjectID = reader.GetInt32(0);
                        }
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)1, newSubject });

				Workfield.Subjects.Add(newSubject);
				CreateSubjectMenu_Button.IsChecked = false;
				EditSubjectMenu_Button.IsChecked = false;
				Subjects_ListTab.IsSelected = true;
				UpdateSubjectList();
			}
		}
		#endregion

		/*
			Отмена создания предмета
		*/
		#region
		private void ButtonC_CreateSubject_Cancel(object sender, RoutedEventArgs e)
		{
			CreateSubjectMenu_Button.IsChecked = false;
			EditSubjectMenu_Button.IsChecked = false;
			Subjects_ListTab.IsSelected = true;
		}
		#endregion

		/*
			Редактирование предмета
		*/
		#region
		private void ButtonC_EditSubject_Edit(object sender, RoutedEventArgs e)
		{
			if (EditSubject_Name.Text.Trim(' ') != "" && EditSubject_Type.SelectedIndex != 0)
			{
				Subject SelectedSubject = (Subject)SubjectsList.SelectedItem;
				SelectedSubject.SubjectCode = EditSubject_SubjectCode.Text.Trim(' ') == "" ? (short)-1 : short.Parse(EditSubject_SubjectCode.Text.Trim(' '));
				SelectedSubject.EventCode = EditSubject_EventCode.Text.Trim(' ') == "" ? (short)-1 : short.Parse(EditSubject_EventCode.Text.Trim(' '));
				SelectedSubject.Name = EditSubject_Name.Text.Trim(' ');
				SelectedSubject.Description = EditSubject_Description.Text.Trim(' ');
				SelectedSubject.Type = Workfield.SubjectTypes[EditSubject_Type.SelectedIndex+1];
				SelectedSubject.MinScore = EditSubject_MinScore.Text.Trim(' ') == "" ? (short)-1 : short.Parse(EditSubject_MinScore.Text.Trim(' '));
				//SelectedSubject.ProjectFolderPath = EditSubject_ProjectFolderPath.Text.Trim(' ');
				//SelectedSubject.RegistrationForm = EditSubject_RegistrationForm.Text.Trim(' ');
				//SelectedSubject.AnswersForm1 = EditSubject_AnswersForm1.Text.Trim(' ');
				//SelectedSubject.AnswersForm2 = EditSubject_AnswersForm2.Text.Trim(' ');
				//SelectedSubject.LogFile = EditSubject_LogFile.Text.Trim(' ');
				SelectedSubject.IsMark = EditSubject_MarkSystem.IsChecked.Value;

				try
				{
					string command =
						"UPDATE Subject SET SubjectCode="+SelectedSubject.SubjectCode.ToString()+", EventCode="
						+SelectedSubject.EventCode.ToString()+", SubjectName='"+SelectedSubject.Name+"', Description='"
						+SelectedSubject.Description+"', ControlEvents_FK="+ (Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, SelectedSubject.Type)).ToString()+", " +
                        "MinScore="+SelectedSubject.MinScore.ToString()+", ProjectFolderPath='"+SelectedSubject.ProjectFolderPath
						+"', RegistrationFormName='"+SelectedSubject.RegistrationForm+"', AnswersForm1Name='"
						+SelectedSubject.AnswersForm1+"', AnswersForm2Name='"+SelectedSubject.AnswersForm2+"', LogFileName='"
						+SelectedSubject.LogFile+"', IsMark="+ Convert.ToInt32(SelectedSubject.IsMark).ToString() 
						+" WHERE Subject_ID="+SelectedSubject.SubjectID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)1, SelectedSubject });

				Subjects_ListTab.IsSelected = true;
				CurrentSubjectName1.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType1.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName2.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType2.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName3.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType3.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName4.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType4.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName5.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType5.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName6.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType6.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName7.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType7.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName8.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType8.Content = Workfield.CurrentSubject.Type;
				CreateSubjectMenu_Button.IsChecked = false;
				EditSubjectMenu_Button.IsChecked = false;
				UpdateSubjectList();

				foreach (Blank currentBlank in SelectedSubject.Blanks)
                {
					currentBlank.CurrentBlankTypes = Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, SelectedSubject.Type)];
					currentBlank.Type = currentBlank.CurrentBlankTypes[1];

					try
					{
						string command =
							"UPDATE Blank SET BlankType_FK=" + Workfield.KeyByValue<int, string>(Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, Workfield.CurrentSubject.Type)], currentBlank.Type).ToString()
							+ ", FilePath='" + currentBlank.Path + "' WHERE Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString()
							+ " AND Blank_ID=" + currentBlank.BlankID.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							com.ExecuteNonQuery();
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)5, SelectedSubject, currentBlank });
				}

				Workfield.CurrentBlankTypes = Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, SelectedSubject.Type)];
				BlankList.Items.Refresh();
			}
		}
        #endregion

        /*
			Отмена редактирования предмета
		*/
        #region
        private void ButtonC_EditSubject_Cancel(object sender, RoutedEventArgs e)
		{
			CreateSubjectMenu_Button.IsChecked = false;
			EditSubjectMenu_Button.IsChecked = false;
			Subjects_ListTab.IsSelected = true;
		}
		#endregion

		#endregion
		//------------------------------------------------------------------------------------------------------------------------------------



		//------------------------------------------------------------------------------------------------------------------------------------
		//---Кнопки переключения режимов настройки мероприятия--------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------------------------------------------
		#region

		/*
			Кнопка перехода в режим редактирования шаблонов вопросов
		*/
		#region
		private void ButtonC_EditQuestionsMode(object sender, RoutedEventArgs e)
		{
			EditQuestionsMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = true;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = false;
			EditBlanksMode_Button.IsChecked = false;
			PrintBlanksMode_Button.IsChecked = false;
			LoadCSV_Button.IsChecked = false;
			CheckResultsMode_Button.IsChecked = false;
			PrintReportMode_Button.IsChecked = false;
		}
		#endregion

		/*
			Кнопка перехода в режим редактирования системы шкалирования
		*/
		#region
		private void ButtonC_EditScaleMode(object sender, RoutedEventArgs e)
		{
			EditScaleMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = true;
			EditAnswersMode_Button.IsChecked = false;
			EditBlanksMode_Button.IsChecked = false;
			PrintBlanksMode_Button.IsChecked = false;
			LoadCSV_Button.IsChecked = false;
			CheckResultsMode_Button.IsChecked = false;
			PrintReportMode_Button.IsChecked = false;
		}
		#endregion

		/*
			Кнопка перехода в режим редактирования ответов
		*/
		#region
		private void ButtonC_EditAnswersMode(object sender, RoutedEventArgs e)
		{
			EditAnswersMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = true;
			EditBlanksMode_Button.IsChecked = false;
			PrintBlanksMode_Button.IsChecked = false;
			LoadCSV_Button.IsChecked = false;
			CheckResultsMode_Button.IsChecked = false;
			PrintReportMode_Button.IsChecked = false;
		}
        #endregion

        /*
			Кнопка перехода в режим редактирования бланков
		*/
        #region
        private void ButtonC_EditBlanksMode(object sender, RoutedEventArgs e)
		{
			EditBlanksMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = false;
			EditBlanksMode_Button.IsChecked = true;
			PrintBlanksMode_Button.IsChecked = false;
			LoadCSV_Button.IsChecked = false;
			CheckResultsMode_Button.IsChecked = false;
			PrintReportMode_Button.IsChecked = false;
		}
        #endregion

        /*
			Кнопка перехода в режим печати бланков
		*/
        #region
        private void ButtonC_PrintBlanksMode(object sender, RoutedEventArgs e)
		{
			PrintBlankMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = false;
			EditBlanksMode_Button.IsChecked = false;
			PrintBlanksMode_Button.IsChecked = true;
			LoadCSV_Button.IsChecked = false;
			CheckResultsMode_Button.IsChecked = false;
			PrintReportMode_Button.IsChecked = false;
		}
        #endregion

        /*
			Кнопка перехода в режим загрузки данных из CSV файлов
		*/
        #region
        private void LoadCSV_Button_Click(object sender, RoutedEventArgs e)
		{
			LoadCSVMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = false;
			EditBlanksMode_Button.IsChecked = false;
			PrintBlanksMode_Button.IsChecked = false;
			LoadCSV_Button.IsChecked = true;
			CheckResultsMode_Button.IsChecked = false;
			PrintReportMode_Button.IsChecked = false;
		}
        #endregion

        /*
			Кнопка перехода в режим просмотра результатов
		*/
        #region
        private void ButtonC_CheckResultMode(object sender, RoutedEventArgs e)
		{
			CheckResultsMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = false;
			EditBlanksMode_Button.IsChecked = false;
			PrintBlanksMode_Button.IsChecked = false;
			LoadCSV_Button.IsChecked = false;
			CheckResultsMode_Button.IsChecked = true;
			PrintReportMode_Button.IsChecked = false;
		}
        #endregion

        /*
			Кнопка перехода в режим печати отчета
		*/
        #region
        private void ButtonC_PrintReportMode(object sender, RoutedEventArgs e)
		{
			PrintReportMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = false;
			EditBlanksMode_Button.IsChecked = false;
			PrintBlanksMode_Button.IsChecked = false;
			LoadCSV_Button.IsChecked = false;
			CheckResultsMode_Button.IsChecked = false;
			PrintReportMode_Button.IsChecked = true;
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Создание, редактирование и удаление вариантов------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Кнопка перехода в режим редактирования названия варианта
		*/
        #region
        private void ButtonC_EditVariantNameMode(object sender, RoutedEventArgs e)
		{
			CurrentVariantTextbox.Text = Workfield.CurrentVariant.Name;
			EditVariantMode.IsSelected = true;
		}
        #endregion

        /*
			Кнопка создания варианта
		*/
        #region
        private void ButtonC_CreateVariant(object sender, RoutedEventArgs e)
		{

			Workfield.CurrentSubject.Variants.Add(new Variant());
			EditVariantMode.IsSelected = true;
			CurrentVariantCBox.SelectedIndex = Workfield.CurrentSubject.Variants.Count - 1;
			CurrentVariantCBox.Items.Refresh();
			Workfield.CurrentVariant = Workfield.CurrentSubject.Variants[Workfield.CurrentSubject.Variants.Count - 1];

			try
			{
				string command =
					"INSERT INTO Variant (Subject_FK, VariantName, VariantFilePath) VALUES("+Workfield.CurrentSubject.SubjectID.ToString()
					+", '"+Workfield.CurrentVariant.Name+"', '"+Workfield.CurrentVariant.VariantFilePath+"')";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			try
			{
				string command =
					"SELECT MAX(Variant_ID) FROM Variant";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						reader.Read();
						Workfield.CurrentVariant.VariantID = reader.GetInt32(0);
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			Workfield.CurrentVariant.Name = "•" + Workfield.CurrentVariant.VariantID.ToString();
			if (Workfield.CurrentVariant.Name.Length > 20) Workfield.CurrentVariant.Name = Workfield.CurrentVariant.Name.Substring(0, 20);

			try
			{
				string command =
					"UPDATE Variant SET VariantName='" + Workfield.CurrentVariant.Name + "' WHERE Variant_ID=" + Workfield.CurrentVariant.VariantID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			foreach (AnswerCharacteristic currentQuestion in Workfield.CurrentSubject.Questions)
			{
				Question newQuestion = new Question(currentQuestion)
				{
					InVariant = currentQuestion.Number,
					QuestionType = currentQuestion.QuestionType,
					MaxScore = currentQuestion.MaxScore,
					CheckType = currentQuestion.CheckType,
					Criterion = currentQuestion.Criterion
				};
				Workfield.CurrentVariant.QuestionsPerVar.Add(newQuestion);

				try
				{
					string command =
						"INSERT INTO Question (Variant_FK, QuestionType_FK, TaskNumber, AnswerCharacteristic_FK, MaxScore, CheckType_FK, Criterion) VALUES("
						+ Workfield.CurrentVariant.VariantID.ToString() + ", "
						+ Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, newQuestion.QuestionType).ToString() + ", "
						+ newQuestion.InVariant.ToString() + ", " + newQuestion.QuestionTemplate.AnswerCharacteristicID.ToString()
						+ ", " + newQuestion.QuestionTemplate.MaxScore.ToString() + ", " 
						+ Workfield.KeyByValue<int, string>(Workfield.CheckTypes, newQuestion.CheckType).ToString() + ", '" 
						+ newQuestion.Criterion + "')";
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				try
				{
					string command =
						"SELECT MAX(Question_ID) FROM Question";
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						using (SqlDataReader reader = com.ExecuteReader())
						{
							reader.Read();
							newQuestion.QuestionID = reader.GetInt32(0);
						}
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.NoReferenceResponce, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, newQuestion, false });
				AlertManager.CheckAlerts(AlertType.NotEnoughOrExcessScoreForQuestion, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, newQuestion, null });
			}
			CurrentVariantTextbox.Text = Workfield.CurrentVariant.Name;
			AnswerList.ItemsSource = Workfield.CurrentVariant.QuestionsPerVar;
			AnswerList.Items.Refresh();
		}
        #endregion

        /*
			Кнопка удаления варианта
		*/
        #region
        private void ButtonC_DeleteVariant(object sender, RoutedEventArgs e)
		{
			if (Workfield.CurrentVariant == null) return;

			Variant currentVariant = CurrentVariantCBox.SelectedItem as Variant;

			try
			{
				string command =
					"DELETE FROM Variant WHERE Variant_ID="+currentVariant.VariantID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			foreach (Question currentQuestion in Workfield.CurrentVariant.QuestionsPerVar)
			{
				foreach (Answer currentAnswer in currentQuestion.Answers)
                {
					AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)2, Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, currentAnswer }, true);
				}
				AlertManager.CheckAlerts(AlertType.NoReferenceResponce, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, false }, true);
				AlertManager.CheckAlerts(AlertType.NotEnoughOrExcessScoreForQuestion, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, null }, true);
			}

			Workfield.CurrentSubject.Variants.Remove(currentVariant);
			CurrentVariantCBox.SelectedIndex = -1;
			CurrentVariantCBox.SelectedIndex = 0;
			CurrentVariantCBox.Items.Refresh();
			if (Workfield.CurrentSubject.Variants.Count <= 0)
			{
				Workfield.CurrentVariant = null;
				AnswerList.ItemsSource = null;
			}
			else
			{
				Workfield.CurrentVariant = Workfield.CurrentSubject.Variants[0];
				AnswerList.ItemsSource = Workfield.CurrentVariant.QuestionsPerVar;
			}
			AnswerList.Items.Refresh();
		}
        #endregion

        /*
			Кнопка редактирования названия варианта
		*/
        #region
        private void ButtonC_EditVariantSuccess(object sender, RoutedEventArgs e)
		{
			Workfield.CurrentVariant.Name = CurrentVariantTextbox.Text.Trim(' ');
			if (Workfield.CurrentVariant.Name.Length > 20) Workfield.CurrentVariant.Name = Workfield.CurrentVariant.Name.Substring(0, 20);

			try
			{
				string command =
					"UPDATE Variant SET VariantName='"+Workfield.CurrentVariant.Name+"' WHERE Variant_ID="+Workfield.CurrentVariant.VariantID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			foreach (Question currentQuestion in Workfield.CurrentVariant.QuestionsPerVar)
            {
				foreach (Answer currentAnswer in currentQuestion.Answers)
				{
					AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)2, Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, currentAnswer });
				}
				AlertManager.CheckAlerts(AlertType.NoReferenceResponce, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, false });
				AlertManager.CheckAlerts(AlertType.NotEnoughOrExcessScoreForQuestion, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, null });
			}

			CurrentVariantTextbox.Text = "";
			int currentIndex = CurrentVariantCBox.SelectedIndex;
			CurrentVariantCBox.SelectedIndex = -1;
			CurrentVariantCBox.SelectedIndex = currentIndex;
			CurrentVariantCBox.Items.Refresh();
			SelectVariantMode.IsSelected = true;
			FocusManager.SetFocusedElement(FocusManager.GetFocusScope(CurrentVariantTextbox), null);
			Keyboard.ClearFocus();
		}
        #endregion

        /*
			Кнопка отмены редактирования названия варианта
		*/
        #region
        private void ButtonC_EditVariantCancel(object sender, RoutedEventArgs e)
		{
			CurrentVariantTextbox.Text = "";
			SelectVariantMode.IsSelected = true;
		}
        #endregion

        /*
			Событие выбора варианта в ComboBox
		*/
        #region
        private void CurrentVariantCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CurrentVariantCBox.SelectedIndex < 0) return;
			if (Workfield.CurrentSubject.Variants.Count <= 0) return;

			Workfield.CurrentVariant = Workfield.CurrentSubject.Variants[CurrentVariantCBox.SelectedIndex];

			VariantFilePathLabel.Text = Workfield.CurrentVariant.VariantFilePath;
			if (VariantFilePathLabel.Text.Trim(' ') == "") VariantFilePathLabel.Text = "Выберите путь к файлу варианта";

			AnswerList.ItemsSource = Workfield.CurrentVariant.QuestionsPerVar;
			AnswerList.Items.Refresh();
		}
        #endregion

        /*
			Событие нажатия Enter при редактировании названия варианта
		*/
        #region
        private void CurrentVariantTextbox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				ButtonC_EditVariantSuccess(sender, e);
			}
		}
        #endregion

        /*
			Кнопка выбора файла бланка варианта
		*/
        #region
        private void OpenVariantBlankButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (Workfield.CurrentVariant != null && openFileDialog.ShowDialog() == true)
            {
				Workfield.CurrentVariant.VariantFilePath = openFileDialog.FileName;
				VariantFilePathLabel.Text = openFileDialog.FileName;

				try
				{
					string command =
						"UPDATE Variant SET VariantFilePath='" + Workfield.CurrentVariant.VariantFilePath + "' WHERE Variant_ID=" + Workfield.CurrentVariant.VariantID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}
			}
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Поиск и выбор предметов----------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие выбора предмета в DataGrid
		*/
        #region
        private void SubjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            if (!(SubjectsList.CurrentItem is Subject currentSubject)) return;

			if (Workfield.CurrentSubject != null)
			{
				Workfield.CurrentSubject.Variants = null;
				Workfield.CurrentSubject.Questions = null;
				Workfield.CurrentSubject.Alerts = null;
			}
			Workfield.CurrentVariant = null;
			VariantFilePathLabel.Text = "Выберите путь к файлу варианта";
			Workfield.CurrentQuestion = null;
			Workfield.CurrentAnswerCharacteristic = null;
			Workfield.CurrentBlank = null;
			Workfield.CurrentCodeField = null;
			Workfield.CurrentBlankTypes = new Dictionary<int, string>() { };

            Workfield.CurrentSubject = currentSubject;

			if (currentSubject.Alerts == null)
            {
				currentSubject.Alerts = new ObservableCollection<Alert>()
				{
					new Alert(AlertType.FieldNotFilled),
					new Alert(AlertType.ScoreInconsequence),
					new Alert(AlertType.NoReferenceResponce),
					new Alert(AlertType.NotEnoughOrExcessScoreForQuestion),
					new Alert(AlertType.NoErrorScaleSystem)
				};
			}

			AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)1, currentSubject });

			currentSubject.Blanks = new ObservableCollection<Blank>() { };
			try
			{
				string command =
					"SELECT * FROM Blank WHERE Subject_FK=" + currentSubject.SubjectID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							Blank currentBlank = new Blank()
							{
								BlankID = reader.GetInt32(0),
								Type = Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, currentSubject.Type)][reader.GetInt32(1)],
								Path = reader.GetString(3),
								CurrentBlankTypes = Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, Workfield.CurrentSubject.Type)]
							};
							currentSubject.Blanks.Add(currentBlank);
							AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)5, currentSubject, currentBlank });
						}
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}
			foreach (Blank currentBlank in currentSubject.Blanks)
            {
				currentBlank.CodeFields = new ObservableCollection<CodeField>() { };
				try
				{
					string command =
						"SELECT * FROM CodeField WHERE Blank_FK=" + currentBlank.BlankID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						using (SqlDataReader reader = com.ExecuteReader())
						{
							while (reader.Read())
							{
								CodeField currentCodeField = new CodeField()
								{
									CodeFieldID = reader.GetInt32(0),
									CodeType = Workfield.CodeTypes[reader.GetInt32(1)],
									Orientation = Workfield.OrientationTypes[reader.GetInt32(2)],
									X1 = reader.GetInt32(4),
									Y1 = reader.GetInt32(5),
									X2 = reader.GetInt32(6),
									Y2 = reader.GetInt32(7)
								};
								currentBlank.CodeFields.Add(currentCodeField);
								AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)6, currentSubject, currentBlank, currentCodeField });
							}
						}
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}
			}

			currentSubject.Questions = new ObservableCollection<AnswerCharacteristic>() { };
			try
			{
				string command =
					"SELECT * FROM AnswerCharacteristic WHERE Subject_FK=" + currentSubject.SubjectID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							AnswerCharacteristic currentAnswerCharacteristic = new AnswerCharacteristic()
                            {
								AnswerCharacteristicID = reader.GetInt32(0),
								Number = reader.GetInt16(2),
								Criterion = reader.GetString(3),
								ValidChars = reader.GetString(4),
								QuestionType = Workfield.QuestionTypes[reader.GetInt32(5)],
								CheckType = Workfield.CheckTypes[reader.GetInt32(6)],
								MaxScore = reader.GetInt16(7)
                            };
							if (currentAnswerCharacteristic.QuestionType == Workfield.QuestionTypes[1] || currentAnswerCharacteristic.QuestionType == Workfield.QuestionTypes[4])
                            {
								currentAnswerCharacteristic.CheckTypeList = Workfield.CheckTypes3;
                            }
							else if (currentAnswerCharacteristic.QuestionType == Workfield.QuestionTypes[2])
                            {
								currentAnswerCharacteristic.CheckTypeList = Workfield.CheckTypes1;
                            }
							else if (currentAnswerCharacteristic.QuestionType == Workfield.QuestionTypes[3])
                            {
								currentAnswerCharacteristic.CheckTypeList = Workfield.CheckTypes2;
                            }
							currentSubject.Questions.Add(currentAnswerCharacteristic);
							AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)3, currentSubject, currentAnswerCharacteristic });
						}
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}
			foreach (AnswerCharacteristic currentAnswerCharacteristic in currentSubject.Questions)
            {
				currentAnswerCharacteristic.Errors = new List<ErrorScaleUnit>() { };
				try
				{
					string command =
						"SELECT * FROM ErrorScaleUnit WHERE AnswerCharacteristic_FK=" + currentAnswerCharacteristic.AnswerCharacteristicID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						using (SqlDataReader reader = com.ExecuteReader())
						{
							while (reader.Read())
							{
								currentAnswerCharacteristic.Errors.Add(new ErrorScaleUnit()
								{
									ErrorScaleUnitID = reader.GetInt32(0),
									ErrorCount = reader.GetInt16(1),
									Score = reader.GetInt16(2)
								});
							}
						}
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.NoErrorScaleSystem, new List<object>() { currentSubject, currentAnswerCharacteristic, false });
			}

			currentSubject.Variants = new ObservableCollection<Variant>() { };
			try
			{
				string command =
					"SELECT * FROM Variant WHERE Subject_FK=" + currentSubject.SubjectID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							currentSubject.Variants.Add(new Variant()
							{
								VariantID = reader.GetInt32(0),
								Name = reader.GetString(2),
								VariantFilePath = reader.GetString(3)
							});
						}
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			foreach (Variant currentVariant in currentSubject.Variants)
            {
				currentVariant.QuestionsPerVar = new List<Question>() { };
				try
				{
					string command =
						"SELECT * FROM Question WHERE Variant_FK=" + currentVariant.VariantID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						using (SqlDataReader reader = com.ExecuteReader())
						{
							while (reader.Read())
							{
								AnswerCharacteristic templateQuestion = null;
								int ACID = reader.GetInt32(4);

								foreach (AnswerCharacteristic curAnswerCharacteristic in currentSubject.Questions)
                                {
									if (curAnswerCharacteristic.AnswerCharacteristicID == ACID)
                                    {
										templateQuestion = curAnswerCharacteristic;
                                    }
                                }

								currentVariant.QuestionsPerVar.Add(new Question(templateQuestion)
								{
									QuestionID = reader.GetInt32(0),
									QuestionType = Workfield.QuestionTypes[reader.GetInt32(2)],
									InVariant = (short)reader.GetInt32(3),
									MaxScore = (short)reader.GetInt32(5),
									CheckType = Workfield.CheckTypes[reader.GetInt32(6)],
									Criterion = reader.GetString(7)
								});
							}
						}
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				foreach (Question currentQuestion in currentVariant.QuestionsPerVar)
				{
					currentQuestion.Answers = new List<Answer>() { };
					try
					{
						string command =
							"SELECT * FROM Answer WHERE Question_FK=" + currentQuestion.QuestionID.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							using (SqlDataReader reader = com.ExecuteReader())
							{
								while (reader.Read())
								{
									Answer currentAnswer = new Answer()
									{
										AnswerID = reader.GetInt32(0),
										RightAnswer = reader.GetString(2),
										Score = reader.GetInt16(3)
									};
									currentQuestion.Answers.Add(currentAnswer);
									AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)2, currentSubject, currentVariant, currentQuestion, currentAnswer});
								}
							}
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					AlertManager.CheckAlerts(AlertType.NoReferenceResponce, new List<object>() { currentSubject, currentVariant, currentQuestion, false });
					AlertManager.CheckAlerts(AlertType.NotEnoughOrExcessScoreForQuestion, new List<object>() { currentSubject, currentVariant, currentQuestion, null });
				}
			}

			short maxScore = 0;
			currentSubject.ScaleSystem = new ObservableCollection<ScaleUnit>() { };
			try
			{
				string command =
					"SELECT * FROM ScaleUnit WHERE Subject_FK=" + currentSubject.SubjectID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							currentSubject.ScaleSystem.Add(new ScaleUnit()
							{
								FirstScore = reader.GetInt16(1),
								Mark = reader.GetInt16(2),
								SecondScore = reader.GetInt16(3)
							});
							maxScore = reader.GetInt16(1);
						}
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}
			Workfield.CurrentSubject.MaxScore = maxScore;
			MaxScoreTextBlock.Text = maxScore.ToString();

			AlertManager.CheckAlerts(AlertType.ScoreInconsequence, new List<object>() { currentSubject });

			AlertList.Items.Refresh();
			CurrentSubjectName1.Content = currentSubject.Name;
			CurrentSubjectType1.Content = currentSubject.Type;
			CurrentSubjectName2.Content = currentSubject.Name;
			CurrentSubjectType2.Content = currentSubject.Type;
			CurrentSubjectName3.Content = currentSubject.Name;
			CurrentSubjectType3.Content = currentSubject.Type;
			CurrentSubjectName4.Content = currentSubject.Name;
			CurrentSubjectType4.Content = currentSubject.Type;
			CurrentSubjectName5.Content = currentSubject.Name;
			CurrentSubjectType5.Content = currentSubject.Type;
			CurrentSubjectName6.Content = currentSubject.Name;
			CurrentSubjectType6.Content = currentSubject.Type;
			CurrentSubjectName7.Content = currentSubject.Name;
			CurrentSubjectType7.Content = currentSubject.Type;
			CurrentSubjectName8.Content = currentSubject.Name;
			CurrentSubjectType8.Content = currentSubject.Type;
			ScaleList.ItemsSource = currentSubject.ScaleSystem;
			ScaleList.Items.Refresh();
			QuestionList.ItemsSource = currentSubject.Questions;
			QuestionList.Items.Refresh();
			CurrentVariantCBox.ItemsSource = currentSubject.Variants;
			if (Workfield.CurrentSubject.Variants.Count <= 0)
			{
				Workfield.CurrentVariant = null;
				AnswerList.ItemsSource = null;
			}
			else
			{
				Workfield.CurrentVariant = Workfield.CurrentSubject.Variants[0];
				AnswerList.ItemsSource = Workfield.CurrentVariant.QuestionsPerVar;
			}
			CurrentVariantCBox.SelectedIndex = -1;
			CurrentVariantCBox.SelectedIndex = 0;
			CurrentVariantCBox.Items.Refresh();
			AnswerList.Items.Refresh();
			Workfield.CurrentBlankTypes = Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, currentSubject.Type)];
			BlankList.ItemsSource = currentSubject.Blanks;
			BlankList.Items.Refresh();
			EditSubjectMenu_Button.IsEnabled = true;
			DeleteSubject_Button.IsEnabled = true;
			PrintBlankList.ItemsSource = currentSubject.PrintBlanks;
			PrintBlankList.Items.Refresh();
			MainField.IsEnabled = true;
		}
        #endregion

        /*
			Обновление списка предметов во время поиска
		*/
        #region
        public void UpdateSubjectList()
		{
			string searchName = SearchBox.Text.ToLower().Trim(' ');

			Workfield.ActualSubjects = new ObservableCollection<Subject>() { };

			foreach (Subject curSubject in Workfield.Subjects)
			{
				//if (curSubject.Name.ToLower().Contains(searchName) && ((SubjectTypeEnum)SearchTypes.SelectedItem == SubjectTypeEnum.UNDEFINED || ((SubjectTypeEnum)SearchTypes.SelectedItem != SubjectTypeEnum.UNDEFINED) && curSubject.Type == (SubjectTypeEnum)SearchTypes.SelectedItem))
				if (curSubject.Name.ToLower().Contains(searchName) && (((KeyValuePair<int, string>)SearchTypes.SelectedItem).Value == Workfield.SubjectTypes[1] || (((KeyValuePair<int, string>)SearchTypes.SelectedItem).Value != Workfield.SubjectTypes[1]) && curSubject.Type == ((KeyValuePair<int, string>)SearchTypes.SelectedItem).Value))
				{
					Workfield.ActualSubjects.Add(curSubject);
				}
			}

			SubjectsList.ItemsSource = Workfield.ActualSubjects;
			SubjectsList.Items.Refresh();
		}
        #endregion

        /*
			События изменения текста в TextBox поиска предметов
		*/
        #region
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateSubjectList();
		}
        #endregion

        /*
			Событие выбора типа предмета при поиске в ComboBox
		*/
        #region
        private void SearchTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateSubjectList();
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Создание, редактирование и удаление шаблонов вопросов----------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие добавления нового шаблона вопросов в DataGrid
		*/
        #region
        private void QuestionList_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			e.NewItem = new AnswerCharacteristic();

			int tempNumber = -1;
			try
			{
				string command =
					"SELECT Count(*) FROM AnswerCharacteristic WHERE Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						reader.Read();
						if (reader.GetInt32(0) > 0)
                        {
							tempNumber = reader.GetInt32(0);
						}
                        else
                        {
							((AnswerCharacteristic)e.NewItem).Number = 1;
						}
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			if (tempNumber != -1)
			{
				try
				{
					string command =
						"SELECT MAX(TaskNumber) FROM AnswerCharacteristic WHERE Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						using (SqlDataReader reader = com.ExecuteReader())
						{
							reader.Read();
							((AnswerCharacteristic)e.NewItem).Number = (short)(reader.GetInt16(0) + 1);
						}
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}
			}

			AnswerCharacteristic currentQuestion = e.NewItem as AnswerCharacteristic;

			try
			{
				string command =
					"INSERT INTO AnswerCharacteristic (Subject_FK, QuestionType_FK, CheckType_FK, TaskNumber) VALUES("+Workfield.CurrentSubject.SubjectID.ToString()+", 1, 1, " + ((AnswerCharacteristic)e.NewItem).Number.ToString() + ")";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			try
			{
				string command =
					"SELECT MAX(AnswerCharacteristic_ID) FROM AnswerCharacteristic";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						reader.Read();
						((AnswerCharacteristic)e.NewItem).AnswerCharacteristicID = reader.GetInt32(0);
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			if (((AnswerCharacteristic)e.NewItem).MaxScore >= 0)
				((AnswerCharacteristic)e.NewItem).Errors[0].Score = ((AnswerCharacteristic)e.NewItem).MaxScore;
			else
				((AnswerCharacteristic)e.NewItem).Errors[0].Score = 0;

			try
			{
				string command =
					"INSERT INTO ErrorScaleUnit (AnswerCharacteristic_FK, ErrorCount, Score) VALUES(" + ((AnswerCharacteristic)e.NewItem).AnswerCharacteristicID.ToString() + ", "
					+ ((AnswerCharacteristic)e.NewItem).Errors[0].ErrorCount.ToString() + ", " + ((AnswerCharacteristic)e.NewItem).Errors[0].Score.ToString() + ")";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			try
			{
				string command =
					"SELECT MAX(ErrorScaleUnit_ID) FROM ErrorScaleUnit";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						reader.Read();
						((AnswerCharacteristic)e.NewItem).Errors[0].ErrorScaleUnitID = reader.GetInt32(0);
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			foreach (Variant currentVariant in Workfield.CurrentSubject.Variants)
			{
				Question newQuestion = new Question(currentQuestion)
				{
					Answers = new List<Answer>() { }
				};
				currentVariant.QuestionsPerVar.Add(newQuestion);

				try
				{
					string command =
						"INSERT INTO Question (Variant_FK, QuestionType_FK, TaskNumber, AnswerCharacteristic_FK, CheckType_FK) VALUES(" + currentVariant.VariantID.ToString()
						+ ", " + Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, newQuestion.QuestionType).ToString() + ", " + newQuestion.InVariant.ToString() + ", "
						+ newQuestion.QuestionTemplate.AnswerCharacteristicID.ToString() + ", " 
						+ Workfield.KeyByValue<int, string>(Workfield.CheckTypes, newQuestion.CheckType).ToString() + ")";
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
				}

				try
				{
					string command =
						"SELECT MAX(Question_ID) FROM Question";
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						using (SqlDataReader reader = com.ExecuteReader())
						{
							reader.Read();
							newQuestion.QuestionID = reader.GetInt32(0);
						}
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}
			}
			AnswerList.Items.Refresh();
		}
        #endregion

        /*
			Событие удаления шаблона вопроса в DataGrid
		*/
        #region
        private void QuestionList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			DataGrid grid = (DataGrid)sender;
			if (e.Command == DataGrid.DeleteCommand)
			{
				Workfield.focusedControl = FocusManager.GetFocusedElement(this);
				if (Workfield.focusedControl is DataGridCell && ((DataGridCell)Workfield.focusedControl).DataContext is ErrorScaleUnit) return;

				AnswerCharacteristic currentQuestion = grid.SelectedItem as AnswerCharacteristic;

				foreach (Variant currentVariant in Workfield.CurrentSubject.Variants)
				{
					Question questionToDelete = null;

					foreach (Question questionPost in currentVariant.QuestionsPerVar)
					{
						if (questionPost.QuestionTemplate.Equals(currentQuestion))
						{
							questionToDelete = questionPost;

							foreach (Answer currentAnswer in questionPost.Answers)
							{
								AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)2, Workfield.CurrentSubject, Workfield.CurrentVariant, questionPost, currentAnswer }, true);
							}
							AlertManager.CheckAlerts(AlertType.NoReferenceResponce, new List<object>() { Workfield.CurrentSubject, currentVariant, questionPost, false }, true);
							AlertManager.CheckAlerts(AlertType.NotEnoughOrExcessScoreForQuestion, new List<object>() { Workfield.CurrentSubject, currentVariant, questionPost, null }, true);
						}
					}

					if (questionToDelete != null)
					{
						currentVariant.QuestionsPerVar.Remove(questionToDelete);
					}
				}

				try
				{
					string command =
						"DELETE FROM Question WHERE AnswerCharacteristic_FK=" + currentQuestion.AnswerCharacteristicID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AnswerList.Items.Refresh();

				try
				{
					string command =
						"DELETE FROM AnswerCharacteristic WHERE AnswerCharacteristic_ID=" + currentQuestion.AnswerCharacteristicID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				Workfield.CurrentSubject.Questions.Remove(currentQuestion);
				QuestionList.Items.Refresh();

				short maxScore = 0;
				foreach (AnswerCharacteristic curQuestion in Workfield.CurrentSubject.Questions)
				{
					if (curQuestion.MaxScore <= 0) continue;
					maxScore += curQuestion.MaxScore;
				}

				short preCount = (short)Workfield.CurrentSubject.ScaleSystem.Count;

				if (preCount > maxScore)
				{
					for (short i = preCount; i > maxScore; i--)
					{
						Workfield.CurrentSubject.ScaleSystem.RemoveAt(i - 1);
					}

					try
					{
						string command =
							"DELETE FROM ScaleUnit WHERE Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString() + " AND FirstScore > " + maxScore.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							com.ExecuteNonQuery();
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					ScaleList.Items.Refresh();
				}
				Workfield.CurrentSubject.MaxScore = maxScore;
				MaxScoreTextBlock.Text = maxScore.ToString();

				e.Handled = true;

				AlertManager.CheckAlerts(AlertType.ScoreInconsequence, new List<object>() { Workfield.CurrentSubject });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)3, Workfield.CurrentSubject, currentQuestion }, true);
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)4, Workfield.CurrentSubject, currentQuestion }, true);
				AlertManager.CheckAlerts(AlertType.NoErrorScaleSystem, new List<object>() { Workfield.CurrentSubject, currentQuestion, false }, true);
			}
		}
        #endregion

        /*
			Событие подтверждения редактирования шаблона вопроса в DataGrid
		*/
        #region
        private bool flagfix = true;
		private void QuestionList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit && flagfix)
			{
				int tempNumber = -1;
				int tempID = -1;
				short oldNumber = -1;
				short maxNumber = -1;

				if (CheckQuestionNumber(((AnswerCharacteristic)e.Row.Item).Number, ((AnswerCharacteristic)e.Row.Item).Criterion))
                {
					try
					{
						string command =
							"SELECT TaskNumber FROM AnswerCharacteristic WHERE Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString() + " AND AnswerCharacteristic_ID=" + ((AnswerCharacteristic)e.Row.Item).AnswerCharacteristicID.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							using (SqlDataReader reader = com.ExecuteReader())
							{
								reader.Read();
								oldNumber = reader.GetInt16(0);
							}
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					try
					{
						string command =
							"SELECT Count(*) FROM AnswerCharacteristic WHERE Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							using (SqlDataReader reader = com.ExecuteReader())
							{
								reader.Read();
								if (reader.GetInt32(0) > 0)
								{
									tempNumber = reader.GetInt32(0);
								}
								else
								{
									((AnswerCharacteristic)e.Row.Item).Number = 1;
								}
							}
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					try
					{
						string command =
							"SELECT MAX(TaskNumber) FROM AnswerCharacteristic WHERE Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							using (SqlDataReader reader = com.ExecuteReader())
							{
								reader.Read();
								maxNumber = reader.GetInt16(0);
							}
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					try
					{
						string command =
							"SELECT AnswerCharacteristic_ID FROM AnswerCharacteristic WHERE Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString() + " AND TaskNumber=" + ((AnswerCharacteristic)e.Row.Item).Number.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							using (SqlDataReader reader = com.ExecuteReader())
							{
								reader.Read();
								tempID = reader.GetInt32(0);
							}
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					if (tempNumber != -1 && tempID != ((AnswerCharacteristic)e.Row.Item).AnswerCharacteristicID)
					{
						if (((AnswerCharacteristic)e.Row.Item).Number == oldNumber)
							((AnswerCharacteristic)e.Row.Item).Number = ++maxNumber;
						else
							((AnswerCharacteristic)e.Row.Item).Number = oldNumber;
					}
				}

				AnswerCharacteristic currentQuestion = e.Row.Item as AnswerCharacteristic;

				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)4, Workfield.CurrentSubject, currentQuestion });
				try
				{
					string command =
						"UPDATE AnswerCharacteristic SET TaskNumber="+currentQuestion.Number.ToString()+", Criterion='"+currentQuestion.Criterion+"', AllowedChars='"
						+currentQuestion.ValidChars+"', QuestionType_FK="+Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, currentQuestion.QuestionType).ToString()+", CheckType_FK="
						+Workfield.KeyByValue<int, string>(Workfield.CheckTypes, currentQuestion.CheckType).ToString()+", MaxScore="+currentQuestion.MaxScore.ToString()+" WHERE AnswerCharacteristic_ID="
						+currentQuestion.AnswerCharacteristicID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				if (currentQuestion.Errors.Count == 1)
                {
					if (currentQuestion.MaxScore >= 0)
						((AnswerCharacteristic)e.Row.Item).Errors[0].Score = currentQuestion.MaxScore;
					else
						((AnswerCharacteristic)e.Row.Item).Errors[0].Score = 0;

					try
					{
						string command =
							"UPDATE ErrorScaleUnit SET Score=" + currentQuestion.MaxScore.ToString() + " WHERE ErrorScaleUnit_ID="
							+ currentQuestion.Errors[0].ErrorScaleUnitID.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							com.ExecuteNonQuery();
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}
				}

				foreach (Variant currentVariant in Workfield.CurrentSubject.Variants)
				{
					foreach (Question questionPost in currentVariant.QuestionsPerVar)
					{
						if (questionPost.QuestionTemplate.Equals(currentQuestion))
						{
							questionPost.InVariant = currentQuestion.Number;
							questionPost.QuestionType = currentQuestion.QuestionType;
							questionPost.MaxScore = currentQuestion.MaxScore;
							questionPost.Criterion = currentQuestion.Criterion;
							questionPost.CheckType = currentQuestion.CheckType;

							try
							{
								string command =
									"UPDATE Question SET TaskNumber='" + currentQuestion.Number.ToString() + "', QuestionType_FK="
									+ Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, currentQuestion.QuestionType).ToString()
									+ ", MaxScore=" + currentQuestion.MaxScore.ToString() + ", Criterion='" 
									+ currentQuestion.Criterion + "' , CheckType_FK=" 
									+ Workfield.KeyByValue<int, string>(Workfield.CheckTypes, currentQuestion.CheckType).ToString()
									+ " WHERE Question_ID=" + questionPost.QuestionID.ToString() + " AND Variant_FK="+currentVariant.VariantID.ToString();
								using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
								{
									Workfield.SQLConnection.Open();
									com.ExecuteNonQuery();
									Workfield.SQLConnection.Close();
								}
							}
							catch (Exception error)
							{
								Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
								Workfield.WorkWindow.IsEnabled = false;
								Workfield.SQLErrorWindow.Show();
								Workfield.isFatalError = true;
								return;
							}

							foreach (Answer currentAnswer in questionPost.Answers)
                            {
								AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)2, Workfield.CurrentSubject, Workfield.CurrentVariant, questionPost, currentAnswer });
							}
							AlertManager.CheckAlerts(AlertType.NoReferenceResponce, new List<object>() { Workfield.CurrentSubject, currentVariant, questionPost, false });
							AlertManager.CheckAlerts(AlertType.NotEnoughOrExcessScoreForQuestion, new List<object>() { Workfield.CurrentSubject, currentVariant, questionPost, null });
						}
					}
				}
				AnswerList.Items.Refresh();

				short maxScore = 0;
				foreach (AnswerCharacteristic curQuestion in Workfield.CurrentSubject.Questions)
				{
					if (curQuestion.MaxScore <= 0) continue;
					maxScore += curQuestion.MaxScore;
				}

				short preCount = (short)Workfield.CurrentSubject.ScaleSystem.Count;

				if (preCount > maxScore)
				{
					for (short i = preCount; i > maxScore; i--)
					{
						Workfield.CurrentSubject.ScaleSystem.RemoveAt(i - 1);
					}

					try
					{
						string command =
							"DELETE FROM ScaleUnit WHERE Subject_FK="+Workfield.CurrentSubject.SubjectID.ToString()+" AND FirstScore > "+maxScore.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							com.ExecuteNonQuery();
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					ScaleList.Items.Refresh();
				}
				else if (preCount < maxScore)
				{
					for (short i = 1; i <= maxScore - preCount; i++)
					{
						ScaleUnit newScaleUnit = new ScaleUnit() { FirstScore = (short)(preCount + i) };
						Workfield.CurrentSubject.ScaleSystem.Add(newScaleUnit);

						try
						{
							string command =
								"INSERT INTO ScaleUnit (FirstScore, Mark, SecondScore, Subject_FK) VALUES("+newScaleUnit.FirstScore.ToString()+", "
								+newScaleUnit.Mark.ToString()+", "+newScaleUnit.SecondScore.ToString()+", "+Workfield.CurrentSubject.SubjectID.ToString()+")";
							using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
							{
								Workfield.SQLConnection.Open();
								com.ExecuteNonQuery();
								Workfield.SQLConnection.Close();
							}
						}
						catch (Exception error)
						{
							Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
							Workfield.WorkWindow.IsEnabled = false;
							Workfield.SQLErrorWindow.Show();
							Workfield.isFatalError = true;
							return;
						}
					}
					ScaleList.Items.Refresh();
				}
				Workfield.CurrentSubject.MaxScore = maxScore;
				MaxScoreTextBlock.Text = maxScore.ToString();

				AlertManager.CheckAlerts(AlertType.ScoreInconsequence, new List<object>() { Workfield.CurrentSubject });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)3, Workfield.CurrentSubject, currentQuestion });
				AlertManager.CheckAlerts(AlertType.NoErrorScaleSystem, new List<object>() { Workfield.CurrentSubject, currentQuestion, false });

				flagfix = false;
				QuestionList.CommitEdit();
				QuestionList.CommitEdit();
				flagfix = true;
				QuestionList.Items.Refresh();
			}
		}
        #endregion

        /*
			Проверка наличия номера вопроса в базе
		*/
        #region
        private bool CheckQuestionNumber(int number, string criterion)
        {
			bool result = false;

			try
			{
				string command = 
					"SELECT Count(*) FROM AnswerCharacteristic WHERE TaskNumber=" + number.ToString() + " AND Criterion='" + criterion + "' AND Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString();
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						reader.Read();
						result = reader.GetInt32(0) > 0 ? true : false;
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return result;
			}

			return result;
        }
        #endregion

        /*
			Перезагрузка содержимого datagrid
		*/
        #region
        private void QuestionList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			int temp = QuestionList.SelectedIndex;
			try
			{
				DataGridRow row1 = (DataGridRow)QuestionList.ItemContainerGenerator.ContainerFromIndex(QuestionList.SelectedIndex);
				if (row1 == null) return;
				QuestionList.Items.Refresh();
				QuestionList.SelectedIndex = temp;
				if (temp >= QuestionList.Items.Count) return;
				DataGridRow row = (DataGridRow)QuestionList.ItemContainerGenerator.ContainerFromIndex(temp);
				if (row == null)
				{
					QuestionList.UpdateLayout();
					QuestionList.ScrollIntoView(QuestionList.Items[temp]);
					row = (DataGridRow)QuestionList.ItemContainerGenerator.ContainerFromIndex(temp);
				}
				if (row != null)
				{
					row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
				}
			}
			catch { }
		}
		#endregion

		#endregion
		//------------------------------------------------------------------------------------------------------------------------------------



		//------------------------------------------------------------------------------------------------------------------------------------
		//---Создание, редактирование и удаление эталонных ответов----------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------------------------------------------
		#region

		/*
			Событие добавления нового эталонного ответа в DataGrid 
		*/
		#region
		private void AnswersPerQuestionList_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			Question currentQuestion = AnswerList.SelectedItem as Question;

			e.NewItem = new Answer();
			((Answer)e.NewItem).Score = currentQuestion.MaxScore;
			Answer newAnswer = e.NewItem as Answer;

			try
			{
				string command =
					"INSERT INTO Answer (Question_FK, RightAnswer, Score) VALUES(" + currentQuestion.QuestionID.ToString() + ", '"
					+ newAnswer.RightAnswer + "', " + newAnswer.Score.ToString() + ")";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			try
			{
				string command =
					"SELECT MAX(Answer_ID) FROM Answer";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						reader.Read();
						((Answer)e.NewItem).AnswerID = reader.GetInt32(0);
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}
		}
        #endregion

        /*
			Событие удаления эталонного ответа в DataGrid 
		*/
        #region
        private void AnswersPerQuestionList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			DataGrid grid = (DataGrid)sender;
			if (e.Command == DataGrid.DeleteCommand)
			{
				Answer currentAnswer = grid.SelectedItem as Answer;
				Question currentQuestion = AnswerList.SelectedItem as Question;

				try
				{
					string command =
						"DELETE FROM Answer WHERE Answer_ID=" + currentAnswer.AnswerID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)2, Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, currentAnswer }, true);
				AlertManager.CheckAlerts(AlertType.NotEnoughOrExcessScoreForQuestion, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, currentAnswer });
				AlertManager.CheckAlerts(AlertType.NoReferenceResponce, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, true });
			}
		}
        #endregion

        /*
			Событие подтверждения редактирования эталонного ответа в DataGrid
		*/
        #region
        private void AnswersPerQuestionList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				Answer currentAnswer = e.Row.Item as Answer;
				Question currentQuestion = AnswerList.SelectedItem as Question;

				try
				{
					string command =
						"UPDATE Answer SET RightAnswer='" + currentAnswer.RightAnswer + "', Score="
						+ currentAnswer.Score.ToString() + " WHERE Question_FK=" + currentQuestion.QuestionID.ToString()
						+ " AND Answer_ID=" + currentAnswer.AnswerID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)2, Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, currentAnswer });
				AlertManager.CheckAlerts(AlertType.NotEnoughOrExcessScoreForQuestion, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, null });
				AlertManager.CheckAlerts(AlertType.NoReferenceResponce, new List<object>() { Workfield.CurrentSubject, Workfield.CurrentVariant, currentQuestion, false });
			}
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Редактирование системы шкалирования----------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие подтверждения редактирования в DataGrid системы шкалирования
		*/
        #region
        private void ScaleList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				ScaleUnit currentScaleUnit = e.Row.Item as ScaleUnit;

				if (currentScaleUnit.Mark > 5) currentScaleUnit.Mark = 5;
				if (currentScaleUnit.Mark <= 0) currentScaleUnit.Mark = 1;
				if (currentScaleUnit.SecondScore <= 0) currentScaleUnit.SecondScore = 1;

				try
				{
					string command =
						"UPDATE ScaleUnit SET Mark=" + currentScaleUnit.Mark.ToString() + ", SecondScore="
						+ currentScaleUnit.SecondScore.ToString() + " WHERE FirstScore=" + currentScaleUnit.FirstScore.ToString()
						+ " AND Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.ScoreInconsequence, new List<object>() { Workfield.CurrentSubject });
			}
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Создание, редактирование и удаление разбалловки ошибок---------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие добавления нового юнита разбаловки ошибок в DataGrid
		*/
        #region
        private void ErrorsPerQuestionList_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			e.NewItem = new ErrorScaleUnit();
			ErrorScaleUnit newErrorScaleUnit = e.NewItem as ErrorScaleUnit;

			AnswerCharacteristic currentAnswerCharacteristic = QuestionList.SelectedItem as AnswerCharacteristic;

			try
			{
				string command =
					"INSERT INTO ErrorScaleUnit (AnswerCharacteristic_FK, ErrorCount, Score) VALUES(" + currentAnswerCharacteristic.AnswerCharacteristicID.ToString() + ", "
					+ newErrorScaleUnit.ErrorCount.ToString() + ", " + newErrorScaleUnit.Score.ToString() + ")";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			try
			{
				string command =
					"SELECT MAX(ErrorScaleUnit_ID) FROM ErrorScaleUnit";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						reader.Read();
						((ErrorScaleUnit)e.NewItem).ErrorScaleUnitID = reader.GetInt32(0);
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}
		}
        #endregion

        /*
			Событие удаления юнита разбаловки ошибок в DataGrid
		*/
        #region
        private void ErrorsPerQuestionList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			DataGrid grid = (DataGrid)sender;
			if (e.Command == DataGrid.DeleteCommand)
			{
				AnswerCharacteristic currentAnswerCharacteristic = QuestionList.SelectedItem as AnswerCharacteristic;
				ErrorScaleUnit currentErrorScaleUnit = grid.SelectedItem as ErrorScaleUnit;

				try
				{
					string command =
						"DELETE FROM ErrorScaleUnit WHERE ErrorScaleUnit_ID=" + currentErrorScaleUnit.ErrorScaleUnitID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.NoErrorScaleSystem, new List<object>() { Workfield.CurrentSubject, currentAnswerCharacteristic, true });
			}
		}
        #endregion

        /*
			Событие подтверждения редактирования юнита разбаловки ошибок в DataGrid
		*/
        #region
        private void ErrorsPerQuestionList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				ErrorScaleUnit currentErrorScaleUnit = e.Row.Item as ErrorScaleUnit;
				AnswerCharacteristic currentAnswerCharacteristic = QuestionList.SelectedItem as AnswerCharacteristic;

				try
				{
					string command =
						"UPDATE ErrorScaleUnit SET ErrorCount=" + currentErrorScaleUnit.ErrorCount.ToString() + ", Score="
						+ currentErrorScaleUnit.Score.ToString() + " WHERE AnswerCharacteristic_FK=" + currentAnswerCharacteristic.AnswerCharacteristicID.ToString()
						+ " AND ErrorScaleUnit_ID=" + currentErrorScaleUnit.ErrorScaleUnitID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.NoErrorScaleSystem, new List<object>() { Workfield.CurrentSubject, currentAnswerCharacteristic, false });
			}
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Создание, редактирование и удаление бланков--------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие добавления нового бланка в DataGrid
		*/
        #region
        private void BlankList_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			e.NewItem = new Blank();
			Blank newBlank = e.NewItem as Blank;

			try
			{
				string command =
					"INSERT INTO Blank (Subject_FK, BlankType_FK, FilePath) VALUES(" + Workfield.CurrentSubject.SubjectID.ToString() + ", "
					+ Workfield.KeyByValue<int, string>(Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, Workfield.CurrentSubject.Type)], newBlank.Type).ToString() 
					+ ", '" + newBlank.Path + "')";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			try
			{
				string command =
					"SELECT MAX(Blank_ID) FROM Blank";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						reader.Read();
						((Blank)e.NewItem).BlankID = reader.GetInt32(0);
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}
		}
        #endregion

        /*
			Событие удаления бланка в DataGrid
		*/
        #region
        private void BlankList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			DataGrid grid = (DataGrid)sender;
			if (e.Command == DataGrid.DeleteCommand)
			{
				Blank currentBlank = grid.SelectedItem as Blank;

				try
				{
					string command =
						"DELETE FROM Blank WHERE Blank_ID=" + currentBlank.BlankID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				Workfield.CurrentBlank = null;
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)5, Workfield.CurrentSubject, currentBlank }, true);
			}
		}
        #endregion

        /*
			Событие подтверждения редактирования бланка в DataGrid
		*/
        #region
        private void BlankList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				Blank currentBlank = e.Row.Item as Blank;

				try
				{
					string command =
						"UPDATE Blank SET BlankType_FK=" + Workfield.KeyByValue<int, string>(Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, Workfield.CurrentSubject.Type)], currentBlank.Type).ToString()
						+ ", FilePath='" + currentBlank.Path + "' WHERE Subject_FK=" + Workfield.CurrentSubject.SubjectID.ToString()
						+ " AND Blank_ID=" + currentBlank.BlankID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)5, Workfield.CurrentSubject, currentBlank });
			}
		}
        #endregion

        /*
			Кнопка выбора файла бланка
		*/
        #region
        private void OpenFileBlankButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			Blank currentBlank = BlankList.SelectedItem as Blank;

			openFileDialog.Filter = "Страница бланка в формате изображения|*png;*jpeg;*jpg";

			if (currentBlank != null && openFileDialog.ShowDialog() == true)
            {
				currentBlank.Path = openFileDialog.FileName;
				if (currentBlank.Path.Trim(' ') != "")
                {
					BlankImageField.Source = new BitmapImage(new Uri(currentBlank.Path, UriKind.Absolute));
				}
            }
		}
        #endregion

        /*
			Выбор типа бланка
		*/
        #region
        private void BlankList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Blank currentBlank = BlankList.SelectedItem as Blank;

			if (currentBlank != null)
            {
				Workfield.CurrentBlank = currentBlank;
				if (currentBlank.Path.Trim(' ') != "" && File.Exists(currentBlank.Path) 
					&& (currentBlank.Path.ToLower().Substring(Math.Max(0, currentBlank.Path.Length - 4)) == ".jpg" ||
					currentBlank.Path.ToLower().Substring(Math.Max(0, currentBlank.Path.Length - 4)) == ".png" ||
					currentBlank.Path.ToLower().Substring(Math.Max(0, currentBlank.Path.Length - 5)) == ".jpeg"))
				{
					BlankImageField.Source = new BitmapImage(new Uri(currentBlank.Path, UriKind.Absolute));
					CodeFieldsList.ItemsSource = currentBlank.CodeFields;
				}
			}
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Создание, редактирование и удаление полей кода-----------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие добавления поля кода в DataGrid
		*/
        #region
        private void CodeFieldsList_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			e.NewItem = new CodeField();
			CodeField newCodeField = e.NewItem as CodeField;

			try
			{
				string command =
					"INSERT INTO CodeField (Blank_FK, CodeType_FK, Orientation, X1, Y1, X2, Y2) VALUES(" + Workfield.CurrentBlank.BlankID.ToString() + ", "
					+ Workfield.KeyByValue<int, string>(Workfield.CodeTypes, newCodeField.CodeType).ToString()
					+ ", " + Workfield.KeyByValue<int, string>(Workfield.OrientationTypes, newCodeField.Orientation).ToString() + ", " 
					+ newCodeField.X1.ToString() + ", " + newCodeField.Y1.ToString() + ", " + newCodeField.X2.ToString() + ", " + newCodeField.Y2.ToString() + ")";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					com.ExecuteNonQuery();
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}

			try
			{
				string command =
					"SELECT MAX(CodeField_ID) FROM CodeField";
				using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
				{
					Workfield.SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						reader.Read();
						((CodeField)e.NewItem).CodeFieldID = reader.GetInt32(0);
					}
					Workfield.SQLConnection.Close();
				}
			}
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
			}
		}
        #endregion

        /*
			Событие подтверждения редактирования поля кода в DataGrid
		*/
        #region
        private void CodeFieldsList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			DataGrid grid = (DataGrid)sender;
			if (e.Command == DataGrid.DeleteCommand)
			{
				CodeField currentCodeField = grid.SelectedItem as CodeField;

				try
				{
					string command =
						"DELETE FROM CodeField WHERE CodeField_ID=" + currentCodeField.CodeFieldID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				Workfield.CurrentCodeField = null;
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)6, Workfield.CurrentSubject, Workfield.CurrentBlank, currentCodeField }, true);
				CodeBorder.Visibility = Visibility.Hidden;
			}
		}
        #endregion

        /*
			Событие удаления поля кода в DataGrid
		*/
        #region
        private void CodeFieldsList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				CodeField currentCodeField = e.Row.Item as CodeField;

				try
				{
					string command =
						"UPDATE CodeField SET CodeType_FK=" + Workfield.KeyByValue<int, string>(Workfield.CodeTypes, currentCodeField.CodeType).ToString()
						+ ", Orientation=" + Workfield.KeyByValue<int, string>(Workfield.OrientationTypes, currentCodeField.Orientation).ToString() 
						+ ", X1=" + currentCodeField.X1.ToString() + ", Y1=" + currentCodeField.Y1.ToString()
						+ ", X2=" + currentCodeField.X2.ToString() + ", Y2=" + currentCodeField.Y2.ToString()
						+ " WHERE CodeField_ID=" + currentCodeField.CodeFieldID.ToString();
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (Exception error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
					return;
				}

				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new List<object>() { (int)6, Workfield.CurrentSubject, Workfield.CurrentBlank, currentCodeField });
			}
		}
        #endregion

        /*
			Выбор поля кода
		*/
        #region
        private void CodeFieldsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CodeField currentCodeField = CodeFieldsList.SelectedItem as CodeField;

			if (currentCodeField != null)
            {
				Workfield.CurrentCodeField = currentCodeField;
				UpdateRectangle();
				CodeBorder.Visibility = Visibility.Visible;
            }
		}
        #endregion

        /*
			Нажатие ЛКМ по полю изображения бланка
		*/
        #region
        private void BlankImageField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			LMBBlankImageEvent(sender, e.GetPosition(BlankImageField));
		}
        #endregion

        /*
			Нажатие ПКМ по полю изображения бланка
		*/
        #region
        private void BlankImageField_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			RMBBlankImageEvent(sender, e.GetPosition(BlankImageField));
		}
        #endregion

        /*
			Событие обновления визуализации поля кода
		*/
        #region
        private void UpdateRectangle()
        {
			if (Workfield.CurrentCodeField == null) return;
			if (Workfield.CurrentCodeField.X2 < Workfield.CurrentCodeField.X1)
			{
				Canvas.SetLeft(CodeBorder, (double)Workfield.CurrentCodeField.X2 * BlankImageField.ActualWidth / (BlankImageField.Source as BitmapSource).PixelWidth);
				CodeBorder.SetValue(Canvas.WidthProperty, (double)(Workfield.CurrentCodeField.X1 - Workfield.CurrentCodeField.X2) * BlankImageField.ActualWidth / (BlankImageField.Source as BitmapSource).PixelWidth);
			}
			else
			{
				Canvas.SetLeft(CodeBorder, (double)Workfield.CurrentCodeField.X1 * BlankImageField.ActualWidth / (BlankImageField.Source as BitmapSource).PixelWidth);
				CodeBorder.SetValue(Canvas.WidthProperty, (double)(Workfield.CurrentCodeField.X2 - Workfield.CurrentCodeField.X1) * BlankImageField.ActualWidth / (BlankImageField.Source as BitmapSource).PixelWidth);
			}
			if (Workfield.CurrentCodeField.Y2 < Workfield.CurrentCodeField.Y1)
			{
				Canvas.SetTop(CodeBorder, (double)Workfield.CurrentCodeField.Y2 * BlankImageField.ActualHeight / (BlankImageField.Source as BitmapSource).PixelHeight);
				CodeBorder.SetValue(Canvas.HeightProperty, (double)(Workfield.CurrentCodeField.Y1 - Workfield.CurrentCodeField.Y2) * BlankImageField.ActualHeight / (BlankImageField.Source as BitmapSource).PixelHeight);
			}
			else
			{
				Canvas.SetTop(CodeBorder, (double)Workfield.CurrentCodeField.Y1 * BlankImageField.ActualHeight / (BlankImageField.Source as BitmapSource).PixelHeight);
				CodeBorder.SetValue(Canvas.HeightProperty, (double)(Workfield.CurrentCodeField.Y2 - Workfield.CurrentCodeField.Y1) * BlankImageField.ActualHeight / (BlankImageField.Source as BitmapSource).PixelHeight);
			}
		}
        #endregion

        /*
			Событие изменения размеров области изображения
		*/
        #region
        private void BlankImageField_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateRectangle();
		}
        #endregion

        /*
			Событие движения мыши по области изображения
		*/
        #region
        private void BlankImageField_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
            {
				LMBBlankImageEvent(sender, e.GetPosition(BlankImageField));
            }
			if (e.RightButton == MouseButtonState.Pressed)
            {
				RMBBlankImageEvent(sender, e.GetPosition(BlankImageField));
            }
		}
        #endregion

        /*
			Метод обработки первой точки поля на изображении бланка
		*/
        #region
        private void LMBBlankImageEvent(object sender, Point mousePosition)
        {
			if (Workfield.CurrentCodeField != null)
			{
				if (BlankImageField != null && BlankImageField.Source != null)
				{
					// Convert from control space to image space
					int x = (int)Math.Floor(mousePosition.X * (BlankImageField.Source as BitmapSource).PixelWidth / BlankImageField.ActualWidth);
					int y = (int)Math.Floor(mousePosition.Y * (BlankImageField.Source as BitmapSource).PixelHeight / BlankImageField.ActualHeight);

					Workfield.CurrentCodeField.X1 = x;
					Workfield.CurrentCodeField.Y1 = y;
					CodeFieldsList.Items.Refresh();

					try
					{
						string command =
							"UPDATE CodeField SET CodeType_FK=" + Workfield.KeyByValue<int, string>(Workfield.CodeTypes, Workfield.CurrentCodeField.CodeType).ToString()
							+ ", Orientation=" + Workfield.KeyByValue<int, string>(Workfield.OrientationTypes, Workfield.CurrentCodeField.Orientation).ToString()
							+ ", X1=" + Workfield.CurrentCodeField.X1.ToString() + ", Y1=" + Workfield.CurrentCodeField.Y1.ToString()
							+ ", X2=" + Workfield.CurrentCodeField.X2.ToString() + ", Y2=" + Workfield.CurrentCodeField.Y2.ToString()
							+ " WHERE CodeField_ID=" + Workfield.CurrentCodeField.CodeFieldID.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							com.ExecuteNonQuery();
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					UpdateRectangle();
					CodeBorder.Visibility = Visibility.Visible;
				}
			}
		}
        #endregion

        /*
			Метод обработки второй точки поля на изображении бланка
		*/
        #region
        private void RMBBlankImageEvent(object sender, Point mousePosition)
        {
			if (Workfield.CurrentCodeField != null)
			{
				if (BlankImageField != null && BlankImageField.Source != null)
				{
					// Convert from control space to image space
					int x = (int)Math.Floor(mousePosition.X * (BlankImageField.Source as BitmapSource).PixelWidth / BlankImageField.ActualWidth);
					int y = (int)Math.Floor(mousePosition.Y * (BlankImageField.Source as BitmapSource).PixelHeight / BlankImageField.ActualHeight);

					Workfield.CurrentCodeField.X2 = x;
					Workfield.CurrentCodeField.Y2 = y;
					CodeFieldsList.Items.Refresh();

					try
					{
						string command =
							"UPDATE CodeField SET CodeType_FK=" + Workfield.KeyByValue<int, string>(Workfield.CodeTypes, Workfield.CurrentCodeField.CodeType).ToString()
							+ ", Orientation=" + Workfield.KeyByValue<int, string>(Workfield.OrientationTypes, Workfield.CurrentCodeField.Orientation).ToString()
							+ ", X1=" + Workfield.CurrentCodeField.X1.ToString() + ", Y1=" + Workfield.CurrentCodeField.Y1.ToString()
							+ ", X2=" + Workfield.CurrentCodeField.X2.ToString() + ", Y2=" + Workfield.CurrentCodeField.Y2.ToString()
							+ " WHERE CodeField_ID=" + Workfield.CurrentCodeField.CodeFieldID.ToString();
						using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
						{
							Workfield.SQLConnection.Open();
							com.ExecuteNonQuery();
							Workfield.SQLConnection.Close();
						}
					}
					catch (Exception error)
					{
						Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
						Workfield.WorkWindow.IsEnabled = false;
						Workfield.SQLErrorWindow.Show();
						Workfield.isFatalError = true;
						return;
					}

					UpdateRectangle();
					CodeBorder.Visibility = Visibility.Visible;
				}
			}
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Создание, редактирование и удаление настроек загрузки CSV файлов-----------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие добавления настройки загрузки CSV файла в DataGrid
		*/
        #region
        private void CSVList_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{

		}
        #endregion

        /*
			Событие подтверждения редактирования настройки загрузки CSV файла в DataGrid
		*/
        #region
        private void CSVList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{

		}
        #endregion

        /*
			Событие удаления настройки загрузки CSV файла в DataGrid
		*/
        #region
        private void CSVList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{

		}
        #endregion

        /*
			Событие загрузки CSV файла
		*/
        #region
        private void OpenCSVBlankButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			BlankCSV currentBlank = CSVList.SelectedItem as BlankCSV;

			openFileDialog.Filter = "Файл с данными отсканированных бланков|*csv";

			if (currentBlank != null && openFileDialog.ShowDialog() == true)
			{
				currentBlank.CSVPath = openFileDialog.FileName;
			}
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Редактирование и удаление записей о тестируемых----------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие подтверждения редактирования записи о тестируемых в DataGrid
		*/
        #region
        private void CheckResultsList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{

		}
        #endregion

        /*
			Событие удаления записи о тестируемых в DataGrid
		*/
        #region
        private void CheckResultsList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{

		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Обработка ошибок предметов-------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие наведения мыши на иконку ошибки
		*/
        #region
        private void Alert_MouseEnter(object sender, MouseEventArgs e)
		{
			//Image AlertImage = sender as Image;
			//Border AlertBorder = AlertImage.Parent as Border;
			Border AlertBorder = sender as Border;
			Grid AlertGrid = AlertBorder.Parent as Grid;
			ContentPresenter currentListBoxItem = AlertGrid.TemplatedParent as ContentPresenter;
			Alert currentAlert = currentListBoxItem.Content as Alert;
			Popup currentPopup = currentListBoxItem.ContentTemplate.FindName("AlertPopup", currentListBoxItem) as Popup;
			currentPopup.IsOpen = true;
		}
        #endregion

        /*
			Событие выхода курсора мыши из окна ошибки
		*/
        #region
        private void AlertPopup_MouseLeave(object sender, MouseEventArgs e)
		{
			Popup currentPopup = sender as Popup;
			Grid AlertGrid = currentPopup.Parent as Grid;
			ContentPresenter currentListBoxItem = AlertGrid.TemplatedParent as ContentPresenter;
			Alert currentAlert = currentListBoxItem.Content as Alert;
			Border currentIcon = currentListBoxItem.ContentTemplate.FindName("AlertIcon", currentListBoxItem) as Border;
			if (!currentIcon.IsMouseOver)
				currentPopup.IsOpen = false;
		}
        #endregion

        /*
			Событие выхода курсора мыши из иконки ошибки
		*/
        #region
        private void Alert_MouseLeave(object sender, MouseEventArgs e)
        {
			//Image AlertImage = sender as Image;
			//Border AlertBorder = AlertImage.Parent as Border;
			Border AlertBorder = sender as Border;
			Grid AlertGrid = AlertBorder.Parent as Grid;
			ContentPresenter currentListBoxItem = AlertGrid.TemplatedParent as ContentPresenter;
			Alert currentAlert = currentListBoxItem.Content as Alert;
			Popup currentPopup = currentListBoxItem.ContentTemplate.FindName("AlertPopup", currentListBoxItem) as Popup;
			if (!currentPopup.IsMouseOver)
				currentPopup.IsOpen = false;
		}
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Обработка справки----------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        /*
			Событие наведения мыши на иконку советов
		*/
        #region
        private void Info_MouseEnter(object sender, MouseEventArgs e)
		{
			InfoPopup.IsOpen = true;
		}
        #endregion

        /*
			Событие выхода курсора мыши из окна советов
		*/
        #region
        private void InfoPopup_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!InfoIcon.IsMouseOver)
				InfoPopup.IsOpen = false;
		}
        #endregion

        /*
			Событие выхода курсора мыши из иконки советов
		*/
        #region
        private void Info_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!InfoPopup.IsMouseOver)
				InfoPopup.IsOpen = false;
		}
        #endregion

        /*
			Кнопка очистки очереди на печать
		*/
        #region
        private void ButtonC_ClearQueueButton(object sender, RoutedEventArgs e)
        {
			Workfield.CurrentSubject.PrintBlanks = new ObservableCollection<PrintBlank>() { };
			PrintBlankList.ItemsSource = Workfield.CurrentSubject.PrintBlanks;
			PrintBlankList.Items.Refresh();
        }
        #endregion

        /*
			Кнопка загрузки результатов
		*/
        #region
        private void LoadResultsButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        /*
			Кнопка отправки отчета на печать
		*/
        #region
        private void PrintReportButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        /*
			Кнопка подготовки кодов в бланках
		*/
        #region
        private void PrepareCodesButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        /*
			Кнопка отправки бланков на печать
		*/
        #region
        private void PrintBlanksButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------
    }
    //========================================================================================================================================
}
