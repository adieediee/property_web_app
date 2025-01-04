using Microsoft.AspNetCore.Components.Authorization;

namespace PropertyWebApp.Models.Services
{
    public class UserStateService
    {
        public string UserName { get; private set; } = "NEZNAMY";

        // Event na notifikáciu Blazor komponentov o zmene
        public event Action? OnChange;

        public void SetUserName(string userName)
        {
            UserName = userName;
            //NotifyStateChanged();
        }
        
        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }

}
