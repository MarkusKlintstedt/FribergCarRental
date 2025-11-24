using Microsoft.AspNetCore.Mvc;
using ToDoListAPI.Data;
using ToDoListAPI.Models;

namespace ToDoListAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoList : ControllerBase
    {
        TodoItemRepository TodoItemRepository { get; set; }
        public ToDoList(TodoItemRepository todoItemRepository)
        {
            TodoItemRepository = todoItemRepository;
        }

        [HttpGet]
        public ActionResult<List<ToDoItem>> Get()
        {
            return TodoItemRepository.Items;
        }

        [HttpPost]
        public void Post(string task)
        {
            TodoItemRepository.Add(task);
        }
    }
}
