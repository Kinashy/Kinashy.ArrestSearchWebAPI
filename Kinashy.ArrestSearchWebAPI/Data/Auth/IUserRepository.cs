namespace Kinashy.ArrestSearchWebAPI.Data.Auth
{
    public interface IUserRepository
    {
        UserDto GetUser(UserModel userModel);
    }
}
