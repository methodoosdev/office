using App.Core.Domain.Assignment;
using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Assignment
{
    public partial interface IAssignmentPrototypeAssignmentPrototypeActionMappingService
    {
        IQueryable<AssignmentPrototypeAssignmentPrototypeActionMapping> Table { get; }
        Task<IList<AssignmentPrototype>> GetAssignmentPrototypesByAssignmentPrototypeActionIdAsync(int assignmentPrototypeActionId);
        Task<IList<AssignmentPrototypeAction>> GetAssignmentPrototypeActionsByAssignmentPrototypeIdAsync(int assignmentPrototypeId);
        Task RemoveAssignmentPrototypeAssignmentPrototypeActionAsync(AssignmentPrototype assignmentPrototype, AssignmentPrototypeAction assignmentPrototypeAction);
        Task InsertAssignmentPrototypeAssignmentPrototypeActionAsync(AssignmentPrototype assignmentPrototype, AssignmentPrototypeAction assignmentPrototypeAction);
    }
    public partial class AssignmentPrototypeAssignmentPrototypeActionMappingService : IAssignmentPrototypeAssignmentPrototypeActionMappingService
    {
        private readonly IRepository<AssignmentPrototypeAction> _assignmentPrototypeActionRepository;
        private readonly IRepository<AssignmentPrototype> _assignmentPrototypeRepository;
        private readonly IRepository<AssignmentPrototypeAssignmentPrototypeActionMapping> _assignmentPrototypeAssignmentPrototypeActionMappingRepository;

        public AssignmentPrototypeAssignmentPrototypeActionMappingService(
            IRepository<AssignmentPrototypeAction> assignmentPrototypeActionRepository,
            IRepository<AssignmentPrototype> assignmentPrototypeRepository,
            IRepository<AssignmentPrototypeAssignmentPrototypeActionMapping> assignmentPrototypeAssignmentPrototypeActionMappingRepository)
        {
            _assignmentPrototypeActionRepository = assignmentPrototypeActionRepository;
            _assignmentPrototypeRepository = assignmentPrototypeRepository;
            _assignmentPrototypeAssignmentPrototypeActionMappingRepository = assignmentPrototypeAssignmentPrototypeActionMappingRepository;
        }

        public virtual IQueryable<AssignmentPrototypeAssignmentPrototypeActionMapping> Table => _assignmentPrototypeAssignmentPrototypeActionMappingRepository.Table;

        public virtual async Task<IList<AssignmentPrototype>> GetAssignmentPrototypesByAssignmentPrototypeActionIdAsync(int assignmentPrototypeActionId)
        {
            return await _assignmentPrototypeRepository.GetAllAsync(query =>
            {
                return from cp in query
                       join cpc in _assignmentPrototypeAssignmentPrototypeActionMappingRepository.Table on cp.Id equals cpc.AssignmentPrototypeId
                       where cpc.AssignmentPrototypeActionId == assignmentPrototypeActionId
                       select cp;
            });
        }

        public virtual async Task<IList<AssignmentPrototypeAction>> GetAssignmentPrototypeActionsByAssignmentPrototypeIdAsync(int assignmentPrototypeId)
        {
            return await _assignmentPrototypeActionRepository.GetAllAsync(query =>
            {
                return from c in query
                       join cpc in _assignmentPrototypeAssignmentPrototypeActionMappingRepository.Table on c.Id equals cpc.AssignmentPrototypeActionId
                       where cpc.AssignmentPrototypeId == assignmentPrototypeId
                       select c;
            });
        }

        public virtual async Task RemoveAssignmentPrototypeAssignmentPrototypeActionAsync(AssignmentPrototype assignmentPrototype, AssignmentPrototypeAction assignmentPrototypeAction)
        {
            if (assignmentPrototype == null)
                throw new ArgumentNullException(nameof(assignmentPrototype));

            if (assignmentPrototypeAction is null)
                throw new ArgumentNullException(nameof(assignmentPrototypeAction));

            if (await _assignmentPrototypeAssignmentPrototypeActionMappingRepository.Table
                .FirstOrDefaultAsync(x => x.AssignmentPrototypeId == assignmentPrototype.Id && x.AssignmentPrototypeActionId == assignmentPrototypeAction.Id) is AssignmentPrototypeAssignmentPrototypeActionMapping mapping)
            {
                await _assignmentPrototypeAssignmentPrototypeActionMappingRepository.DeleteAsync(mapping);
            }
        }

        public virtual async Task InsertAssignmentPrototypeAssignmentPrototypeActionAsync(AssignmentPrototype assignmentPrototype, AssignmentPrototypeAction assignmentPrototypeAction)
        {
            if (assignmentPrototype is null)
                throw new ArgumentNullException(nameof(assignmentPrototype));

            if (assignmentPrototypeAction is null)
                throw new ArgumentNullException(nameof(assignmentPrototypeAction));

            if (await _assignmentPrototypeAssignmentPrototypeActionMappingRepository.Table
                .FirstOrDefaultAsync(x => x.AssignmentPrototypeId == assignmentPrototype.Id && x.AssignmentPrototypeActionId == assignmentPrototypeAction.Id) is null)
            {
                var mapping = new AssignmentPrototypeAssignmentPrototypeActionMapping
                {
                    AssignmentPrototypeId = assignmentPrototype.Id,
                    AssignmentPrototypeActionId = assignmentPrototypeAction.Id
                };

                await _assignmentPrototypeAssignmentPrototypeActionMappingRepository.InsertAsync(mapping);
            }
        }

    }
}