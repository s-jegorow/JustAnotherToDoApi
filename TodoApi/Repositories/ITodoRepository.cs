using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    List<Todo> GetAll();

    Todo? GetById(int id);
    
    Todo Add(string title);

    Todo? Update(int id, string title, bool isCompleted);

    bool Delete(int id);
}