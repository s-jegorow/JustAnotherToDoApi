using System.Net.Http.Json;
using TodoClient.Models;

using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:5555");

Console.WriteLine("===== ToDo Client =====");

while (true)
{
    Console.WriteLine("\n1) Alle Tasks anzeigen");
    Console.WriteLine("2) Neuen Task hinzufügen");
    Console.WriteLine("3) Task bearbeiten / abhaken");
    Console.WriteLine("4) Task löschen");
    Console.WriteLine("5) Beenden");
    Console.Write("Auswahl: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await ShowTodos();
            break;
        case "2":
            await AddTodo();
            break;
        case "3":
            await UpdateTodo();
            break;
        case "4":
            await DeleteTodo();
            break;
        case "5":
            return;
        default:
            Console.WriteLine("Ungültige Auswahl.");
            break;
    }
}

async Task ShowTodos()
{
    var todos = await httpClient.GetFromJsonAsync<List<Todo>>("/todos");

    if (todos == null || todos.Count == 0)
    {
        Console.WriteLine("Keine Tasks vorhanden.");
        return;
    }

    foreach (Todo todo in todos)
    {
        var status = todo.IsCompleted ? "[X]" : "[ ]";
        Console.WriteLine($"{status} {todo.Id}: {todo.Title}");
    }
}

async Task AddTodo()
{
    Console.Write("Titel: ");
    var title = Console.ReadLine() ?? "";

    var request = new CreateTodoRequest(title);
    var response = await httpClient.PostAsJsonAsync("/todos", request);

    if (response.IsSuccessStatusCode)
    {
        var created = await response.Content.ReadFromJsonAsync<Todo>();
        Console.WriteLine($"Task hinzugefügt: {created?.Id} - '{created?.Title}'");
    }
    else
    {
        Console.WriteLine($"Fehler: {response.StatusCode}");
    }
}

async Task UpdateTodo()
{
    Console.Write("Id des Tasks: ");
    var idInput = Console.ReadLine();

    if (!int.TryParse(idInput, out var id))
    {
        Console.WriteLine("Ungültige Id.");
        return;
    }

    Console.Write("Neuer Titel: ");
    var title = Console.ReadLine() ?? "";

    Console.Write("Erledigt? (j/n): ");
    var isCompleted = Console.ReadLine()?.Trim().ToLower() == "j";

    var request = new UpdateTodoRequest(title, isCompleted);
    var response = await httpClient.PutAsJsonAsync($"/todos/{id}", request);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Task aktualisiert.");
    }
    else
    {
        Console.WriteLine($"Fehler: {response.StatusCode}");
    }
}

async Task DeleteTodo()
{
    Console.Write("Id des Tasks: ");
    var idInput = Console.ReadLine();

    if (!int.TryParse(idInput, out var id))
    {
        Console.WriteLine("Ungültige Id.");
        return;
    }

    var response = await httpClient.DeleteAsync($"/todos/{id}");

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Task gelöscht.");
    }
    else
    {
        Console.WriteLine($"Fehler: {response.StatusCode}");
    }
}
