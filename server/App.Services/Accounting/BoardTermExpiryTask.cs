using App.Services.ScheduleTasks;

namespace App.Services.Accounting
{
    public partial class BoardTermExpiryTask : IScheduleTask
    {
        private readonly IBoardTermExpiryService _boardTermExpiryService;

        public BoardTermExpiryTask(IBoardTermExpiryService boardTermExpiryService)
        {
            _boardTermExpiryService = boardTermExpiryService;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _boardTermExpiryService.Check();
        }
    }
}