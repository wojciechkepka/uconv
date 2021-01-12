using System.ComponentModel.DataAnnotations;

namespace UConv.Core
{
    public class Rating
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(100)]
        public string name { get; set; }

        public int rating { get; set; }

    }
}