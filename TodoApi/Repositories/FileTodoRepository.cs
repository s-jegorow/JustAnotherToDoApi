using System.Text.Json;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class FileTodoRepository : ITodoRepository
{
    private readonly string _filePath = "todos.json";
    private readonly List<Todo> _todos; 
    private int _nextId;

    public FileTodoRepository()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _todos = JsonSerializer.Deserialize<List<Todo>>(json) ?? new List<Todo>();
        }
        else
        {
            _todos = new List<Todo>
            {
                new Todo(1, "Do the Testtest", true),
                new Todo(2, "Do the CheckCheck", false)
            };
            SaveToFileAsync().GetAwaiter().GetResult();
        }

        _nextId = _todos.Count > 0 ? _todos.Max(t => t.Id) + 1 : 1;
    }

    public Task<List<Todo>> GetAllAsync() => Task.FromResult(_todos);

    public Task<Todo?> GetByIdAsync(int id) => Task.FromResult(_todos.FirstOrDefault(t => t.Id == id));

    public async Task<Todo> AddAsync(string title)
    {
        var todo = new Todo(_nextId++, title, false);
        _todos.Add(todo);
        await SaveToFileAsync();
        return todo;
    }

    public async Task<Todo?> UpdateAsync(int id, string title, bool isCompleted)
    {
        var existing = _todos.FirstOrDefault(t => t.Id == id);
        if (existing is null)
        {
            return null;
        }

        existing.Title = title;
        existing.IsCompleted = isCompleted;
        await SaveToFileAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = _todos.FirstOrDefault(t => t.Id == id);
        if (existing is null)
        {
            return false;
        }

        _todos.Remove(existing);
        await SaveToFileAsync();
        return true;
    }

    private async Task SaveToFileAsync()
    {
        var json = JsonSerializer.Serialize(_todos, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }
}