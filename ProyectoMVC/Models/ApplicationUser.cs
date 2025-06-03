using Microsoft.AspNetCore.Identity;

namespace ProyectoMVC.Models
{
    public partial class ApplicationUser : IdentityUser
    {
        //campos personalizados
        public required string name { get; set; }
        public required string secondName { get; set; }
        public DateTime registerDate { get; set; }
        public DateTime lastLogin { get; set; }
        public bool active { get; set; }

    }
}