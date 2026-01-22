using System.ComponentModel.DataAnnotations;

namespace CollegeEnrolment.Web.ViewModels;

public sealed class StudentCreateVm
{
    [Required, StringLength(20)]
    public string StudentNumber { get; set; } = "";

    [Required, StringLength(100)]
    public string FirstName { get; set; } = "";

    [Required, StringLength(100)]
    public string LastName { get; set; } = "";

    [Required, EmailAddress, StringLength(255)]
    public string Email { get; set; } = "";
}
