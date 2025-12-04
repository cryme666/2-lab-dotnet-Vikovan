using Lab2.DAL;
using Lab2.DAL.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Transactions;
using MongoStudent = Lab2.DAL.Models.Student;

namespace Lab2.Services;

public class MenuService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly StudentService _studentService;

    public MenuService(IServiceScopeFactory serviceScopeFactory, StudentService studentService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _studentService = studentService;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Distributed Database Management System ===");
            Console.WriteLine();
            Console.WriteLine("1. View students from SQL Server");
            Console.WriteLine("2. View students from MongoDB");
            Console.WriteLine("3. Compare data between databases");
            Console.WriteLine("4. Synchronize SQL Server -> MongoDB");
            Console.WriteLine("5. Create student in MongoDB");
            Console.WriteLine("6. Update student in MongoDB");
            Console.WriteLine("7. Delete student from MongoDB");
            Console.WriteLine("8. Get student by ID from MongoDB");
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("Select option: ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await ViewSqlServerStudentsAsync();
                        break;
                    case "2":
                        await ViewMongoDbStudentsAsync();
                        break;
                    case "3":
                        await CompareDatabasesAsync();
                        break;
                    case "4":
                        await SynchronizeAsync();
                        break;
                    case "5":
                        await CreateStudentInMongoDbAsync();
                        break;
                    case "6":
                        await UpdateStudentInMongoDbAsync();
                        break;
                    case "7":
                        await DeleteStudentFromMongoDbAsync();
                        break;
                    case "8":
                        await GetStudentByIdFromMongoDbAsync();
                        break;
                    case "0":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task ViewSqlServerStudentsAsync()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<LabDbContext>();

        var students = await dbContext.Students
            .Include(s => s.Department)
            .ToListAsync();

        Console.Clear();
        Console.WriteLine("=== Students from SQL Server ===");
        Console.WriteLine();

        if (!students.Any())
        {
            Console.WriteLine("No students found.");
        }
        else
        {
            Console.WriteLine($"Total: {students.Count} students");
            Console.WriteLine();
            foreach (var student in students)
            {
                Console.WriteLine($"ID: {student.StudentId}");
                Console.WriteLine($"Name: {student.FirstName} {student.LastName}");
                Console.WriteLine($"Email: {student.Email}");
                Console.WriteLine($"Date of Birth: {student.DateOfBirth:yyyy-MM-dd}");
                Console.WriteLine($"Enrollment Date: {student.EnrollmentDate:yyyy-MM-dd}");
                Console.WriteLine($"Department: {student.Department?.Name ?? "N/A"}");
                Console.WriteLine(new string('-', 50));
            }
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewMongoDbStudentsAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Students from MongoDB ===");
        Console.WriteLine();
        Console.WriteLine("Connecting to MongoDB...");

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var students = await _studentService.GetAsync(cts.Token);

            Console.Clear();
            Console.WriteLine("=== Students from MongoDB ===");
            Console.WriteLine();

            if (!students.Any())
            {
                Console.WriteLine("No students found.");
            }
            else
            {
                Console.WriteLine($"Total: {students.Count} students");
                Console.WriteLine();
                foreach (var student in students)
                {
                    Console.WriteLine($"ID: {student.Id}");
                    Console.WriteLine($"Name: {student.FirstName} {student.LastName}");
                    Console.WriteLine($"Email: {student.Email}");
                    Console.WriteLine($"Date of Birth: {student.DateOfBirth:yyyy-MM-dd}");
                    Console.WriteLine($"Enrollment Date: {student.EnrollmentDate:yyyy-MM-dd}");
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        catch (OperationCanceledException)
        {
            Console.Clear();
            Console.WriteLine("=== Students from MongoDB ===");
            Console.WriteLine();
            Console.WriteLine("Error: Connection timeout. MongoDB server might not be running.");
            Console.WriteLine("Please make sure MongoDB is running on localhost:27017");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        catch (MongoException ex)
        {
            Console.Clear();
            Console.WriteLine("=== Students from MongoDB ===");
            Console.WriteLine();
            Console.WriteLine($"MongoDB Error: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Possible causes:");
            Console.WriteLine("1. MongoDB server is not running");
            Console.WriteLine("2. MongoDB is not accessible on localhost:27017");
            Console.WriteLine("3. Connection string is incorrect");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine("=== Students from MongoDB ===");
            Console.WriteLine();
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            Console.WriteLine();
            Console.WriteLine("Possible causes:");
            Console.WriteLine("1. MongoDB server is not running");
            Console.WriteLine("2. MongoDB is not accessible on localhost:27017");
            Console.WriteLine("3. Connection string is incorrect");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    private async Task CompareDatabasesAsync()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<LabDbContext>();

        var sqlStudents = await dbContext.Students.ToListAsync();
        var mongoStudents = await _studentService.GetAsync();

        Console.Clear();
        Console.WriteLine("=== Database Comparison ===");
        Console.WriteLine();
        Console.WriteLine($"SQL Server students: {sqlStudents.Count}");
        Console.WriteLine($"MongoDB students: {mongoStudents.Count}");
        Console.WriteLine();

        var sqlEmails = sqlStudents.Select(s => s.Email).ToHashSet();
        var mongoEmails = mongoStudents.Select(s => s.Email).ToHashSet();

        var onlyInSql = sqlEmails.Except(mongoEmails).ToList();
        var onlyInMongo = mongoEmails.Except(sqlEmails).ToList();
        var inBoth = sqlEmails.Intersect(mongoEmails).ToList();

        Console.WriteLine($"Students in both databases: {inBoth.Count}");
        Console.WriteLine($"Students only in SQL Server: {onlyInSql.Count}");
        Console.WriteLine($"Students only in MongoDB: {onlyInMongo.Count}");
        Console.WriteLine();

        if (onlyInSql.Any())
        {
            Console.WriteLine("Students only in SQL Server:");
            foreach (var email in onlyInSql)
            {
                var student = sqlStudents.First(s => s.Email == email);
                Console.WriteLine($"  - {student.FirstName} {student.LastName} ({email})");
            }
            Console.WriteLine();
        }

        if (onlyInMongo.Any())
        {
            Console.WriteLine("Students only in MongoDB:");
            foreach (var email in onlyInMongo)
            {
                var student = mongoStudents.First(s => s.Email == email);
                Console.WriteLine($"  - {student.FirstName} {student.LastName} ({email})");
            }
            Console.WriteLine();
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task SynchronizeAsync()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<LabDbContext>();

        Console.Clear();
        Console.WriteLine("=== Synchronizing SQL Server -> MongoDB ===");
        Console.WriteLine();

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var students = await dbContext.Students.ToListAsync();
            int synced = 0;
            int skipped = 0;

            foreach (var student in students)
            {
                var mongoId = student.Email + student.DateOfBirth.ToLongDateString();
                var existing = await _studentService.GetByIdAsync(mongoId);

                if (existing == null)
                {
                    var mongoStudent = new MongoStudent
                    {
                        Id = mongoId,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        Email = student.Email,
                        DateOfBirth = student.DateOfBirth,
                        EnrollmentDate = student.EnrollmentDate
                    };

                    await _studentService.CreateAsync(mongoStudent);
                    synced++;
                }
                else
                {
                    skipped++;
                }
            }

            transaction.Complete();

            Console.WriteLine($"Synchronization completed!");
            Console.WriteLine($"  - New students synced: {synced}");
            Console.WriteLine($"  - Students already exist: {skipped}");
            Console.WriteLine($"  - Total processed: {students.Count}");
        }
        catch
        {
            transaction.Dispose();
            throw;
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task CreateStudentInMongoDbAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Create Student in MongoDB ===");
        Console.WriteLine();

        Console.Write("First Name: ");
        var firstName = Console.ReadLine() ?? string.Empty;

        Console.Write("Last Name: ");
        var lastName = Console.ReadLine() ?? string.Empty;

        Console.Write("Email: ");
        var email = Console.ReadLine() ?? string.Empty;

        Console.Write("Date of Birth (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out var dateOfBirth))
        {
            Console.WriteLine("Invalid date format.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enrollment Date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out var enrollmentDate))
        {
            Console.WriteLine("Invalid date format.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var student = new MongoStudent
        {
            Id = email + dateOfBirth.ToLongDateString(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = dateOfBirth,
            EnrollmentDate = enrollmentDate
        };

        await _studentService.CreateAsync(student);

        Console.WriteLine();
        Console.WriteLine("Student created successfully!");
        Console.WriteLine($"ID: {student.Id}");
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task UpdateStudentInMongoDbAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Update Student in MongoDB ===");
        Console.WriteLine();

        Console.Write("Student ID: ");
        var id = Console.ReadLine() ?? string.Empty;

        var existing = await _studentService.GetByIdAsync(id);
        if (existing == null)
        {
            Console.WriteLine("Student not found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Current: {existing.FirstName} {existing.LastName} ({existing.Email})");
        Console.WriteLine();

        Console.Write("First Name (press Enter to keep current): ");
        var firstName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(firstName))
            firstName = existing.FirstName;

        Console.Write("Last Name (press Enter to keep current): ");
        var lastName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(lastName))
            lastName = existing.LastName;

        Console.Write("Email (press Enter to keep current): ");
        var email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
            email = existing.Email;

        Console.Write("Date of Birth (yyyy-MM-dd, press Enter to keep current): ");
        var dobInput = Console.ReadLine();
        var dateOfBirth = existing.DateOfBirth;
        if (!string.IsNullOrWhiteSpace(dobInput) && DateTime.TryParse(dobInput, out var parsedDob))
            dateOfBirth = parsedDob;

        Console.Write("Enrollment Date (yyyy-MM-dd, press Enter to keep current): ");
        var enrollInput = Console.ReadLine();
        var enrollmentDate = existing.EnrollmentDate;
        if (!string.IsNullOrWhiteSpace(enrollInput) && DateTime.TryParse(enrollInput, out var parsedEnroll))
            enrollmentDate = parsedEnroll;

        var updatedStudent = new MongoStudent
        {
            Id = existing.Id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = dateOfBirth,
            EnrollmentDate = enrollmentDate
        };

        await _studentService.UpdateAsync(id, updatedStudent);

        Console.WriteLine();
        Console.WriteLine("Student updated successfully!");
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task DeleteStudentFromMongoDbAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Student from MongoDB ===");
        Console.WriteLine();

        Console.Write("Student ID: ");
        var id = Console.ReadLine() ?? string.Empty;

        var existing = await _studentService.GetByIdAsync(id);
        if (existing == null)
        {
            Console.WriteLine("Student not found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Student: {existing.FirstName} {existing.LastName} ({existing.Email})");
        Console.Write("Are you sure you want to delete this student? (y/n): ");
        var confirm = Console.ReadLine()?.ToLower();

        if (confirm == "y" || confirm == "yes")
        {
            await _studentService.RemoveAsync(id);
            Console.WriteLine("Student deleted successfully!");
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task GetStudentByIdFromMongoDbAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Get Student by ID from MongoDB ===");
        Console.WriteLine();

        Console.Write("Student ID: ");
        var id = Console.ReadLine() ?? string.Empty;

        var student = await _studentService.GetByIdAsync(id);

        if (student == null)
        {
            Console.WriteLine("Student not found.");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"ID: {student.Id}");
            Console.WriteLine($"Name: {student.FirstName} {student.LastName}");
            Console.WriteLine($"Email: {student.Email}");
            Console.WriteLine($"Date of Birth: {student.DateOfBirth:yyyy-MM-dd}");
            Console.WriteLine($"Enrollment Date: {student.EnrollmentDate:yyyy-MM-dd}");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}

