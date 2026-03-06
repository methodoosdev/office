using System.Threading.Tasks;

namespace App.Automation
{
    public interface IWorkerService
    {
        public Task BuildAsync(PlaywrightTest playwrightInstance);
        public Task ResetAsync();
        public Task DisposeAsync();
    }
}

