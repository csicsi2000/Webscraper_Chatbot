// See https://aka.ms/new-console-template for more information
using Backend.Logic;
using Backend.Logic.Components;
using Backend.Logic.Components.Logic;
using Backend.Logic.Data.Json;
using log4net;
using log4net.Config;

ILog log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

XmlConfigurator.Configure(new FileInfo("log4net.config"));
log4.Info("Server started.");

//contextWorkflow.ExtractHtmls("https://uni-eszterhazy.hu/matinf", excludedUrls);
//contextWorkflow.ExtractHtmls();

ChatbotServices chatbotServices = new ChatbotServices(new ServerSettings() { DbPath= "database-simplemma.sqlite", ModelApiURL= "http://localhost:5555" });
chatbotServices.ExtractContexts(true);
Console.WriteLine("Finished");

/*
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
*/




