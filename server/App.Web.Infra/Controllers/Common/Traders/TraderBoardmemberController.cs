using App.Core;
using App.Core.Domain.Logging;
using App.Core.Domain.Traders;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Mapper;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class TraderBoardMemberController : BaseProtectController
    {
        //private readonly IHubContext<ChatHub> _hub;
        private readonly ITraderService _traderService;
        private readonly ITraderRelationshipService _traderRelationshipService;
        private readonly ITraderMembershipService _traderMembershipService;
        private readonly ITraderBoardMemberTypeService _traderBoardMemberTypeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly PlaywrightHttpClient _httpClient;

        public TraderBoardMemberController(
            //IHubContext<ChatHub> hub,
            ITraderService traderService,
            ITraderRelationshipService traderRelationshipService,
            ITraderMembershipService traderMembershipService,
            ITraderBoardMemberTypeService traderBoardMemberTypeService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            PlaywrightHttpClient httpClient)
        {
            //_hub = hub;
            _traderService = traderService;
            _traderRelationshipService = traderRelationshipService;
            _traderMembershipService = traderMembershipService;
            _traderBoardMemberTypeService = traderBoardMemberTypeService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _httpClient = httpClient;
        }

        private async Task<int> GetBoardMemberPerson(string vat, string lastName)
        {
            var trader = await _traderService.GetTraderByVatAsync(vat);
            if (trader == null)
            {
                trader = new Trader { 
                    Vat = AesEncryption.Encrypt(vat), LastName = lastName, 
                    NonRepresentationOfNaturalPerson = true, 
                    CustomerTypeId = (int)CustomerType.NaturalPerson
                };
                await _traderService.InsertTraderAsync(trader);
            }

            return trader.Id;
        }


        [HttpPost]
        public virtual async Task<IActionResult> Import([FromBody] ICollection<int> selectedIds, string connectionId)
        {
            var traders = await _traderService.GetTradersByIdsAsync(selectedIds.ToArray());

            // 1 Προσωπική Εταιρεία, 2 Νομικό Πρόσωπο, 3 Κοινοπραξία, 4 Κοινωνία
            traders = traders
                .Where(k => !k.Deleted && k.Active).ToList();

            var custActivity = new CustomerActivityResult();

            foreach (var _trader in traders)
            {
                var trader = _trader.ToModel<TraderModel>();

                var traderName = trader.FullName();

                var format = "{0}?traderName={1}&userName={2}&password={3}&connectionId={4}";
                var url = string.Format(format,
                    "api/traderBoardMember/list",
                    WebUtility.UrlEncode(trader.FullName()),
                    WebUtility.UrlEncode(trader.TaxisUserName.Trim()),
                    WebUtility.UrlEncode(trader.TaxisPassword.Trim()),
                    connectionId == "undefined" ? null : connectionId);

                var result = await _httpClient.SendAsync(HttpMethod.Post, url);
                if (result.Success)
                {
                    var response = JsonConvert.DeserializeObject<TraderBoardMemberDto>(result.Content);
                    if (response.Success)
                    {
                        foreach (var relation in response.TraderRelationships)
                        {
                            var vat = relation.Vat?.Trim();
                            var relationshipName = relation.RelationshipName?.Trim();
                            if (string.IsNullOrEmpty(relationshipName) || string.IsNullOrEmpty(vat))
                            {
                                custActivity.AddError($"<b>Εσφαλμένη πληρ.ΑΑΔΕ :</b> {trader.FullName()}");
                                break;
                            }

                            var name = relation.RelationshipName.Trim();
                            var traderBoardMemberType = await _traderBoardMemberTypeService.Table.FirstOrDefaultAsync(x => x.Name == name);
                            if (traderBoardMemberType == null)
                            {
                                var boardMemberTypeId = name == "ΔΙΕΥΘΥΝΩΝ ΣΥΜΒΟΥΛΟΣ" || name == "ΔΙΑΧΕΙΡΙΣΤΗΣ" ? (int)BoardMemberType.Admin : (int)BoardMemberType.Member;
                                traderBoardMemberType = new TraderBoardMemberType { Name = name, BoardMemberTypeId = boardMemberTypeId };
                                await _traderBoardMemberTypeService.InsertTraderBoardMemberTypeAsync(traderBoardMemberType);
                            }

                            relation.TraderId = trader.Id;
                            relation.TraderBoardMemberTypeId = traderBoardMemberType.Id;
                            relation.ParentId = await GetBoardMemberPerson(vat.Trim(), relation.SurnameFatherName.Trim());
                        }

                        foreach (var member in response.TraderMemberships)
                        {
                            var vat = member.Vat?.Trim();
                            var participationName = member.ParticipationName?.Trim();
                            if (string.IsNullOrEmpty(participationName) || string.IsNullOrEmpty(vat))
                            {
                                custActivity.AddError($"<b>Εσφαλμένη πληρ.ΑΑΔΕ :</b> {trader.FullName()}");
                                break;
                            }

                            var name = member.ParticipationName.Trim();
                            var traderBoardMemberType = await _traderBoardMemberTypeService.Table.FirstOrDefaultAsync(x => x.Name == name);
                            if (traderBoardMemberType == null)
                            {
                                var boardMemberTypeId = name == "ΔΙΕΥΘΥΝΩΝ ΣΥΜΒΟΥΛΟΣ" || name == "ΔΙΑΧΕΙΡΙΣΤΗΣ" ? (int)BoardMemberType.Admin : (int)BoardMemberType.Member;
                                traderBoardMemberType = new TraderBoardMemberType { Name = name, BoardMemberTypeId = boardMemberTypeId };
                                await _traderBoardMemberTypeService.InsertTraderBoardMemberTypeAsync(traderBoardMemberType);
                            }

                            member.TraderId = trader.Id;
                            member.TraderBoardMemberTypeId = traderBoardMemberType.Id;
                            member.ParentId = await GetBoardMemberPerson(vat.Trim(), member.SurnameFatherName.Trim());
                        }

                        // Delete old entities
                        var deletedTraderRelationships = await _traderRelationshipService.GetAllTraderRelationshipsAsync(trader.Id);
                        var deletedTraderMemberships = await _traderMembershipService.GetAllTraderMembershipsAsync(trader.Id);

                        await _traderRelationshipService.DeleteTraderRelationshipAsync(deletedTraderRelationships);
                        await _traderMembershipService.DeleteTraderMembershipAsync(deletedTraderMemberships);

                        // Insert new entities
                        var traderRelationships = response.TraderRelationships.Select(x => AutoMapperConfiguration.Mapper.Map<TraderRelationship>(x)).ToList();
                        await _traderRelationshipService.InsertTraderRelationshipAsync(traderRelationships);
                        custActivity.AddSuccess($"<b>Δημιουργία σχέσεις νομικού προσώπου:</b> {traderName}");

                        var traderMemberships = response.TraderMemberships.Select(x => AutoMapperConfiguration.Mapper.Map<TraderMembership>(x)).ToList();
                        await _traderMembershipService.InsertTraderMembershipAsync(traderMemberships);
                        custActivity.AddSuccess($"<b>Δημιουργία στοιχείων μελών νομ.προσώπου:</b> {traderName}");
                    }
                    else
                        custActivity.AddError(response.Error);
                }
                else
                    custActivity.AddError(result.Error);
            }

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.TraderBoardMember, custActivity.ToString());

            return Ok();
        }
    }
}