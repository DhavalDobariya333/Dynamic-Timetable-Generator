using Dynamic_Time_Table_Generator.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dynamic_Time_Table_Generator.Controllers
{
    public class TimetableController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new TimetableInputModel());
        }

        [HttpPost]
        public IActionResult GenerateTimetable(TimetableInputModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            try
            {
                // Validate that the number of working days is within the allowed range
                if (model.WorkingDays < 1 || model.WorkingDays > 7)
                {
                    ModelState.AddModelError("", "Working days must be between 1 and 7.");
                    return View("Index", model);
                }

                // Validate that the number of subjects per day is within the allowed range
                if (model.SubjectsPerDay < 1 || model.SubjectsPerDay > 8)
                {
                    ModelState.AddModelError("", "Subjects per day must be between 1 and 8.");
                    return View("Index", model);
                }

                // Calculate total hours entered by summing all subject hours
                int totalHoursEntered = model.Subjects.GroupBy(s => s.SubjectName)
                                                     .Sum(g => g.Sum(s => s.Hours));
                int expectedTotalHours = model.WorkingDays * model.SubjectsPerDay;

                // Validate that the total entered hours match the expected total hours
                if (totalHoursEntered != expectedTotalHours)
                {
                    ModelState.AddModelError("", $"The total hours entered ({totalHoursEntered}) do not match the required total hours ({expectedTotalHours}).");
                    return View("Index", model);
                }

                // Generate the timetable and return the view
                var timetable = GenerateTable(model);
                return View("Timetable", timetable);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while generating the timetable. Please check your inputs.");
                return View("Index", model);
            }
        }

        private List<List<string>> GenerateTable(TimetableInputModel model)
        {
            List<List<string>> timetable = new List<List<string>>();
            Random random = new Random();

            // Create a dictionary to track total hours for each subject
            Dictionary<string, int> subjectHours = model.Subjects
                .GroupBy(s => s.SubjectName)
                .ToDictionary(g => g.Key, g => g.Sum(s => s.Hours));

            // Create a list of subjects, each appearing as many times as its allocated hours
            List<string> subjectPool = subjectHours.SelectMany(s => Enumerable.Repeat(s.Key, s.Value)).ToList();
            subjectPool = subjectPool.OrderBy(x => random.Next()).ToList(); // Shuffle subjects randomly

            for (int i = 0; i < model.WorkingDays; i++)
            {
                List<string> daySchedule = new List<string>();
                HashSet<string> usedSubjects = new HashSet<string>(); // Track subjects used in a day to avoid repetition

                for (int j = 0; j < model.SubjectsPerDay; j++)
                {
                    if (!subjectPool.Any()) break; // Stop if no subjects are left

                    string selectedSubject = subjectPool[random.Next(subjectPool.Count)];
                    int attempts = 0;

                    while (usedSubjects.Contains(selectedSubject) && attempts < 100)
                    {
                        selectedSubject = subjectPool[random.Next(subjectPool.Count)];
                        attempts++;
                    }

                    usedSubjects.Add(selectedSubject);
                    daySchedule.Add(selectedSubject);
                    subjectPool.Remove(selectedSubject);
                }

                timetable.Add(daySchedule);
            }

            return timetable;
        }
    }
}
