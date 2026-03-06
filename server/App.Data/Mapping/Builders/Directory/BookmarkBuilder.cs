using App.Core.Domain.Directory;
using FluentMigrator.Builders.Create.Table;

namespace App.Data.Mapping.Builders.Directory
{
    public partial class BookmarkBuilder : NopEntityBuilder<Bookmark>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Bookmark.UrlPath)).AsString(1023).NotNullable()
                .WithColumn(nameof(Bookmark.Description)).AsString(255).NotNullable();
        }
    }
}