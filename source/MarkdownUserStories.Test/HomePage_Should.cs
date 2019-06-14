using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using MarkdownUserStories.Controllers;
using MarkdownUserStories.Models;
using MarkdownUserStories.Services;

namespace MarkdownUserStories.Test
{
    public class Homepage_Should
    {
        StoryServiceMock _storyService = new StoryServiceMock();

        public Homepage_Should()
        {
            _storyService.UserStories = MarkdownStoriesMocks.CurrentStories;
        }

        private HomeController GetHomeController()
        {
            return new HomeController(_storyService);
        }

        [Fact]
        public void Return_ViewResult_and_UserStories_Model()
        {
            //arrange
            var controller = GetHomeController(); ;

            //act
            var result = controller.Index();

            //assert
            result.Should().BeOfType<ViewResult>();
            var view = (ViewResult)result;
            view.ViewData.Model.Should().NotBeNull();
            view.ViewData.Model.Should().BeAssignableTo<IEnumerable<UserStory>>();
        }

        [Fact]
        public void Return_3_stories()
        {
            // arrange
            var controller = GetHomeController();

            // act
            var result = controller.Index();

            // assert            
            result.UserStoriesViewModel().Should().HaveCount(3);
        }

        [Fact]
        public void Return_a_blank_new_story()
        {
            // arrange
            var controller = GetHomeController();
            UserStory expectedModel = MarkdownStoriesMocks.NewStory;

            // act
            var result = controller.AddStory();
            var model = result.UserStoryViewModel();

            // assert
            result.Should().BeOfType<ViewResult>();
            //special assertion, be sure CreatedOn is within a five seconds either way. If so, make them equal.
            model.CreatedOn.Should().NotBeAfter(expectedModel.CreatedOn.AddSeconds(5));
            model.CreatedOn.Should().NotBeBefore(expectedModel.CreatedOn.AddSeconds(-5));
            model.CreatedOn = expectedModel.CreatedOn;

            //Now the objects should have the same properties.
            model.Should().BeEquivalentTo<UserStory>(expectedModel);
        }

        [Fact]
        public void On_save_with_model_errors_return_ViewResult()
        {
            // arrange
            var controller = GetHomeController();
            var model = MarkdownStoriesMocks.NewStory;
            controller.ViewData.ModelState.AddModelError("Role", "Role cannot be empty.");

            // act
            var result = controller.AddStory(model);

            // assert
            result.Should().BeOfType<ViewResult>();
        }

        //This kind of testing can't be done because
        //model-binding isn't performed except in actual use.
        //So, Controller.ViewData.Model won't exist, and adding it
        //makes no sense. You'd just be testing what you added.

        //[Fact]
        //public void On_save_model_propery_changes_are_returned()
        //{
        //    // arrange
        //    var controller = GetHomeController();
        //    var model = MarkdownStoriesMocks.NewStory;
        //    _storyService.UserStory = model;
        //    model.Sequence = 99;
        //    var testDateTime = DateTime.Now;
        //    model.StartedOn = testDateTime;

        //    // act
        //    var result = controller.AddStory(model);
        //    var savedModel = (UserStory)controller.ViewData.Model;

        //    // assert
        //    savedModel.Sequence.Should().Be(99);
        //    savedModel.StartedOn.Should().Be(testDateTime);
        //}

        //What DOES make sense is testing that the service's SaveStory method was called.
        [Fact]
        public void On_save_call_service_SaveStory_method_once()
        {
            // arrange
            var controller = GetHomeController();
            var model = MarkdownStoriesMocks.NewStory;

            // act
            var result = controller.AddStory(model);

            // assert
            _storyService.CalledMethods["SaveStory"].Count.Should().Be(1);
        }

        [Fact]
        public void On_edit_GET_call_service_GetStory_method_once()
        {
            // arrange
            var controller = GetHomeController();
            var model = MarkdownStoriesMocks.CurrentStories.First();
            _storyService.UserStory = model;

            // act
            var result = controller.EditStory(model.Role, model.Want, model.Why);

            // assert
            result.Should().BeOfType<ViewResult>();
            result.ViewResult().ViewData.Model.Should().BeAssignableTo<UserStory>();
            _storyService.CalledMethods["GetStory"].Count.Should().Be(1);
        }

        [Fact]
        public void On_edit_POST_call_service_SaveStory_method_once()
        {
            // arrange
            var controller = GetHomeController();
            var model = MarkdownStoriesMocks.CurrentStories.First();
            _storyService.UserStory = model;

            // act
            var result = controller.EditStory(model);

            // assert
            result.Should().BeOfType<RedirectToActionResult>();
            _storyService.CalledMethods["SaveStory"].Count.Should().Be(1);
        }


        [Fact]
        public void On_save_with_no_errors_redirect_to_Index()
        {
            // arrange
            var controller = GetHomeController();
            var model = MarkdownStoriesMocks.NewStory;

            // act
            var result = controller.AddStory(model);
            var redirectResult = result as RedirectToActionResult;

            // assert
            result.Should().BeOfType<RedirectToActionResult>();
            redirectResult.ControllerName.Should().BeNull();
            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public void On_edit_call_service_SaveStory_once()
        {
            // arrange
            var controller = GetHomeController();
            var model = MarkdownStoriesMocks.NewStory;

            // act
            var result = controller.EditStory(model);

            // assert
            _storyService.CalledMethods["SaveStory"].Count.Should().Be(1);
        }

        [Fact]
        public void On_delete_call_service_DeleteStory_once()
        {
            // arrange
            var controller = GetHomeController();

            // act
            var result = controller.DeleteStory("","","");

            // assert
            _storyService.CalledMethods["DeleteStory"].Count.Should().Be(1);
        }

        [Fact]
        public void On_GetStoriesWithStatus_call_service_GetStoriesWithStatus_once()
        {
            // arrange
            var controller = GetHomeController();

            // act
            var result = controller.GetStoriesWithStatus(new string[] { });

            // assert
            _storyService.CalledMethods["GetStoriesWithStatus"].Count.Should().Be(1);
        }

        [Fact]
        public void On_GetStoriesWithStatus_return_Index_View()
        {
            // arrange
            var controller = GetHomeController();
            string[] statuses = new[] { "Backlog", "Done" };

            // act
            var result = controller.GetStoriesWithStatus(new string[] { });

            // assert
            result.Should().BeOfType<ViewResult>();
            result.ViewResult().ViewName.Should().Be("Index");
        }

        [Fact]
        public void On_GetStoriesWithStatus_return_3_stories()
        {
            // arrange
            var controller = GetHomeController();
            string[] statuses = new[] { "Backlog", "Done" };

            // act
            var result = controller.GetStoriesWithStatus(statuses);

            // assert
            result.ViewResult().Model.Should().BeAssignableTo<IEnumerable<UserStory>>();
            result.UserStoriesViewModel().Count().Should().Be(3);
        }

        [Fact]
        public void On_UpdateStories_return_OK()
        {
            // arrange
            var controller = GetHomeController();

            // act
            var result = controller.UpdateStories(MarkdownStoriesMocks.CurrentStories);

            // assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void On_UpdateStories_call_SaveStories_once()
        {
            // arrange
            var controller = GetHomeController();

            // act
            var result = controller.UpdateStories(MarkdownStoriesMocks.CurrentStories);

            // assert
            _storyService.CalledMethods["SaveStories"].Count.Should().Be(1);
        }
 
        [Fact]
        public void On_UpdateStories_error_should_return_HTML()
        {
            // arrange
            var controller = GetHomeController();
            _storyService.Exception = new Exception("MyProperty Error1 Error2");

            // act
            var result = controller.UpdateStories(MarkdownStoriesMocks.CurrentStories);

            // assert
            result.Should().BeOfType<ContentResult>();
            var content = (ContentResult)result;
            content.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            content.ContentType.Should().Be(System.Net.Mime.MediaTypeNames.Text.Html);
            content.Content.Should().ContainAll("MyProperty", "Error1", "Error2");
        }


    }

    public static class ControllerHelpers
    {
        public static ViewResult ViewResult(this IActionResult actionResult)
        {
            return (ViewResult)actionResult;
        }

        public static IEnumerable<UserStory> UserStoriesViewModel(this IActionResult actionResult)
        {
            var view = actionResult as ViewResult;
            var modelObject = view.ViewData.Model;
            var model = modelObject as IEnumerable<UserStory>;
            return model;
        }

        public static UserStory UserStoryViewModel(this IActionResult actionResult)
        {
            var view = actionResult as ViewResult;
            var modelObject = view.ViewData.Model;
            var model = modelObject as UserStory;
            return model;

        }

    }
}
