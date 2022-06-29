using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

/*
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

1-2
2-4
~(А-Я)~(а-я)-
А-Я
диапазон
2 алерта
непр доп симв
несоотв эталон отв доп симв

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
------алерты в трее
------Привязка к БД
------Разбаловка шаблона вопроса
------кнопки переключения видимости подтаблиц
------нумерация вопросов
------пофиксить -1 у макс балла
------пофиксить автоввод 0 у кода мер и пред
------пофиксить алерт шкалирования ошибок
------пофиксить алерт эталонного ответа (не появляется)
------добавить enter у ввода названия варика
------пофиксить указание нумерации вопроса в алертах
------пофиксить пустое название варика в алертах (вызов алерта до изменения названия варика)
------навесить изменение алертов на изменении и удалении вариантов
------навесить изменение алертов при изменении взаимосвязанных данных (номер в варике например)
------пресеты доп символов
------подсказки
------соответствие эталонного ответа с доп симв
------нах множ выделение
------уникальное название варика неправ варики начинаются на !
------запрет вставки в вводе
------тип проверки уникальный
------Макс балл в трее
------пофиксить -1 балл у эталонного ответа
пока есть тесты прорешанные нельзя удалить
район код района   и   школа, название код   в бд - для организаци id школы в районе, у теста ссылка на id школы
номер кима
номер бланка рег
номер бланка отв 1
номер бланка отв 2 
номер бланка отв 2 доп

допилить алерты под коды
допилить правильное обновление состояния прямоугольника

наносить номер в прямоугольнике
формировать пдф
отправлять на печать


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
		static public Dictionary<int, string> CurrentBlankTypes = new Dictionary<int, string>() { };
		static public Blank CurrentBlank = null;
		static public CodeField CurrentCodeField = null;

		//SQL подключение
		static public SqlConnectionStringBuilder SQLBuilder = new SqlConnectionStringBuilder();
		static public SqlConnection SQLConnection = null;
		static public bool isFatalError = false;

		//Перечисления
		static public Dictionary<int, string> SubjectTypes = new Dictionary<int, string>() { };
		static public Dictionary<int, string> QuestionTypes = new Dictionary<int, string>() { };
		static public Dictionary<int, string> CheckTypes = new Dictionary<int, string>() { };
		static public Dictionary<int, string> CheckTypes1 = new Dictionary<int, string>() { };
		static public Dictionary<int, string> CheckTypes2 = new Dictionary<int, string>() { };
		static public Dictionary<int, string> CheckTypes3 = new Dictionary<int, string>() { };
		static public Dictionary<int, Dictionary<int, string>> BlankTypes = new Dictionary<int, Dictionary<int, string>>() { };
		static public Dictionary<int, string> CodeTypes = new Dictionary<int, string>() { };
		static public Dictionary<int, string> OrientationTypes = new Dictionary<int, string>() { };

		static public ObservableCollection<BlankCSV> blankCSVs = new ObservableCollection<BlankCSV>() { };
		static public ObservableCollection<Kit> kits = new ObservableCollection<Kit>() { };
		static public ObservableCollection<Report> reports = new ObservableCollection<Report>() { };

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
			BlankTypes.Add(0, new Dictionary<int, string>() { });
			BlankTypes[0].Add(1, "НЕ ОБОЗНАЧЕНО");
			CodeTypes.Add(1, "НЕ ОБОЗНАЧЕНО");

			OrientationTypes.Add(0, "0°");
			OrientationTypes.Add(1, "90°");
			OrientationTypes.Add(2, "180°");
			OrientationTypes.Add(3, "270°");

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
							BlankTypes.Add(reader.GetInt32(0), new Dictionary<int, string>() { });
							BlankTypes[reader.GetInt32(0)].Add(1, "НЕ ОБОЗНАЧЕНО");
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

			CheckTypes1.Add(1, CheckTypes[1]);
			CheckTypes1.Add(2, CheckTypes[2]);
			CheckTypes1.Add(3, CheckTypes[3]);
			CheckTypes2.Add(1, CheckTypes[1]);
			CheckTypes2.Add(4, CheckTypes[4]);
			CheckTypes2.Add(5, CheckTypes[5]);
			CheckTypes2.Add(6, CheckTypes[6]);
			CheckTypes3.Add(1, CheckTypes[1]);

			try
			{
				using (SqlCommand com = new SqlCommand("SELECT * FROM BlankType", SQLConnection))
				{
					SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							if (reader.GetInt32(0) == 1) continue;
							BlankTypes[reader.GetInt32(2)].Add(reader.GetInt32(0), reader.GetString(1));
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
				using (SqlCommand com = new SqlCommand("SELECT * FROM CodeType", SQLConnection))
				{
					SQLConnection.Open();
					using (SqlDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							if (reader.GetInt32(0) == 1) continue;
							CodeTypes.Add(reader.GetInt32(0), reader.GetString(1));
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
			kits.Add(new Kit()
			{
				SecondName = "Иванов",
				FirstName = "Иван",
				Patronymic = "Иванович",
				Serial = "7118",
				Number = "076111",
				School = 237020,
				ClassNumber = 11,
				ClassLetter = "A",
				VariantName = "1101",
				RegPacketNumber = "3101110100067",
				Ans1PacketNumber = "1101110100067",
				Ans2PacketNumber = "2101110100067",
				PrPacketNumber = "5101110100067"
			});
			kits.Add(new Kit()
			{
				SecondName = "Жуков",
				FirstName = "Георгий",
				Patronymic = "Анатольевич",
				Serial = "7118",
				Number = "076121",
				School = 237020,
				ClassNumber = 11,
				ClassLetter = "A",
				VariantName = "1103",
				RegPacketNumber = "3101110300075",
				Ans1PacketNumber = "1101110300075",
				Ans2PacketNumber = "2101110300075",
				PrPacketNumber = "5101110300075"
			});
			kits.Add(new Kit()
			{
				SecondName = "Кузнецова",
				FirstName = "Полина",
				Patronymic = "Семеновна",
				Serial = "7118",
				Number = "076728",
				School = 237020,
				ClassNumber = 11,
				ClassLetter = "А",
				VariantName = "1102",
				RegPacketNumber = "3101110200041",
				Ans1PacketNumber = "1101110200041",
				Ans2PacketNumber = "2101110200041",
				PrPacketNumber = "5101110200041"
			});
			kits.Add(new Kit()
			{
				SecondName = "Григорьев",
				FirstName = "Олег",
				Patronymic = "Александрович",
				Serial = "7118",
				Number = "076344",
				School = 237020,
				ClassNumber = 11,
				ClassLetter = "А",
				VariantName = "1104",
				RegPacketNumber = "3101110400077",
				Ans1PacketNumber = "1101110400077",
				Ans2PacketNumber = "2101110400077",
				PrPacketNumber = "5101110400077"
			});
			kits.Add(new Kit()
			{
				SecondName = "Серый",
				FirstName = "Петр",
				Patronymic = "Петрович",
				Serial = "7118",
				Number = "076112",
				School = 237020,
				ClassNumber = 11,
				ClassLetter = "А",
				VariantName = "1102",
				RegPacketNumber = "3101110200098",
				Ans1PacketNumber = "1101110200098",
				Ans2PacketNumber = "2101110200098",
				PrPacketNumber = "5101110200098"
			});
			kits.Add(new Kit()
			{
				SecondName = "Борисенко",
				FirstName = "Вадим",
				Patronymic = "Евгеньевич",
				Serial = "7118",
				Number = "076624",
				School = 237020,
				ClassNumber = 11,
				ClassLetter = "А",
				VariantName = "1103",
				RegPacketNumber = "3101110300024",
				Ans1PacketNumber = "1101110300024",
				Ans2PacketNumber = "2101110300024",
				PrPacketNumber = "5101110300024"
			});

			reports.Add(new Report()
			{
				Number = 1,
				CodeOU = 237020,
				SecondName = "Иванов",
				FirstName = "Иван",
				Patronymic = "Иванович",
				VariantName = "1101",
				FirstScore = 49,
				Score = 80,
				ScoreA = "+++++++4+++++++-+++++-+++4",
				ScoreB = "1(1)4(6)1(1)1(1)2(2)1(2)3(3)2(3)1(2)1(2)1(1)1(1)",
				Percent = "94%"
			});
			reports.Add(new Report()
			{
				Number = 2,
				CodeOU = 237020,
				SecondName = "Жуков",
				FirstName = "Георгий",
				Patronymic = "Анатольевич",
				VariantName = "1103",
				FirstScore = 51,
				Score = 85,
				ScoreA = "+++++++5-+++++++++-++++++4",
				ScoreB = "1(1)5(6)1(1)1(1)2(2)1(2)3(3)1(3)2(2)1(2)1(1)1(1)",
				Percent = "64%"
			});
			reports.Add(new Report()
			{
				Number = 3,
				CodeOU = 237020,
				SecondName = "Кузнецова",
				FirstName = "Полина",
				Patronymic = "Семеновна",
				VariantName = "1102",
				FirstScore = 47,
				Score = 76,
				ScoreA = "-+++++-4-++-++++++-+++---4",
				ScoreB = "1(1)6(6)1(1)1(1)2(2)2(2)2(3)2(3)2(2)2(2)1(1)1(1)",
				Percent = "80%"
			});
			reports.Add(new Report()
			{
				Number = 4,
				CodeOU = 237020,
				SecondName = "Григорьев",
				FirstName = "Олег",
				Patronymic = "Александрович",
				VariantName = "1104",
				FirstScore = 25,
				Score = 48,
				ScoreA = "--+-++-3--+-+-+------+-+-2",
				ScoreB = "1(1)3(6)1(1)1(1)1(2)1(2)1(3)0(3)0(2)1(2)1(1)1(1)",
				Percent = "80%"
			});
			reports.Add(new Report()
			{
				Number = 5,
				CodeOU = 237020,
				SecondName = "Серый",
				FirstName = "Петр",
				Patronymic = "Петрович",
				VariantName = "1102",
				FirstScore = 43,
				Score = 70,
				ScoreA = "-+++-++4+++-+++-+++--++++4",
				ScoreB = "1(1)4(6)0(1)0(1)1(2)2(2)3(3)1(3)2(2)1(2)1(1)1(1)",
				Percent = "80%"
			});
			reports.Add(new Report()
			{
				Number = 6,
				CodeOU = 237020,
				SecondName = "Борисенко",
				FirstName = "Вадим",
				Patronymic = "Евгеньевич",
				VariantName = "1103",
				FirstScore = 45,
				Score = 72,
				ScoreA = "+++++++3++--+--++++++++++4",
				ScoreB = "1(1)2(6)1(1)0(1)2(2)2(2)3(3)2(3)1(2)2(2)1(1)1(1)",
				Percent = "80%"
			});
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
			Получение дочернего элемента в древе WPF
		*/
        #region
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
		#endregion

		/*
			Получение элемента DataGridCell в DataGrid
		*/
		#region
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
		#endregion

		/*
			Итераторы перечислений
		*/
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
		[Description("Отсутствует эталонный ответ")]
		NoReferenceResponce,
		[Description("Избыточное кол-во баллов за вопрос")]
		NotEnoughOrExcessScoreForQuestion,
		[Description("Отсутствие разбалловки ошибок")]
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
		public short MaxScore { get; set; }
		public ObservableCollection<Alert> Alerts = null;
		public ObservableCollection<ScaleUnit> ScaleSystem { get; set; }

		public ObservableCollection<Variant> Variants = null;

		public ObservableCollection<AnswerCharacteristic> Questions = null;
		public ObservableCollection<Blank> Blanks = null;
		public ObservableCollection<PrintBlank> PrintBlanks = null;
		public ObservableCollection<Kit> Kits = null;

		public Subject()
		{
			SubjectCode = -1; EventCode = -1;
			MinScore = -1; SubjectID = 0;
			Name = ""; Description = "";
			ProjectFolderPath = ""; RegistrationForm = "";
			AnswersForm1 = ""; AnswersForm2 = "";
			LogFile = ""; IsMark = false;
			Type = Workfield.SubjectTypes[1];
			MaxScore = 0;

			Questions = new ObservableCollection<AnswerCharacteristic> { };
			Variants = new ObservableCollection<Variant> { };
			ScaleSystem = new ObservableCollection<ScaleUnit>() { };
			Blanks = new ObservableCollection<Blank>() { };
			PrintBlanks = new ObservableCollection<PrintBlank>() { };
			Kits = new ObservableCollection<Kit>() { };
			Alerts = new ObservableCollection<Alert>() 
			{ 
				new Alert(AlertType.FieldNotFilled),
				new Alert(AlertType.ScoreInconsequence),
				new Alert(AlertType.NoReferenceResponce),
				new Alert(AlertType.NotEnoughOrExcessScoreForQuestion),
				new Alert(AlertType.NoErrorScaleSystem)
			};
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
		private string criterion;
		public string Criterion { get { return criterion; } set { if (value == null) criterion = ""; else criterion = value; } }
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
				switch (value)
                {
					case 1:
						CheckTypeList = Workfield.CheckTypes3;
						CheckType = CheckTypeList[1];
						break;
					case 2:
						CheckTypeList = Workfield.CheckTypes1;
						CheckType = CheckTypeList[1];
						break;
					case 3:
						CheckTypeList = Workfield.CheckTypes2;
						CheckType = CheckTypeList[1];
						break;
					case 4:
						CheckTypeList = Workfield.CheckTypes3;
						CheckType = CheckTypeList[1];
						break;
				}
			}
		}
		public string CheckType { get; set; }
		public int CheckTypeKey
		{
			get
			{
				return Workfield.KeyByValue<int, string>(CheckTypeList, CheckType);
			}
			set
			{
				CheckType = CheckTypeList[value];
			}
		}
		public short MaxScore { get; set; }
		public List<ErrorScaleUnit> Errors { get; set; }
		public Dictionary<int, string> CheckTypeList { get; set; }

		public AnswerCharacteristic()
		{
			AnswerCharacteristicID = 0;
			Number = -1; Criterion = "";
			ValidChars = ""; MaxScore = -1;
			CheckTypeList = Workfield.CheckTypes3;
			CheckType = CheckTypeList[1];
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
		private string criterion;
		public string Criterion { get { return criterion; } set { if (value == null) criterion = ""; else criterion = value; } }
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
		public List<Answer> Answers { get; set; }

		public AnswerCharacteristic QuestionTemplate = null;

		public Question(AnswerCharacteristic questionTemplate)
		{
			QuestionID = 0; Criterion = "";
			Number = 0; InVariant = 0; MaxScore = 0;
			QuestionType = Workfield.QuestionTypes[1];
			CheckType = Workfield.CheckTypes[1];
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
			RightAnswer = ""; Score = 1;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс ErrorScaleUnit для разбаловки ошибок-------------------------------------------------------------------------------------------
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
		public bool IsActive { 
			get 
			{ 
				if (PostAlerts.Count <= 0)
                {
					return false;
                }
				else
                {
					return true;
                }
			} 
			set { } 
		}
		public List<PostAlert> PostAlerts { get; set; }

		public Alert(AlertType alertType)
		{
			AlertID = 0;
			this.alertType = alertType;
			Header = Workfield.GetEnumDescription(alertType);
			PostAlerts = new List<PostAlert>() { };
			switch (alertType)
			{
				case AlertType.FieldNotFilled:
					Icon = "Icons/FieldError.png";
					break;
				case AlertType.NoReferenceResponce:
					Icon = "Icons/AnswerError.png";
					break;
				case AlertType.NoErrorScaleSystem:
					Icon = "Icons/ErrorsError.png";
					break;
				case AlertType.NotEnoughOrExcessScoreForQuestion:
					Icon = "Icons/ScoreError.png";
					break;
				case AlertType.ScoreInconsequence:
					Icon = "Icons/ScaleError.png";
					break;
			}
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс PostAlert для подошибок настройки предметов------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class PostAlert
    {
		public int PostAlertID { get; set; }
		public string Description { get; set; }
		public List<object> ProblemData { get; set; }
		public int ProblemCode { get; set; }

		/*
			0 - undefined
			1 - FieldNotFilled, 1
			2 - FieldNotFilled, 2
			3 - FieldNotFilled, 3
			4 - ScoreInconsequnce Mark
			5 - ScoreInconsequnce Score
			6 - NoReferenceResponce
			7 - NotEnoughOrExcessScoreForQuestion
			8 - NoErrorScaleSystem
			9 - FieldNotFilled, 4
			10 - FIeldNotFilled, 5
		*/

		public PostAlert(string Description, List<object> ProblemData, int ProblemCode)
        {
            PostAlertID = 0;
            this.Description = Description;
            this.ProblemData = ProblemData;
			this.ProblemCode = ProblemCode;
        }
    }
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс Blank для бланков предмета-----------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class Blank
	{
		public int BlankID { get; set; }
		public string Type { get; set; }
		public int TypeKey
        {
			get
            {
				return Workfield.KeyByValue<int, string>(CurrentBlankTypes, Type);
            }
			set
            {
				Type = CurrentBlankTypes[value];
            }
        }
		public string Path { get; set; }
		public Dictionary<int, string> CurrentBlankTypes { get; set; }
		public ObservableCollection<CodeField> CodeFields { get; set; }

		public Blank()
		{
			BlankID = 0;	Path = "";
			Type = Workfield.BlankTypes[0][1];
			CurrentBlankTypes = Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, Workfield.CurrentSubject.Type)];
			CodeFields = new ObservableCollection<CodeField>() { };
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс PrintBlank для очереди бланков на печать---------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class PrintBlank
	{
		public Variant Variant { get; set; }
		public int Count { get; set; }
		public ObservableCollection<Variant> CurrentVariants { get; set; }

		public PrintBlank()
		{
			Variant = null;
			Count = 0;
			CurrentVariants = Workfield.CurrentSubject.Variants;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс CodeField для полей с кодами---------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class CodeField
	{
		public int CodeFieldID { get; set; }
		public string CodeType { get; set; }
		public int CodeTypeKey
        {
            get
            {
				return Workfield.KeyByValue<int, string>(Workfield.CodeTypes, CodeType);
            }
            set
            {
				CodeType = Workfield.CodeTypes[value];
            }
        }
		public string Orientation { get; set; }
		public int OrientationKey
        {
			get
            {
				return Workfield.KeyByValue<int, string>(Workfield.OrientationTypes, Orientation);
            }
			set
            {
				Orientation = Workfield.OrientationTypes[value];
            }
        }
		public int X1 { get; set; }
		public int Y1 { get; set; }
		public int X2 { get; set; }
		public int Y2 { get; set; }

		public CodeField()
		{
			Orientation = Workfield.OrientationTypes[0];
			X1 = 0; Y1 = 0; X2 = 0; Y2 = 0; 
			CodeType = Workfield.CodeTypes[1];
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс BlankCSV для загрузки CSV фалов бланков----------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class BlankCSV
	{
		public string Type { get; set; }
		public int TypeKey
		{
			get
			{
				return Workfield.KeyByValue<int, string>(CurrentBlankTypes, Type);
			}
			set
			{
				Type = CurrentBlankTypes[value];
			}
		}
		public string CSVPath { get; set; }
		public Dictionary<int, string> CurrentBlankTypes { get; set; }

		public BlankCSV()
		{
			CSVPath = "";
			Type = Workfield.BlankTypes[0][1];
			CurrentBlankTypes = Workfield.BlankTypes[Workfield.KeyByValue<int, string>(Workfield.SubjectTypes, Workfield.CurrentSubject.Type)];
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс Kit для тестируемых-----------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class Kit
	{
		public int KitID { get; set; }
		public string SecondName { get; set; }
		public string FirstName { get; set; }
		public string Patronymic { get; set; }
		public string Serial { get; set; }
		public string Number { get; set; }
		public int School { get; set; }
		public int ClassNumber { get; set; }
		public string ClassLetter { get; set; }
		public DateTime TestingDate { get; set; }
		public string VariantName { get; set; }
		public bool HasAnswerBlank1 { get; set; }
		public bool HasAnswerBlank2 { get; set; }
		public string RegPacketNumber { get; set; }
		public int RegImageNumber { get; set; }
		public string Ans1PacketNumber { get; set; }
		public int Ans1ImageNumber { get; set; }
		public string Ans2PacketNumber { get; set; }
		public int Ans2ImageNumber { get; set; }
		public string PrPacketNumber { get; set; }
		public int PrImageNumber { get; set; }
		public ObservableCollection<ScannedBlank> ScannedBlanks { get; set; }

		public Kit()
		{
			SecondName = "";
			FirstName = "";
			Patronymic = "";
			Serial = ""; Number = "";
			School = 0; ClassNumber = 0;
			ClassLetter = "";
			TestingDate = new DateTime();
			HasAnswerBlank1 = false;
			HasAnswerBlank2 = false;
			RegPacketNumber = ""; RegImageNumber = 0;
			Ans1PacketNumber = ""; Ans1ImageNumber = 0;
			Ans2PacketNumber = ""; Ans2ImageNumber = 0;
			PrPacketNumber = ""; PrImageNumber = 0;
			VariantName = "";
			ScannedBlanks = new ObservableCollection<ScannedBlank>() { };
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс ActualAnswer для отсканированных ответов---------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class ActualAnswer
	{
		public int ActualAnswerID { get; set; }
		public Question Question { get; set; }
		public string Answer { get; set; }

		public ActualAnswer()
		{
			Question = null;
			Answer = "";
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс ScannedBlank для отсканированных бланков---------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class ScannedBlank
	{
		public int BlankID { get; set; }
		public string RegBlankCode { get; set; }
		public string AnswerBlank1Code { get; set; }
		public string AnswerBlank2Code { get; set; }
		public string TestCode { get; set; }
		public Variant Variant { get; set; }
		public string Test9Code { get; set; }

		public ScannedBlank()
		{
			RegBlankCode = "";
			AnswerBlank1Code = "";
			AnswerBlank2Code = "";
			TestCode = ""; Test9Code = "";
			Variant = null;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс FullAnswerBlank для отсканированных бланков с полным ответом-------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class FullAnswerBlank
	{
		public int FullAnswerBlankID { get; set; }
		public ScannedBlank ScannedBlank { get; set; }
		public Protocol Protocol { get; set; }
		public string PacketNumber { get; set; }
		public int ImageNumber { get; set; }

		public FullAnswerBlank()
		{
			PacketNumber = ""; ImageNumber = 0;
			ScannedBlank = null; Protocol = null;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс Protocol для протоколов--------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class Protocol
	{
		public int ProtocolID { get; set; }
		public string CSVFile { get; set; }
		public bool IsPrinted { get; set; }
		public string Number { get; set; }

		public Protocol()
		{
			CSVFile = ""; Number = "";
			IsPrinted = false;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс Report для формирования записей в отчете---------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	class Report
    {
		public string SecondName { get; set; }
		public string FirstName { get; set; }
		public string Patronymic { get; set; }
		public int Number { get; set; }
		public int CodeOU { get; set; }
		public string VariantName { get; set; }
		public int FirstScore { get; set; }
		public string Percent { get; set; }
		public int Score { get; set; }
		public string ScoreA { get; set; }
		public string ScoreB { get; set; }
		public Report()
        {
			SecondName = "";
			FirstName = "";
			Patronymic = "";
			Number = 0;
			CodeOU = 0;
			VariantName = "";
			FirstScore = 0;
			Percent = "";
			Score = 0;
			ScoreA = "";
			ScoreB = "";
        }
    }
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------------------------------------------
	//---Класс WorkChecker для проверки прорешанных бланков-----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------
	#region
	static class WorkChecker
	{
		static public bool CheckWork(Subject currentSubject, Kit currentKit)
        {
			bool result = false;



			return result;
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
		static public bool CheckAlerts(AlertType alertTypeToCheck, List<object> checkData = null, bool toDelete = false)
        {
			bool result = false;

			switch (alertTypeToCheck)
            {
				case AlertType.FieldNotFilled:
					result = CheckAlert_FieldNotFilled(checkData, toDelete);
					break;
				case AlertType.ScoreInconsequence:
					result = CheckAlert_ScoreInconsequence(checkData, toDelete);
					break;
				case AlertType.NoReferenceResponce:
					result = CheckAlert_NoReferenceResponce(checkData, toDelete);
					break;
				case AlertType.NotEnoughOrExcessScoreForQuestion:
					result = CheckAlert_NotEnoughOrExcessScoreForQuestion(checkData, toDelete);
					break;
				case AlertType.NoErrorScaleSystem:
					result = CheckAlert_NoErrorScaleSystem(checkData, toDelete);
					break;
            }

			if (Workfield.CurrentSubject != null)
			{
				Workfield.WorkWindow.AlertList.ItemsSource = Workfield.CurrentSubject.Alerts;
				Workfield.WorkWindow.AlertList.Items.Refresh();
			}

			return result;
        }

		static private bool CheckAlert_FieldNotFilled(List<object> checkData, bool toDelete)
        {
			bool result = false;
			int eventNumber = (int)checkData[0];

			bool checkedBefore = false;
			PostAlert checkedAlert = null;

			switch (eventNumber)
            {
				case 1: //Поля в настройках предмета
					{
						Subject checkSubject = checkData[1] as Subject;

						int errorCount = 0;
						string errorFields = "";

						foreach (PostAlert currentAlert in checkSubject.Alerts[0].PostAlerts)
                        {
                            if(currentAlert.ProblemCode == 1 && ((Subject)currentAlert.ProblemData[1]).SubjectID == checkSubject.SubjectID)
                            {
								checkedBefore = true;
								checkedAlert = currentAlert;
                            }
                        }

						if (toDelete)
                        {
							if (checkedBefore)
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							break;
						}

						if (checkSubject.EventCode == -1)
						{
							errorFields += "кода мероприятия";
							errorCount++;
						}
						if (checkSubject.SubjectCode == -1)
						{
							errorFields += errorFields == "" ? "кода предмета" : ", кода предмета";
							errorCount++;
						}
						if (checkSubject.MinScore == -1)
						{
							errorFields += errorFields == "" ? "минимального балла" : ", минимального балла";
							errorCount++;
						}
						/*if (checkSubject.ProjectFolderPath == "")
						{
							errorFields += errorFields == "" ? "пути к папке с файлами КМ" : ", пути к папке с файлами КМ";
							errorCount++;
						}
						if (checkSubject.RegistrationForm == "")
						{
							errorFields += errorFields == "" ? "имени бланка регистрации" : ", имени бланка регистрации";
							errorCount++;
						}
						if (checkSubject.AnswersForm1 == "")
						{
							errorFields += errorFields == "" ? "имени бланка ответов №1" : ", имени бланка ответов №1";
							errorCount++;
						}
						if (checkSubject.AnswersForm2 == "")
						{
							errorFields += errorFields == "" ? "имени бланка ответов №2" : ", имени бланка ответов №2";
							errorCount++;
						}
						if (checkSubject.LogFile == "")
						{
							errorFields += errorFields == "" ? "имени файла протоколов" : ", имени файла протоколов";
							errorCount++;
						}*/

						if (checkedBefore)
						{
							if (errorCount <= 0)
                            {
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							}
                            else
							{
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
								checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("В настройках предмета не настроено(-ы) поле(-я) " + errorFields + ".", checkData, 1));
							}
						}
                        else
                        {
							if (errorCount > 0)
							{
								checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("В настройках предмета не настроено(-ы) поле(-я) " + errorFields + ".", checkData, 1));
							}
						}
					}

					break;

				case 2: //Таблица эталонного ответа
					{
						Subject checkSubject = checkData[1] as Subject;
						Variant checkVariant = checkData[2] as Variant;
						Question checkQuestion = checkData[3] as Question;
						Answer checkAnswer = checkData[4] as Answer;

						int isError = 0;

						foreach (PostAlert currentAlert in checkSubject.Alerts[0].PostAlerts)
						{
							if (currentAlert.ProblemCode == 2 && ((Subject)currentAlert.ProblemData[1]).SubjectID == checkSubject.SubjectID &&
								((Variant)currentAlert.ProblemData[2]).VariantID == checkVariant.VariantID && 
								((Question)currentAlert.ProblemData[3]).QuestionID == checkQuestion.QuestionID &&
								((Answer)currentAlert.ProblemData[4]).AnswerID == checkAnswer.AnswerID)
							{
								checkedBefore = true;
								checkedAlert = currentAlert;
							}
						}

						if (toDelete)
						{
							if (checkedBefore)
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							break;
						}

						if (checkAnswer.RightAnswer == "")
                        {
							isError = 1;
                        }
						else
                        {
							bool isNotFnd = false;
							for (int i = 0; i < checkAnswer.RightAnswer.Length; i++)
                            {
								if (!checkQuestion.QuestionTemplate.ValidChars.Contains(checkAnswer.RightAnswer[i]))
                                {
									isNotFnd = true;
									break;
                                }
                            }
							if (isNotFnd)
                            {
								isError = 2;
                            }
                        }

						if (checkedBefore)
						{
							if (isError == 1)
                            {
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
								if (checkQuestion.Criterion.Trim(' ') == "")
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Не введен эталонный ответ у вопроса №" + checkQuestion.InVariant.ToString() + " в варианте " + checkVariant.Name + ".", checkData, 2));
								else
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Не введен эталонный ответ у вопроса №" + checkQuestion.InVariant.ToString()+"-"+checkQuestion.Criterion + " в варианте " + checkVariant.Name + ".", checkData, 2));
							}
							else if (isError == 2)
							{
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
								if (checkQuestion.Criterion.Trim(' ') == "")
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Эталонный ответ у вопроса №" + checkQuestion.InVariant.ToString() + " в варианте " + checkVariant.Name + " содержит недопустимые символы.", checkData, 2));
								else
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Эталонный ответ у вопроса №" + checkQuestion.InVariant.ToString() + "-" + checkQuestion.Criterion + " в варианте " + checkVariant.Name + " содержит недопустимые символы.", checkData, 2));
							}
							else
                            {
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							}
						}
						else
						{
							if (isError == 1)
							{
								if (checkQuestion.Criterion.Trim(' ') == "")
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Не введен эталонный ответ у вопроса №" + checkQuestion.InVariant.ToString() + " в варианте " + checkVariant.Name + ".", checkData, 2));
								else
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Не введен эталонный ответ у вопроса №" + checkQuestion.InVariant.ToString() + "-" + checkQuestion.Criterion + " в варианте " + checkVariant.Name + ".", checkData, 2));
							}
							else if (isError == 2)
							{
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
								if (checkQuestion.Criterion.Trim(' ') == "")
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Эталонный ответ у вопроса №" + checkQuestion.InVariant.ToString() + " в варианте " + checkVariant.Name + " содержит недопустимые символы.", checkData, 2));
								else
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Эталонный ответ у вопроса №" + checkQuestion.InVariant.ToString() + "-" + checkQuestion.Criterion + " в варианте " + checkVariant.Name + " содержит недопустимые символы.", checkData, 2));
							}
						}
					}

					break;

				case 3: //Таблица шаблонов вопросов
                    {
						Subject checkSubject = checkData[1] as Subject;
						AnswerCharacteristic checkAnswerCharacteristic = checkData[2] as AnswerCharacteristic;

						int errorCount = 0;
						string errorFields = "";

						foreach (PostAlert currentAlert in checkSubject.Alerts[0].PostAlerts)
						{
							if (currentAlert.ProblemCode == 3 && ((Subject)currentAlert.ProblemData[1]).SubjectID == checkSubject.SubjectID &&
								((AnswerCharacteristic)currentAlert.ProblemData[2]).AnswerCharacteristicID == checkAnswerCharacteristic.AnswerCharacteristicID)
							{
								checkedBefore = true;
								checkedAlert = currentAlert;
							}
						}

						if (toDelete)
						{
							if (checkedBefore)
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							break;
						}

						if (checkAnswerCharacteristic.Number == -1)
						{
							errorFields += "номера вопроса";
							errorCount++;
						}
						if (checkAnswerCharacteristic.MaxScore == -1)
						{
							errorFields += errorFields == "" ? "максимального балла" : ", максимального балла";
							errorCount++;
						}
						if (checkAnswerCharacteristic.QuestionType == Workfield.QuestionTypes[1])
						{
							errorFields += errorFields == "" ? "типа вопроса" : ", типа вопроса";
							errorCount++;
						}
						if (checkAnswerCharacteristic.CheckType == Workfield.CheckTypes[1] && checkAnswerCharacteristic.QuestionType != Workfield.QuestionTypes[4])
						{
							errorFields += errorFields == "" ? "типа проверки" : ", типа проверки";
							errorCount++;
						}
						if (checkAnswerCharacteristic.ValidChars == "")
						{
							errorFields += errorFields == "" ? "допустимых символов" : ", допустимых символов";
							errorCount++;
						}

						if (checkedBefore)
						{
							if (errorCount <= 0)
                            {
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							}
							else
                            {
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
								if (checkAnswerCharacteristic.Criterion.Trim(' ') == "")
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("В шаблоне вопроса №" + checkAnswerCharacteristic.Number.ToString() + " не настроено(-ы) поле(-я) " + errorFields + ".", checkData, 3));
								else
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("В шаблоне вопроса №" + checkAnswerCharacteristic.Number.ToString() + "-" + checkAnswerCharacteristic.Criterion + " не настроено(-ы) поле(-я) " + errorFields + ".", checkData, 3));
							}
						}
						else
						{
							if (errorCount > 0)
							{
								if (checkAnswerCharacteristic.Criterion.Trim(' ') == "")
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("В шаблоне вопроса №" + checkAnswerCharacteristic.Number.ToString() + " не настроено(-ы) поле(-я) " + errorFields + ".", checkData, 3));
								else
									checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("В шаблоне вопроса №" + checkAnswerCharacteristic.Number.ToString() + "-" + checkAnswerCharacteristic.Criterion + " не настроено(-ы) поле(-я) " + errorFields + ".", checkData, 3));
							}
						}
					}

					break;

				case 4: //Проверка допустимых символов
					{
						Subject checkSubject = checkData[1] as Subject;
						AnswerCharacteristic checkAnswerCharacteristic = checkData[2] as AnswerCharacteristic;

						foreach (PostAlert currentAlert in checkSubject.Alerts[0].PostAlerts)
						{
							if (currentAlert.ProblemCode == 9 && ((Subject)currentAlert.ProblemData[1]).SubjectID == checkSubject.SubjectID &&
								((AnswerCharacteristic)currentAlert.ProblemData[2]).AnswerCharacteristicID == checkAnswerCharacteristic.AnswerCharacteristicID)
							{
								checkedBefore = true;
								checkedAlert = currentAlert;
							}
						}

						if (toDelete)
						{
							if (checkedBefore)
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							break;
						}

						if (checkAnswerCharacteristic.ValidChars != "")
                        {
							string curStr = checkAnswerCharacteristic.ValidChars;
							string resStr = "";

							bool tilFnd = false;
							bool openBrace = false;
							bool endBrace = false;

							int isError = 0;

							int startIndex = -1;
							int endIndex = -1;

							for (int i = 0; i < curStr.Length; i++)
                            {
								switch (curStr[i])
                                {
									case '~':
										if (tilFnd && !openBrace)
										{
											resStr += "~";
											tilFnd = false;
										}
										if (i == curStr.Length - 1)
                                        {
											resStr += curStr[i];
                                        }
										else if (!endBrace)
										{
											tilFnd = true;
										}
										break;
									case '(':
										if (tilFnd && !openBrace && !endBrace)
										{
											openBrace = true;
											if (curStr.Length-1 > i)
                                            {
												startIndex = i + 1;
                                            }
										}
										else
                                        {
											resStr += curStr[i];
                                        }
										break;
									case ')':
										if (tilFnd && openBrace)
										{
											endBrace = true;
											endIndex = i - 1;
										}
										else
                                        {
											resStr += curStr[i];
										}
										break;
									default:
										if (tilFnd && openBrace)
                                        {
											break;
                                        }
										if (tilFnd && !openBrace)
										{
											resStr += "~";
											tilFnd = false;
										}
										resStr += curStr[i];

										break;
                                }

								if (tilFnd && openBrace && !endBrace && i == curStr.Length-1)
                                {
									isError = 2;
                                }

								if (tilFnd && openBrace && endBrace)
                                {
									string expresion = curStr.Substring(startIndex, endIndex - startIndex + 1);

									switch (expresion)
                                    {
										case "A-Z":
											resStr += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
											break;
										case "a-z":
											resStr += "abcdefghijklmnopqrstuvwxyz";
											break;
										case "А-Я":
											resStr += "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
											break;
										case "а-я":
											resStr += "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
											break;
										case "0-9":
											resStr += "0123456789";
											break;
										default:
											isError = 1;
											break;
                                    }

									tilFnd = false;
									openBrace = false;
									endBrace = false;
                                }
                            }

							if (isError == 0)
							{
								checkAnswerCharacteristic.ValidChars = resStr;
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							}

							if (checkedBefore)
							{
								if (isError >= 1)
								{
									checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
									if (checkAnswerCharacteristic.Criterion.Trim(' ') == "")
										checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Неверно составлено выражение диапазона символов в вопросе №" + checkAnswerCharacteristic.Number.ToString() + ".", checkData, 9));
									else
										checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Неверно составлено выражение диапазона символов в вопросе №" + checkAnswerCharacteristic.Number.ToString() + "-" + checkAnswerCharacteristic.Criterion + ".", checkData, 9));
								}
							}
							else
							{
								if (isError >= 1)
								{
									if (checkAnswerCharacteristic.Criterion.Trim(' ') == "")
										checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Неверно составлено выражение диапазона символов в вопросе №" + checkAnswerCharacteristic.Number.ToString() + ".", checkData, 9));
									else
										checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("Неверно составлено выражение диапазона символов в вопросе №" + checkAnswerCharacteristic.Number.ToString() + "-" + checkAnswerCharacteristic.Criterion + ".", checkData, 9));
								}
							}
						}
					}

					break;

				case 5: //Таблица бланков
					{
						Subject checkSubject = checkData[1] as Subject;
						Blank checkBlank = checkData[2] as Blank;

						int errorCount = 0;
						string errorFields = "";

						foreach (PostAlert currentAlert in checkSubject.Alerts[0].PostAlerts)
						{
							if (currentAlert.ProblemCode == 10 && ((Subject)currentAlert.ProblemData[1]).SubjectID == checkSubject.SubjectID &&
								((Blank)currentAlert.ProblemData[2]).BlankID == checkBlank.BlankID)
							{
								checkedBefore = true;
								checkedAlert = currentAlert;
							}
						}

						if (toDelete)
						{
							if (checkedBefore)
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							break;
						}

						if (checkBlank.Type == Workfield.BlankTypes[0][1])
						{
							errorFields += errorFields == "" ? "типа бланка" : ", типа бланка";
							errorCount++;
						}
						if (checkBlank.Path == "")
						{
							errorFields += errorFields == "" ? "пути к файлу бланка" : ", пути к файлу бланка";
							errorCount++;
						}

						if (checkedBefore)
						{
							if (errorCount <= 0)
							{
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
							}
							else
							{
								checkSubject.Alerts[0].PostAlerts.Remove(checkedAlert);
								checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("В настройках бланка не настроено(-ы) поле(-я) " + errorFields + ".", checkData, 10));
							}
						}
						else
						{
							if (errorCount > 0)
							{
								checkSubject.Alerts[0].PostAlerts.Add(new PostAlert("В настройках бланка не настроено(-ы) поле(-я) " + errorFields + ".", checkData, 10));
							}
						}
					}

					break;
			}
			return result;
        }

		static private bool CheckAlert_ScoreInconsequence(List<object> checkData, bool toDelete)
		{
			bool result = false;
			bool checkedBefore = false;
			bool noError = true;
			PostAlert checkedAlert = null;

			Subject checkSubject = checkData[0] as Subject;

			short currentScore = -1;

			foreach (PostAlert currentAlert in checkSubject.Alerts[1].PostAlerts)
			{
				if (currentAlert.ProblemCode == 4 && ((Subject)currentAlert.ProblemData[0]).SubjectID == checkSubject.SubjectID)
				{
					checkedBefore = true;
					checkedAlert = currentAlert;
				}
			}

			foreach (ScaleUnit currentScaleUnit in checkSubject.ScaleSystem)
            {
				if (checkedBefore)
                {
					if (currentScaleUnit.Mark < currentScore)
					{
						checkSubject.Alerts[1].PostAlerts.Remove(checkedAlert);
						checkSubject.Alerts[1].PostAlerts.Add(new PostAlert("Оценки в системе шкалирования расположены некорректно(не по неубыванию).", checkData, 4));

						noError = false;
					}
				}
				else
                {
					if (currentScaleUnit.Mark < currentScore)
					{
						checkSubject.Alerts[1].PostAlerts.Add(new PostAlert("Оценки в системе шкалирования расположены некорректно(не по неубыванию).", checkData, 4));

						noError = false;
					}
				}

				if (!noError)
                {
					checkedBefore = false;
					checkedAlert = null;
					break;
				}

				currentScore = currentScaleUnit.Mark;
            }

			if (noError)
            {
				checkSubject.Alerts[1].PostAlerts.Remove(checkedAlert);
			}

			noError = true;
			checkedBefore = false;
			checkedAlert = null;

			currentScore = -1;

			foreach (PostAlert currentAlert in checkSubject.Alerts[1].PostAlerts)
			{
				if (currentAlert.ProblemCode == 5 && ((Subject)currentAlert.ProblemData[0]).SubjectID == checkSubject.SubjectID)
				{
					checkedBefore = true;
					checkedAlert = currentAlert;
				}
			}

			foreach (ScaleUnit currentScaleUnit in checkSubject.ScaleSystem)
			{
				if (checkedBefore)
                {
					if (currentScaleUnit.SecondScore < currentScore)
					{
						checkSubject.Alerts[1].PostAlerts.Remove(checkedAlert);
						checkSubject.Alerts[1].PostAlerts.Add(new PostAlert("Итоговые баллы в системе шкалирования расположены некорректно(не по неубыванию).", checkData, 5));

						noError = false;
					}
				}
                else
                {
					if (currentScaleUnit.SecondScore < currentScore)
					{
						checkSubject.Alerts[1].PostAlerts.Add(new PostAlert("Итоговые баллы в системе шкалирования расположены некорректно(не по неубыванию).", checkData, 5));

						noError = false;
					}
				}

				if (!noError)
				{
					checkedBefore = false;
					checkedAlert = null;
					break;
				}

				currentScore = currentScaleUnit.SecondScore;
			}

			if (noError)
			{
				checkSubject.Alerts[1].PostAlerts.Remove(checkedAlert);
			}

			return result;
		}

		static private bool CheckAlert_NoReferenceResponce(List<object> checkData, bool toDelete)
		{
			bool result = false;
			bool checkedBefore = false;
			PostAlert checkedAlert = null;

			Subject checkSubject = checkData[0] as Subject;
			Variant checkVariant = checkData[1] as Variant;
			Question checkQuestion = checkData[2] as Question;
			bool isDeleting = (bool)checkData[3];

			foreach (PostAlert currentAlert in checkSubject.Alerts[2].PostAlerts)
			{
				if (currentAlert.ProblemCode == 6 && ((Subject)currentAlert.ProblemData[0]).SubjectID == checkSubject.SubjectID &&
					((Variant)currentAlert.ProblemData[1]).VariantID == checkVariant.VariantID &&
					((Question)currentAlert.ProblemData[2]).QuestionID == checkQuestion.QuestionID)
				{
					checkedBefore = true;
					checkedAlert = currentAlert;
				}
			}

			if (toDelete)
			{
				if (checkedBefore)
					checkSubject.Alerts[2].PostAlerts.Remove(checkedAlert);
				return result;
			}

			if (checkedBefore)
            {
				if (checkQuestion.Answers.Count <= 0 || (isDeleting && checkQuestion.Answers.Count <= 1))
				{
					checkSubject.Alerts[2].PostAlerts.Remove(checkedAlert);
					if (checkQuestion.Criterion.Trim(' ') == "")
						checkSubject.Alerts[2].PostAlerts.Add(new PostAlert("Отсутствуют эталонные ответы у вопроса №" + checkQuestion.InVariant.ToString() + " в варианте " + checkVariant.Name + ".", checkData, 6));
					else
						checkSubject.Alerts[2].PostAlerts.Add(new PostAlert("Отсутствуют эталонные ответы у вопроса №" + checkQuestion.InVariant.ToString() + "-" + checkQuestion.Criterion + " в варианте " + checkVariant.Name + ".", checkData, 6));
				}
				else
                {
					checkSubject.Alerts[2].PostAlerts.Remove(checkedAlert);
				}
			}
            else
            {
				if (checkQuestion.Answers.Count <= 0 || (isDeleting && checkQuestion.Answers.Count <= 1))
				{
					if (checkQuestion.Criterion.Trim(' ') == "")
						checkSubject.Alerts[2].PostAlerts.Add(new PostAlert("Отсутствуют эталонные ответы у вопроса №" + checkQuestion.InVariant.ToString() + " в варианте " + checkVariant.Name + ".", checkData, 6));
					else
						checkSubject.Alerts[2].PostAlerts.Add(new PostAlert("Отсутствуют эталонные ответы у вопроса №" + checkQuestion.InVariant.ToString() + "-" + checkQuestion.Criterion + " в варианте " + checkVariant.Name + ".", checkData, 6));
				}
			}

			return result;
		}

		static private bool CheckAlert_NotEnoughOrExcessScoreForQuestion(List<object> checkData, bool toDelete)
		{
			bool result = false;
			bool checkedBefore = false;
			PostAlert checkedAlert = null;

			Subject checkSubject = checkData[0] as Subject;
			Variant checkVariant = checkData[1] as Variant;
			Question checkQuestion = checkData[2] as Question;
			Answer checkAnswer = checkData[3] as Answer;

			foreach (PostAlert currentAlert in checkSubject.Alerts[3].PostAlerts)
			{
				if (currentAlert.ProblemCode == 7 && ((Subject)currentAlert.ProblemData[0]).SubjectID == checkSubject.SubjectID &&
					((Variant)currentAlert.ProblemData[1]).VariantID == checkVariant.VariantID &&
					((Question)currentAlert.ProblemData[2]).QuestionID == checkQuestion.QuestionID)
				{
					checkedBefore = true;
					checkedAlert = currentAlert;
				}
			}

			if (toDelete)
			{
				if (checkedBefore)
					checkSubject.Alerts[3].PostAlerts.Remove(checkedAlert);
				return result;
			}

			short maxScore = 0;

			foreach (Answer currentAnswer in checkQuestion.Answers)
            {
				if (checkAnswer != null && currentAnswer.AnswerID == checkAnswer.AnswerID) continue;
				if (currentAnswer.Score > maxScore) maxScore = currentAnswer.Score;
            }

			if (checkedBefore)
            {
				if (maxScore > checkQuestion.MaxScore)
				{
					checkSubject.Alerts[3].PostAlerts.Remove(checkedAlert);
					if (checkQuestion.Criterion.Trim(' ') == "")
						checkSubject.Alerts[3].PostAlerts.Add(new PostAlert("Избыток баллов с эталонных ответов на вопрос №" + checkQuestion.InVariant.ToString() + " в варианте " + checkVariant.Name + ".", checkData, 7));
					else
						checkSubject.Alerts[3].PostAlerts.Add(new PostAlert("Избыток баллов с эталонных ответов на вопрос №" + checkQuestion.InVariant.ToString() + "-" + checkQuestion.Criterion + " в варианте " + checkVariant.Name + ".", checkData, 7));
				}
				else
                {
					checkSubject.Alerts[3].PostAlerts.Remove(checkedAlert);
				}

			}
			else
            {
				if (maxScore > checkQuestion.MaxScore)
				{
					if (checkQuestion.Criterion.Trim(' ') == "")
						checkSubject.Alerts[3].PostAlerts.Add(new PostAlert("Избыток баллов с эталонных ответов на вопрос №" + checkQuestion.InVariant.ToString() + " в варианте " + checkVariant.Name + ".", checkData, 7));
					else
						checkSubject.Alerts[3].PostAlerts.Add(new PostAlert("Избыток баллов с эталонных ответов на вопрос №" + checkQuestion.InVariant.ToString() + "-" + checkQuestion.Criterion + " в варианте " + checkVariant.Name + ".", checkData, 7));
				}
			}
			

			return result;
		}

		static private bool CheckAlert_NoErrorScaleSystem(List<object> checkData, bool toDelete)
		{
			bool result = false;
			bool checkedBefore = false;
			PostAlert checkedAlert = null;

			Subject checkSubject = checkData[0] as Subject;
			AnswerCharacteristic checkAnswerCharacteristic = checkData[1] as AnswerCharacteristic;
			bool isDeleting = (bool)checkData[2];

			foreach (PostAlert currentAlert in checkSubject.Alerts[4].PostAlerts)
			{
				if (currentAlert.ProblemCode == 8 && ((Subject)currentAlert.ProblemData[0]).SubjectID == checkSubject.SubjectID &&
					((AnswerCharacteristic)currentAlert.ProblemData[1]).AnswerCharacteristicID == checkAnswerCharacteristic.AnswerCharacteristicID)
				{
					checkedBefore = true;
					checkedAlert = currentAlert;
				}
			}

			if (toDelete)
			{
				if (checkedBefore)
					checkSubject.Alerts[4].PostAlerts.Remove(checkedAlert);
				return result;
			}

			if (checkedBefore)
            {
				if ((checkAnswerCharacteristic.Errors.Count <= 0 && !isDeleting) || (checkAnswerCharacteristic.Errors.Count <= 1 && isDeleting))
				{
					checkSubject.Alerts[4].PostAlerts.Remove(checkedAlert);
					if (checkAnswerCharacteristic.Criterion.Trim(' ') == "")
						checkSubject.Alerts[4].PostAlerts.Add(new PostAlert("Отсутствует разбалловка по количеству ошибок шаблона вопроса №" + checkAnswerCharacteristic.Number.ToString() + ".", checkData, 8));
					else
						checkSubject.Alerts[4].PostAlerts.Add(new PostAlert("Отсутствует разбалловка по количеству ошибок шаблона вопроса №" + checkAnswerCharacteristic.Number.ToString() + "-" + checkAnswerCharacteristic.Criterion + ".", checkData, 8));
				}
				else
                {
					checkSubject.Alerts[4].PostAlerts.Remove(checkedAlert);
				}
			}
            else
            {
				if ((checkAnswerCharacteristic.Errors.Count <= 0 && !isDeleting) || (checkAnswerCharacteristic.Errors.Count <= 1 && isDeleting))
				{
					if (checkAnswerCharacteristic.Criterion.Trim(' ') == "")
						checkSubject.Alerts[4].PostAlerts.Add(new PostAlert("Отсутствует разбалловка по количеству ошибок шаблона вопроса №" + checkAnswerCharacteristic.Number.ToString() + ".", checkData, 8));
					else
						checkSubject.Alerts[4].PostAlerts.Add(new PostAlert("Отсутствует разбалловка по количеству ошибок шаблона вопроса №" + checkAnswerCharacteristic.Number.ToString() + "-" + checkAnswerCharacteristic.Criterion + ".", checkData, 8));
				}
			}

			return result;
		}
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------

	#endregion
	//========================================================================================================================================
}
