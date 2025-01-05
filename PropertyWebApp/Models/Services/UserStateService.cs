namespace PropertyWebApp.Models.Services
{
    public class UserStateService
    {
        public string UserName { get; private set; } = "NEZNAMY";
        public string Id { get; private set; }

        public Task SetUserName(string userName, string id)
        {
            UserName = userName;
            Id = id;
            return Task.CompletedTask;
        }
    }
}
