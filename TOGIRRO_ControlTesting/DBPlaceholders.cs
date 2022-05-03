using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
------Запилить вспомогательный класс для отображения в таблице ответов
------Реализовать шкалирование (Класс)
------Настроить таблицу шкалирования
Отслеживание ввода в таблице шкалирования
------Настроить таблицу ответов
Добавление изменение и удаление вариантов
Добавление и удаление вопросов
Поиск в предметах
Привязка данных текущего предмета
Макс балл и алерты в трее
*/

namespace TOGIRRO_ControlTesting
{
    static class Workfield
    {
        static public ObservableCollection<Subject> Subjects = new ObservableCollection<Subject> { };
		static public ObservableCollection<Question> Questions = new ObservableCollection<Question> { };

        static public Subject CurrentSubject = null;
        static public Variant CurrentVariant = null;
        static public Question CurrentQuestion = null;

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

            Subjects[0].Questions.Add(new Question(Subjects[0])
			{
				Number = 1,
				QuestionType = QuestionTypeEnum.Type1,
				MaxScore = 1
			});
            Subjects[0].Questions.Add(new Question(Subjects[0])
            {
                Number = 2,
				QuestionType = QuestionTypeEnum.Type1,
                MaxScore = 1
            });

            Questions = Subjects[0].Questions;
		}

		static public IEnumerable<SubjectTypeEnum> SubjectTypeEnumValues
		{
			get { return Enum.GetValues(typeof(SubjectTypeEnum)).Cast<SubjectTypeEnum>(); }
		}
		static public IEnumerable<QuestionTypeEnum> QuestionTypeEnumValues
		{
			get { return Enum.GetValues(typeof(QuestionTypeEnum)).Cast<QuestionTypeEnum>(); }
		}
		static public IEnumerable<CheckTypeEnum> CheckTypeEnumValues
		{
			get { return Enum.GetValues(typeof(CheckTypeEnum)).Cast<CheckTypeEnum>(); }
		}
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

	struct ScaleUnit
    {
		public short FirstScore { get; set; }
		public string Mark { get; set; }
		public short SecondScore { get; set; }

		public ScaleUnit(short FirstScore = 0, string Mark = "-", short SecondScore = 0)
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
		public ObservableCollection<ScaleUnit> ScaleSystem { get; set; }

        public ObservableCollection<Variant> Variants = null;

		public ObservableCollection<Question> Questions = null;

        public Subject()
        {
			SubjectCode = 0;		EventCode = 0;
			MinScore = 0;
			Name = "";				Description = "";
			ProjectFolderPath = "";	RegistrationForm = "";
			AnswersForm1 = "";		AnswersForm2 = "";
			LogFile = "";			IsMark = false;
			Type = SubjectTypeEnum.UNDEFINED;

			Questions = new ObservableCollection<Question> { };
			Variants = new ObservableCollection<Variant> { new Variant(this) };
			ScaleSystem = new ObservableCollection<ScaleUnit>() { };
			ScaleSystem.Add(new ScaleUnit(1, "2", 1));
			ScaleSystem.Add(new ScaleUnit(2, "2", 2));
			ScaleSystem.Add(new ScaleUnit(3, "3", 3));
			ScaleSystem.Add(new ScaleUnit(4, "4", 4));
			ScaleSystem.Add(new ScaleUnit(5, "5", 5));
		}
    }

    class Variant
    {
        public string Name { get; set; }
		public string VariantFilePath { get; set; }
        public Subject ParentSubject = null;
		public List<QuestionPost> Answers = null;

        public Variant(Subject parentSubject)
        {
			Name = "";	VariantFilePath = "";	
			ParentSubject = parentSubject;
			Answers = new List<QuestionPost> { new QuestionPost(this), new QuestionPost(this) { Number = 23} };
        }
    }

	class Question
	{
		public short Number { get; set; }
		public short Criterion { get; set; }
		public string ValidChars { get; set; }
		public QuestionTypeEnum QuestionType { get; set; }
		public CheckTypeEnum CheckType { get; set; }
		public short MaxScore { get; set; }

		public Subject ParentSubject = null;

		public Question(Subject parentSubject)
        {
			Number = 0;			Criterion = 0;
			ValidChars = "";	MaxScore = 0; 
			ParentSubject = parentSubject;
			CheckType = CheckTypeEnum.UNDEFINED;
			QuestionType = QuestionTypeEnum.UNDEFINED;
		}
    }

	class QuestionPost
    {
		public short Number { get; set; }
		public short InVariant { get; set; }
		public QuestionTypeEnum QuestionType { get; set; }
		public List<Answer> Answers { get; set; }

		public Variant ParentVariant = null;

		public QuestionPost(Variant parentVariant)
        {
			Number = 0;		InVariant = 0;
			QuestionType = QuestionTypeEnum.UNDEFINED;
			Answers = new List<Answer>() { };
			ParentVariant = parentVariant;

			Answers.Add(new Answer() { RightAnswer = "123", Score = 2 });
			Answers.Add(new Answer() { RightAnswer = "123123", Score = 4 });
			Answers.Add(new Answer() { RightAnswer = "2", Score = 3 });
		}
    }

	class Answer
	{
		public string RightAnswer { get; set; }
		public short Score { get; set; }

		public Answer()
		{
			RightAnswer = "";	Score = 0;
		}
	}
}
