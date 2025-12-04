namespace Lab2.DAL;

public class Department
{
    public int DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
}

