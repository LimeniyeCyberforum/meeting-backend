using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public interface IUsersRepository
    {
        void Add(UserDto userDto);
        IEnumerable<UserDto> GetAll();
    }
}
