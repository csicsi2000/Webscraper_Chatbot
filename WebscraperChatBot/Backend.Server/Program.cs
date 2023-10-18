// See https://aka.ms/new-console-template for more information
using Backend.Server.Workflows;
using Backend.SqLiteDatabaseHandler;
using log4net;
using log4net.Config;

ILog log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

XmlConfigurator.Configure(new FileInfo("log4net.config"));
log4.Info("Server started.");

const string dbPath = "database.sqlite";
var contextWorkflow = new ExtractContextWorkflow(new SqLiteDataBaseComponent(dbPath, true));
var excludedUrls = new List<string>() { "https://uni-eszterhazy.hu/api" };

contextWorkflow.ExtractHtml("https://uni-eszterhazy.hu/matinf", excludedUrls);
//contextWorkflow.ExtraxtContext("https://uni-eszterhazy.hu/", excludedUrls);

