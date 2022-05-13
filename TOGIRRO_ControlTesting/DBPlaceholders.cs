using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

Ошибки:
Непоследовательность итоговых баллов
Непоследовательность оценок
Отсутствие эталонного ответа
Отсутствие балла за ответ
Отсутствие максимального балла за шаблон вопроса
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
	static class Workfield
	{
		static public ObservableCollection<Subject> Subjects = new ObservableCollection<Subject> { };
		static public ObservableCollection<Subject> ActualSubjects = new ObservableCollection<Subject> { };

		static public Subject CurrentSubject = null;
		static public Variant CurrentVariant = null;
		static public AnswerCharacteristic CurrentQuestion = null;
		static public ObservableCollection<Alert> CurrentAlerts = null;

		static public void Init()
		{
			Subjects.Add(new Subject()
			{
				Name = "Русский язык",
				Type = SubjectTypeEnum.Type1,
				Description = "Русский язык. 11 класс. 2019 г.",
				IsMark = true
			});
			Subjects.Add(new Subject()
			{
				Name = "История",
				Type = SubjectTypeEnum.Type1,
				Description = "История. 11 класс. 2019 г.",
				IsMark = true
			});
			Subjects.Add(new Subject()
			{
				Name = "Обществознание",
				Type = SubjectTypeEnum.Type2,
				Description = "Обществознание. 9 класс. 2020 г.",
				IsMark = false
			});

			Subjects[0].Questions.Add(new AnswerCharacteristic()
			{
				Number = 1,
				QuestionType = QuestionTypeEnum.Type1,
				MaxScore = 1
			});
			Subjects[0].Questions.Add(new AnswerCharacteristic()
			{
				Number = 2,
				QuestionType = QuestionTypeEnum.Type1,
				MaxScore = 1
			});

			Subjects[0].Alerts.Add(new Alert(AlertType.NoScoreForAnswer, "Описание ошибки"));
			Subjects[1].Alerts.Add(new Alert(AlertType.NotEnoughScoreForQuestion, "Описание ошибки 2"));

			ActualSubjects = Subjects;
		}

		public static string GetEnumDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
			{
				return attributes.First().Description;
			}

			return value.ToString();
		}

		static public IEnumerable<SubjectTypeEnum> SubjectTypeEnumValues => Enum.GetValues(typeof(SubjectTypeEnum)).Cast<SubjectTypeEnum>();
		static public IEnumerable<QuestionTypeEnum> QuestionTypeEnumValues => Enum.GetValues(typeof(QuestionTypeEnum)).Cast<QuestionTypeEnum>();
		static public IEnumerable<CheckTypeEnum> CheckTypeEnumValues => Enum.GetValues(typeof(CheckTypeEnum)).Cast<CheckTypeEnum>();
		static public IEnumerable<AlertType> AlertTypeValues => Enum.GetValues(typeof(AlertType)).Cast<AlertType>();
	}

	[TypeConverter(typeof(DescriptionConverter))]
	enum SubjectTypeEnum
	{
		[Description("НЕ ОБОЗНАЧЕНО")]
		UNDEFINED = 0,
		[Description("Тип 1")]
		Type1,
		[Description("Тип 2")]
		Type2,
		[Description("Тип 3")]
		Type3,
		[Description("Тип 4")]
		Type4
	}

	[TypeConverter(typeof(DescriptionConverter))]
	enum QuestionTypeEnum
	{
		[Description("НЕ ОБОЗНАЧЕНО")]
		UNDEFINED = 0,
		[Description("Тип 1")]
		Type1,
		[Description("Тип 2")]
		Type2,
		[Description("Тип 3")]
		Type3,
		[Description("Тип 4")]
		Type4
	}

	[TypeConverter(typeof(DescriptionConverter))]
	enum CheckTypeEnum
	{
		[Description("НЕ ОБОЗНАЧЕНО")]
		UNDEFINED = 0,
		[Description("Тип 1")]
		Type1,
		[Description("Тип 2")]
		Type2,
		[Description("Тип 3")]
		Type3,
		[Description("Тип 4")]
		Type4
	}

	enum AlertType
	{
		[Description("Поле не заполнено")]
		FieldNotFilled,
		[Description("Итоговые баллы заполнены не по возрастанию")]
		ScoreInconsequence,
		[Description("Оценки заполнены не по возрастанию")]
		MarkInconsequence,
		[Description("Отсутствует эталонный ответ")]
		NoReferenceResponce,
		[Description("Отсутствует балл за ответ")]
		NoScoreForAnswer,
		[Description("Отсутствует макс. балл за вопрос")]
		NoScoreForQuestion,
		[Description("Недостаточное кол-во баллов за вопрос")]
		NotEnoughScoreForQuestion,
		[Description("Избыточное кол-во баллов за вопрос")]
		ExcessScoreForQuestion
	}

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

	class Subject
	{
		public short SubjectCode { get; set; }
		public short EventCode { get; set; }
		public short MinScore { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public SubjectTypeEnum Type { get; set; }
		public string ProjectFolderPath { get; set; }
		public string RegistrationForm { get; set; }
		public string AnswersForm1 { get; set; }
		public string AnswersForm2 { get; set; }
		public string LogFile { get; set; }
		public bool IsMark { get; set; }
		public ObservableCollection<Alert> Alerts = null;
		public ObservableCollection<ScaleUnit> ScaleSystem { get; set; }

		public ObservableCollection<Variant> Variants = null;

		public ObservableCollection<AnswerCharacteristic> Questions = null;

		public Subject()
		{
			SubjectCode = 0; EventCode = 0;
			MinScore = 0;
			Name = ""; Description = "";
			ProjectFolderPath = ""; RegistrationForm = "";
			AnswersForm1 = ""; AnswersForm2 = "";
			LogFile = ""; IsMark = false;
			Type = SubjectTypeEnum.UNDEFINED;

			Questions = new ObservableCollection<AnswerCharacteristic> { };
			Variants = new ObservableCollection<Variant> { };
			ScaleSystem = new ObservableCollection<ScaleUnit>() { };
			Alerts = new ObservableCollection<Alert>() { };
		}
	}

	class Variant
	{
		public string Name { get; set; }
		public string VariantFilePath { get; set; }
		public List<Question> Answers = null;

		public Variant()
		{
			Name = ""; VariantFilePath = "";
			Answers = new List<Question> { };
		}
	}

	class AnswerCharacteristic
	{
		public short Number { get; set; }
		public string Criterion { get; set; }
		public string ValidChars { get; set; }
		public QuestionTypeEnum QuestionType { get; set; }
		public CheckTypeEnum CheckType { get; set; }
		public short MaxScore { get; set; }

		public AnswerCharacteristic()
		{
			Number = 0; Criterion = "";
			ValidChars = ""; MaxScore = 0;
			CheckType = CheckTypeEnum.UNDEFINED;
			QuestionType = QuestionTypeEnum.UNDEFINED;
		}
	}

	class Question
	{
		public short Number { get; set; }
		public short InVariant { get; set; }
		public short MaxScore { get; set; }
		public QuestionTypeEnum QuestionType { get; set; }
		public List<Answer> Answers { get; set; }

		public AnswerCharacteristic QuestionTemplate = null;

		public Question(AnswerCharacteristic questionTemplate)
		{
			Number = 0; InVariant = 0; MaxScore = 0;
			QuestionType = QuestionTypeEnum.UNDEFINED;
			Answers = new List<Answer>() { };
			QuestionTemplate = questionTemplate;
		}
	}

	class Answer
	{
		public string RightAnswer { get; set; }
		public short Score { get; set; }

		public Answer()
		{
			RightAnswer = ""; Score = 0;
		}
	}

	class Alert
    {
		public string Icon { get; set; }
		public string Header { get; set; }
		public AlertType alertType { get; set; }
		public string Description { get; set; }

		public Alert(AlertType alertType, string Description)
        {
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
				case AlertType.NoScoreForAnswer:
					Icon = "Icons/Delete.png";
					break;
				case AlertType.NoScoreForQuestion:
					Icon = "Icons/Ok.png";
					break;
				case AlertType.NotEnoughScoreForQuestion:
					Icon = "Icons/Cancel.png";
					break;
				case AlertType.ExcessScoreForQuestion:
					Icon = "Icons/ArrowL.png";
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
}
