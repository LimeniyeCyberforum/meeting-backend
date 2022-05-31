using MeetingGrpc.Server.Model;
using MeetingGrpc.Server.Repositories;
using System.Reactive.Linq;

namespace MeetingGrpc.Repositories.LocalServices
{
    public class UsersService
    {
        private readonly ILogger<UsersService> _logger;

        private readonly IUsersRepository _repository;

        public UsersService(ILogger<UsersService> logger, IUsersRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        private event Action<UserDto> Added;
        private event Action<UserDto> Removed;

        public void Add(UserDto user)
        {
            _logger.LogInformation($"{user.Name}: connected");
            _repository.Add(user);
            Added?.Invoke(user);
        }

        public IObservable<UserDto> GetChatLogsAsObservable()
        {
            var oldUsers = _repository.GetAll().ToObservable();
            var userRemoved = Observable.FromEvent<UserDto>((x) => Removed += x, (x) => Added -= x);
            var usedAdded = Observable.FromEvent<UserDto>((x) => Added += x, (x) => Added -= x);

            return oldUsers.Concat(userRemoved).Concat(usedAdded);
        }
    }
}
