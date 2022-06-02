﻿using Grpc.Core;
using MeetingGrpc.Server.Model;
using MeetingGrpc.Server.Repositories;
using System.Reactive.Linq;

namespace MeetingGrpc.Repositories.LocalServices
{
    public class LocalUsersService
    {
        private readonly ILogger<LocalUsersService> _logger;

        private readonly IRepository<User> _repository;

        public LocalUsersService(ILogger<LocalUsersService> logger, IRepository<User> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        private event Action<User> Added;
        private event Action<User> Removed;

        public void Add(User user)
        {
            _repository.Add(user);
            Added?.Invoke(user);
            _logger.LogInformation($"{user.Name}: connected");
        }

        public void Remove(User user)
        {
            _repository.Remove(user);
            Removed?.Invoke(new User(user.UserGuid, user.Name, false, user.Token));
            _logger.LogInformation($"{user.Name}: left");
        }

        public User? GetUserFromToken(string token)
        {
           return _repository.GetAll().FirstOrDefault(x => string.Equals(x.Token?.JwtToken, token));
        }

        public bool IsNameExists(string? name)
        {
            _logger.LogInformation($"{name} is exists?");
            var user = _repository.GetAll()?.FirstOrDefault(x => x.Name == name);
            return user == null ? false : true;
        }

        public IObservable<User> GetUsersAsObservable()
        {
            var oldUsers = _repository.GetAll().ToObservable();
            var userRemoved = Observable.FromEvent<User>((x) => Removed += x, (x) => Removed -= x);
            var usedAdded = Observable.FromEvent<User>((x) => Added += x, (x) => Added -= x);

            return oldUsers.Concat(userRemoved).Concat(usedAdded);
        }
    }
}
