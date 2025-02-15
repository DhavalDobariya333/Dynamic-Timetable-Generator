using System.ComponentModel.DataAnnotations;

namespace Dynamic_Time_Table_Generator.Models
{
    public class TimetableInputModel
    {
        [Range(1, 7, ErrorMessage = "Working days must be between 1 and 7.")]
        public int WorkingDays { get; set; }

        [Range(1, 8, ErrorMessage = "Subjects per day must be between 1 and 8.")]
        public int SubjectsPerDay { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Total subjects must be at least 1.")]
        public int TotalSubjects { get; set; }

        public List<SubjectHours> Subjects { get; set; } = new();

        public int TotalHours => WorkingDays * SubjectsPerDay;
    }

    public class SubjectHours
    {
        [Required(ErrorMessage = "Subject name is required.")]
        public string SubjectName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Hours must be at least 1.")]
        public int Hours { get; set; }
    }
}
