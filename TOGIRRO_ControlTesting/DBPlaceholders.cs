using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOGIRRO_ControlTesting
{
    static class Workfield
    {
        static public ObservableCollection<Subject> Subjects = new ObservableCollection<Subject> { };
        static public ObservableCollection<Subject> ActiveSubjects = new ObservableCollection<Subject> { };
		static public ObservableCollection<Quastion> Quastions = new ObservableCollection<Quastion> { };
        static public WorkWindow workWindow = null;

        static public void Init()
        {
            Subjects.Add(new Subject()
            {
                Name = "Русский язык",
                Type = "ЕГЭ",
                ChangeDate = DateTime.Now,
                CreateDate = DateTime.Now,
                Description = "Русский язык. 11 класс. 2019 г."
            });
			Subjects.Add(new Subject()
			{
				Name = "История",
				Type = "ЕГЭ",
				ChangeDate = DateTime.Now,
				CreateDate = DateTime.Now,
				Description = "История. 11 класс. 2019 г."
			});
			Subjects.Add(new Subject()
			{
				Name = "Обществознание",
				Type = "ОГЭ",
				ChangeDate = DateTime.Now,
				CreateDate = DateTime.Now,
				Description = "Обществознание. 9 класс. 2020 г."
			});

			Quastions.Add(new Quastion()
			{
				N1 = "1",
				N2 = "Одиночный выбор ответа",
				N3 = "1",
				N4 = "2"
			});
			Quastions.Add(new Quastion()
			{
				N1 = "2",
				N2 = "Одиночный выбор ответа",
				N3 = "1",
				N4 = "1"
			});
			Quastions.Add(new Quastion()
			{
				N1 = "3",
				N2 = "Одиночный выбор ответа",
				N3 = "1",
				N4 = "1"
			});
			Quastions.Add(new Quastion()
			{
				N1 = "4",
				N2 = "Одиночный выбор ответа",
				N3 = "1",
				N4 = "3"
			});
			Quastions.Add(new Quastion()
			{
				N1 = "5",
				N2 = "Одиночный выбор ответа",
				N3 = "1",
				N4 = "4"
			});
			Quastions.Add(new Quastion()
			{
				N1 = "6",
				N2 = "Одиночный выбор ответа",
				N3 = "1",
				N4 = "2"
			});
			Quastions.Add(new Quastion()
			{
				N1 = "7",
				N2 = "Одиночный выбор ответа",
				N3 = "1",
				N4 = "3"
			});
		}
    }

    enum SubjectType
    {
        Type1 = 1,
        Type2,
        Type3,
        Type4
    }

    enum QuestionType
    {
        Type1 = 1,
        Type2,
        Type3,
        Type4
    }

	class Quastion
	{
		public string N1 { get; set; }
		public string N2 { get; set; }
		public string N3 { get; set; }
		public string N4 { get; set; }
	}

    class Subject
    {
        public string Name { get; set; }//"SubjectName";
        public DateTime CreateDate { get; set; }//= DateTime.Now;
        public DateTime ChangeDate { get; set; }//= DateTime.Now;
        public string Description { get; set; }//= "SubjectDescription";
        public string Type { get; set; }//= SubjectType.Type1;
        public ObservableCollection<Variant> Variants = null;

        public Subject()
        {
            Variants = new ObservableCollection<Variant> { new Variant(this) };
        }
    }

    class Variant
    {
        public string Name { get; set; }
        public Subject ParentSubject = null;
        public ObservableCollection<Chapter> Chapters = null;

        public Variant(Subject parentSubject)
        {
            ParentSubject = parentSubject;
            Name = "Variant 1";
            Chapters = new ObservableCollection<Chapter> { new Chapter(this) };
        }
    }

    class Chapter
    {
        public string Name { get; set; }
        public Variant ParentVariant = null;
        public ObservableCollection<Question> Questions = null;

        public Chapter(Variant parentVariant)
        {
            ParentVariant = parentVariant;
            Name = "Chapter 1";
            Questions = new ObservableCollection<Question> { new Question(this) };
        }
    }

    class Question
    {
        public string Name { get; set; }
        public QuestionType Type = QuestionType.Type1;
        public Chapter ParentChapter = null;
        public int MaxScore = 1;

        public Question(Chapter parentChapter)
        {
            Name = "Question 1";
            ParentChapter = parentChapter;
        }
    }
}
