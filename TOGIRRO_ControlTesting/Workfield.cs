﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

/*
------Запилить вспомогательный класс для отображения в таблице ответов
------Реализовать шкалирование (Класс)
------Настроить таблицу шкалирования
------Настроить таблицу ответов
------Добавление изменение и удаление вариантов
------Добавление и удаление вопросов
------Привязка данных текущего предмета
------Авто отслеживание и сопоставление всех трех таблиц
------Доп инфа о текущем предмете в ListView
------Автонумерация таблицы
------DataGridNumericColumn
------Отслеживание ввода в таблице шкалирования
------Поиск в предметах
Макс балл и алерты в трее
------Привязка к БД
------Разбаловка шаблона вопроса
Довести до ума интерфейс
------кнопки переключения видимости подтаблиц
обязательное уникальное название варианта
переход к месту ошибки

Ошибки:
Непоследовательность итоговых баллов
Непоследовательность оценок
Отсутствие эталонного ответа
Отсутствие балла за ответ
Отсутствие максимального балла за шаблон вопроса
Отсутствие разбаловки ошибок
Недостаточное кол-во баллов за ответы на вопрос
Избыточное кол-во баллов за ответы на вопрос
Незаполнено какое-либо поле

Печать
Нумерация

тит
рег
отв1
отв2
доп бланк
ким 13знач(11кл) и 7знач(9кл ГВЭ)

выбрать какие бланки будут
загрузка изображений бланка (превый бланк с рег данными)
Открыть изображение и указать какие номера где печатать

Формирование номеров
1 - вид бланка (№1 - 1. №2 - 2. рег - 3. доп - 4. номер ким - 5(13зн). протокол - 6(7зн). номер ким - 7(7зн 9кл). рег устн - 8)
у протокола:
2и3 - код мероприятия
4и5и6и7 - порядковый номер
у 13зн:
2 - тип мероприятия
3и4 - код мероприятия
5и6и7и8 - номер варианта
посл5 - поярдковый номер
у доп:
2 - тип мероприятия
3и4и5и6 - год
до13 - порядковый номер
*/

namespace TOGIRRO_ControlTesting
{
    //========================================================================================================================================
    //===Вспомогательный класс-посредник Workfield============================================================================================
    //========================================================================================================================================
    #region
    static class Workfield
	{
		//Окна
		static public WorkWindow WorkWindow = null;
		static public SQLErrorWindow SQLErrorWindow = new SQLErrorWindow();

		//Списки предметов
		static public ObservableCollection<Subject> Subjects = new ObservableCollection<Subject> { };
		static public ObservableCollection<Subject> ActualSubjects = new ObservableCollection<Subject> { };

		static public IInputElement focusedControl = null;

		//Указатели на текущий предмет, вариант, шаблон вопроса, вопрос и список ошибок
		static public Subject CurrentSubject = null;
		static public Variant CurrentVariant = null;
		static public AnswerCharacteristic CurrentAnswerCharacteristic = null;
		static public Question CurrentQuestion = null;
		static public ObservableCollection<Alert> CurrentAlerts = null;

		//SQL подключение
		static public SqlConnectionStringBuilder SQLBuilder = new SqlConnectionStringBuilder();
		static public SqlConnection SQLConnection = null;
		static public bool isFatalError = false;

		//Перечисления
		static public Dictionary<int, string> SubjectTypes = new Dictionary<int, string>() { };
		static public Dictionary<int, string> QuestionTypes = new Dictionary<int, string>() { };
		static public Dictionary<int, string> CheckTypes = new Dictionary<int, string>() { };

		/*
			Получение ключа по значению из Dictionary
		*/
		#region
		public static T KeyByValue<T, W>(this Dictionary<T, W> dict, W val)
		{
			T key = default;
			foreach (KeyValuePair<T, W> pair in dict)
			{
				if (EqualityComparer<W>.Default.Equals(pair.Value, val))
				{
					key = pair.Key;
					break;
				}
			}
			return key;
		}
		#endregion

		/*
			Инициализация параметров
		*/
		#region
		static public void Init()
		{
			SubjectTypes.Add(1, "НЕ ОБОЗНАЧЕНО");
			QuestionTypes.Add(1, "НЕ ОБОЗНАЧЕНО");
			CheckTypes.Add(1, "НЕ ОБОЗНАЧЕНО");

			INIReader ConfigFile = new INIReader("Config/Config.ini");
			SQLBuilder.DataSource = ConfigFile.Read("DataSource", "SQL");
			SQLBuilder.IntegratedSecurity = bool.Parse(ConfigFile.Read("IntegratedSecurity", "SQL"));
			if (SQLBuilder.IntegratedSecurity == false)
            {
				SQLBuilder.UserID = ConfigFile.Read("UserID", "SQL");
				SQLBuilder.Password = ConfigFile.Read("Password", "SQL");
            }
			SQLBuilder.InitialCatalog = ConfigFile.Read("InitialCatalog", "SQL");
			SQLBuilder.TrustServerCertificate = bool.Parse(ConfigFile.Read("TrustServerCertificate", "SQL"));
			SQLConnection = new SqlConnection(SQLBuilder.ConnectionString);

			try
			{
				using (SqlCommand com = new SqlCommand("SELECT * FROM ControlEvent", SQLConnection))
				{
					SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							if (reader.GetInt32(0) == 1) continue;
							SubjectTypes.Add(reader.GetInt32(0), reader.GetString(1));
						}
					}
					SQLConnection.Close();
				}
			}
			catch (Exception e)
			{
				SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + e.ToString();
				WorkWindow.IsEnabled = false;
				SQLErrorWindow.Show();
				isFatalError = true;
				return;
			}

			try
			{
				using (SqlCommand com = new SqlCommand("SELECT * FROM QuestionType", SQLConnection))
				{
					SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							if (reader.GetInt32(0) == 1) continue;
							QuestionTypes.Add(reader.GetInt32(0), reader.GetString(1));
						}
					}
					SQLConnection.Close();
				}
			}
			catch (Exception e)
			{
				SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + e.ToString();
				WorkWindow.IsEnabled = false;
				SQLErrorWindow.Show();
				isFatalError = true;
				return;
			}

			try
			{
				using (SqlCommand com = new SqlCommand("SELECT * FROM CheckType", SQLConnection))
				{
					SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							if (reader.GetInt32(0) == 1) continue;
							CheckTypes.Add(reader.GetInt32(0), reader.GetString(1));
						}
					}
					SQLConnection.Close();
				}
			}
			catch (Exception e)
			{
				SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + e.ToString();
				WorkWindow.IsEnabled = false;
				SQLErrorWindow.Show();
				isFatalError = true;
				return;
			}

			ActualSubjects = Subjects;
		}
        #endregion

        /*
			Предзагрузка тестовых данных
		*/
        #region
        static public void PreLoad()
        {
		}
        #endregion

        /*
			Выгрузка предметов из базы данных
		*/
        #region
		static public void LoadFromDB()
        {
			try
			{
				using (SqlCommand com = new SqlCommand("SELECT * FROM Subject", SQLConnection))
				{
					SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							Subjects.Add(new Subject()
							{
								SubjectID = reader.GetInt32(0),
								SubjectCode = reader.GetInt16(1),
								EventCode = reader.GetInt16(2),
								MinScore = reader.GetInt16(3),
								Name = reader.GetString(4),
								Description = reader.GetString(5),
								Type = SubjectTypes[reader.GetInt32(6)],
								ProjectFolderPath = reader.GetString(7),
								RegistrationForm = reader.GetString(8),
								AnswersForm1 = reader.GetString(9),
								AnswersForm2 = reader.GetString(10),
								LogFile = reader.GetString(11),
								IsMark = reader.GetBoolean(12)
							});
						}
					}
					SQLConnection.Close();
				}
			}
			catch (Exception e)
			{
				SQLErrorWindow.SQLErrorTextBlock.Text += "\n\n\n" + e.ToString();
				WorkWindow.IsEnabled = false;
				SQLErrorWindow.Show();
				isFatalError = true;
				return;
			}
		}
        #endregion

        /*
			Получение описания в перечислении
		*/
        #region
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
        #endregion

        /*
			Получение элемента по индексу в массиве строк
		*/
        #region
        public static int GetIndexByElement(string value, string[] values)
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
		#endregion

		/*
			Итераторы перечислений (для корректного вывода описания перечислений)
		*/

		static public T GetVisualChild<T>(Visual parent) where T : Visual
		{
			T child = default(T);
			int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < numVisuals; i++)
			{
				Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
				child = v as T;
				if (child == null)
				{
					child = GetVisualChild<T>(v);
				}
				if (child != null)
				{
					break;
				}
			}
			return child;
		}

		static public DataGridCell GetCell(this DataGrid grid, DataGridRow row, int columnIndex)
		{
			if (row != null)
			{
				DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

				if (presenter == null)
				{
					grid.ScrollIntoView(row, grid.Columns[columnIndex]);
					presenter = GetVisualChild<DataGridCellsPresenter>(row);
				}

				DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
				return cell;
			}
			return null;
		}
		#region
		static public IEnumerable<AlertType> AlertTypeValues => Enum.GetValues(typeof(AlertType)).Cast<AlertType>();
        #endregion
    }
    #endregion
    //========================================================================================================================================



    //========================================================================================================================================
    //===Вспомогательные классы для БД========================================================================================================
    //========================================================================================================================================
    #region
	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Перечисление для типов ошибок--------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	[TypeConverter(typeof(DescriptionConverter))]
	enum AlertType
	{
		[Description("Поле не заполнено")]
		FieldNotFilled = 1,
		[Description("Итоговые баллы заполнены не по возрастанию")]
		ScoreInconsequence,
		[Description("Оценки заполнены не по возрастанию")]
		MarkInconsequence,
		[Description("Отсутствует эталонный ответ")]
		NoReferenceResponce,
		[Description("Недостаточное или избыточное кол-во баллов за вопрос")]
		NotEnoughOrExcessScoreForQuestion,
        [Description("Отсутствие разбаловки ошибок")]
		NoErrorScaleSystem
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс ScaleUnit для единиц системы шкалирования--------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class ScaleUnit
	{
		public short FirstScore { get; set; }
		public short Mark { get; set; }
        public short SecondScore { get; set; }

		public ScaleUnit(short FirstScore = 1, short Mark = 1, short SecondScore = 1)
		{
			this.FirstScore = FirstScore;
			this.Mark = Mark;
			this.SecondScore = SecondScore;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс Subject для предметов----------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class Subject
	{
		public int SubjectID { get; set; }
		public short SubjectCode { get; set; }
		public short EventCode { get; set; }
		public short MinScore { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public string ProjectFolderPath { get; set; }
		public string RegistrationForm { get; set; }
		public string AnswersForm1 { get; set; }
		public string AnswersForm2 { get; set; }
		public string LogFile { get; set; }
		public bool IsMark { get; set; }
		public string IsMarkValue
        {
			get
			{
				if (IsMark)
                {
					return "Оценка";
                }
                else
                {
					return "Балл";
                }

			}
			set { }
            
        }
		public ObservableCollection<Alert> Alerts = null;
		public ObservableCollection<ScaleUnit> ScaleSystem { get; set; }

		public ObservableCollection<Variant> Variants = null;

		public ObservableCollection<AnswerCharacteristic> Questions = null;

		public Subject()
		{
			SubjectCode = -1; EventCode = -1;
			MinScore = -1; SubjectID = 0;
			Name = ""; Description = "";
			ProjectFolderPath = ""; RegistrationForm = "";
			AnswersForm1 = ""; AnswersForm2 = "";
			LogFile = ""; IsMark = false;
			Type = Workfield.SubjectTypes[1];

			Questions = new ObservableCollection<AnswerCharacteristic> { };
			Variants = new ObservableCollection<Variant> { };
			ScaleSystem = new ObservableCollection<ScaleUnit>() { };
			Alerts = new ObservableCollection<Alert>() { };
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс Variant для вариантов----------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class Variant
	{
		public int VariantID { get; set; }
		public string Name { get; set; }
		public string VariantFilePath { get; set; }
		public List<Question> QuestionsPerVar = null;

		public Variant()
		{
			VariantID = 0;
			Name = ""; VariantFilePath = "";
			QuestionsPerVar = new List<Question> { };
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс AnswerCharacteristic для шаблонов вопросов-------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class AnswerCharacteristic
	{
		public int AnswerCharacteristicID { get; set; }
		public short Number { get; set; }
		public string Criterion { get; set; }
		public string ValidChars { get; set; }
		public string QuestionType { get; set; }
		public int QuestionTypeKey
		{
			get
			{
				return Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, QuestionType);
			}
			set
			{
				QuestionType = Workfield.QuestionTypes[value];
			}
		}
		public string CheckType { get; set; }
		public int CheckTypeKey
		{
			get
			{
				return Workfield.KeyByValue<int, string>(Workfield.CheckTypes, CheckType);
			}
			set
			{
				CheckType = Workfield.CheckTypes[value];
			}
		}
		public short MaxScore { get; set; }
		public List<ErrorScaleUnit> Errors { get; set; }

		public AnswerCharacteristic()
		{
			AnswerCharacteristicID = 0;
			Number = -1; Criterion = "";
			ValidChars = ""; MaxScore = -1;
			CheckType = Workfield.CheckTypes[1];
			QuestionType = Workfield.QuestionTypes[1];
			Errors = new List<ErrorScaleUnit>() { new ErrorScaleUnit() { ErrorCount = 0, Score = 1 } };
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс Question для вопросов на варианты----------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class Question
	{
		public int QuestionID { get; set; }
		public short Number { get; set; }
		public short InVariant { get; set; }
		public short MaxScore { get; set; }
		public string QuestionType { get; set; }
		public int QuestionTypeKey 
		{
            get
            {
				return Workfield.KeyByValue<int, string>(Workfield.QuestionTypes, QuestionType);
            }
            set
            {
				QuestionType = Workfield.QuestionTypes[value];
            }
		}
		public List<Answer> Answers { get; set; }

		public AnswerCharacteristic QuestionTemplate = null;

		public Question(AnswerCharacteristic questionTemplate)
		{
			QuestionID = 0;
			Number = 0; InVariant = 0; MaxScore = 0;
			QuestionType = Workfield.QuestionTypes[1];
			Answers = new List<Answer>() { };
			QuestionTemplate = questionTemplate;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс Answer для эталонных ответов---------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class Answer
	{
		public int AnswerID { get; set; }
		public string RightAnswer { get; set; }
		public short Score { get; set; }

		public Answer()
		{
			AnswerID = 0;
			RightAnswer = ""; Score = -1;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс ErrorScaleUnit для разбаловки ошибок---------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class ErrorScaleUnit
    {
		public int ErrorScaleUnitID { get; set; }
		public short ErrorCount { get; set; }
		public short Score { get; set; }

		public ErrorScaleUnit()
        {
			ErrorScaleUnitID = 0;
			ErrorCount = 0; Score = 1;
		}
    }
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс Alert для ошибок настройки предметов-------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class Alert
    {
		public int AlertID { get; set; }
		public string Icon { get; set; }
		public string Header { get; set; }
		public AlertType alertType { get; set; }
		public string Description { get; set; }

		public Alert(AlertType alertType, string Description)
        {
			AlertID = 0;
			this.alertType = alertType;
			Header = Workfield.GetEnumDescription(alertType);
			this.Description = Description;
			switch (alertType)
			{
				case AlertType.FieldNotFilled:
					Icon = "Icons/Plus.png";
					break;
				case AlertType.NoReferenceResponce:
					Icon = "Icons/Edit.png";
					break;
				case AlertType.NoErrorScaleSystem:
					Icon = "Icons/Delete.png";
					break;
				case AlertType.NotEnoughOrExcessScoreForQuestion:
					Icon = "Icons/Cancel.png";
					break;
				case AlertType.MarkInconsequence:
					Icon = "Icons/CheckResult.png";
					break;
				case AlertType.ScoreInconsequence:
					Icon = "Icons/Settings.png";
					break;
			}
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс AlertManager для отслеживания статуса ошибок в настройке предмета--------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	static class AlertManager
	{
		static public bool CheckAlerts(AlertType alertTypeToCheck, object currentElement, List<object> checkData = null)
        {
			bool result = false;

			switch (alertTypeToCheck)
            {
				case AlertType.FieldNotFilled:
					result = CheckAlert_FieldNotFilled(currentElement, checkData);
					break;
				case AlertType.ScoreInconsequence:
					result = CheckAlert_ScoreInconsequence(currentElement);
					break;
				case AlertType.MarkInconsequence:
					result = CheckAlert_MarkInconsequence(currentElement);
					break;
				case AlertType.NoReferenceResponce:
					result = CheckAlert_NoReferenceResponce(currentElement, checkData);
					break;
				case AlertType.NotEnoughOrExcessScoreForQuestion:
					result = CheckAlert_NotEnoughOrExcessScoreForQuestion(currentElement, checkData);
					break;
				case AlertType.NoErrorScaleSystem:
					result = CheckAlert_NoErrorScaleSystem(currentElement, checkData);
					break;
            }

			if (Workfield.CurrentSubject != null)
			{
				Workfield.WorkWindow.AlertList.ItemsSource = Workfield.CurrentSubject.Alerts;
				Workfield.WorkWindow.AlertList.Items.Refresh();
			}

			return result;
        }

		static private bool CheckAlert_FieldNotFilled(object currentElement, List<object> checkData)
        {
			bool result = false;
			int eventNumber = (int)checkData[0];
			switch (eventNumber)
            {
				case 1: //Поля в настройках предмета
					{
						TextBox currentTextBox = currentElement as TextBox;
						string currentLabel = checkData[1] as string;
						Subject currentSubject = checkData[2] as Subject;
						bool isNumber = (bool)checkData[3];

						if ((!isNumber && currentTextBox.Text.Trim(' ') == "") || (isNumber && currentTextBox.Text.Trim(' ') == "-1"))
						{
							result = true;
							currentSubject.Alerts.Add(new Alert(AlertType.FieldNotFilled, "Не заполнено поле \"" + currentLabel + "\" в свойствах текущего предмета \"" + currentSubject.Name + ". " + currentSubject.Type + "\"."));
						}
					}

					break;

				case 2: //Таблица шкалирования
					{
						DataGridRow currentDataGridRow = currentElement as DataGridRow;
						DataGridColumn currentDataGridColumn = checkData[1] as DataGridColumn;
						int index = Workfield.WorkWindow.ScaleList.Columns.Single(c => c.Header.ToString().ToUpper() == currentDataGridColumn.Header.ToString().ToUpper()).DisplayIndex;
						DataGridCell dgc = Workfield.GetCell(Workfield.WorkWindow.ScaleList, currentDataGridRow, index);
						string str = Convert.ToString(((TextBlock)dgc.Content).Text);

						if (str.Trim(' ') == "")
						{
							result = true;
							Workfield.CurrentSubject.Alerts.Add(new Alert(AlertType.FieldNotFilled, "Не заполнено поле в таблице шкалирования текущего предмета: Первичный балл - " + ((ScaleUnit)currentDataGridRow.Item).FirstScore.ToString() + ", Столбец - " + currentDataGridColumn.Header.ToString() + "."));
						}
					}

					break;

				case 3: //Таблица эталонного ответа
					{
						DataGridRow currentDataGridRow = currentElement as DataGridRow;
						DataGridColumn currentDataGridColumn = checkData[1] as DataGridColumn;
						DataGridRow parentDataGridRow = checkData[2] as DataGridRow;
						DataGrid currentDataGrid = checkData[3] as DataGrid;
						bool isNumber = (bool)checkData[4];
						int index = Workfield.WorkWindow.ScaleList.Columns.Single(c => c.Header.ToString().ToUpper() == currentDataGridColumn.Header.ToString().ToUpper()).DisplayIndex;
						DataGridCell dgc = Workfield.GetCell(currentDataGrid, currentDataGridRow, index);
						string str = Convert.ToString(((TextBlock)dgc.Content).Text);

						if ((!isNumber && str.Trim(' ') == "") || (isNumber && str.Trim(' ') == "-1"))
						{
							result = true;
							Workfield.CurrentSubject.Alerts.Add(new Alert(AlertType.FieldNotFilled, "Не заполнено поле в таблице эталонных ответов: Номер эталонного ответа - " + currentDataGrid.Items.IndexOf(currentDataGridRow.Item).ToString() + ", Столбец - " + currentDataGridColumn.Header.ToString() + ", Номер вопроса - " + Workfield.WorkWindow.AnswerList.Items.IndexOf(parentDataGridRow.Item).ToString() + "."));
						}
					}

					break;

				case 4: //Таблица шкалирования ошибок
					{
						DataGridRow currentDataGridRow = currentElement as DataGridRow;
						DataGridColumn currentDataGridColumn = checkData[1] as DataGridColumn;
						DataGridRow parentDataGridRow = checkData[2] as DataGridRow;
						DataGrid currentDataGrid = checkData[3] as DataGrid;
						int index = Workfield.WorkWindow.ScaleList.Columns.Single(c => c.Header.ToString().ToUpper() == currentDataGridColumn.Header.ToString().ToUpper()).DisplayIndex;
						DataGridCell dgc = Workfield.GetCell(currentDataGrid, currentDataGridRow, index);
						string str = Convert.ToString(((TextBlock)dgc.Content).Text);

						if (str.Trim(' ') == "")
						{
							result = true;
							Workfield.CurrentSubject.Alerts.Add(new Alert(AlertType.FieldNotFilled, "Не заполнено поле в таблице шкалирования ошибок: Номер строки - " + currentDataGrid.Items.IndexOf(currentDataGridRow.Item).ToString() + ", Столбец - " + currentDataGridColumn.Header.ToString() + ", Номер вопроса - " + Workfield.WorkWindow.QuestionList.Items.IndexOf(parentDataGridRow.Item).ToString() + "."));
						}
					}

					break;

				case 5: //Таблица шаблонов вопросов
                    {
						DataGridRow currentDataGridRow = currentElement as DataGridRow;
						DataGridColumn currentDataGridColumn = checkData[1] as DataGridColumn;
						bool isNumber = (bool)checkData[2];
						int index = Workfield.WorkWindow.ScaleList.Columns.Single(c => c.Header.ToString().ToUpper() == currentDataGridColumn.Header.ToString().ToUpper()).DisplayIndex;
						DataGridCell dgc = Workfield.GetCell(Workfield.WorkWindow.QuestionList, currentDataGridRow, index);

						if (dgc.Content is TextBlock)
						{
							string str = Convert.ToString(((TextBlock)dgc.Content).Text);

							if ((!isNumber && str.Trim(' ') == "") || (isNumber && str.Trim(' ') == "-1"))
							{
								result = true;
								Workfield.CurrentSubject.Alerts.Add(new Alert(AlertType.FieldNotFilled, "Не заполнено поле в таблице шаблонов вопросов: Номер вопроса - " + Workfield.WorkWindow.QuestionList.Items.IndexOf(currentDataGridRow.Item).ToString() + ", Столбец - " + currentDataGridColumn.Header.ToString() + "."));
							}
						}
						else if (dgc.Content is ComboBox)
                        {
							string str = Convert.ToString(((KeyValuePair<int, string>)((ComboBox)dgc.Content).SelectedItem).Value);

							if (str == "НЕ ОБОЗНАЧЕНО")
							{
								result = true;
								Workfield.CurrentSubject.Alerts.Add(new Alert(AlertType.FieldNotFilled, "Не заполнено поле в таблице шаблонов вопросов: Номер вопроса - " + Workfield.WorkWindow.QuestionList.Items.IndexOf(currentDataGridRow.Item).ToString() + ", Столбец - " + currentDataGridColumn.Header.ToString() + "."));
							}
						}
					}					
					break;
            }
			return result;
        }

		static private bool CheckAlert_ScoreInconsequence(object currentElement)
		{
			bool result = false;

			return result;
		}

		static private bool CheckAlert_MarkInconsequence(object currentElement)
		{
			bool result = false;

			return result;
		}

		static private bool CheckAlert_NoReferenceResponce(object currentElement, List<object> checkData)
		{
			bool result = false;

			return result;
		}

		static private bool CheckAlert_NotEnoughOrExcessScoreForQuestion(object currentElement, List<object> checkData)
		{
			bool result = false;

			return result;
		}

		static private bool CheckAlert_NoErrorScaleSystem(object currentElement, List<object> checkData)
		{
			bool result = false;

			return result;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	#endregion
	//========================================================================================================================================
}
