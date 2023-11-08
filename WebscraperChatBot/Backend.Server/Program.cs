// See https://aka.ms/new-console-template for more information
using Backend.Logic;
using Backend.Logic.Components;
using Backend.Logic.Components.Logic;
using Backend.QuestionAnswerModel;
using Backend.SqLiteDatabaseHandler;
using General.Interfaces.Data;
using log4net;
using log4net.Config;

ILog log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

XmlConfigurator.Configure(new FileInfo("log4net.config"));
log4.Info("Server started.");

const string dbPath = "database.sqlite";
var databaseHandler = new SqLiteDataBaseComponent(dbPath, true);
var contextWorkflow = new ChatbotServices(databaseHandler);
var excludedUrls = new List<string>() { "https://uni-eszterhazy.hu/api" };

//contextWorkflow.ExtractHtmls("https://uni-eszterhazy.hu/matinf", excludedUrls);
//contextWorkflow.ExtraxtContext("https://uni-eszterhazy.hu/", excludedUrls);
var stopWordReader = new StopWordReader();
var tokenConverter = new TokenConverter(stopWordReader.GetStopwords());

var htmlParser = new HtmlParserComponent(tokenConverter);
//htmlParser.FindCommonElements(databaseHandler.GetHtmlFiles().Take(10).ToList());


var retriever = new RetrieverComponent(tokenConverter);

var questionAnswer = new Python_DebertaModel("C:\\Users\\csics\\AppData\\Local\\Programs\\Python\\Python310\\python310.dll");
IEnumerable<IContext> contexts = databaseHandler.GetContexts();
while (true)
{
    Console.WriteLine("Mi a kérdésed?");
    string question = Console.ReadLine();
    retriever.CalculateContextScores(contexts,question);
    var bestContexts = contexts.OrderByDescending(x => x.Score).Take(10);

    var duplications = bestContexts.Last();

    bestContexts = bestContexts.Where(x => x.Score != duplications.Score);

    Console.WriteLine(ObjectDumper.Dump(bestContexts, new DumpOptions()
    {
        MaxLevel = 1
    }));
    if (bestContexts.Count() == 0)
    {
        Console.WriteLine("Sajnos nincs válasz.");
    }
    else
    {
        var answer = questionAnswer.AnswerFromContext(bestContexts.First().Text, question);
        Console.WriteLine(answer);
    }
    Console.WriteLine();
}





