using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarkdownUserStories.Models;
using MarkdownUserStories.ViewModels;
using MarkdownUserStories.Services;
using Microsoft.AspNetCore.Hosting;

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
            IEnumerable<UserStory> model = _storyService.GetStories()
                .OrderByDescending(a =>
                    {
                        switch (a.Status.ToLower())
                        {
                            case "backlog": return 1;
                            case "in process": return 2;
                            case "waiting":return 3;
                            case "done": return 4;
                            default: return 0;
                        }
                    }
                )
                .ThenByDescending(a =>
                    //Done is always sorted by date
                    a.Status.ToLower() == "done" ? 0 : a.Sequence
                )
                .ThenByDescending(a => a.CompletedOn)
                .ThenByDescending(a => a.StartedOn)
                .ThenByDescending(a => a.CreatedOn)
                ;
            return View(model);
        }

        [HttpGet]
        public IActionResult AddStory()
        {
            return View("EditStory", new UserStoryEdit() { CreatedOn = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStory([FromForm]UserStoryEdit model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditStory", model);
            }
            //Save
            //The service takes care of setting the Sequence property on new models.
            _storyService.SaveStory(model);

            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStories([FromForm] IEnumerable<UserStoryEdit> model)
        {
            try
            {
                _storyService.SaveStories(model);
            }
            catch (Exception ex)
            {                
                var code = Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict;
                var type = System.Net.Mime.MediaTypeNames.Text.Html;
                var msg =  ex.GetBaseException().Message;
                msg = System.Net.WebUtility.HtmlEncode(msg);
                return new ContentResult() { Content = msg, StatusCode = code, ContentType =  type };
            }
            return Ok();
        }


        [HttpGet]
        public IActionResult DeleteStory(string id)
        {
            UserStory model = _storyService.GetStory(id);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteStory([FromForm] UserStory model)
        {
            _storyService.DeleteStory(model.Id);
            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpGet]
        public IActionResult EditStory(string id)
        {
            UserStory userStory = _storyService.GetStory(id);
            UserStoryEdit model = new UserStoryEdit(userStory);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStory([FromForm]UserStoryEdit model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Save
            //The service takes care of setting the Sequence property on new models.
            _storyService.SaveStory(model);

            return RedirectToAction(actionName: nameof(Index));
        }

        [HttpGet]
        public IActionResult GetStoriesWithStatus([FromQuery] string[] statuses)
        {
            var model = _storyService.GetStoriesWithStatus(statuses);
            return View(nameof(Index), model);
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
