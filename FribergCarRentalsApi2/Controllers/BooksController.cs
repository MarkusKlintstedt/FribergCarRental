using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListAPI.Constants;
using ToDoListAPI.Data;
using ToDoListAPI.Models;

namespace ToDoListAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        public BookRepository BookRepository { get; set; }
        public BooksController(BookRepository bookRepository)
        {
            BookRepository = bookRepository;
        }

        [HttpGet]
        public ActionResult<List<Book>> Get()
        {
            return BookRepository.Books;
        }


        [HttpGet("{id}")]
        public ActionResult<Book> GetById(int id)
        {
            var book = BookRepository.GetById(id);

            if (book == null)
                return NotFound($"Ingen bok med id {id} hittades.");

            return Ok(book);
        }



        [HttpPost]
        public void Post(string title, string author, int year)
        {
            BookRepository.Add(title, author, year);
        }

        [HttpDelete]
        [Authorize(Roles = ApiRoles.Superuser)]
        public void Delete(int id)
        {
            BookRepository.Delete(id);
        }


    }
}
