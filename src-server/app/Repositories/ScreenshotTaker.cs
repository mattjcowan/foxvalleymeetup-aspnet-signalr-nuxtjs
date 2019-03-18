using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.NodeServices;
using ServiceStack.Text;

namespace app.Repositories
{
    public interface IScreenshotTaker
    {
        Task<bool> TryTakeScreenshotAsync(string url, string pngFilePath);
    }

    public class ScreenshotTaker : IScreenshotTaker
    {
        private readonly INodeServices nodeServices;

        public ScreenshotTaker(INodeServices nodeServices)
        {
            this.nodeServices = nodeServices;
        }

        public async Task<bool> TryTakeScreenshotAsync(string url, string pngFilePath)
        {
            try
            {
                var file = await nodeServices.InvokeAsync<string>("nsTakeScreenshot.js", url, pngFilePath);
                return File.Exists(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to take screenshot: " + ex.Message);
                // Console.WriteLine(ex.Dump());
                return false;
            }
        }
    }
}