using HtmlAgilityPack;
using System.Diagnostics;
using System.Security.Policy;

namespace Backend.Logic.Tests.Support
{
    internal class StaticSiteStarter
    {
        private Process pythonServerProcess;
        private string PORT;

        public StaticSiteStarter(string port)
        {
            PORT = port;
        }

        public bool StartHttpServer(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"-m http.server {PORT} -d {path}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false, // true to display
                CreateNoWindow = false // false to display
            };

            pythonServerProcess = new Process { StartInfo = startInfo };

            pythonServerProcess.Start();
            return WaitForServerStart($"http://localhost:{PORT}",20, "Directory listing for");
        }

        private bool WaitForServerStart(string url, int timeout, string searchText)
        {
            DateTime startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < timeout)
            {
                // Make an HTTP request to the webpage
                using (HttpClient httpClient = new HttpClient())
                {
                    string htmlContent = httpClient.GetStringAsync(url).Result;

                    // Use HtmlAgilityPack to parse the HTML content
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);

                    // Find the specified text in the HTML
                    if (htmlDocument.DocumentNode.InnerText.Contains(searchText))
                    {
                        Console.WriteLine($"The text '{searchText}' was found on the webpage.");
                        return true; // Exit the program when the text is found
                    }
                    else
                    {
                        Console.WriteLine($"The text '{searchText}' was not found on the webpage. Retrying in 1 seconds...");

                        // Wait for 5 seconds before making the next attempt
                        Task.Delay(1000).Wait();
                    }
                }
            }
            Console.WriteLine("The static webserver did not start up.");
            return false;
        }
        public void StopHttpServer()
        {
            pythonServerProcess.Kill();

            // Wait for the process to exit (you might need to adjust the delay)
            pythonServerProcess.WaitForExit(2000);
        }

    }
}
