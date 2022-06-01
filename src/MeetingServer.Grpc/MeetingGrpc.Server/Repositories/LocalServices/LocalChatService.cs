using MeetingGrpc.Server.Model;
using System.Reactive.Linq;

namespace MeetingGrpc.Server.Repositories.LocalServices
{
    public class LocalChatService
    {
        private readonly ILogger<LocalChatService> _logger;

        private readonly IRepository<Message> _repository;

        public LocalChatService(ILogger<LocalChatService> logger, IRepository<Message> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        private event Action<Message> Added;

        public void Add(Message message)
        {
            _repository.Add(message);

            _logger.LogInformation($"{message.User.Name}: {message.Content}\n{message.DateTime}");

            Added?.Invoke(message);
        }

        public IObservable<Message> GetMessagesAsObservable()
        {
            var oldLogs = _repository.GetAll().ToObservable();
            var newLogs = Observable.FromEvent<Message>((x) => Added += x, (x) => Added -= x);

            return oldLogs.Concat(newLogs);
        }
    }
}
