using App.Core.Caching;
using App.Core.Domain.Blogs;
using App.Core.Domain.Configuration;
using App.Core.Domain.Localization;
using App.Core.Domain.Media;
using App.Core.Domain.News;
using App.Core.Domain.Polls;
using App.Core.Events;
using App.Services.Events;
using System.Threading.Tasks;

namespace App.Web.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        //languages
        IConsumer<EntityInsertedEvent<Language>>,
        IConsumer<EntityUpdatedEvent<Language>>,
        IConsumer<EntityDeletedEvent<Language>>,
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //Picture
        IConsumer<EntityInsertedEvent<Picture>>,
        IConsumer<EntityUpdatedEvent<Picture>>,
        IConsumer<EntityDeletedEvent<Picture>>,
        //polls
        IConsumer<EntityInsertedEvent<Poll>>,
        IConsumer<EntityUpdatedEvent<Poll>>,
        IConsumer<EntityDeletedEvent<Poll>>,
        //blog posts
        IConsumer<EntityInsertedEvent<BlogPost>>,
        IConsumer<EntityUpdatedEvent<BlogPost>>,
        IConsumer<EntityDeletedEvent<BlogPost>>,
        //news items
        IConsumer<EntityInsertedEvent<NewsItem>>,
        IConsumer<EntityUpdatedEvent<NewsItem>>,
        IConsumer<EntityDeletedEvent<NewsItem>>
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region Languages

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        #endregion

        #region Setting

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ManufacturerNavigationPrefixCacheKey); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            await _staticCacheManager.RemoveAsync(NopModelCacheDefaults.VendorNavigationModelKey); //depends on VendorSettings.VendorBlockItemsToDisplay
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryAllPrefixCacheKey); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.BlogPrefixCacheKey); //depends on BlogSettings.NumberOfTags
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.NewsPrefixCacheKey); //depends on NewsSettings.MainPageNewsCount
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey); //depends on distinct sitemap settings
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.WidgetPrefixCacheKey); //depends on WidgetSettings and certain settings of widgets
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.StoreLogoPathPrefixCacheKey); //depends on StoreInformationSettings.LogoPictureId
        }

        #endregion

        #endregion

        #region Pictures

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.OrderPicturePrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.OrderPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ProductOverviewPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.VendorPicturePrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.OrderPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ProductOverviewPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.VendorPicturePrefixCacheKey);
        }

        #endregion

        #region Polls

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.PollsPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.PollsPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.PollsPrefixCacheKey);
        }

        #endregion

        #region Blog posts

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region News items

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion
    }
}