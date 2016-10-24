using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PEMinutes.ViewModels
{
    public class SubstituteViewModel
    {
        [Required]
        
        public string SubstituteName { get; set; }

        [Required]
        public string InstructionTime { get; set; }

        [Required]
        public int? Minutes { get; set; }

        [Required]
        public string Activity { get; set; }

        public string BadgeNumber { get; set; }



    }
}