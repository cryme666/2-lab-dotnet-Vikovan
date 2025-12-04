using Lab2.DAL;
using Lab2.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using MongoStudent = Lab2.DAL.Models.Student;

namespace Lab2.Services;

public class UpdateService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UpdateService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var studentService = scope.ServiceProvider.GetRequiredService<StudentService>();
            using (var dbContext = scope.ServiceProvider.GetRequiredService<LabDbContext>())
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var students = await dbContext.Students.ToListAsync(cancellationToken);

                    foreach (var student in students)
                    {
                        var mongoStudent = new MongoStudent
                        {
                            Id = student.Email + student.DateOfBirth.ToLongDateString(),
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            Email = student.Email,
                            DateOfBirth = student.DateOfBirth,
                            EnrollmentDate = student.EnrollmentDate
                        };

                        await studentService.CreateAsync(mongoStudent);
                    }

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    throw;
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

