using App.Core.Domain.Employees;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Employees
{
    public partial interface IJobTitleService
    {
        IQueryable<JobTitle> Table { get; }
        Task<JobTitle> GetJobTitleByIdAsync(int jobTitleId);
        Task<IList<JobTitle>> GetJobTitlesByIdsAsync(int[] jobTitleIds);
        Task<IList<JobTitle>> GetAllJobTitlesAsync();
        Task DeleteJobTitleAsync(JobTitle jobTitle);
        Task DeleteJobTitleAsync(IList<JobTitle> jobTitles);
        Task InsertJobTitleAsync(JobTitle jobTitle);
        Task UpdateJobTitleAsync(JobTitle jobTitle);
    }
    public partial class JobTitleService : IJobTitleService
    {
        private readonly IRepository<JobTitle> _jobTitleRepository;

        public JobTitleService(
            IRepository<JobTitle> jobTitleRepository)
        {
            _jobTitleRepository = jobTitleRepository;
        }

        public virtual IQueryable<JobTitle> Table => _jobTitleRepository.Table;

        public virtual async Task<JobTitle> GetJobTitleByIdAsync(int jobTitleId)
        {
            return await _jobTitleRepository.GetByIdAsync(jobTitleId);
        }

        public virtual async Task<IList<JobTitle>> GetJobTitlesByIdsAsync(int[] jobTitleIds)
        {
            return await _jobTitleRepository.GetByIdsAsync(jobTitleIds);
        }

        public virtual async Task<IList<JobTitle>> GetAllJobTitlesAsync()
        {
            var entities = await _jobTitleRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteJobTitleAsync(JobTitle jobTitle)
        {
            await _jobTitleRepository.DeleteAsync(jobTitle);
        }

        public virtual async Task DeleteJobTitleAsync(IList<JobTitle> jobTitles)
        {
            await _jobTitleRepository.DeleteAsync(jobTitles);
        }

        public virtual async Task InsertJobTitleAsync(JobTitle jobTitle)
        {
            await _jobTitleRepository.InsertAsync(jobTitle);
        }

        public virtual async Task UpdateJobTitleAsync(JobTitle jobTitle)
        {
            await _jobTitleRepository.UpdateAsync(jobTitle);
        }
    }
}