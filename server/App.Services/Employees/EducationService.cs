using App.Core.Domain.Employees;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Employees
{
    public partial interface IEducationService
    {
        IQueryable<Education> Table { get; }
        Task<Education> GetEducationByIdAsync(int educationId);
        Task<IList<Education>> GetEducationsByIdsAsync(int[] educationIds);
        Task<IList<Education>> GetAllEducationsAsync();
        Task DeleteEducationAsync(Education education);
        Task DeleteEducationAsync(IList<Education> educations);
        Task InsertEducationAsync(Education education);
        Task UpdateEducationAsync(Education education);
    }
    public partial class EducationService : IEducationService
    {
        private readonly IRepository<Education> _educationRepository;

        public EducationService(
            IRepository<Education> educationRepository)
        {
            _educationRepository = educationRepository;
        }

        public virtual IQueryable<Education> Table => _educationRepository.Table;

        public virtual async Task<Education> GetEducationByIdAsync(int educationId)
        {
            return await _educationRepository.GetByIdAsync(educationId);
        }

        public virtual async Task<IList<Education>> GetEducationsByIdsAsync(int[] educationIds)
        {
            return await _educationRepository.GetByIdsAsync(educationIds);
        }

        public virtual async Task<IList<Education>> GetAllEducationsAsync()
        {
            var entities = await _educationRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteEducationAsync(Education education)
        {
            await _educationRepository.DeleteAsync(education);
        }

        public virtual async Task DeleteEducationAsync(IList<Education> educations)
        {
            await _educationRepository.DeleteAsync(educations);
        }

        public virtual async Task InsertEducationAsync(Education education)
        {
            await _educationRepository.InsertAsync(education);
        }

        public virtual async Task UpdateEducationAsync(Education education)
        {
            await _educationRepository.UpdateAsync(education);
        }
    }
}