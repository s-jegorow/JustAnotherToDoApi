using System.Net.Http.Json;
using Spectre.Console;
using TodoClient.Models;

using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:5555");

while (true)
{
    AnsiConsole.Clear();
    AnsiConsole.Write(
        new FigletText("Todo Client")
            .LeftJustified()
            .Color(Color.Cyan1));

    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[yellow]Hey Pal, what would you like to do?[/]")
            .PageSize(6)
            .AddChoices(new[]
            {
                "View all tasks",
                "Add new task",
                "Update / toggle task",
                "Delete task",
                "Exit"
            }));

    try
    {
        switch (choice[0])
        {
            case '1':
                await ShowTodos();
                break;
            case '2':
                await AddTodo();
                break;
            case '3':
                await UpdateTodo();
                break;
            case '4':
                await DeleteTodo();
                break;
            case '5':
                return;
        }
    }
    catch (HttpRequestException)
    {
        AnsiConsole.MarkupLine("\n[red]Error: Cannot reach server. Is the API running or maybe the wrong port or so?[/]");
        PressAnyKey();
    }
}

void RenderTodosTable(List<Todo> todos)
{
    var table = new Table();
    table.Border(TableBorder.Rounded);
    table.AddColumn(new TableColumn("[bold]ID[/]").Centered());
    table.AddColumn(new TableColumn("[bold]Status[/]").Centered());
    table.AddColumn(new TableColumn("[bold]Title[/]"));

    foreach (var todo in todos)
    {
        var status = todo.IsCompleted ? "[green]✓ Done[/]" : "[yellow]○ Pending[/]";
        var title = todo.IsCompleted ? $"[grey]{Markup.Escape(todo.Title)}[/]" : Markup.Escape(todo.Title);
        table.AddRow(todo.Id.ToString(), status, title);
    }

    AnsiConsole.Write(table);
}

void PressAnyKey()
{
    AnsiConsole.Markup("\n[grey]Press any key to continue..[/]");
    Console.ReadKey(true);
}

async Task<List<Todo>?> FetchTodos()
{
    var todos = await httpClient.GetFromJsonAsync<List<Todo>>("/todos");
    if (todos == null || todos.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]Well, no tasks found.[/]");
        return null;
    }
    return todos;
}

async Task ShowTodos()
{
    var todos = await FetchTodos();
    if (todos != null)
    {
        RenderTodosTable(todos);
    }
    PressAnyKey();
}

async Task AddTodo()
{
    var title = AnsiConsole.Ask<string>("[green]Task title:[/] ");

    var request = new CreateTodoRequest(title);
    var response = await httpClient.PostAsJsonAsync("/todos", request);

    if (response.IsSuccessStatusCode)
    {
        var created = await response.Content.ReadFromJsonAsync<Todo>();
        AnsiConsole.MarkupLine($"[green]✓ Added task #{created?.Id}:[/] '{Markup.Escape(created?.Title ?? "")}'");
    }
    else
    {
        AnsiConsole.MarkupLine($"[red]Error:[/] {response.StatusCode}");
    }
    PressAnyKey();
}

async Task UpdateTodo()
{
    var todos = await FetchTodos();
    if (todos == null)
    {
        PressAnyKey();
        return;
    }

    RenderTodosTable(todos);

    var id = AnsiConsole.Ask<int>("Task ID: ");
    var title = AnsiConsole.Ask<string>("New title: ");
    var isCompleted = AnsiConsole.Confirm("Mark as done?");

    var request = new UpdateTodoRequest(title, isCompleted);
    var response = await httpClient.PutAsJsonAsync($"/todos/{id}", request);

    if (response.IsSuccessStatusCode)
    {
        AnsiConsole.MarkupLine("[green]✓ Task updated.[/]");
    }
    else
    {
        AnsiConsole.MarkupLine($"[red]Error:[/] {response.StatusCode}");
    }
    PressAnyKey();
}

async Task DeleteTodo()
{
    var todos = await FetchTodos();
    if (todos == null)
    {
        PressAnyKey();
        return;
    }

    RenderTodosTable(todos);

    var id = AnsiConsole.Ask<int>("Task ID: ");
    var response = await httpClient.DeleteAsync($"/todos/{id}");

    if (response.IsSuccessStatusCode)
    {
        AnsiConsole.MarkupLine("[green]✓ Task deleted.[/]");
    }
    else
    {
        AnsiConsole.MarkupLine($"[red]Error:[/] {response.StatusCode}");
    }
    PressAnyKey();
}
