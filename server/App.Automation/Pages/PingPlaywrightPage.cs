using System.Threading.Tasks;

namespace App.Automation.Pages
{
    public class PingPlaywrightPage : PageTest
    {
        public PingPlaywrightPage() : base(connectionId: string.Empty)
        {
        }

        protected override Task LogoutAsync()
        {
            return Task.CompletedTask;
        }
    }
}
