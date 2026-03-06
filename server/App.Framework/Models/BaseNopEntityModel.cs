
namespace App.Framework.Models
{
    /// <summary>
    /// Represents base appCommerce entity model
    /// </summary>
    public partial record BaseNopEntityModel : BaseNopModel
    {
        /// <summary>
        /// Gets or sets model identifier
        /// </summary>
        public virtual int Id { get; set; }
    }
}