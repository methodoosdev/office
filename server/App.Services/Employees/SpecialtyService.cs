using App.Core;
using App.Core.Domain.Employees;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Employees
{
    public partial interface ISpecialtyService
    {
        IQueryable<Specialty> Table { get; }
        Task<Specialty> GetSpecialtyByIdAsync(int specialtyId);
        Task<IList<Specialty>> GetSpecialtiesByIdsAsync(int[] specialtyIds);
        Task<IList<Specialty>> GetAllSpecialtiesAsync();
        Task DeleteSpecialtyAsync(Specialty specialty);
        Task DeleteSpecialtyAsync(IList<Specialty> specialties);
        Task InsertSpecialtyAsync(Specialty specialty);
        Task UpdateSpecialtyAsync(Specialty specialty);
    }
    public partial class SpecialtyService : ISpecialtyService
    {
        private readonly IRepository<Specialty> _specialtyRepository;

        public SpecialtyService(
            IRepository<Specialty> specialtyRepository)
        {
            _specialtyRepository = specialtyRepository;
        }

        public virtual IQueryable<Specialty> Table => _specialtyRepository.Table;

        public virtual async Task<Specialty> GetSpecialtyByIdAsync(int specialtyId)
        {
            return await _specialtyRepository.GetByIdAsync(specialtyId);
        }

        public virtual async Task<IList<Specialty>> GetSpecialtiesByIdsAsync(int[] specialtyIds)
        {
            return await _specialtyRepository.GetByIdsAsync(specialtyIds);
        }

        public virtual async Task<IList<Specialty>> GetAllSpecialtiesAsync()
        {
            var entities = await _specialtyRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteSpecialtyAsync(Specialty specialty)
        {
            await _specialtyRepository.DeleteAsync(specialty);
        }

        public virtual async Task DeleteSpecialtyAsync(IList<Specialty> specialties)
        {
            await _specialtyRepository.DeleteAsync(specialties);
        }

        public virtual async Task InsertSpecialtyAsync(Specialty specialty)
        {
            await _specialtyRepository.InsertAsync(specialty);
        }

        public virtual async Task UpdateSpecialtyAsync(Specialty specialty)
        {
            await _specialtyRepository.UpdateAsync(specialty);
        }
    }
}