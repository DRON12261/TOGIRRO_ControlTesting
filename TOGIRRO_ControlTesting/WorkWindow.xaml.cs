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
		public WorkWindow()
		{
			InitializeComponent();
			Workfield.WorkWindow = this;
			Workfield.PreLoad();
			Workfield.Init();

			SearchTypes.ItemsSource = Workfield.SubjectTypeEnumValues;
			CreateSubject_Type.ItemsSource = Workfield.QuestionTypeEnumValues;
			EditSubject_Type.ItemsSource = Workfield.CheckTypeEnumValues;
			SearchTypes.SelectedIndex = 0;
			CreateSubject_Type.SelectedIndex = 0;
			EditSubject_Type.SelectedIndex = 0;

			Workfield.CurrentSubject = null;
			MainField.IsEnabled = false;
			SubjectsList.ItemsSource = Workfield.ActualSubjects;
			AlertList.ItemsSource = Workfield.CurrentAlerts;
		}

		/*
			Ограничение ввода для числового TextBox
		*/
		private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		/*
			Событие закрытия окна WorkWindow
		*/
		private void WorkWindow_Closing(object sender, CancelEventArgs e)
		{
			Application.Current.Shutdown();
		}
		#endregion
		//------------------------------------------------------------------------------------------------------------------------------------



		//------------------------------------------------------------------------------------------------------------------------------------
		//---Создание, редактирование и удаление предметов------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------------------------------------------
		#region
		/*
			Кнопка включения окна создания предмета
		*/
		private void ButtonC_CreateSubject(object sender, RoutedEventArgs e)
		{
			if (Subjects_CreateTab.IsSelected) { Subjects_ListTab.IsSelected = true; return; }
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

		/*
			Кнопка включения окна редактирования предмета
		*/
		private void ButtonC_EditSubject(object sender, RoutedEventArgs e)
		{
			if (Subjects_EditTab.IsSelected) { Subjects_ListTab.IsSelected = true; return; }
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

		/*
			Удаление предмета
		*/
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
			catch (SqlException error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
			}

			Workfield.Subjects.Remove(Workfield.CurrentSubject);
			Workfield.CurrentSubject = null;
			MainField.IsEnabled = false;
			AlertList.ItemsSource = null;
			Workfield.CurrentAlerts = null;
			AlertList.Items.Refresh();
			QuestionList.ItemsSource = null;
			QuestionList.Items.Refresh();
			AnswerList.ItemsSource = null;
			AnswerList.Items.Refresh();
			ScaleList.ItemsSource = null;
			ScaleList.Items.Refresh();
			CurrentVariantCBox.ItemsSource = null;
			CurrentVariantCBox.SelectedIndex = -1;
			CurrentVariantCBox.Items.Refresh();
			CurrentSubjectName1.Content = "";
			CurrentSubjectType1.Content = "";
			CurrentSubjectName2.Content = "";
			CurrentSubjectType2.Content = "";
			CurrentSubjectName3.Content = "";
			CurrentSubjectType3.Content = "";
			UpdateSubjectList();
		}

		/*
			Создание предмета
		*/
		private void ButtonC_CreateSubject_Create(object sender, RoutedEventArgs e)
		{
			if (CreateSubject_Name.Text.Trim(' ') != "" && CreateSubject_Type.SelectedIndex != 0)
			{
                short subjectCode;
                if (CreateSubject_SubjectCode.Text.Trim(' ') == "") subjectCode = 0;
                else subjectCode = short.Parse(CreateSubject_SubjectCode.Text.Trim(' '));
                short eventCode;
                if (CreateSubject_EventCode.Text.Trim(' ') == "") eventCode = 0;
                else eventCode = short.Parse(CreateSubject_EventCode.Text.Trim(' '));
                short minScore;
                if (CreateSubject_MinScore.Text.Trim(' ') == "") minScore = 0;
                else minScore = short.Parse(CreateSubject_MinScore.Text.Trim(' '));

                Subject newSubject = new Subject()
				{
					SubjectCode = subjectCode,
					EventCode = eventCode,
					Name = CreateSubject_Name.Text.Trim(' '),
					Description = CreateSubject_Description.Text.Trim(' '),
					Type = (SubjectTypeEnum)CreateSubject_Type.SelectedIndex,
					MinScore = minScore,
					ProjectFolderPath = CreateSubject_ProjectFolderPath.Text.Trim(' '),
					RegistrationForm = CreateSubject_RegistrationForm.Text.Trim(' '),
					AnswersForm1 = CreateSubject_AnswersForm1.Text.Trim(' '),
					AnswersForm2 = CreateSubject_AnswersForm2.Text.Trim(' '),
					LogFile = CreateSubject_LogFile.Text.Trim(' '),
					IsMark = CreateSubject_MarkSystem.IsChecked.Value
				};
				

				try
				{
					string command = 
						"INSERT INTO Subject (SubjectCode, EventCode, MinScore, SubjectName, Description, ControlEvents_FK, " +
                        "ProjectFolderPath, RegistrationFormName, AnswersForm1Name, AnswersForm2Name, LogFileName, isMark) VALUES("
						+newSubject.SubjectCode.ToString()+", "+newSubject.EventCode.ToString()+", "+newSubject.MinScore.ToString()
						+", '"+newSubject.Name+"', '"+newSubject.Description+"', "+((int)newSubject.Type).ToString()+", '"+
						newSubject.ProjectFolderPath+"', '"+newSubject.RegistrationForm+"', '"+newSubject.AnswersForm1+"', '"+
						newSubject.AnswersForm2+"', '"+newSubject.LogFile+"', "+Convert.ToInt32(newSubject.IsMark).ToString()+")";
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (SqlException error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
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
				catch (SqlException error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
				}

				Workfield.Subjects.Add(newSubject);
				Subjects_ListTab.IsSelected = true;
				UpdateSubjectList();
			}
		}

		/*
			Отмена создания предмета
		*/
		private void ButtonC_CreateSubject_Cancel(object sender, RoutedEventArgs e)
		{
			Subjects_ListTab.IsSelected = true;
		}

		/*
			Редактирование предмета
		*/
		private void ButtonC_EditSubject_Edit(object sender, RoutedEventArgs e)
		{
			if (EditSubject_Name.Text.Trim(' ') != "" && EditSubject_Type.SelectedIndex != 0)
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

				try
				{
					string command =
						"UPDATE Subject SET SubjectCode="+SelectedSubject.SubjectCode.ToString()+", EventCode="
						+SelectedSubject.EventCode.ToString()+", SubjectName='"+SelectedSubject.Name+"', Description='"
						+SelectedSubject.Description+"', ControlEvents_FK="+((int)SelectedSubject.Type).ToString()+", " +
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
				catch (SqlException error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
				}

				Subjects_ListTab.IsSelected = true;
				CurrentSubjectName1.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType1.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName2.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType2.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName3.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType3.Content = Workfield.CurrentSubject.Type;
				UpdateSubjectList();
			}
		}

		/*
			Отмена редактирования предмета
		*/
		private void ButtonC_EditSubject_Cancel(object sender, RoutedEventArgs e)
		{
			Subjects_ListTab.IsSelected = true;
		}
		#endregion
		//------------------------------------------------------------------------------------------------------------------------------------



		//------------------------------------------------------------------------------------------------------------------------------------
		//---Кнопки переключения режимов настройки мероприятия--------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------------------------------------------
		#region
		/*
			Кнопка перехода в режим редактирования шаблонов вопросов
		*/
		private void ButtonC_EditQuestionsMode(object sender, RoutedEventArgs e)
		{
			EditQuestionsMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = true;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = false;
		}

		/*
			Кнопка перехода в режим редактирования системы шкалирования
		*/
		private void ButtonC_EditScaleMode(object sender, RoutedEventArgs e)
		{
			EditScaleMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = true;
			EditAnswersMode_Button.IsChecked = false;
		}

		/*
			Кнопка перехода в режим редактирования ответов
		*/
		private void ButtonC_EditAnswersMode(object sender, RoutedEventArgs e)
		{
			EditAnswersMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = false;
			EditScaleMode_Button.IsChecked = false;
			EditAnswersMode_Button.IsChecked = true;
		}
        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Создание, редактирование и удаление вариантов------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region
        /*
			Кнопка перехода в режим редактирования названия варианта
		*/
        private void ButtonC_EditVariantNameMode(object sender, RoutedEventArgs e)
		{
			CurrentVariantTextbox.Text = Workfield.CurrentVariant.Name;
			EditVariantMode.IsSelected = true;
		}

		/*
			Кнопка создания варианта
		*/
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
			catch (SqlException error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
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
			catch (SqlException error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
			}

			foreach (AnswerCharacteristic currentQuestion in Workfield.CurrentSubject.Questions)
			{
				Question newQuestion = new Question(currentQuestion)
				{
					InVariant = currentQuestion.Number,
					QuestionType = currentQuestion.QuestionType,
					MaxScore = currentQuestion.MaxScore
				};
				Workfield.CurrentVariant.Answers.Add(newQuestion);

				try
				{
					string command =
						"INSERT INTO Question (Variant_FK, QuestionType_FK, TaskNumber, AnswerCharacteristic_FK) VALUES(" + Workfield.CurrentVariant.VariantID.ToString()
						+ ", " + ((int)newQuestion.QuestionType).ToString() + ", " + newQuestion.InVariant.ToString() + ", "
						+ newQuestion.QuestionTemplate.AnswerCharacteristicID.ToString() + ")";
					using (SqlCommand com = new SqlCommand(command, Workfield.SQLConnection))
					{
						Workfield.SQLConnection.Open();
						com.ExecuteNonQuery();
						Workfield.SQLConnection.Close();
					}
				}
				catch (SqlException error)
				{
					Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
					Workfield.WorkWindow.IsEnabled = false;
					Workfield.SQLErrorWindow.Show();
					Workfield.isFatalError = true;
				}
			}
			CurrentVariantTextbox.Text = Workfield.CurrentVariant.Name;
			AnswerList.ItemsSource = Workfield.CurrentVariant.Answers;
			AnswerList.Items.Refresh();
		}

		/*
			Кнопка удаления варианта
		*/
		private void ButtonC_DeleteVariant(object sender, RoutedEventArgs e)
		{
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
			catch (SqlException error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
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
				AnswerList.ItemsSource = Workfield.CurrentVariant.Answers;
			}
			AnswerList.Items.Refresh();
		}

		/*
			Кнопка редактирования названия варианта
		*/
		private void ButtonC_EditVariantSuccess(object sender, RoutedEventArgs e)
		{
			Workfield.CurrentVariant.Name = CurrentVariantTextbox.Text.Trim(' ');
			if (Workfield.CurrentVariant.Name.Length > 4) Workfield.CurrentVariant.Name = Workfield.CurrentVariant.Name.Substring(0, 4);

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
			catch (SqlException error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text = error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
			}

			CurrentVariantTextbox.Text = "";
			int currentIndex = CurrentVariantCBox.SelectedIndex;
			CurrentVariantCBox.SelectedIndex = -1;
			CurrentVariantCBox.SelectedIndex = currentIndex;
			CurrentVariantCBox.Items.Refresh();
			SelectVariantMode.IsSelected = true;
		}

		/*
			Кнопка отмены редактирования названия варианта
		*/
		private void ButtonC_EditVariantCancel(object sender, RoutedEventArgs e)
		{
			CurrentVariantTextbox.Text = "";
			SelectVariantMode.IsSelected = true;
		}

		/*
			Событие выбора варианта в ComboBox
		*/
		private void CurrentVariantCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CurrentVariantCBox.SelectedIndex < 0) return;
			if (Workfield.CurrentSubject.Variants.Count <= 0) return;
			Workfield.CurrentVariant = Workfield.CurrentSubject.Variants[CurrentVariantCBox.SelectedIndex];
			AnswerList.ItemsSource = Workfield.CurrentVariant.Answers;
			AnswerList.Items.Refresh();
		}
        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Поиск и выбор предметов----------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region
        /*
			Событие выбора предмета в DataGrid
		*/
        private void SubjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            if (!(SubjectsList.CurrentItem is Subject currentSubject)) return;
            Workfield.CurrentSubject = currentSubject;
			Workfield.CurrentAlerts = currentSubject.Alerts;
			AlertList.ItemsSource = Workfield.CurrentAlerts;
			AlertList.Items.Refresh();
			CurrentSubjectName1.Content = currentSubject.Name;
			CurrentSubjectType1.Content = currentSubject.Type;
			CurrentSubjectName2.Content = currentSubject.Name;
			CurrentSubjectType2.Content = currentSubject.Type;
			CurrentSubjectName3.Content = currentSubject.Name;
			CurrentSubjectType3.Content = currentSubject.Type;
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
				AnswerList.ItemsSource = Workfield.CurrentVariant.Answers;
			}
			CurrentVariantCBox.SelectedIndex = -1;
			CurrentVariantCBox.SelectedIndex = 0;
			CurrentVariantCBox.Items.Refresh();
			AnswerList.Items.Refresh();
			MainField.IsEnabled = true;
		}

		/*
			Обновление списка предметов во время поиска
		*/
		public void UpdateSubjectList()
		{
			string searchName = SearchBox.Text.ToLower().Trim(' ');

			Workfield.ActualSubjects = new ObservableCollection<Subject>() { };

			foreach (Subject curSubject in Workfield.Subjects)
			{
				if (curSubject.Name.ToLower().Contains(searchName) && ((SubjectTypeEnum)SearchTypes.SelectedItem == SubjectTypeEnum.UNDEFINED || ((SubjectTypeEnum)SearchTypes.SelectedItem != SubjectTypeEnum.UNDEFINED) && curSubject.Type == (SubjectTypeEnum)SearchTypes.SelectedItem))
				{
					Workfield.ActualSubjects.Add(curSubject);
				}
			}

			SubjectsList.ItemsSource = Workfield.ActualSubjects;
			SubjectsList.Items.Refresh();
		}

		/*
			События изменения текста в TextBox поиска предметов
		*/
		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateSubjectList();
		}

		/*
			Событие выбора типа предмета при поиске в ComboBox
		*/
		private void SearchTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateSubjectList();
		}
        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Создание, редактирование и удаление шаблонов вопросов----------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region
        /*
			Событие добавления нового шаблона вопросов в DataGrid
		*/
        private void QuestionList_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			e.NewItem = new AnswerCharacteristic();
			AnswerCharacteristic currentQuestion = e.NewItem as AnswerCharacteristic;

			foreach (Variant currentVariant in Workfield.CurrentSubject.Variants)
			{
				currentVariant.Answers.Add(new Question(currentQuestion)
				{
					Answers = new List<Answer>() { }
				});
			}
			AnswerList.Items.Refresh();
		}

		/*
			Событие удаления шаблона вопроса в DataGrid
		*/
		private void QuestionList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			DataGrid grid = (DataGrid)sender;
			if (e.Command == DataGrid.DeleteCommand)
			{
				AnswerCharacteristic currentQuestion = grid.SelectedItem as AnswerCharacteristic;

				foreach (Variant currentVariant in Workfield.CurrentSubject.Variants)
				{
					Question questionToDelete = null;

					foreach (Question questionPost in currentVariant.Answers)
					{
						if (questionPost.QuestionTemplate.Equals(currentQuestion))
						{
							questionToDelete = questionPost;
						}
					}

					if (questionToDelete != null) currentVariant.Answers.Remove(questionToDelete);
				}
				AnswerList.Items.Refresh();

				Workfield.CurrentSubject.Questions.Remove(currentQuestion);
				QuestionList.Items.Refresh();

				short maxScore = 0;
				foreach (AnswerCharacteristic curQuestion in Workfield.CurrentSubject.Questions)
				{
					maxScore += curQuestion.MaxScore;
				}

				short preCount = (short)Workfield.CurrentSubject.ScaleSystem.Count;

				if (preCount > maxScore)
				{
					for (short i = preCount; i > maxScore; i--)
					{
						Workfield.CurrentSubject.ScaleSystem.RemoveAt(i - 1);
					}
					ScaleList.Items.Refresh();
				}
				else if (preCount < maxScore)
				{
					for (short i = 1; i <= maxScore - preCount; i++)
					{
						Workfield.CurrentSubject.ScaleSystem.Add(new ScaleUnit() { FirstScore = (short)(preCount + i) });
					}
					ScaleList.Items.Refresh();
				}

				e.Handled = true;
			}
		}

		/*
			Событие подтверждения редактирования шаблона вопроса в DataGrid
		*/
		private void QuestionList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				AnswerCharacteristic currentQuestion = e.Row.Item as AnswerCharacteristic;

				foreach (Variant currentVariant in Workfield.CurrentSubject.Variants)
				{
					foreach (Question questionPost in currentVariant.Answers)
					{
						if (questionPost.QuestionTemplate.Equals(currentQuestion))
						{
							questionPost.InVariant = currentQuestion.Number;
							questionPost.QuestionType = currentQuestion.QuestionType;
							questionPost.MaxScore = currentQuestion.MaxScore;
						}
					}
				}
				AnswerList.Items.Refresh();

				short maxScore = 0;
				foreach (AnswerCharacteristic curQuestion in Workfield.CurrentSubject.Questions)
				{
					maxScore += curQuestion.MaxScore;
				}

				short preCount = (short)Workfield.CurrentSubject.ScaleSystem.Count;

				if (preCount > maxScore)
				{
					for (short i = preCount; i > maxScore; i--)
					{
						Workfield.CurrentSubject.ScaleSystem.RemoveAt(i - 1);
					}
					ScaleList.Items.Refresh();
				}
				else if (preCount < maxScore)
				{
					for (short i = 1; i <= maxScore - preCount; i++)
					{
						Workfield.CurrentSubject.ScaleSystem.Add(new ScaleUnit() { FirstScore = (short)(preCount + i) });
					}
					ScaleList.Items.Refresh();
				}
			}
		}
        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Создание, редактирование и удаление эталонных ответов----------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Редактирование системы шкалирования----------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region
        /*
			Событие подтверждения редактирования в DataGrid системы шкалирования
		*/
        private void ScaleList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				ScaleUnit currentScaleUnit = e.Row.Item as ScaleUnit;

				if (currentScaleUnit.Mark > 5) currentScaleUnit.Mark = 5;
				if (currentScaleUnit.Mark <= 0) currentScaleUnit.Mark = 1;
				if (currentScaleUnit.SecondScore <= 0) currentScaleUnit.SecondScore = 1;
			}
		}
        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------



        //------------------------------------------------------------------------------------------------------------------------------------
        //---Обработка ошибок предметов-------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        #region
        /*
			Событие наведения мыши на иконку ошибки
		*/
        private void Alert_MouseEnter(object sender, MouseEventArgs e)
		{
			Image AlertImage = sender as Image;
			Border AlertBorder = AlertImage.Parent as Border;
			Grid AlertGrid = AlertBorder.Parent as Grid;
			ContentPresenter currentListBoxItem = AlertGrid.TemplatedParent as ContentPresenter;
			Alert currentAlert = currentListBoxItem.Content as Alert;
			Popup currentPopup = currentListBoxItem.ContentTemplate.FindName("AlertPopup", currentListBoxItem) as Popup;
			currentPopup.IsOpen = true;
		}

		/*
			Событие выхода курсора мыши из окна ошибки
		*/
		private void AlertPopup_MouseLeave(object sender, MouseEventArgs e)
		{
			Popup currentPopup = sender as Popup;
			currentPopup.IsOpen = false;
		}
        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------
    }
	//========================================================================================================================================
}
