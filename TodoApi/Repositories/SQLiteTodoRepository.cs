using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class SqliteTodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;

    public SqliteTodoRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Todo>> GetAllAsync() => await _context.Todos.ToListAsync();

    public async Task<Todo?> GetByIdAsync(int id) => await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Todo> AddAsync(string title)
    {
        Todo newtodo = new Todo(0, title, false);
        _context.Todos.Add(newtodo);
        await _context.SaveChangesAsync();

        return newtodo;
    }

    public async Task<Todo?> UpdateAsync(int id, string title, bool isCompleted)
    {
        var existing = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);
        if (existing is null)
        {
            return null;
        }

        existing.Title = title;
        existing.IsCompleted = isCompleted;
        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);
        if (existing is null)
        {
            return false;
        }

        _context.Todos.Remove(existing);
        await _context.SaveChangesAsync();

        return true;
    }
}