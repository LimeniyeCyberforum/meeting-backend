using MeetingGrpc.Protos;
using MeetingGrpc.Server.Model;
using System.Reactive.Linq;

namespace MeetingGrpc.Server.Repositories.LocalServices
{
    public class LocalChatService
    {
        private readonly ILogger<LocalChatService> _logger;

        private readonly IRepository<Guid, Message> _repository;

        private event Action<(EventAction Action, Message Message)> Added;

        public LocalChatService(ILogger<LocalChatService> logger, IRepository<Guid, Message> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public void Add(Message message)
        {
            _repository.Add(message.Guid, message);

            _logger.LogInformation($"{message.User.Name}: {message.Content}\n{message.DateTime}");

            Added?.Invoke((EventAction.Added, message));
        }

        public IObservable<(EventAction Action, Message Message)> GetMessagesAsObservable()
        {
            var oldLogs = _repository.GetAll().Select(x => (EventAction.Added, x)).ToObservable();
            var newLogs = Observable.FromEvent<(EventAction Action, Message Message)>((x) => Added += x, (x) => Added -= x);

            return oldLogs.Concat(newLogs);
        }
    }
}
