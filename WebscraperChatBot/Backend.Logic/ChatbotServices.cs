﻿using Backend.Logic.Components;
using Backend.Logic.Components.Logic;
using Backend.Logic.Data;
using Backend.Logic.Utils;
using Backend.SqLiteDatabaseHandler;
using General.Interfaces.Backend.Components;
using General.Interfaces.Backend.Logic;
using General.Interfaces.Data;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using NTextCat.Commons;

namespace Backend.Logic
{
    public class ChatbotServices : IChatbotServices
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IDatabaseHandler DatabaseHandler;
        ITokenConverter _tokenConverter;
        IContextRetriever _retriever;
        IHtmlParser _htmlParser;
        IQuestionAnswerModel _questionAnswerModel;

        private Object _exractLock = new Object();

        private static Object _startServiceLock = new Object();
        private static IServiceState _serviceState = new ServiceState();

        SettingsManager _settingsManager = new SettingsManager();
        public ChatbotServices()
        {
            InitializeService();
        }
        public ChatbotServices(IServerSettings settings):this()
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            _settingsManager.SetServerSettings(settings);
        }

        private void InitializeService()
        {
            DatabaseHandler = new SqLiteDataBaseComponent(Path.GetFullPath(_settingsManager.GetServerSettings().DbPath), true);
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            var stopWordReader = new StopWordReader();
            //_tokenConverter = new TokenConverter(stopWordReader.GetStopwords());
            _tokenConverter = new SimplemmaTokenConverter(stopWordReader.GetStopwords(), _settingsManager.GetServerSettings().ModelApiURL);

            _htmlParser = new HtmlParserComponent(_tokenConverter);
            //htmlParser.FindCommonElements(databaseHandler.GetHtmlFiles().Take(10).ToList());
            _questionAnswerModel = new QuestionAnswerApiComponent(_settingsManager.GetServerSettings().ModelApiURL);

            _retriever = new BM25RetrieverComponent(_tokenConverter);

            _log4.Info("Model api: " + _settingsManager.GetServerSettings().ModelApiURL);
        }

        /// <summary>
        /// Get current settings
        /// </summary>
        /// <returns></returns>
        public IServerSettings GetSettings()
        {
            return _settingsManager.GetServerSettings();
        }

        public void SetSettings(IServerSettings settings)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            _settingsManager.SetServerSettings(settings);
            _log4.Warn("Settings changed: " + JsonConvert.SerializeObject(settings));
        }

        /// <summary>
        /// Extracts all html files from an url
        /// </summary>
        public void ExtractHtmls()
        {
            if (_serviceState.IsHtmlExtractionRunning)
            {
                return;
            }
            lock (_startServiceLock)
            {
                _serviceState.IsHtmlExtractionRunning = true;
            }
            var settings = _settingsManager.GetServerSettings();
            var htmlFileExtractor = new HtmlFileExtractorComponent(settings.WaitedClassName, settings.ExcludedUrls,DatabaseHandler);

            htmlFileExtractor.GetHtmlFiles(settings.RootUrl);
            lock (_startServiceLock)
            {
                _serviceState.IsHtmlExtractionRunning = false;
            }
        }

        /// <summary>
        /// Extracts all context from existing html files
        /// </summary>
        /// <param name="isSetup"></param>
        public void ExtractContexts(bool isSetup = false)
        {
            if(_serviceState.IsContextExtractionRunning)
            {
                return;
            }
            lock (_startServiceLock)
            {
                _serviceState.IsContextExtractionRunning = true;
            }
            var tokenConverter = _tokenConverter;
            var htmlParser = new HtmlParserComponent(tokenConverter); // common elements

            if (isSetup)
            {
                htmlParser.FindCommonElements(DatabaseHandler.GetHtmlFiles().Take(15).ToList());
            }

            Parallel.ForEach(DatabaseHandler.GetHtmlFiles(),new ParallelOptions { MaxDegreeOfParallelism=6},  file =>
            {
                var context = htmlParser.ExtractRelevantContent(file);
                lock(_exractLock)
                {
                    DatabaseHandler.InsertOrUpdateContext(context);
                }
            });
            lock (_startServiceLock)
            {
                _serviceState.IsContextExtractionRunning = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="question"></param>
        /// <returns>null if no answer found</returns>
        public IAnswer GetAnswer(string question)
        {
            IEnumerable<IContext> contexts = DatabaseHandler.GetContexts();

            var scores = _retriever.CalculateContextScores(contexts, question);
            var bestContexts = scores.OrderByDescending(x => x.Score).Take(10);

            var duplications = bestContexts.Last();

            bestContexts = bestContexts.Where(x => x.Score != duplications.Score);
            GC.Collect();

            if (bestContexts.Count() == 0)
            {
                return null;
            }

            var answer = _questionAnswerModel.AnswerFromContext(contexts.First(x => x.Id == bestContexts.First().Id).Text, question);
            return answer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public IList<IContext> GetAdvancedAnswer(string question)
        {
            IEnumerable<IContext> contexts = DatabaseHandler.GetContexts();

            var scores = _retriever.CalculateContextScores(contexts, question);
            var bestContexts = scores.OrderByDescending(x => x.Score).Take(10);

            var duplications = bestContexts.Last();

            bestContexts = bestContexts.Where(x => x.Score != duplications.Score);
            GC.Collect();

            if (bestContexts.Count() == 0)
            {
                return null;
            }
            IList<IContext> results = new List<IContext>();
            foreach ( var score in bestContexts)
            {
                var foundContext = contexts.First(x => x.Id == score.Id);
                foundContext.Score = score.Score;
                results.Add(foundContext);
            }
            return results.OrderByDescending(x => x.Score).ToList();
        }

        /// <summary>
        /// Count of the html files
        /// </summary>
        /// <returns></returns>
        public int GetHtmlCount()
        {
            return DatabaseHandler.GetHtmlFiles().Count();
        }

        /// <summary>
        /// Count of the contexts
        /// </summary>
        /// <returns></returns>
        public int GetContextCount()
        {
            return DatabaseHandler.GetContexts().Count();
        }

        public IServiceState GetServiceState()
        {
            return _serviceState;
        }
    }
}
