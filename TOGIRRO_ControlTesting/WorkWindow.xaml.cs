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
			//Workfield.PreLoad();
			Workfield.Init();
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
			AlertList.ItemsSource = Workfield.CurrentAlerts;

			EditQuestionsMode.IsSelected = true;
			EditQuestionsMode_Button.IsChecked = true;
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
			Workfield.CurrentAlerts = null;
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
					Type = Workfield.SubjectTypes[CreateSubject_Type.SelectedIndex+1],
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

				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = newSubject.SubjectCode.ToString() }, new List<object>() { (int)1 , "Код предмета", newSubject, true });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = newSubject.EventCode.ToString() }, new List<object>() { (int)1 , "Код мероприятия", newSubject, true });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = newSubject.Description }, new List<object>() { (int)1, "Описание", newSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = newSubject.MinScore.ToString() }, new List<object>() { (int)1, "Минимальный балл", newSubject, true });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = newSubject.ProjectFolderPath }, new List<object>() { (int)1, "Путь к папке с файлами КМ", newSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = newSubject.RegistrationForm }, new List<object>() { (int)1, "Имя бланка регистрации", newSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = newSubject.AnswersForm1 }, new List<object>() { (int)1, "Имя бланка ответов №1", newSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = newSubject.AnswersForm2 }, new List<object>() { (int)1, "Имя бланка ответов №2", newSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = newSubject.LogFile }, new List<object>() { (int)1, "Имя файла протоколов", newSubject, false });

				Workfield.Subjects.Add(newSubject);
				CreateSubjectMenu_Button.IsChecked = false;
				EditSubjectMenu_Button.IsChecked = false;
				Subjects_ListTab.IsSelected = true;
				UpdateSubjectList();
			}
		}

		/*
			Отмена создания предмета
		*/
		private void ButtonC_CreateSubject_Cancel(object sender, RoutedEventArgs e)
		{
			CreateSubjectMenu_Button.IsChecked = false;
			EditSubjectMenu_Button.IsChecked = false;
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
				SelectedSubject.Type = Workfield.SubjectTypes[EditSubject_Type.SelectedIndex+1];
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

				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = SelectedSubject.SubjectCode.ToString() }, new List<object>() { (int)1, "Код предмета", SelectedSubject, true });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = SelectedSubject.EventCode.ToString() }, new List<object>() { (int)1, "Код мероприятия", SelectedSubject, true });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = SelectedSubject.Description }, new List<object>() { (int)1, "Описание", SelectedSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = SelectedSubject.MinScore.ToString() }, new List<object>() { (int)1, "Минимальный балл", SelectedSubject, true });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = SelectedSubject.ProjectFolderPath }, new List<object>() { (int)1, "Путь к папке с файлами КМ", SelectedSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = SelectedSubject.RegistrationForm }, new List<object>() { (int)1, "Имя бланка регистрации", SelectedSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = SelectedSubject.AnswersForm1 }, new List<object>() { (int)1, "Имя бланка ответов №1", SelectedSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = SelectedSubject.AnswersForm2 }, new List<object>() { (int)1, "Имя бланка ответов №2", SelectedSubject, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, new TextBox() { Text = SelectedSubject.LogFile }, new List<object>() { (int)1, "Имя файла протоколов", SelectedSubject, false });

				Subjects_ListTab.IsSelected = true;
				CurrentSubjectName1.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType1.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName2.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType2.Content = Workfield.CurrentSubject.Type;
				CurrentSubjectName3.Content = Workfield.CurrentSubject.Name;
				CurrentSubjectType3.Content = Workfield.CurrentSubject.Type;
				CreateSubjectMenu_Button.IsChecked = false;
				EditSubjectMenu_Button.IsChecked = false;
				UpdateSubjectList();
			}
		}

		/*
			Отмена редактирования предмета
		*/
		private void ButtonC_EditSubject_Cancel(object sender, RoutedEventArgs e)
		{
			CreateSubjectMenu_Button.IsChecked = false;
			EditSubjectMenu_Button.IsChecked = false;
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

			foreach (AnswerCharacteristic currentQuestion in Workfield.CurrentSubject.Questions)
			{
				Question newQuestion = new Question(currentQuestion)
				{
					InVariant = currentQuestion.Number,
					QuestionType = currentQuestion.QuestionType,
					MaxScore = currentQuestion.MaxScore
				};
				Workfield.CurrentVariant.QuestionsPerVar.Add(newQuestion);

				try
				{
					string command =
						"INSERT INTO Question (Variant_FK, QuestionType_FK, TaskNumber, AnswerCharacteristic_FK, MaxScore) VALUES("
						+ Workfield.CurrentVariant.VariantID.ToString() + ", "
						+ Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, newQuestion.QuestionType).ToString() + ", "
						+ newQuestion.InVariant.ToString() + ", " + newQuestion.QuestionTemplate.AnswerCharacteristicID.ToString()
						+ ", " + newQuestion.QuestionTemplate.MaxScore.ToString() + ")";
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
			}
			CurrentVariantTextbox.Text = Workfield.CurrentVariant.Name;
			AnswerList.ItemsSource = Workfield.CurrentVariant.QuestionsPerVar;
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
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
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
			catch (Exception error)
			{
				Workfield.SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + error.ToString();
				Workfield.WorkWindow.IsEnabled = false;
				Workfield.SQLErrorWindow.Show();
				Workfield.isFatalError = true;
				return;
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
			AnswerList.ItemsSource = Workfield.CurrentVariant.QuestionsPerVar;
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

			if (Workfield.CurrentSubject != null)
			{
				Workfield.CurrentSubject.Variants = null;
				Workfield.CurrentSubject.Questions = null;
				Workfield.CurrentSubject.Alerts = null;
			}
			Workfield.CurrentVariant = null;
			Workfield.CurrentQuestion = null;
			Workfield.CurrentAnswerCharacteristic = null;
			Workfield.CurrentAlerts = null;

            Workfield.CurrentSubject = currentSubject;

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
							currentSubject.Questions.Add(new AnswerCharacteristic()
                            {
								AnswerCharacteristicID = reader.GetInt32(0),
								Number = reader.GetInt16(2),
								Criterion = reader.GetString(3),
								ValidChars = reader.GetString(4),
								QuestionType = Workfield.QuestionTypes[reader.GetInt32(5)],
								CheckType = Workfield.CheckTypes[reader.GetInt32(6)],
								MaxScore = reader.GetInt16(7)
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
									MaxScore = (short)reader.GetInt32(5)
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
									currentQuestion.Answers.Add(new Answer()
									{
										AnswerID = reader.GetInt32(0),
										RightAnswer = reader.GetString(2),
										Score = reader.GetInt16(3)
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
				}
			}

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
				AnswerList.ItemsSource = Workfield.CurrentVariant.QuestionsPerVar;
			}
			CurrentVariantCBox.SelectedIndex = -1;
			CurrentVariantCBox.SelectedIndex = 0;
			CurrentVariantCBox.Items.Refresh();
			AnswerList.Items.Refresh();
			EditSubjectMenu_Button.IsEnabled = true;
			DeleteSubject_Button.IsEnabled = true;
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
				//if (curSubject.Name.ToLower().Contains(searchName) && ((SubjectTypeEnum)SearchTypes.SelectedItem == SubjectTypeEnum.UNDEFINED || ((SubjectTypeEnum)SearchTypes.SelectedItem != SubjectTypeEnum.UNDEFINED) && curSubject.Type == (SubjectTypeEnum)SearchTypes.SelectedItem))
				if (curSubject.Name.ToLower().Contains(searchName) && (((KeyValuePair<int, string>)SearchTypes.SelectedItem).Value == Workfield.SubjectTypes[1] || (((KeyValuePair<int, string>)SearchTypes.SelectedItem).Value != Workfield.SubjectTypes[1]) && curSubject.Type == ((KeyValuePair<int, string>)SearchTypes.SelectedItem).Value))
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

			try
			{
				string command =
					"INSERT INTO AnswerCharacteristic (Subject_FK, QuestionType_FK, CheckType_FK) VALUES("+Workfield.CurrentSubject.SubjectID.ToString()+", 1, 1)";
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
						"INSERT INTO Question (Variant_FK, QuestionType_FK, TaskNumber, AnswerCharacteristic_FK) VALUES(" + currentVariant.VariantID.ToString()
						+ ", " + Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, newQuestion.QuestionType).ToString() + ", " + newQuestion.InVariant.ToString() + ", "
						+ newQuestion.QuestionTemplate.AnswerCharacteristicID.ToString() + ")";
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

		/*
			Событие удаления шаблона вопроса в DataGrid
		*/
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

				foreach (Variant currentVariant in Workfield.CurrentSubject.Variants)
				{
					foreach (Question questionPost in currentVariant.QuestionsPerVar)
					{
						if (questionPost.QuestionTemplate.Equals(currentQuestion))
						{
							questionPost.InVariant = currentQuestion.Number;
							questionPost.QuestionType = currentQuestion.QuestionType;
							questionPost.MaxScore = currentQuestion.MaxScore;

							try
							{
								string command =
									"UPDATE Question SET TaskNumber='" + currentQuestion.Number.ToString() + "', QuestionType_FK="
									+ Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, currentQuestion.QuestionType).ToString()
									+ ", MaxScore=" + currentQuestion.MaxScore.ToString()
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
			}
		}
		#endregion
		//------------------------------------------------------------------------------------------------------------------------------------



		//------------------------------------------------------------------------------------------------------------------------------------
		//---Создание, редактирование и удаление эталонных ответов----------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------------------------------------------
		#region
		/*
			Событие добавления нового эталонного ответа в DataGrid 
		*/
		private void AnswersPerQuestionList_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			e.NewItem = new Answer();
			Answer newAnswer = e.NewItem as Answer;

			Question currentQuestion = AnswerList.SelectedItem as Question;

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

		/*
			Событие удаления эталонного ответа в DataGrid 
		*/
		private void AnswersPerQuestionList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			DataGrid grid = (DataGrid)sender;
			if (e.Command == DataGrid.DeleteCommand)
			{
				Answer currentAnswer = grid.SelectedItem as Answer;

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
			}
		}

		/*
			Событие подтверждения редактирования эталонного ответа в DataGrid
		*/
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

				
				DataGrid answerGrid = AnswerList;
				
				DataGrid currentDataGrid = e.Row.VisualParent as DataGrid;
				DataGridRow parentDataGridRow = VisualTreeHelper.GetParent(currentDataGrid) as DataGridRow;


				AlertManager.CheckAlerts(AlertType.FieldNotFilled, e.Row, new List<object>() { (int)3, currentDataGrid.Columns[0], parentDataGridRow, currentDataGrid, false });
				AlertManager.CheckAlerts(AlertType.FieldNotFilled, e.Row, new List<object>() { (int)3, currentDataGrid.Columns[1], parentDataGridRow, currentDataGrid, true });
			}
		}
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
			}
		}
		#endregion
		//------------------------------------------------------------------------------------------------------------------------------------



		//------------------------------------------------------------------------------------------------------------------------------------
		//---Создание, редактирование и удаление разбаловки ошибок----------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------------------------------------------
		#region
		/*
			Событие добавления нового юнита разбаловки ошибок в DataGrid
		*/
		private void ErrorsPerQuestionList_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			e.NewItem = new ErrorScaleUnit();
			ErrorScaleUnit newErrorScaleUnit = e.NewItem as ErrorScaleUnit;

			AnswerCharacteristic currentAnswerCharacteristic = QuestionList.SelectedItem as AnswerCharacteristic;

			try
			{
				string command =
					"INSERT INTO ErrorScaleUnit (AnswerCharacteristic_FK, ErrorCount, Score) VALUES(" + currentAnswerCharacteristic.AnswerCharacteristicID.ToString() + ", '"
					+ newErrorScaleUnit.ErrorCount + "', " + newErrorScaleUnit.Score.ToString() + ")";
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

		/*
			Событие удаления юнита разбаловки ошибок в DataGrid
		*/
		private void ErrorsPerQuestionList_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			DataGrid grid = (DataGrid)sender;
			if (e.Command == DataGrid.DeleteCommand)
			{
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
			}
		}

		/*
			Событие подтверждения редактирования юнита разбаловки ошибок в DataGrid
		*/
		private void ErrorsPerQuestionList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				ErrorScaleUnit currentErrorScaleUnit = e.Row.Item as ErrorScaleUnit;
				AnswerCharacteristic currentAnswerCharacteristic = QuestionList.SelectedItem as AnswerCharacteristic;

				try
				{
					string command =
						"UPDATE ErrorScaleUnit SET ErrorCount=" + currentErrorScaleUnit.ErrorCount + ", Score="
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

        private void AnswerList_ChangeTableVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
			if (AnswerList.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
			{
				AnswerList.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
			}
			else
			{
				AnswerList.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
			}
        }

		private void QuestionList_ChangeTableVisibilityButton_Click(object sender, RoutedEventArgs e)
		{
			if (QuestionList.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
			{
				QuestionList.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
			}
			else
			{
				QuestionList.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
			}
		}
        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------
    }
	//========================================================================================================================================
}
