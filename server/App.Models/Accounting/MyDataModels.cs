using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace App.Models.Accounting
{
    public class RequestedBookInfoResult
    {
        public bookInfoResult bookInfo { get; set; }
    }
    public class bookInfoResult
    {
        public string counterVatNumber { get; set; }
        public DateTime issueDate { get; set; }
        public string invType { get; set; }
        public bool selfpricing { get; set; }
        public int invoiceDetailType { get; set; }
        public double netValue { get; set; }
        public double vatAmount { get; set; }
        public double withheldAmount { get; set; }
        public double otherTaxesAmount { get; set; }
        public double stampDutyAmount { get; set; }
        public double feesAmount { get; set; }
        public double deductionsAmount { get; set; }
        public double thirdPartyAmount { get; set; }
        public double grossValue { get; set; }
        public int count { get; set; }
        public string minMark { get; set; }
        public string maxMark { get; set; }
    }

    public class issuerResult
    {
        public string vatNumber { get; set; }
        public string country { get; set; }
        public string branch { get; set; }
    }
    public class partyResult
    {
        public string vatNumber { get; set; }
        public string country { get; set; }
        public int branch { get; set; }
        public string name { get; set; }
        public addressResult address { get; set; }
        public string documentIdNo { get; set; }
        public string supplyAccountNo { get; set; }
    }
    public class addressResult
    {
        public string street { get; set; }
        public string number { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
    }
    public class invoiceHeaderResult
    {
        public string series { get; set; }
        public string aa { get; set; }
        public DateTime issueDate { get; set; }
        public string invoiceType { get; set; }
        public bool vatPaymentSuspension { get; set; }
        public string currency { get; set; }
        public decimal exchangeRate { get; set; }
        public long correlatedInvoices { get; set; }
        public bool selfPricing { get; set; }
        public DateTime dispatchDate { get; set; }
        public string dispatchTime { get; set; }
        public string vehicleNumber { get; set; }
        public int movePurpose { get; set; }
        public bool fuelInvoice { get; set; }
        public int specialInvoiceCategory { get; set; }
        public int invoiceVariationType { get; set; }
    }
    public class paymentMethodDetailsResult
    {
        public int type { get; set; }
        public decimal amount { get; set; }
        public string paymentMethodInfo { get; set; }
    }
    public class paymentMethodsResult
    {
        [JsonConverter(typeof(DynamicPropertyConverter<paymentMethodDetailsResult>))]
        public IList<paymentMethodDetailsResult> paymentMethodDetails { get; set; }
    }
    public class fuelCodesResult
    {
        public int type { get; set; }
        public decimal amount { get; set; }
        public string paymentMethodInfo { get; set; }
    }
    public class shipResult
    {
        public string applicationId { get; set; }
        public DateTime applicationDate { get; set; }
        public string doy { get; set; }
        public string shipID { get; set; }
    }
    public class incomeClassificationResult
    {
        public string classificationType { get; set; }
        public string classificationCategory { get; set; }
        public decimal amount { get; set; }
        public int id { get; set; }
    }
    public class expensesClassificationResult
    {
        public string classificationType { get; set; }
        public string classificationCategory { get; set; }
        public decimal amount { get; set; }
        public int id { get; set; }
    }

    public class invoiceRowResult
    {
        public int lineNumber { get; set; }
        public int recType { get; set; }
        public fuelCodesResult FuelCode { get; set; }
        public decimal quantity { get; set; }
        public int measurementUnit { get; set; }
        public int invoiceDetailType { get; set; }
        public decimal netValue { get; set; }
        public int vatCategory { get; set; }
        public decimal vatAmount { get; set; }
        public int vatExemptionCategory { get; set; }
        public shipResult dienergia { get; set; }
        public bool discountOption { get; set; }
        public decimal withheldAmount { get; set; }
        public int withheldPercentCategory { get; set; }
        public decimal stampDutyAmount { get; set; }
        public int stampDutyPercentCategory { get; set; }
        public decimal feesAmount { get; set; }
        public int feesPercentCategory { get; set; }
        public int otherTaxesPercentCategory { get; set; }
        public decimal otherTaxesAmount { get; set; }
        public decimal deductionsAmount { get; set; }
        public string lineComments { get; set; }
        [JsonIgnore]
        public incomeClassificationResult incomeClassification { get; set; }
        [JsonIgnore]
        public expensesClassificationResult expensesClassification { get; set; }
        public decimal quantity15 { get; set; }
        public string itemDescr { get; set; }
    }
    public class invoiceSummaryResult
    {
        public decimal totalNetValue { get; set; }
        public decimal totalVatAmount { get; set; }
        public decimal totalWithheldAmount { get; set; }
        public decimal totalFeesAmount { get; set; }
        public decimal totalStampDutyAmount { get; set; }
        public decimal totalOtherTaxesAmount { get; set; }
        public decimal totalDeductionsAmount { get; set; }
        public decimal totalGrossValue { get; set; }
        [JsonIgnore]
        public incomeClassificationResult incomeClassification { get; set; }
        [JsonIgnore]
        public expensesClassificationResult expensesClassification { get; set; }
    }

    public class taxesResult
    {
        public int taxType { get; set; }
        public int taxCategory { get; set; }
        public decimal underlyingValue { get; set; }
        public decimal taxAmount { get; set; }
        public int id { get; set; }
    }
    public class taxesTotalsResult
    {
        [JsonConverter(typeof(DynamicPropertyConverter<taxesResult>))]
        public IList<taxesResult> taxes { get; set; }
    }
    public class invoiceResult
    {
        public string uid { get; set; }
        public long mark { get; set; }
        public long cancelledByMark { get; set; }
        public string authenticationCode { get; set; }
        public int transmissionFailure { get; set; }
        public partyResult issuer { get; set; }
        public partyResult counterpart { get; set; }
        public paymentMethodsResult paymentMethods { get; set; }
        public invoiceHeaderResult invoiceHeader { get; set; }

        [JsonConverter(typeof(DynamicPropertyConverter<invoiceRowResult>))]
        public IList<invoiceRowResult> invoiceDetails { get; set; }
        public taxesTotalsResult taxesTotals { get; set; }
        public invoiceSummaryResult invoiceSummary { get; set; }
    }

    public class invoicesDocResult
    {
        public IList<invoiceResult> invoice { get; set; }
    }
    public class continuationTokenResult
    {
        public string nextPartitionKey { get; set; }
        public string nextRowKey { get; set; }
    }
    public class RequestedDocResult
    {
        public continuationTokenResult continuationToken { get; set; }
        public invoicesDocResult invoicesDoc { get; set; }
    }
}