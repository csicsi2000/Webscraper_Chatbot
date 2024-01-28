using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Backend.Logic.Components.Logic
{
    internal class WebDriverManager : IDisposable
    {
        public IWebDriver WebDriver {get;}
        static Object portLocker = new Object(); 
        public WebDriverManager(bool withUi = false) 
        {
            //var options = new EdgeOptions();
            var options = new ChromeOptions();
            //var edgeOptions = EdgeDriverService.CreateDefaultService();
            var chromeOptions = ChromeDriverService.CreateDefaultService();
            if (!withUi)
            {
                options.AddArgument("--headless");
                options.AddArgument("--ignore-certificate-errors");
            }

            lock (portLocker)
            {
                chromeOptions.Port = GetRandomAvailablePort();
                WebDriver = new ChromeDriver(chromeOptions, options);
            }
        }

        static int GetRandomAvailablePort()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // Bind the socket to an available port (port number 0)
                socket.Bind(new IPEndPoint(IPAddress.Any, 0));

                // Get the local endpoint with the assigned port
                var endPoint = (IPEndPoint)socket.LocalEndPoint;

                // Return the port number
                return endPoint.Port;
            }
        }
        public void Dispose()
        {
            WebDriver.Close();
            WebDriver.Dispose();
        }
    }
}
