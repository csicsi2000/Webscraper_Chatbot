using Backend.Logic.Components;
using Backend.Logic.Components.Logic;
using Backend.Logic.Data.Json;
using General.Interfaces.Backend.Components;
using General.Interfaces.Backend.Logic;
using General.Interfaces.Data;
using log4net.Config;

namespace Backend.Logic
{
    public class ChatbotServices : IChatbotServices
    {
        IDatabaseHandler _databaseHandler;
        ITokenConverter _tokenConverter;
        IContextRetriever _retriever;
        IHtmlParser _htmlParser;
        IQuestionAnswerModel _questionAnswerModel;

        ServerSettings _settings = new ServerSettings();
        public ChatbotServices(IDatabaseHandler databaseHandler, ServerSettings settings) : this(databaseHandler)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public ChatbotServices(IDatabaseHandler databaseHandler)
        {
            _databaseHandler = databaseHandler ?? throw new ArgumentNullException(nameof(databaseHandler));
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            var stopWordReader = new StopWordReader();
            _tokenConverter = new TokenConverter(stopWordReader.GetStopwords());

            _htmlParser = new HtmlParserComponent(_tokenConverter);
            //htmlParser.FindCommonElements(databaseHandler.GetHtmlFiles().Take(10).ToList());


            _retriever = new RetrieverComponent(_tokenConverter);

            _questionAnswerModel = _settings.QAModel;
        }

        /// <summary>
        /// Extracts all html files from an url
        /// </summary>
        public void ExtractHtmls()
        {
            var htmlFileExtractor = new HtmlFileExtractorComponent("main-top", _settings.ExcludedUrls);

            foreach (var file in htmlFileExtractor.GetHtmlFiles(_settings.RootUrl))
            {
                _databaseHandler.InsertOrUpdateHtmlFile(file);
            }
        }

        public void ExtractContexts(bool isSetup = false)
        {
            var tokenConverter = new TokenConverter(new StopWordReader().GetStopwords());
            var htmlParser = new HtmlParserComponent(tokenConverter); // common elements

            if (isSetup)
            {
                htmlParser.FindCommonElements(_databaseHandler.GetHtmlFiles().Take(15).ToList());
            }
            foreach (var htmlFile in _databaseHandler.GetHtmlFiles())
            {
                var context = htmlParser.ExtractRelevantContent(htmlFile);
                _databaseHandler.InsertOrUpdateContext(context);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="question"></param>
        /// <returns>null if no answer found</returns>
        public string GetAnswer(string question)
        {
            IEnumerable<IContext> contexts = _databaseHandler.GetContexts();

            _retriever.CalculateContextScores(contexts, question);
            var bestContexts = contexts.OrderByDescending(x => x.Score).Take(10);

            var duplications = bestContexts.Last();

            bestContexts = bestContexts.Where(x => x.Score != duplications.Score);

            if (bestContexts.Count() == 0)
            {
                return null;
            }

            var answer = _questionAnswerModel.AnswerFromContext(bestContexts.First().Text, question);
            return answer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public IList<IContext> GetAdvanceAnswer(string question)
        {
            IEnumerable<IContext> contexts = _databaseHandler.GetContexts();

            _retriever.CalculateContextScores(contexts, question);
            var bestContexts = contexts.OrderByDescending(x => x.Score).Take(10);

            var duplications = bestContexts.Last();

            bestContexts = bestContexts.Where(x => x.Score != duplications.Score);

            if (bestContexts.Count() == 0)
            {
                return null;
            }

            return bestContexts.ToList();
        }
    }
}
