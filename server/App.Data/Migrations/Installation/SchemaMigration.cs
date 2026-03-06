using App.Core.Domain.Accounting;
using App.Core.Domain.Assignment;
using App.Core.Domain.Blogs;
using App.Core.Domain.Common;
using App.Core.Domain.Configuration;
using App.Core.Domain.Customers;
using App.Core.Domain.Directory;
using App.Core.Domain.Employees;
using App.Core.Domain.Financial;
using App.Core.Domain.Forums;
using App.Core.Domain.Gdpr;
using App.Core.Domain.Localization;
using App.Core.Domain.Logging;
using App.Core.Domain.Media;
using App.Core.Domain.Messages;
using App.Core.Domain.News;
using App.Core.Domain.Offices;
using App.Core.Domain.Payroll;
using App.Core.Domain.Polls;
using App.Core.Domain.ScheduleTasks;
using App.Core.Domain.Security;
using App.Core.Domain.Seo;
using App.Core.Domain.SimpleTask;
using App.Core.Domain.Stores;
using App.Core.Domain.Traders;
using App.Core.Domain.VatExemption;
using App.Data.Extensions;
using FluentMigrator;

namespace App.Data.Migrations.Installation
{
    [NopMigration("2020/01/31 11:24:16:2551771", "App.Data base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// <remarks>
        /// We use an explicit table creation order instead of an automatic one
        /// due to problems creating relationships between tables
        /// </remarks>
        /// </summary>
        public override void Up()
        {
            Create.TableFor<GenericAttribute>();
            Create.TableFor<SearchTerm>();
            Create.TableFor<Country>();
            Create.TableFor<Currency>();
            Create.TableFor<Language>();
            Create.TableFor<Customer>();
            Create.TableFor<CustomerPassword>();
            Create.TableFor<CustomerRole>();
            Create.TableFor<CustomerCustomerRoleMapping>();
            Create.TableFor<Store>();
            Create.TableFor<StoreMapping>();
            Create.TableFor<LocaleStringResource>();
            Create.TableFor<LocalizedProperty>();
            Create.TableFor<BlogPost>();
            Create.TableFor<BlogComment>();
            Create.TableFor<Download>();
            Create.TableFor<Picture>();
            Create.TableFor<PictureBinary>();
            Create.TableFor<Video>();
            Create.TableFor<Setting>();
            Create.TableFor<PrivateMessage>();
            Create.TableFor<ForumGroup>();
            Create.TableFor<Forum>();
            Create.TableFor<ForumTopic>();
            Create.TableFor<ForumPost>();
            Create.TableFor<ForumPostVote>();
            Create.TableFor<ForumSubscription>();
            Create.TableFor<GdprConsent>();
            Create.TableFor<GdprLog>();
            Create.TableFor<ActivityLogType>();
            Create.TableFor<ActivityLog>();
            Create.TableFor<Log>();
            Create.TableFor<EmailAccount>();
            Create.TableFor<MessageTemplate>();
            Create.TableFor<NewsLetterSubscription>();
            Create.TableFor<QueuedEmail>();
            Create.TableFor<NewsItem>();
            Create.TableFor<NewsComment>();
            Create.TableFor<Poll>();
            Create.TableFor<PollAnswer>();
            Create.TableFor<PollVotingRecord>();
            Create.TableFor<AclRecord>();
            Create.TableFor<PermissionRecord>();
            Create.TableFor<PermissionRecordCustomerRoleMapping>();
            Create.TableFor<UrlRecord>();
            Create.TableFor<ScheduleTask>();

            //Accounting
            Create.TableFor<PeriodicF2>();
            //Assignment
            Create.TableFor<AssignmentPrototype>();
            Create.TableFor<AssignmentPrototypeAction>();
            Create.TableFor<AssignmentPrototypeAssignmentPrototypeActionMapping>();
            Create.TableFor<AssignmentReason>();
            Create.TableFor<AssignmentTask>();
            Create.TableFor<AssignmentTaskAction>();
            //Customers
            Create.TableFor<CustomerToken>();
            Create.TableFor<CustomerPermission>();
            Create.TableFor<CustomerPermissionCustomerMapping>();
            Create.TableFor<CustomerOnline>();
            //Directory
            Create.TableFor<Bookmark>();
            //Employees
            Create.TableFor<Department>();
            Create.TableFor<Education>();
            Create.TableFor<JobTitle>();
            Create.TableFor<Specialty>();
            Create.TableFor<Employee>();
            //Messages
            Create.TableFor<EmailMessage>();
            //Offices
            Create.TableFor<AccountingOffice>();
            Create.TableFor<Chamber>();
            Create.TableFor<PeriodicityItem>();
            Create.TableFor<PersistState>();
            Create.TableFor<TaxFactor>();
            //SimpleTask
            Create.TableFor<SimpleTaskCategory>();
            Create.TableFor<SimpleTaskDepartment>();
            Create.TableFor<SimpleTaskNature>();
            Create.TableFor<SimpleTaskSector>();
            Create.TableFor<SimpleTaskManager>();
            //Trader
            Create.TableFor<AccountingWork>();
            Create.TableFor<TraderGroup>();
            Create.TableFor<WorkingArea>();
            Create.TableFor<Trader>();
            Create.TableFor<TraderKad>();
            Create.TableFor<TraderBranch>();
            Create.TableFor<TraderAccountingWorkMapping>();
            Create.TableFor<TraderEmployeeMapping>();
            Create.TableFor<TraderParentMapping>();
            //Payroll
            Create.TableFor<WorkerSchedule>();
            Create.TableFor<WorkerScheduleDate>();
            Create.TableFor<WorkerScheduleLog>();
            Create.TableFor<WorkerScheduleShift>();
            Create.TableFor<WorkerScheduleWorker>();
            Create.TableFor<WorkerLeaveDetail>();
            //Financial
            Create.TableFor<FinancialObligation>();
            //VatExemption
            Create.TableFor<VatExemptionApproval>();
            Create.TableFor<VatExemptionDoc>();
            Create.TableFor<VatExemptionReport>();
            Create.TableFor<VatExemptionSerial>();

            Create.TableFor<TraderMonthlyBilling>();

        }
    }
}
