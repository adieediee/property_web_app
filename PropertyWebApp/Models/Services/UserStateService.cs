namespace PropertyWebApp.Models.Services
{
    public class UserStateService
    {
        public string UserName { get; private set; } = "";
        public string Id { get; private set; }

        public string Role { get; private set; }

        public Task SetUserInfo(string userName, string id)
        {
            UserName = userName;
            Id = id;
            return Task.CompletedTask;
        }
        public Task SetRole(string role)
        {
            Role = role;
            return Task.CompletedTask;
        }

        public void Clear()
        {
            UserName = "";
            Id = "";
            Role = "";
        }
    }
}
