using App.Core.Domain.Scripts;
using App.Core.Infrastructure.Mapper;
using AutoMapper;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace App.Services.Scripts
{
    public partial interface IScriptCloneByTraderService
    {
        Task ClearAsync(int traderId);
        Task CloneAsync(int sourceTraderId, int targetTraderId);
    }
    public partial class ScriptCloneByTraderService : IScriptCloneByTraderService
    {
        private readonly IScriptFieldService _scriptFieldService;
        private readonly IScriptService _scriptService;
        private readonly IScriptToolService _scriptToolService;
        private readonly IScriptToolItemService _scriptToolItemService;
        private readonly IScriptGroupService _scriptGroupService;
        private readonly IScriptItemService _scriptItemService;
        private readonly IScriptPivotService _scriptPivotService;
        private readonly IScriptPivotItemService _scriptPivotItemService;
        private readonly IScriptTableService _scriptTableService;
        private readonly IScriptTableItemService _scriptTableItemService;

        public ScriptCloneByTraderService(
            IScriptFieldService scriptFieldService,
            IScriptService scriptService,
            IScriptToolService scriptToolService,
            IScriptToolItemService scriptToolItemService,
            IScriptGroupService scriptGroupService,
            IScriptItemService scriptItemService,
            IScriptTableService scriptTableService,
            IScriptTableItemService scriptTableItemService,
            IScriptPivotService scriptPivotService,
            IScriptPivotItemService scriptPivotItemService)
        {
            _scriptFieldService = scriptFieldService;
            _scriptService = scriptService;
            _scriptToolService = scriptToolService;
            _scriptToolItemService = scriptToolItemService;
            _scriptGroupService = scriptGroupService;
            _scriptItemService = scriptItemService;
            _scriptPivotService = scriptPivotService;
            _scriptPivotItemService = scriptPivotItemService;
            _scriptTableService = scriptTableService;
            _scriptTableItemService = scriptTableItemService;
        }

        public async Task ClearAsync(int traderId)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _scriptToolService.DeleteScriptToolAsync(x => x.TraderId == traderId);
            await _scriptService.DeleteScriptAsync(x => x.TraderId == traderId);
            await _scriptPivotService.DeleteScriptPivotAsync(x => x.TraderId == traderId);
            await _scriptFieldService.DeleteScriptFieldAsync(x => x.TraderId == traderId);
            await _scriptTableService.DeleteScriptTableAsync(x => x.TraderId == traderId);
            await _scriptGroupService.DeleteScriptGroupAsync(x => x.TraderId == traderId);

            transaction.Complete();
        }
        public async Task CloneAsync(int sourceTraderId, int targetTraderId)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // 1) Load source data
            var groups = await _scriptGroupService.Table
                .Where(x => x.TraderId == sourceTraderId)
                .ToListAsync();

            // Tables depend on groups
            var tables = await _scriptTableService.Table
                .Where(x => x.TraderId == sourceTraderId && groups.Select(g => g.Id).Contains(x.ScriptGroupId))
                .ToListAsync();

            var tableItems = await _scriptTableItemService.Table
                .Where(x => tables.Select(t => t.Id).Contains(x.ScriptTableId))
                .ToListAsync();

            // Fields depend on tables/groups/trader
            var fields = await _scriptFieldService.Table
                .Where(x => x.TraderId == sourceTraderId
                            && groups.Select(g => g.Id).Contains(x.ScriptGroupId)
                            && tables.Select(t => t.Id).Contains(x.ScriptTableId))
                .ToListAsync();

            // Scripts depend on trader/group
            var scripts = await _scriptService.Table
                .Where(x => x.TraderId == sourceTraderId
                            && groups.Select(g => g.Id).Contains(x.ScriptGroupId))
                .ToListAsync();

            // ScriptItems depend on ScriptId + ScriptFieldId (+ ParentId to Script.Id)
            var scriptItems = await _scriptItemService.Table
                .Where(x => scripts.Select(s => s.Id).Contains(x.ScriptId))
                .ToListAsync();

            // Pivots depend on trader/group; items depend on pivot+field
            var pivots = await _scriptPivotService.Table
                .Where(x => x.TraderId == sourceTraderId
                            && groups.Select(g => g.Id).Contains(x.ScriptGroupId))
                .ToListAsync();

            var pivotItems = await _scriptPivotItemService.Table
                .Where(x => pivots.Select(p => p.Id).Contains(x.ScriptPivotId))
                .ToListAsync();

            // Tools depend on trader; items depend on tool + script
            var tools = await _scriptToolService.Table
                .Where(x => x.TraderId == sourceTraderId)
                .ToListAsync();

            var toolItems = await _scriptToolItemService.Table
                .Where(x => tools.Select(t => t.Id).Contains(x.ScriptToolId))
                .ToListAsync();

            // 2) Old→New ID maps
            var groupMap = new Dictionary<int, int>();
            var tableMap = new Dictionary<int, int>();
            var fieldMap = new Dictionary<int, int>();
            var scriptMap = new Dictionary<int, int>();
            var pivotMap = new Dictionary<int, int>();
            var toolMap = new Dictionary<int, int>();

            // 3) Insert in dependency order, remapping FKs

            // 3.1 Groups
            foreach (var g in groups)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<ScriptGroup>(g);
                clone.Id = 0;
                clone.TraderId = targetTraderId;

                await _scriptGroupService.InsertScriptGroupAsync(clone);
                groupMap[g.Id] = clone.Id;
            }

            // 3.2 Tables
            foreach (var t in tables)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<ScriptTable>(t);
                clone.Id = 0;
                clone.TraderId = targetTraderId;
                clone.ScriptGroupId = groupMap[t.ScriptGroupId];

                await _scriptTableService.InsertScriptTableAsync(clone);
                tableMap[t.Id] = clone.Id;
            }

            // 3.3 TableItems
            foreach (var ti in tableItems)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<ScriptTableItem>(ti);
                clone.Id = 0;
                clone.ScriptTableId = tableMap[ti.ScriptTableId];

                await _scriptTableItemService.InsertScriptTableItemAsync(clone);
            }

            // 3.4 Fields
            foreach (var f in fields)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<ScriptField>(f);
                clone.Id = 0;
                clone.TraderId = targetTraderId;
                clone.ScriptGroupId = groupMap[f.ScriptGroupId];
                clone.ScriptTableId = tableMap[f.ScriptTableId];

                await _scriptFieldService.InsertScriptFieldAsync(clone);
                fieldMap[f.Id] = clone.Id;
            }

            // 3.5 Scripts
            foreach (var s in scripts)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<Script>(s);
                clone.Id = 0;
                clone.TraderId = targetTraderId;
                clone.ScriptGroupId = groupMap[s.ScriptGroupId];

                await _scriptService.InsertScriptAsync(clone);
                scriptMap[s.Id] = clone.Id;
            }

            // 3.6 ScriptItems (needs scriptMap, fieldMap, and ParentId→scriptMap if set)
            foreach (var si in scriptItems)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<ScriptItem>(si);
                clone.Id = 0;
                clone.ScriptId = scriptMap[si.ScriptId];
                clone.ScriptFieldId = fieldMap[si.ScriptFieldId];

                if (si.ParentId != 0 && scriptMap.TryGetValue(si.ParentId, out var newParent))
                    clone.ParentId = newParent;
                else if (si.ParentId != 0)
                    throw new InvalidOperationException($"Parent Script {si.ParentId} not found in source set.");

                await _scriptItemService.InsertScriptItemAsync(clone);
            }

            // 3.7 Pivots
            foreach (var p in pivots)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<ScriptPivot>(p);
                clone.Id = 0;
                clone.TraderId = targetTraderId;
                clone.ScriptGroupId = groupMap[p.ScriptGroupId];

                await _scriptPivotService.InsertScriptPivotAsync(clone);
                pivotMap[p.Id] = clone.Id;
            }

            // 3.8 PivotItems
            foreach (var pi in pivotItems)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<ScriptPivotItem>(pi);
                clone.Id = 0;
                clone.ScriptPivotId = pivotMap[pi.ScriptPivotId];
                clone.ScriptFieldId = fieldMap[pi.ScriptFieldId];

                await _scriptPivotItemService.InsertScriptPivotItemAsync(clone);
            }

            // 3.9 Tools
            foreach (var t in tools)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<ScriptTool>(t);
                clone.Id = 0;
                clone.TraderId = targetTraderId;

                await _scriptToolService.InsertScriptToolAsync(clone);
                toolMap[t.Id] = clone.Id;
            }

            // 3.10 ToolItems
            foreach (var ti in toolItems)
            {
                var clone = AutoMapperConfiguration.Mapper.Map<ScriptToolItem>(ti);
                clone.Id = 0;
                clone.ScriptToolId = toolMap[ti.ScriptToolId];
                clone.ScriptId = scriptMap[ti.ScriptId];

                await _scriptToolItemService.InsertScriptToolItemAsync(clone);
            }

            transaction.Complete();
        }
    }
}