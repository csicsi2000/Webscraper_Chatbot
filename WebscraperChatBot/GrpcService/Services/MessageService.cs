using Backend.Logic;
using Grpc.Core;
using GrpcService.Data;
using log4net.Config;
using log4net;

namespace GrpcService.Services
{
    public class MessageService : ChatbotService.ChatbotServiceBase
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object _lockCE = new object();
        private static bool _isContentExtractionRunning = false;

        private readonly object _lockHE = new object();
        private static bool _isHtmlExtractionRunning = false;

        IChatbotServices _chatbotServices;


        public MessageService() : base()
        {
            ILog log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            _chatbotServices = new ChatbotServices();
            Console.WriteLine(_chatbotServices.GetContextCount());
        }

        // TODO
        public override Task<ServiceStatus> GetStatus(EmptyRequest request, ServerCallContext context)
        {
            var states = _chatbotServices.GetServiceState();

            return Task.FromResult(new ServiceStatus()
            {
                ContextExtraction = states.IsContextExtractionRunning ? StatusEnum.Running.ToString() : StatusEnum.Available.ToString(),
                HtmlExtraction = states.IsHtmlExtractionRunning ? StatusEnum.Running.ToString() : StatusEnum.Available.ToString(),
                HtmlFileCount = _chatbotServices.GetHtmlCount(),
                ContextCount = _chatbotServices.GetContextCount(),
            });
        }

        public override Task<CurrentSettings> GetServerSettings(EmptyRequest request, ServerCallContext context)
        {
            var settings = _chatbotServices.GetSettings();
            var curSettings = new CurrentSettings()
            {
                DbPath = settings.DbPath,
                RootUrl = settings.RootUrl,
                WaitedClassName = settings.WaitedClassName,
                ModelApiUrl = settings.ModelApiURL
            };
            curSettings.IgnoredUrls.AddRange(settings.ExcludedUrls);

            return Task.FromResult(curSettings);
        }

        public override Task<EmptyRequest> SetServerSettings(CurrentSettings request, ServerCallContext context)
        {
            var settings = _chatbotServices.GetSettings();
            settings.DbPath = request.DbPath;
            settings.RootUrl = request.RootUrl;
            settings.WaitedClassName = request.WaitedClassName;
            settings.ExcludedUrls = request.IgnoredUrls;
            settings.ModelApiURL = request.ModelApiUrl;
            _chatbotServices.SetSettings(settings);

            return Task.FromResult(new EmptyRequest());
        }
        // TODO
        public override Task<Message> SendQuestion(Message request, ServerCallContext context)
        {
            var answer = _chatbotServices.GetAnswer(request.Text);
            if (answer == null)
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
            var answers = _chatbotServices.GetAdvancedAnswer(request.Text);

            if (answers == null || answers?.Count == 0)
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
            _chatbotServices.ExtractContexts(true);
            return Task.FromResult(new Message { Text = "Method completed successfully." });
        }

        // 
        public override Task<Message> StartHtmlExtraction(EmptyRequest request, ServerCallContext context)
        {
            _chatbotServices.ExtractHtmls();
            return Task.FromResult(new Message { Text = "Method completed successfully." });
        }
    }
}
