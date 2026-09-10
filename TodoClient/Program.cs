using System.Net.Http.Json;
using TodoClient.Models;


using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:5555");

Console.WriteLine("===== Starting Up ======");

// GET
Console.WriteLine("\n----- Getting current Tasks -----");
var todos = await httpClient.GetFromJsonAsync<List<Todo>>("/todos");

if (todos != null)
{
    foreach (Todo todo in todos)
    {
            var status = todo.IsCompleted ? "[X]" : "[ ]";
            Console.WriteLine($"{status} {todo.Id}: {todo.Title}");
    }
}

// POST
Console.WriteLine("\n----- Adding new Task -----");
var newTodoRequest = new CreateTodoRequest("Finish the console client thing");

var response = await httpClient.PostAsJsonAsync("/todos", newTodoRequest);

if (response.IsSuccessStatusCode)
{
    var createdTodo = await response.Content.ReadFromJsonAsync<Todo>();
    Console.WriteLine($"Task successfully added: Id {createdTodo?.Id} - '{createdTodo?.Title}'");
}
else
{
    Console.WriteLine($"Error: {response.StatusCode}");
}

//GET after POST
Console.WriteLine("\n----- Tasks after adding -----");
var updatedTodos = await httpClient.GetFromJsonAsync<List<Todo>>("/todos");

if (updatedTodos != null)
{
    foreach (Todo todo in updatedTodos)
    {
        var status = todo.IsCompleted ? "[X]" : "[ ]";
        Console.WriteLine($"{status} {todo.Id}: {todo.Title}");
    }
}