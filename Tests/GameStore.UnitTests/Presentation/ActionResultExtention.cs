using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GameStore.UnitTests.Presentation
{
    public static class ActionResultExtention
    {
        public static T GetFromTaskViewResult<T>(this Task<ActionResult<T>> taskActionResult)
        {
            taskActionResult.Result.Result.Should().BeOfType<ViewResult>();
            var viewResult = taskActionResult.Result.Result as ViewResult;
            var result = (T)viewResult.Model;
            return result;
        }
    }
}
