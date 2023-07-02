// See https://aka.ms/new-console-template for more information
using Backend.Server.Workflows;
using log4net;
using log4net.Config;

ILog log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

XmlConfigurator.Configure(new FileInfo("log4net.config"));
log4.Info("Server started.");


var contextWorkflow = new ExtractContextWorkflow();
contextWorkflow.ExtraxtContext("https://uni-eszterhazy.hu/");