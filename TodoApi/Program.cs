using TodoApi.Models;
using TodoApi.Repositories;
using TodoApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoRepository, SqliteTodoRepository>();
builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlite("Data Source=todos.db"));

var app = builder.Build();

app.MapGet("/todos", (ITodoRepository repo) => Results.Ok(repo.GetAll()));

app.MapPost("/todos", (CreateTodoRequest request, ITodoRepository repo) => 
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title shouldn't be empty.");
    }

    var created = repo.Add(request.Title);
    return Results.Created($"/todos/{created.Id}", created);
}
);
app.Run("http://localhost:5555");
