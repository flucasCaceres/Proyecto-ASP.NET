using System.ComponentModel.DataAnnotations;

namespace ProyectoMVC.Models.ViewModels
{
    public class RegisterVM
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string SecondName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}

}
