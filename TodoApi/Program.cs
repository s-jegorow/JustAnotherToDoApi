using TodoApi.Models;
using TodoApi.Repositories;
using TodoApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//singleton -> scoped geändert (lifetime-konflikt im vergleich zum alten file-handling)
builder.Services.AddScoped<ITodoRepository, SqliteTodoRepository>();
builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlite("Data Source=todos.db"));
builder.Services.AddEndpointsApiExplorer(); //swagger
builder.Services.AddSwaggerGen(); //swagger

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

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

app.MapPut("/todos/{id}", (int id, UpdateTodoRequest request, ITodoRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title shouldn't be empty.");
    }

    var updated = repo.Update(id, request.Title, request.IsCompleted);
    if (updated is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(updated);
});

app.MapDelete("/todos/{id}", (int id, ITodoRepository repo) =>
{
    var deleted = repo.Delete(id);
    if (!deleted)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});

app.Run("http://localhost:5555");
