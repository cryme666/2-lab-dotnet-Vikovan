namespace Lab2.DAL;

public class CourseAssignment
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; } = null!;
}

