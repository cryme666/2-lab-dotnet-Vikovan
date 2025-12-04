namespace Lab2.DAL;

public static class DbInitializer
{
    public static void Initialize(LabDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Students.Any())
        {
            return;
        }

        var departments = new Department[]
        {
            new Department { Name = "Computer Science", Budget = 500000 },
            new Department { Name = "Mathematics", Budget = 350000 },
            new Department { Name = "Physics", Budget = 400000 }
        };

        foreach (var department in departments)
        {
            context.Departments.Add(department);
        }
        context.SaveChanges();

        var instructors = new Instructor[]
        {
            new Instructor { FirstName = "Petro", LastName = "Ivanov", HireDate = new DateTime(2015, 3, 15), DepartmentId = departments[0].DepartmentId },
            new Instructor { FirstName = "Anna", LastName = "Petrova", HireDate = new DateTime(2018, 9, 1), DepartmentId = departments[1].DepartmentId },
            new Instructor { FirstName = "Mykola", LastName = "Sydorov", HireDate = new DateTime(2020, 1, 10), DepartmentId = departments[2].DepartmentId }
        };

        foreach (var instructor in instructors)
        {
            context.Instructors.Add(instructor);
        }
        context.SaveChanges();

        var students = new Student[]
        {
            new Student { FirstName = "Valentyn", LastName = "Vikovan", Email = "valentyn.vikovan@university.edu", DateOfBirth = new DateTime(2000, 5, 15), EnrollmentDate = new DateTime(2020, 9, 1), DepartmentId = departments[0].DepartmentId },
            new Student { FirstName = "Anastasia", LastName = "Rarenko", Email = "anastasia.rarenko@university.edu", DateOfBirth = new DateTime(2001, 8, 22), EnrollmentDate = new DateTime(2020, 9, 1), DepartmentId = departments[1].DepartmentId },
            new Student { FirstName = "Oleksandr", LastName = "Kovalenko", Email = "oleksandr.kovalenko@university.edu", DateOfBirth = new DateTime(1999, 3, 10), EnrollmentDate = new DateTime(2019, 9, 1), DepartmentId = departments[0].DepartmentId },
            new Student { FirstName = "Olha", LastName = "Melnyk", Email = "olha.melnyk@university.edu", DateOfBirth = new DateTime(2000, 11, 5), EnrollmentDate = new DateTime(2020, 9, 1), DepartmentId = departments[2].DepartmentId },
            new Student { FirstName = "Dmytro", LastName = "Shevchenko", Email = "dmytro.shevchenko@university.edu", DateOfBirth = new DateTime(2001, 2, 18), EnrollmentDate = new DateTime(2021, 9, 1), DepartmentId = departments[0].DepartmentId }
        };

        foreach (var student in students)
        {
            context.Students.Add(student);
        }
        context.SaveChanges();

        var courses = new Course[]
        {
            new Course { Title = "Mathematics", Description = "Advanced mathematics course covering calculus and algebra", Credits = 5, DepartmentId = departments[1].DepartmentId },
            new Course { Title = "Physics", Description = "Fundamental physics principles and mechanics", Credits = 4, DepartmentId = departments[2].DepartmentId },
            new Course { Title = "Chemistry", Description = "Introduction to chemical reactions and compounds", Credits = 3, DepartmentId = departments[2].DepartmentId },
            new Course { Title = "Programming", Description = "Object-oriented programming and software development", Credits = 6, DepartmentId = departments[0].DepartmentId },
            new Course { Title = "Database Systems", Description = "Database design, SQL, and data management", Credits = 4, DepartmentId = departments[0].DepartmentId }
        };

        foreach (var course in courses)
        {
            context.Courses.Add(course);
        }
        context.SaveChanges();

        var courseAssignments = new CourseAssignment[]
        {
            new CourseAssignment { CourseId = courses[0].CourseId, InstructorId = instructors[1].InstructorId },
            new CourseAssignment { CourseId = courses[1].CourseId, InstructorId = instructors[2].InstructorId },
            new CourseAssignment { CourseId = courses[2].CourseId, InstructorId = instructors[2].InstructorId },
            new CourseAssignment { CourseId = courses[3].CourseId, InstructorId = instructors[0].InstructorId },
            new CourseAssignment { CourseId = courses[4].CourseId, InstructorId = instructors[0].InstructorId }
        };

        foreach (var ca in courseAssignments)
        {
            context.CourseAssignments.Add(ca);
        }
        context.SaveChanges();

        var studentCourses = new StudentCourse[]
        {
            new StudentCourse { StudentId = students[0].StudentId, CourseId = courses[0].CourseId, Grade = 85 },
            new StudentCourse { StudentId = students[0].StudentId, CourseId = courses[3].CourseId, Grade = 92 },
            new StudentCourse { StudentId = students[1].StudentId, CourseId = courses[1].CourseId, Grade = 88 },
            new StudentCourse { StudentId = students[1].StudentId, CourseId = courses[2].CourseId, Grade = 90 },
            new StudentCourse { StudentId = students[2].StudentId, CourseId = courses[3].CourseId, Grade = 95 },
            new StudentCourse { StudentId = students[2].StudentId, CourseId = courses[4].CourseId, Grade = 87 },
            new StudentCourse { StudentId = students[3].StudentId, CourseId = courses[0].CourseId, Grade = 83 },
            new StudentCourse { StudentId = students[3].StudentId, CourseId = courses[4].CourseId, Grade = 91 },
            new StudentCourse { StudentId = students[4].StudentId, CourseId = courses[1].CourseId, Grade = 89 },
            new StudentCourse { StudentId = students[4].StudentId, CourseId = courses[3].CourseId, Grade = 94 }
        };

        foreach (var sc in studentCourses)
        {
            context.StudentCourses.Add(sc);
        }
        context.SaveChanges();
    }
}

