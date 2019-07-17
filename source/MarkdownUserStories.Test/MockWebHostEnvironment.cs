using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownUserStories.Test
{
    public class MockWebHostEnvironment : IWebHostEnvironment
    {
        public IFileProvider WebRootFileProvider { get => new PhysicalFileProvider(Environment.CurrentDirectory); set => throw new NotImplementedException(); }
        public string WebRootPath { get => Environment.CurrentDirectory; set => throw new NotImplementedException(); }
        public string ApplicationName { get => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name; set => throw new NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => new PhysicalFileProvider(Environment.CurrentDirectory); set => throw new NotImplementedException(); }
        public string ContentRootPath { get => Environment.CurrentDirectory; set => throw new NotImplementedException(); }
        public string EnvironmentName { get => "Production"; set => throw new NotImplementedException(); }
    }

}
