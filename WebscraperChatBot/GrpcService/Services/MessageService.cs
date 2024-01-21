using Backend.Logic;
using Grpc.Core;
using GrpcService.Data;
using log4net.Config;
using log4net;
using Backend.Logic.Data.Json;

namespace GrpcService.Services
{
    public class MessageService : ChatbotService.ChatbotServiceBase
    {
        private readonly object _lockCE = new object();
        private static bool _isContentExtractionRunning = false;

        private readonly object _lockHE = new object();
        private static bool _isHtmlExtractionRunning = false;

        private readonly object _lockQuestion = new object();
        private static bool _isAnsweringRunning = false;

        IChatbotServices _chatbotServices;
        public MessageService() :base()
        {
            ILog log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            _chatbotServices = new ChatbotServices(new ServerSettings()
            {
                RootUrl = "https://aries.ektf.hu/~hz/wiki7",
                DbPath = "../database.sqlite",
                WaitedClassName = "main-content"
            });
            Console.WriteLine(_chatbotServices.GetContextCount());
        }

        // TODO
        public override Task<ServiceStatus> GetStatus(EmptyRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ServiceStatus()
            {
                ContextExtraction = _isContentExtractionRunning ? StatusEnum.Running.ToString() : StatusEnum.Available.ToString(),
                HtmlExtraction = _isHtmlExtractionRunning ? StatusEnum.Running.ToString() : StatusEnum.Available.ToString(),
                QuestionAnswer = _isAnsweringRunning ? StatusEnum.Running.ToString() : StatusEnum.Available.ToString(),
                HtmlFileCount = _chatbotServices.GetHtmlCount(),
                ContextCount = _chatbotServices.GetContextCount(),
            }) ; 
        }

        public override Task<CurrentSettings> GetServerSettings(EmptyRequest request, ServerCallContext context)
        {
            var settings = _chatbotServices.GetSettings();
            var curSettings = new CurrentSettings()
            {
                DbPath = settings.DbPath,
                RootUrl = settings.RootUrl,
                WaitedClassName = settings.WaitedClassName,
            };
            curSettings.IgnoredUrls.AddRange(settings.ExcludedUrls);

            return Task.FromResult(curSettings);
        }
        // TODO
        public override Task<Message> SendQuestion(Message request, ServerCallContext context)
        {
            //lock (_lockQuestion)
            //{
            //    if (_isAnsweringRunning)
            //    {
            //        throw new RpcException(new Status(StatusCode.Unavailable, "Answer service is busy."));
            //    }
            //    _isAnsweringRunning = true;
            //}

            var answer = _chatbotServices.GetAnswer(request.Text);
            lock (_lockCE)
            {
                _isAnsweringRunning = false;
            }
            if(answer == null) 
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Answer is not found."));
            }
            return Task.FromResult(new Message()
            {
                Text = answer.Answer,
                Score = answer.Score
            });
        }

        /// <summary>
        /// Send a request for multiple top answers with more detail
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public override Task<AdvancedMessages> SendQuestionAdvanced(Message request, ServerCallContext context)
        {
            lock (_lockQuestion)
            {
                if (_isAnsweringRunning)
                {
                    throw new RpcException(new Status(StatusCode.Unavailable, "Answer service is busy."));
                }
                _isAnsweringRunning = true;
            }

            var answers = _chatbotServices.GetAdvancedAnswer(request.Text);
            lock (_lockCE)
            {
                _isAnsweringRunning = false;
            }

            if(answers == null || answers?.Count == 0 )
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No Answer found."));
            }

            var advancedAnswers = answers.Select(x => new AdvancedMessage() { Text = x.Text, Score = x.Score, SourceUrl = x.OriginUrl });

            var allAnswers = new AdvancedMessages();
            allAnswers.TopAnswers.AddRange(advancedAnswers);
            return Task.FromResult(allAnswers);
        }


        public override Task<Message> StartContextExtraction(EmptyRequest request, ServerCallContext context)
        {
            lock(_lockCE)
            {
                if (_isContentExtractionRunning)
                {
                    throw new RpcException(new Status(StatusCode.ResourceExhausted, "Service is already running."));
                }
                _isContentExtractionRunning=true;
            }

            try
            {
                _chatbotServices.ExtractContexts(true);
                return Task.FromResult(new Message { Text = "Method completed successfully." });
            }
            finally
            {
                lock (_lockCE)
                {
                    _isContentExtractionRunning = false;
                }
            }
        }

        // 
        public override Task<Message> StartHtmlExtraction(EmptyRequest request, ServerCallContext context)
        {
            lock (_lockHE)
            {
                if (_isHtmlExtractionRunning)
                {
                    throw new RpcException(new Status(StatusCode.ResourceExhausted, "Service is already running."));
                }
                _isHtmlExtractionRunning = true;
            }

            try
            {
                _chatbotServices.ExtractHtmls();
                return Task.FromResult(new Message { Text = "Method completed successfully." });
            }
            finally
            {
                lock (_lockHE)
                {
                    _isHtmlExtractionRunning = false;
                }
            }
        }

    }
}
