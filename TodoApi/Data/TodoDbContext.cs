using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class TodoDbContext : DbContext
{
    public DbSet<Todo> Todos { get; set; }

    public TodoDbContext(DbContextOptions<TodoDbContext> options): base(options)
    {
            
    }


}

