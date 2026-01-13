using System;

namespace CollegeEnrolment.Domain.Entities;

public sealed class AuditLog
{
    public int Id { get; set; }

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    // Typically staff username or system process
    public string Actor { get; set; } = "system";

    // Examples: ENROL, WITHDRAW, SWITCH
    public string Action { get; set; } = string.Empty;

    // Examples: Student, Enrolment, CourseOffering
    public string EntityType { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;
}
