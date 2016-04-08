using System.ComponentModel.DataAnnotations;

namespace RansomwareBlaster.Models
{
    internal class Trap
    {
        [Key]
        public string ID { get; set; }

        [Required]
        public string Directory { get; set; }

        [Required]
        public string FileName { get; set; }
    }
}
