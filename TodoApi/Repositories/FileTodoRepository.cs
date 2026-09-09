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
                new Todo (1, "Do the Testtest", true),
                new Todo (2, "Do the CheckCheck", false)
            };
            SaveToFile();
        }

        _nextId = _todos.Count > 0 ? _todos.Max(t => t.Id) + 1 : 1;
    }

    public List<Todo> GetAll() => _todos;

    public Todo? GetById(int id) => _todos.FirstOrDefault(t => t.Id == id);

    public Todo Add(string title)
    {
        var todo = new Todo(_nextId++, title, false);
        _todos.Add(todo);
        SaveToFile();
        return todo;
    }

    public void SaveToFile()
    {
        var json = JsonSerializer.Serialize(_todos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}