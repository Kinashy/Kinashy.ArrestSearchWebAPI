﻿namespace Kinashy.ArrestSearchWebAPI.Data.Auth
{
    public class UserRepository : IUserRepository
    {
        private List<UserDto> _users = new()
        {
            new UserDto("John", "123"),
            new UserDto("Monica", "123"),
            new UserDto("Nancy", "123")
        };
        public UserDto GetUser(UserModel userModel) =>
            _users.FirstOrDefault(u =>
            string.Equals(u.userName, userModel.UserName) &&
            string.Equals(u.password, userModel.Password)) ?? throw new Exception();
    }
}
