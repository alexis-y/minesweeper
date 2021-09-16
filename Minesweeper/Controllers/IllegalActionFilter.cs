using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Minesweeper.Model;

namespace Minesweeper.Controllers
{
    /// <summary>
    /// Turns <see cref="IllegalActionException"/> into HTTP 400 Bad Request
    /// </summary>
    public class IllegalActionFilter : IExceptionFilter
    {
        
        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled && context.Exception is IllegalActionException)
            {
                context.Result = new BadRequestResult();
                context.ExceptionHandled = true;
            }
        }
    }
}
