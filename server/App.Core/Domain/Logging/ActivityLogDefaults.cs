using App.Core.Domain.Logging;

namespace App.Core.Domain.Logging
{
    public static partial class ActivityLogDefaults
    {
        public static string Edit => "Edit";
        public static string Create => "Create";
        public static string Submit => "Submit";
        public static string Calc => "Calc";
        public static string Import => "Import";
        public static string Export => "Export";
        public static string Retrieve => "Retrieve";
        public static string Count => "Count";
        public static string ImpersonationStarted => "Impersonation.Started";
        public static string ImpersonationFinished => "Impersonation.Finished";

        public static string PeriodicityItems => "periodicity-items";
        public static string MonthlyFinancialBulletin => "monthly-financial-bulletin";
        public static string CashAvailable => "cash-available";
        public static string AggregateAnalysis => "aggregate-analysis";
        public static string VatCalculation => "vat-calculation";
        public static string ListingF4 => "listingF4";
        public static string ListingF5 => "listingF5";
        public static string ESend => "eSend";
        public static string SoftoneProject => "softone-project";
        public static string ApdSubmission => "apd-submission";
        public static string FmySubmission => "fmy-submission";
        public static string WorkerSickLeave => "worker-sick-leave";
        public static string WorkerLeave => "worker-leave";
        public static string WorkerLeaveDetail => "worker-leave-detail";
    }
}