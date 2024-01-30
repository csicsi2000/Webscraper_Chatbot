using HtmlAgilityPack;
using System.Diagnostics;
using System.Security.Policy;

namespace Backend.Logic.Tests.Support
{
    internal class StaticSiteStarter
    {
        private Process pythonServerProcess;
        private string PORT;
        private string IP;

        public StaticSiteStarter(string port, string ip)
        {
            PORT = port;
            IP = ip;
        }

        public bool StartHttpServer(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"-m http.server {PORT} -d {path} --bind {IP}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false, // true to display
                CreateNoWindow = true // false to display
            };

            pythonServerProcess = new Process { StartInfo = startInfo };

            pythonServerProcess.Start();
            return WaitForServerStart($"http://{IP}:{PORT}",30, "Directory listing for");
        }

        private bool WaitForServerStart(string url, int timeout, string searchText)
        {
            DateTime startTime = DateTime.Now;
            Task.Delay(3000).Wait();
            while ((DateTime.Now - startTime).TotalSeconds < timeout)
            {
                // Make an HTTP request to the webpage
                using (HttpClient httpClient = new HttpClient())
                {
                    try
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
                    }
                    catch
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
