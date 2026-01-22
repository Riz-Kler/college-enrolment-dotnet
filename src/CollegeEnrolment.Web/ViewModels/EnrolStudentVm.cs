using System.ComponentModel.DataAnnotations;

namespace CollegeEnrolment.Web.ViewModels;

public sealed class EnrolStudentVm
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseOfferingId { get; set; }
}
