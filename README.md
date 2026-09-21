# JustAnotherToDoApi

A small learning project to explore .NET 8 Minimal APIs and Entity Framework Core with SQLite (and optional file storage).

## Features
- ASP.NET Core Minimal API (.NET 8)
- Entity Framework Core with SQLite migrations (or file without EF, which was the base case)
- Swagger 
- Very basic console client for managing tasks
## API Endpoints
- `GET /todos` - Get all tasks
- `GET /todos/{id}` - Get a task by ID
- `POST /todos` - Create a new task
- `PUT /todos/{id}` - Update a task (title, status)
- `DELETE /todos/{id}` - Delete a task

## Getting Started

1. Start the API:
   ```bash
   dotnet run --project TodoApi/TodoApi.csproj
   ```
   The API runs at `http://localhost:5555` (Swagger: `http://localhost:5555/swagger`).

2. Start the Client (in a separate terminal):
   ```bash
   dotnet run --project TodoClient/TodoClient.csproj
   ```

## Next Steps / Todo
- Modern CLI UI with Spectre.Console
- Global error handling & validation
- Probably also todo features (due dates, priorities, categories)