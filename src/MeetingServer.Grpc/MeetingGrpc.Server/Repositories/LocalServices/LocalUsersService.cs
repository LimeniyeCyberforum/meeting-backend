using MeetingGrpc.Server.Model;
using MeetingGrpc.Server.Repositories;
using MeetingGrpc.Server.Repositories.LocalServices;
using System.Reactive.Linq;

namespace MeetingGrpc.Repositories.LocalServices
{
    public class LocalUsersService
    {
        private readonly ILogger<LocalUsersService> _logger;

        private readonly IRepository<Guid, User> _repository;

        public LocalUsersService(ILogger<LocalUsersService> logger, IRepository<Guid, User> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        private event Action<(EventAction, User)> UsersChanged;

        public void Add(User user)
        {
            _repository.Add(user.UserGuid, user);
            UsersChanged?.Invoke((EventAction.Added, user));
            _logger.LogInformation($"{user.Name}: connected");
        }

        public void Remove(User user)
        {
            _repository.Remove(user.UserGuid);
            UsersChanged?.Invoke((EventAction.Removed, user));
            _logger.LogInformation($"{user.Name}: left");
        }

        public User? GetUserFromToken(string token)
        {
            var onlyToken = token.Replace("Bearer ", string.Empty);
           return _repository.GetAll().FirstOrDefault(x => string.Equals(x.Token?.JwtToken, onlyToken));
        }

        public bool IsNameExists(string? name)
        {
            _logger.LogInformation($"{name} is exists?");
            var user = _repository.GetAll()?.FirstOrDefault(x => x.Name == name);
            return user == null ? false : true;
        }

        public IObservable<(EventAction Action, User User)> GetUsersAsObservable()
        {
            var oldUsers = _repository.GetAll().Select(x => (EventAction.Added, x)).ToObservable();
            var usersChanged = Observable.FromEvent<(EventAction, User)>((x) => UsersChanged += x, (x) => UsersChanged -= x);

            return oldUsers.Concat(usersChanged);
        }
    }
}
