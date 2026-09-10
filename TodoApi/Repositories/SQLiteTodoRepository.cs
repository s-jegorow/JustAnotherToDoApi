using TodoApi.Models;
using TodoApi.Data;

namespace TodoApi.Repositories;

public class SqliteTodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;

    public SqliteTodoRepository(TodoDbContext context)
{
    _context = context;
}

public List<Todo> GetAll() => _context.Todos.ToList();

public Todo? GetById(int id) => _context.Todos.FirstOrDefault(t => t.Id == id);

public Todo Add(string title)
{
    Todo newtodo = new Todo(0, title, false); //id egal -> EF auto
    _context.Todos.Add(newtodo);
    _context.SaveChanges();

    return newtodo;
}

}