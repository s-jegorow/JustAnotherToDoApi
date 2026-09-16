using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    Task<List<Todo>> GetAllAsync();

    Task<Todo?> GetByIdAsync(int id);

    Task<Todo> AddAsync(string title);

    Task<Todo?> UpdateAsync(int id, string title, bool isCompleted);

    Task<bool> DeleteAsync(int id);
}