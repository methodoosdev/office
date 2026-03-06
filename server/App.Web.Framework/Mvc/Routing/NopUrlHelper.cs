using App.Core;
using App.Core.Domain.Blogs;
using App.Core.Domain.News;
using App.Core.Domain.Seo;
using App.Services.Seo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents the helper implementation to build specific URLs within an application 
    /// </summary>
    public partial class NopUrlHelper : INopUrlHelper
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IStoreContext _storeContext;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public NopUrlHelper(
            IActionContextAccessor actionContextAccessor,
            IStoreContext storeContext,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService)
        {
            _actionContextAccessor = actionContextAccessor;
            _storeContext = storeContext;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Generate a URL for a product with the specified route values
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="values">An object that contains route values</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
        /// <param name="host">The host name for the URL</param>
        /// <param name="fragment">The fragment for the URL</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the generated URL
        /// </returns>
        protected virtual Task<string> RouteProductUrlAsync(IUrlHelper urlHelper,
            object values = null, string protocol = null, string host = null, string fragment = null)
        {
            var routeValues = new RouteValueDictionary(values);
            if (!routeValues.TryGetValue(NopRoutingDefaults.RouteValue.SeName, out var slug))
                return Task.FromResult(urlHelper.RouteUrl(NopRoutingDefaults.RouteName.Generic.Product, values, protocol, host, fragment));

            var catalogSeName = string.Empty;
            if (string.IsNullOrEmpty(catalogSeName))
                return Task.FromResult(urlHelper.RouteUrl(NopRoutingDefaults.RouteName.Generic.Product, values, protocol, host, fragment));

            routeValues[NopRoutingDefaults.RouteValue.CatalogSeName] = catalogSeName;
            return Task.FromResult(urlHelper.RouteUrl(NopRoutingDefaults.RouteName.Generic.ProductCatalog, routeValues, protocol, host, fragment));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate a generic URL for the specified entity type and route values
        /// </summary>
        /// <typeparam name="TEntity">Entity type that supports slug</typeparam>
        /// <param name="values">An object that contains route values</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
        /// <param name="host">The host name for the URL</param>
        /// <param name="fragment">The fragment for the URL</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the generated URL
        /// </returns>
        public virtual Task<string> RouteGenericUrlAsync<TEntity>(object values = null, string protocol = null, string host = null, string fragment = null)
            where TEntity : BaseEntity, ISlugSupported
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return typeof(TEntity) switch
            {
                var entityType when entityType == typeof(NewsItem)
                    => Task.FromResult(urlHelper.RouteUrl(NopRoutingDefaults.RouteName.Generic.NewsItem, values, protocol, host, fragment)),
                var entityType when entityType == typeof(BlogPost)
                    => Task.FromResult(urlHelper.RouteUrl(NopRoutingDefaults.RouteName.Generic.BlogPost, values, protocol, host, fragment)),                
                var entityType => Task.FromResult(urlHelper.RouteUrl(entityType.Name, values, protocol, host, fragment))
            };
        }

        #endregion
    }
}