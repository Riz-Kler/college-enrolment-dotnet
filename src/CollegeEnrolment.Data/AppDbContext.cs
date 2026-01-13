using CollegeEnrolment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CollegeEnrolment.Data.Reports;


namespace CollegeEnrolment.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseOffering> CourseOfferings => Set<CourseOffering>();
    public DbSet<TimetableSlot> TimetableSlots => Set<TimetableSlot>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<CourseCapacityReportRow> CourseCapacityReport => Set<CourseCapacityReportRow>();


    protected override void OnModelCreating(ModelBuilder model)
    {
        // Table names kept explicit to make Raw SQL easier later
        model.Entity<Student>().ToTable("Students");
        model.Entity<Course>().ToTable("Courses");
        model.Entity<CourseOffering>().ToTable("CourseOfferings");
        model.Entity<TimetableSlot>().ToTable("TimetableSlots");
        model.Entity<Enrolment>().ToTable("Enrolments");
        model.Entity<AuditLog>().ToTable("AuditLogs");

        // Student
        model.Entity<Student>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.StudentNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(255).IsRequired();

            e.HasIndex(x => x.StudentNumber).IsUnique();
            e.HasIndex(x => x.Email);
        });

        // Course
        model.Entity<Course>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Level).HasMaxLength(20).IsRequired();

            e.HasIndex(x => x.Code).IsUnique();
        });

        // CourseOffering
        model.Entity<CourseOffering>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.AcademicYear).HasMaxLength(9).IsRequired(); // e.g. 2025/26
            e.Property(x => x.Capacity).IsRequired();

            e.HasOne(x => x.Course)
                .WithMany(x => x.Offerings)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.CourseId, x.AcademicYear }).IsUnique();
        });

        // TimetableSlot
        model.Entity<TimetableSlot>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Room).HasMaxLength(50).IsRequired();
            e.Property(x => x.DayOfWeek).IsRequired();
            e.Property(x => x.StartTime).IsRequired();
            e.Property(x => x.EndTime).IsRequired();

            e.HasOne(x => x.CourseOffering)
                .WithMany(x => x.Timetable)
                .HasForeignKey(x => x.CourseOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.CourseOfferingId);
        });

        // Enrolment
        model.Entity<Enrolment>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Status).IsRequired();
            e.Property(x => x.CreatedAtUtc).IsRequired();

            e.HasOne(x => x.Student)
                .WithMany(x => x.Enrolments)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.CourseOffering)
                .WithMany(x => x.Enrolments)
                .HasForeignKey(x => x.CourseOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate active enrolment records for same offering
            e.HasIndex(x => new { x.StudentId, x.CourseOfferingId }).IsUnique();
        });

        // AuditLog
        model.Entity<AuditLog>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.TimestampUtc).IsRequired();
            e.Property(x => x.Actor).HasMaxLength(100).IsRequired();
            e.Property(x => x.Action).HasMaxLength(50).IsRequired();
            e.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            e.Property(x => x.EntityId).HasMaxLength(50).IsRequired();
            e.Property(x => x.Details).HasMaxLength(2000).IsRequired();

            e.HasIndex(x => x.TimestampUtc);
            e.HasIndex(x => x.Actor);
        });

        model.Entity<CourseCapacityReportRow>(e =>
        {
            e.HasNoKey();
            e.ToView(null); // not mapped to a real table/view
        });

    }
}
