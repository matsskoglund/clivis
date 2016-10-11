using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;

namespace Clivis
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class TodoControllersTest
    {
        private readonly TodoController _todoController;
        public TodoControllersTest()
        {
            ITodoRepository repo = new TodoRepository();            
            _todoController = new TodoController(repo);
        }
         [Fact]
        public void TodoController_NotNull()
        {
            Assert.NotNull(_todoController);
        }

        [Fact]
        public void TodoController_Index()
        {
            IEnumerable<TodoItem> res = _todoController.GetAll();
            Assert.NotNull(res);
        }

        [Fact]
        public void ValuesController_GetId_Returns_NotNull()
        {
            string res = _todoController.GetById("Nyckel").ToString();
                        
            Assert.NotNull(res);
        }

    }
}
