using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PEMinutes.ViewModels
{


    public class TeacherIndexViewModel
    {
        public string TeacherName { get; set; }
        public int? Minutes { get; set; }
        public string Date { get; set; }
        public double Percentage { get; set; }
        public List<MinuteCount> MinCount { get; set; }
        public TeacherIndexViewModel()
        {
            MinCount = new List<MinuteCount>();
        }
    }

    public class MinuteCount
    {
        public string Date { get; set; }
        public int? Minutes { get; set; }
        public int? ID { get; set; }
        public string Activity { get; set; }

    }

    public class TeacherCreateViewModel
    {
        [Required]
        public int? Minutes { get; set; }
        [Required]
        public DateTime InstructionTime { get; set; }

        public string Activity { get; set; }

    }

    public class TeacherEditViewModel
    {
        public int Id { get; set; }
        [Required]
        public int? Minutes { get; set; }
        [Required]
        public DateTime? InstructionTime { get; set; }
        [Required]
        public string Activity { get; set; }
        public string TeacherName { get; set; }
        public string SubstituteName { get; set; }
        public string ApprovedBy { get; set; }
        public int? BadgeNumber { get; set; }
        public string Grade { get; set; }
        public int? IsApproved { get; set; }
        public string School { get; set; }
        public DateTime? Timestamp { get; set; }
        public DateTime? ApprovedTime { get; set; }


    }
}