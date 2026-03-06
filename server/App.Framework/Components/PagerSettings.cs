using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Framework.Components
{
    public class AggregateItem
    {
        public string Field { get; set; }
        public string Aggregate { get; set; }
    }

    public class GridGroupDescriptor
    {
        public string Field { get; set; }
        public string Dir { get; set; }
        public IList<AggregateItem> Aggregates { get; set; } = new List<AggregateItem>();
    }

    public class GridSortDescriptor
    {
        public string Field { get; set; }
        public string Dir { get; set; }
    }

    public class GridState
    {
        public GridState(string field, string dir) 
        {
            Group = new List<GridGroupDescriptor>();
            Sort = new List<GridSortDescriptor>
            {
                new GridSortDescriptor { Field = field, Dir = dir }
            };
        }

        public int Skip { get; set; }
        public int Take { get; set; }
        public IList<GridGroupDescriptor> Group { get; set; }
        public IList<GridSortDescriptor> Sort { get; set; }
    }

    public class PagerSettings
    {
        public PagerSettings() { }

        public PagerSettings(string pageSizesValue, int buttonCount = 3, bool info = true, string type = "numeric",
            bool previousNext = true, bool responsive = false, string position = "bottom")
        {
            PageSizes = pageSizesValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x, out var pageSize) ? pageSize : 0)
                .Distinct()
                .ToList().Where(w => w > 0).OrderBy(o => o).ToList();
            ButtonCount = buttonCount;
            Info = info;
            Type = type;
            PreviousNext = previousNext;
            Responsive = responsive;
            Position = position;
        }

        public int ButtonCount { get; set; } // Defaults to 10.
        public bool Info { get; set; } // Defaults to true.
        public string Type { get; set; } // Defaults to numeric.
        public List<int> PageSizes { get; set; } // Defaults to false.
        public bool PreviousNext { get; set; } // Defaults to true.
        public bool Responsive { get; set; } // Defaults to true.
        public string Position { get; set; } // The available values are top, bottom, and both. Defaults to bottom.
    }
}