using System.Diagnostics;

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

        public void StartHttpServer(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"-m http.server {PORT} -d {path}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            pythonServerProcess = new Process { StartInfo = startInfo };
            pythonServerProcess.Start();

            // Wait for the server to start (you might need to adjust the delay)
            Thread.Sleep(5000);
        }

        public void StopHttpServer()
        {
            pythonServerProcess.Kill();

            // Wait for the process to exit (you might need to adjust the delay)
            pythonServerProcess.WaitForExit(2000);
        }

    }
}
