using System;
using System.ComponentModel.DataAnnotations;

namespace UConv.Core.Db
{
    public class Record
    {
        [Key] public int id { get; set; }

        [Required] public DateTime date { get; set; }

        [Required] public string hostname { get; set; }

        [Required] [StringLength(20)] public string converter { get; set; }

        [Required] public double inputValue { get; set; }

        [Required] [StringLength(10)] public string inputUnit { get; set; }

        [Required] public double outputValue { get; set; }

        [Required] [StringLength(10)] public string outputUnit { get; set; }
    }
}