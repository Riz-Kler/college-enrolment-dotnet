namespace CollegeEnrolment.Data.Reports;

public sealed class CourseCapacityReportRow
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;

    public int Capacity { get; set; }
    public int ActiveEnrolments { get; set; }

    public decimal UtilisationPercent { get; set; }
}
