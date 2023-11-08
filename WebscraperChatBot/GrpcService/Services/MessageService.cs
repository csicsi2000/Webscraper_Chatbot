using Backend.Logic;
using Backend.SqLiteDatabaseHandler;
using Grpc.Core;

namespace GrpcService.Services
{
    public class MessageService : ChatbotService.ChatbotServiceBase
    {
        private readonly object _lockCE = new object();
        private bool _isContentExtractionRunning = false;

        IChatbotServices _chatbotServices;
        public MessageService() 
        {
            const string dbPath = "database.sqlite";
            var databaseHandler = new SqLiteDataBaseComponent(dbPath, true);


        }

        // TODO
        public override Task<Message> GetStatus(EmptyRequest request, ServerCallContext context)
        {
            return base.GetStatus(request, context);
        }

        // TODO
        public override Task<Message> SendMessage(Message request, ServerCallContext context)
        {
            return base.SendMessage(request, context);
        }

        //
        public override Task<AdvancedMessages> SendMessageAdvanced(Message request, ServerCallContext context)
        {
            return base.SendMessageAdvanced(request, context);
        }

        // TODO 
        public override Task<Message> StartContextExtraction(EmptyRequest request, ServerCallContext context)
        {
            lock(_lockCE)
            {
                if (_isContentExtractionRunning)
                {
                    throw new RpcException(new Status(StatusCode.ResourceExhausted, "Method is already running."));
                }
                _isContentExtractionRunning=true;
            }

            try
            {
                // run content extraction
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
            return base.StartHtmlExtraction(request, context);
        }
    }
}
