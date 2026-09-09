using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    List<Todo> GetAll();

    Todo? GetById(int id);
    
    Todo Add(string title);
}