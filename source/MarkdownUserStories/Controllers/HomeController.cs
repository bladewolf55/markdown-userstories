using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarkdownUserStories.Models;
using MarkdownUserStories.Services;


namespace MarkdownUserStories.Controllers
{
    public class HomeController : Controller
    {
        private IStoryService _storyService = null;

        public HomeController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        public IActionResult Index()
        {
            IEnumerable<UserStory> model = _storyService.GetStories();
            return View(model);
        }

        [HttpGet]
        public IActionResult AddStory()
        {
            return View(new UserStory() { CreatedOn = DateTime.Now });
        }

        [HttpPost]
        public IActionResult AddStory([FromBody]UserStory model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Save
            //The service takes care of setting the Sequence property on new models.
            model = _storyService.SaveStory(model);

            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpPatch]
        public IActionResult UpdateStory([FromBody]UserStory model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Save
            //The service takes care of setting the Sequence property on new models.
            model = _storyService.SaveStory(model);

            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpDelete]
        public IActionResult DeleteStory([FromBody]UserStory model)
        {
            _storyService.DeleteStory(model);
            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpGet]
        public IActionResult EditStory([FromQuery] string role, string what, string why)
        {
            UserStory model = _storyService.GetStory(role, what, why);
            return View(model);
        }

        [HttpPatch]
        public IActionResult EditStory([FromBody]UserStory model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Save
            //The service takes care of setting the Sequence property on new models.
            model = _storyService.SaveStory(model);

            return RedirectToAction(actionName: nameof(Index));
        }


        //--------------------------------------------------

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
