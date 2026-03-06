using App.Core.Infrastructure.Dtos.Trader;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Infrastructure.Dtos
{
    public partial class TraderBranchPageDto
    {
        public TraderBranchPageDto()
        {
            TraderKads = new List<TraderKadDto>();
            TraderBranches = new List<TraderBranchDto>();
            Errors = new List<string>();
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public bool Success => !Errors.Any();
        public string Error => Errors.FirstOrDefault();

        public List<TraderKadDto> TraderKads { get; set; }
        public List<TraderBranchDto> TraderBranches { get; set; }
        public List<string> Errors { get; set; }
    }
}