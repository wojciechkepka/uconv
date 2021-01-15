using System;
using System.ComponentModel.DataAnnotations;

namespace UConv.Core
{
    public class Rating
    {
        [Key] public int id { get; set; }
        [Required] public DateTime date { get; set; }

        [Required] [StringLength(250)] public string name { get; set; }

        [Required] public int rating { get; set; }
    }
}