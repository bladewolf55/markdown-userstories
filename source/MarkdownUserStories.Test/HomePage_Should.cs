using System;
using Xunit;
using MarkdownUserStories.Controllers;
using MarkdownUserStories.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System.Collections.Generic;

namespace MarkdownUserStories.Test
{
    public class Homepage_Should
    {
        [Fact]
        public void Return_ViewResult_and_UserStory_Model()
        {
            //Arrange
            var controller = new HomeController();
            //Act
            var result = controller.Index();
            //Assert
            result.Should().BeOfType<ViewResult>();
            var view = (ViewResult)result;
            view.ViewData.Model.Should().NotBeNull();
            view.ViewData.Model.Should().BeAssignableTo<IEnumerable<UserStory>>();
        }

        [Fact]
        public void Return_3_stories()
        {
            // arrange
            var controller = new HomeController();

            // act
            var result = controller.Index();
            ((ViewResult)result).ViewData.Model = MarkdownStoriesMocks.CurrentStories;

            // assert
            
            result.ViewModel().Should().HaveCount(3);
        }




    }

    public static class ControllerHelpers
    {
        public static IEnumerable<UserStory> ViewModel(this IActionResult actionResult)
        {
            var view = actionResult as ViewResult;
            var modelObject = view.ViewData.Model;
            var model = modelObject as IEnumerable<UserStory>;
            return model;
        }

    }
}
