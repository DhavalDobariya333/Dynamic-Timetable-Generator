namespace Dynamic_Time_Table_Generator.Models
{
    public class TimetableInputModel
    {
        public int WorkingDays { get; set; }
        public int SubjectsPerDay { get; set; }
        public int TotalSubjects { get; set; }
        public List<SubjectHours> Subjects { get; set; }

        public class SubjectHours
        {
            public string SubjectName { get; set; }
            public int Hours { get; set; }
        }

        public int TotalHours => WorkingDays * SubjectsPerDay;
    }

}
