using App.Core.Domain.SimpleTask;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.SimpleTask
{
    public partial interface ISimpleTaskCategoryService
    {
        IQueryable<SimpleTaskCategory> Table { get; }
        Task<SimpleTaskCategory> GetSimpleTaskCategoryByIdAsync(int simpleTaskCategoryId);
        Task<IList<SimpleTaskCategory>> GetSimpleTaskCategoriesByIdsAsync(int[] simpleTaskCategoryIds);
        Task<IList<SimpleTaskCategory>> GetAllSimpleTaskCategoriesAsync();
        Task DeleteSimpleTaskCategoryAsync(SimpleTaskCategory simpleTaskCategory);
        Task DeleteSimpleTaskCategoryAsync(IList<SimpleTaskCategory> simpleTaskCategorys);
        Task InsertSimpleTaskCategoryAsync(SimpleTaskCategory simpleTaskCategory);
        Task UpdateSimpleTaskCategoryAsync(SimpleTaskCategory simpleTaskCategory);
    }
    public partial class SimpleTaskCategoryService : ISimpleTaskCategoryService
    {
        private readonly IRepository<SimpleTaskCategory> _simpleTaskCategoryRepository;

        public SimpleTaskCategoryService(
            IRepository<SimpleTaskCategory> simpleTaskCategoryRepository)
        {
            _simpleTaskCategoryRepository = simpleTaskCategoryRepository;
        }

        public virtual IQueryable<SimpleTaskCategory> Table => _simpleTaskCategoryRepository.Table;

        public virtual async Task<SimpleTaskCategory> GetSimpleTaskCategoryByIdAsync(int simpleTaskCategoryId)
        {
            return await _simpleTaskCategoryRepository.GetByIdAsync(simpleTaskCategoryId);
        }

        public virtual async Task<IList<SimpleTaskCategory>> GetSimpleTaskCategoriesByIdsAsync(int[] simpleTaskCategoryIds)
        {
            return await _simpleTaskCategoryRepository.GetByIdsAsync(simpleTaskCategoryIds);
        }

        public virtual async Task<IList<SimpleTaskCategory>> GetAllSimpleTaskCategoriesAsync()
        {
            var entities = await _simpleTaskCategoryRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteSimpleTaskCategoryAsync(SimpleTaskCategory simpleTaskCategory)
        {
            await _simpleTaskCategoryRepository.DeleteAsync(simpleTaskCategory);
        }

        public virtual async Task DeleteSimpleTaskCategoryAsync(IList<SimpleTaskCategory> simpleTaskCategorys)
        {
            await _simpleTaskCategoryRepository.DeleteAsync(simpleTaskCategorys);
        }

        public virtual async Task InsertSimpleTaskCategoryAsync(SimpleTaskCategory simpleTaskCategory)
        {
            await _simpleTaskCategoryRepository.InsertAsync(simpleTaskCategory);
        }

        public virtual async Task UpdateSimpleTaskCategoryAsync(SimpleTaskCategory simpleTaskCategory)
        {
            await _simpleTaskCategoryRepository.UpdateAsync(simpleTaskCategory);
        }
    }
}