using MongoDB.Driver;
using Lab2.DAL.Settings;
using Lab2.DAL.Models;
using Microsoft.Extensions.Options;

namespace Lab2.Services;

public class StudentService
{
    private readonly IMongoCollection<Student> _studentsCollection;

    public StudentService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _studentsCollection = mongoDatabase.GetCollection<Student>("Students");
    }

    public async Task<List<Student>> GetAsync(CancellationToken cancellationToken = default) =>
        await _studentsCollection.Find(s => true).ToListAsync(cancellationToken);

    public async Task<Student?> GetByIdAsync(string id) =>
        await _studentsCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Student student) =>
        await _studentsCollection.InsertOneAsync(student);

    public async Task UpdateAsync(string id, Student updatedStudent) =>
        await _studentsCollection.ReplaceOneAsync(s => s.Id == id, updatedStudent);

    public async Task RemoveAsync(string id) =>
        await _studentsCollection.DeleteOneAsync(s => s.Id == id);
}

