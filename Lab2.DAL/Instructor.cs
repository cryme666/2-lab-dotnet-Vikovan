namespace Lab2.DAL;

public class Instructor
{
    public int InstructorId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
}

