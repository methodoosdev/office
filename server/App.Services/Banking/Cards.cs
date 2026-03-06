using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace App.Services.Banking
{
    public class CardDetails
    {
        public CardDetailItem Payload { get; set; }
        public double ExecutionTime { get; set; }
    }

    public class CardDetail
    {
        public CardDetailItem Card { get; set; }
    }

    public class CardDetailItem
    {
        public string ResourcerId { get; set; }
        public string BankBIC { get; set; }
        public string Number { get; set; }
        public string ProductName { get; set; }
        public string Alias { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal CreditLine { get; set; }
        public string CardHolderNameLatin { get; set; }
        public string Kind { get; set; }
    }

    public class CardLists
    {
        [JsonProperty("payload")]
        [JsonConverter(typeof(PayloadConverter))]
        public List<CardListItem> Cards { get; set; }
        public double ExecutionTime { get; set; }
    }

    public class CardList
    {
        public List<CardListItem> Cards { get; set; }
    }

    public class CardListItem
    {
        public string Alias { get; set; }
        public double CreditBalance { get; set; }
        public string Kind { get; set; }
        public double CreditLine { get; set; }
        public string CardHolderNameLatin { get; set; }
        public string ResourceId { get; set; }
        public string BankBIC { get; set; }
        public string Number { get; set; }
        public string ProductName { get; set; }
    }

    public class CardTransactions
    {
        public List<CardTransactionItem> Payload { get; set; }
        public double ExecutionTime { get; set; }
    }

    public class CardTransactionItem
    {
        public DateTime TransactionDateTime { get; set; }
        public string ReasonText { get; set; }
        public string Reference { get; set; }
        public string CreditDebitFlag { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public decimal LedgerBalance { get; set; }
        public MerchantDetails MerchantDetails { get; set; }
    }

    public class MerchantDetails
    {
        public string MerchantName { get; set; }
        public string MerchantCategoryCode { get; set; }
    }
    public class CardTransactionsPaged
    {
        public CardTransactionPaged Payload { get; set; }
        public double ExecutionTime { get; set; }
    }

    public class CardTransactionPaged
    {
        public List<CardTransactionItem> Data { get; set; }
        public string PaginationToken { get; set; }
    }
    public class PayloadConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(List<CardListItem>);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            // Case A: payload is directly an array
            if (token.Type == JTokenType.Array)
                return token.ToObject<List<CardListItem>>(serializer);

            // Case B: payload is an object with a "cards" array inside
            if (token.Type == JTokenType.Object && token["cards"]?.Type == JTokenType.Array)
                return token["cards"].ToObject<List<CardListItem>>(serializer);

            // Neither shape matches
            throw new JsonSerializationException("Unexpected payload format");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // If you ever want to serialize back, just write the array directly
            serializer.Serialize(writer, value);
        }
    }
}
