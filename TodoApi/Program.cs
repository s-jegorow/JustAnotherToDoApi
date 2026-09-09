using TodoApi.Models;
using TodoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITodoRepository, FileTodoRepository>();

var app = builder.Build();

app.MapGet("/todos", (ITodoRepository repo) => Results.Ok(repo.GetAll()));

app.MapPost("/todos", (CreateTodoRequest request, ITodoRepository repo) => 
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Der Titel darf nicht leer sein.");
    }

    var created = repo.Add(request.Title);
    return Results.Created($"/todos/{created.Id}", created);
}
)


app.Run("http://localhost:8000");
