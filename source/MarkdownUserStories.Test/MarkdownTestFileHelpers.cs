using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownUserStories.Test
{
    public static class MarkdownTestFileHelpers
    {
        static IWebHostEnvironment _webHostEnvironment = new MockWebHostEnvironment();
        static string _appDataFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "AppData");


        public static void RefreshTestFiles()
        {
            ClearTestFiles();
            //Yuck, but works in this case.
            string parent = @"..\..\..\..\..\DefaultStories";
            string defaultStoriesFolderPath = Path.GetFullPath( Path.Combine(_appDataFolderPath, parent));
            Directory.GetFiles(defaultStoriesFolderPath).ToList()
                .ForEach(a => File.Copy(a, Path.Combine(_appDataFolderPath, Path.GetFileName(a))));
        }

        public static void ClearTestFiles()
        {
            Directory.GetFiles(_appDataFolderPath).ToList().ForEach(a => File.Delete(a));
        }
    }
}
