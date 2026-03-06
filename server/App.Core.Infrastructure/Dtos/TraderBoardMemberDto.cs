using App.Core.Infrastructure.Dtos.Trader;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Infrastructure.Dtos
{
    public partial class TraderBoardMemberDto
    {
        public TraderBoardMemberDto()
        {
            Messages = new List<string>();
            Errors = new List<string>();
            TraderRelationships = new List<TraderRelationshipDto>();
            TraderMemberships = new List<TraderMembershipDto>();
        }

        public bool Success => !Errors.Any();
        public string Message => Messages.FirstOrDefault();
        public string Error => Errors.FirstOrDefault();

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public List<string> Messages { get; set; }
        public List<string> Errors { get; set; }
        public List<TraderRelationshipDto> TraderRelationships { get; set; }
        public List<TraderMembershipDto> TraderMemberships { get; set; }
    }
}