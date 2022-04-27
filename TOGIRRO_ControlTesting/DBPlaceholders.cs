using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOGIRRO_ControlTesting
{
    static class Workfield
    {
        static public ObservableCollection<Subject> Subjects = new ObservableCollection<Subject> { };
		static public ObservableCollection<Question> Questions = new ObservableCollection<Question> { };

        static public Subject CurrentSubject = null;
        static public Variant CurrentVariant = null;
        static public Question CurrentQuestion = null;

        static public string[] SubjectTypes = { "НЕ ОБОЗНАЧЕНО", "Тип 1", "Тип 2", "Тип 3", "Тип 4" };
        static public string[] QuestionTypes = { "НЕ ОБОЗНАЧЕНО", "Тип 1", "Тип 2", "Тип 3", "Тип 4" };

        static public void Init()
        {
            Subjects.Add(new Subject()
            {
                Name = "Русский язык",
                Type = SubjectTypes[(int)SubjectType.Type1],
                Description = "Русский язык. 11 класс. 2019 г."
            });
			Subjects.Add(new Subject()
			{
				Name = "История",
				Type = SubjectTypes[(int)SubjectType.Type1],
				Description = "История. 11 класс. 2019 г."
			});
			Subjects.Add(new Subject()
			{
				Name = "Обществознание",
				Type = SubjectTypes[(int)SubjectType.Type2],
				Description = "Обществознание. 9 класс. 2020 г."
			});

            Subjects[0].Variants[0].Questions.Add(new Question(Subjects[0].Variants[0])
			{
				Number = 1,
				Type = QuestionTypes[(int)QuestionType.Type1],
				RightAnswer = "2",
				MaxScore = 1
			});
            Subjects[0].Variants[0].Questions.Add(new Question(Subjects[0].Variants[0])
            {
                Number = 2,
                Type = QuestionTypes[(int)QuestionType.Type1],
                RightAnswer = "3",
                MaxScore = 1
            });

            Questions = Subjects[0].Variants[0].Questions;
		}
    }

    public enum SubjectType
    {
        UNDEFINED = 0,
        Type1,
        Type2,
        Type3,
        Type4
    }

    public enum QuestionType
    {
        UNDEFINED = 0,
        Type1,
        Type2,
        Type3,
        Type4
    }

    class Subject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public ObservableCollection<Variant> Variants = null;

        public Subject()
        {
            Variants = new ObservableCollection<Variant> { new Variant(this) };
        }
    }

    class Variant
    {
        public int Number { get; set; }
        public Subject ParentSubject = null;
        public ObservableCollection<Question> Questions = null;

        public Variant(Subject parentSubject)
        {
            ParentSubject = parentSubject;
			Questions = new ObservableCollection<Question> { };
        }
    }

    class Question
    {
		public int Number { get; set; }
        public string Type { get; set; }
		public string RightAnswer { get; set; }
        public int MaxScore { get; set; }

		public Variant ParentChapter = null;

		public Question(Variant parentVariant)
        {
            ParentChapter = parentVariant;
        }
    }
}
