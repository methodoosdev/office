using App.Core.Domain.Assignment;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Assignment
{
    public partial interface IAssignmentPrototypeService
    {
        IQueryable<AssignmentPrototype> Table { get; }
        Task<AssignmentPrototype> GetAssignmentPrototypeByIdAsync(int assignmentPrototypeId);
        Task<IList<AssignmentPrototype>> GetAssignmentPrototypesByIdsAsync(int[] assignmentPrototypeIds);
        Task<IList<AssignmentPrototype>> GetAllAssignmentPrototypesAsync(bool showInActive = false);
        Task DeleteAssignmentPrototypeAsync(AssignmentPrototype assignmentPrototype);
        Task DeleteAssignmentPrototypeAsync(IList<AssignmentPrototype> assignmentPrototypes);
        Task InsertAssignmentPrototypeAsync(AssignmentPrototype assignmentPrototype);
        Task UpdateAssignmentPrototypeAsync(AssignmentPrototype assignmentPrototype);
    }
    public partial class AssignmentPrototypeService : IAssignmentPrototypeService
    {
        private readonly IRepository<AssignmentPrototype> _assignmentPrototypeRepository;

        public AssignmentPrototypeService(
            IRepository<AssignmentPrototype> assignmentPrototypeRepository)
        {
            _assignmentPrototypeRepository = assignmentPrototypeRepository;
        }

        public virtual IQueryable<AssignmentPrototype> Table => _assignmentPrototypeRepository.Table;

        public virtual async Task<AssignmentPrototype> GetAssignmentPrototypeByIdAsync(int assignmentPrototypeId)
        {
            return await _assignmentPrototypeRepository.GetByIdAsync(assignmentPrototypeId);
        }

        public virtual async Task<IList<AssignmentPrototype>> GetAssignmentPrototypesByIdsAsync(int[] assignmentPrototypeIds)
        {
            return await _assignmentPrototypeRepository.GetByIdsAsync(assignmentPrototypeIds);
        }

        public virtual async Task<IList<AssignmentPrototype>> GetAllAssignmentPrototypesAsync(bool showInActive = false)
        {
            var entities = await _assignmentPrototypeRepository.GetAllAsync(query =>
            {
                if(!showInActive)
                    query = query.Where(x => x.InActive == false);

                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteAssignmentPrototypeAsync(AssignmentPrototype assignmentPrototype)
        {
            await _assignmentPrototypeRepository.DeleteAsync(assignmentPrototype);
        }

        public virtual async Task DeleteAssignmentPrototypeAsync(IList<AssignmentPrototype> assignmentPrototypes)
        {
            await _assignmentPrototypeRepository.DeleteAsync(assignmentPrototypes);
        }

        public virtual async Task InsertAssignmentPrototypeAsync(AssignmentPrototype assignmentPrototype)
        {
            await _assignmentPrototypeRepository.InsertAsync(assignmentPrototype);
        }

        public virtual async Task UpdateAssignmentPrototypeAsync(AssignmentPrototype assignmentPrototype)
        {
            await _assignmentPrototypeRepository.UpdateAsync(assignmentPrototype);
        }
    }
}