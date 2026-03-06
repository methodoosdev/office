using App.Services.ScheduleTasks;

namespace App.Services.Payroll
{
    public partial class PersonTermExpiryTask : IScheduleTask
    {
        private readonly IPersonTermExpiryService _personTermExpiryService;

        public PersonTermExpiryTask(IPersonTermExpiryService personTermExpiryService)
        {
            _personTermExpiryService = personTermExpiryService;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _personTermExpiryService.Check();
        }
    }
}