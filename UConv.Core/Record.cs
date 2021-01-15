using System;
using System.ComponentModel.DataAnnotations;

namespace UConv.Core
{
    public class Record
    {
        [Key] public int id { get; set; }

        public DateTime date { get; set; }

        [Required] [StringLength(20)] public string converter { get; set; }

        public double inputValue { get; set; }

        [Required] [StringLength(10)] public string inputUnit { get; set; }

        public double outputValue { get; set; }

        [Required] [StringLength(10)] public string outputUnit { get; set; }
    }
}