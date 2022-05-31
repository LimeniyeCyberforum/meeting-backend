using MeetingGrpc.Server.Model;
using MeetingGrpc.Server.Repositories;
using System.Reactive.Linq;

namespace MeetingGrpc.Repositories.LocalServices
{
    public class UsersService
    {
        private readonly ILogger<UsersService> _logger;

        private readonly IRepository<UserDto> _repository;

        public UsersService(ILogger<UsersService> logger, IRepository<UserDto> repository)
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

        public bool IsNameExists(string? name)
        {
            _logger.LogInformation($"{name} is exists?");
            var user = _repository.GetAll()?.FirstOrDefault(x => x.Name == name);
            return user == null ? false : true;
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
