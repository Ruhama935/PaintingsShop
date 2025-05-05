using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }

    }
}
