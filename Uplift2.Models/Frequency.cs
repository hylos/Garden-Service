using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Uplift2.Models
{
    public class Frequency
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Description")]
        public int FrequencyCount { get; set; }
    }
}
