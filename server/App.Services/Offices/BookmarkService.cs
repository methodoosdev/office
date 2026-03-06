using App.Core.Domain.Directory;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Offices
{
    public partial interface IBookmarkService
    {
        IQueryable<Bookmark> Table { get; }
        Task<Bookmark> GetBookmarkByIdAsync(int bookmarkId);
        Task<IList<Bookmark>> GetBookmarksByIdsAsync(int[] bookmarkIds);
        Task<IList<Bookmark>> GetAllBookmarksAsync();
        Task DeleteBookmarkAsync(Bookmark bookmark);
        Task DeleteBookmarkAsync(IList<Bookmark> bookmarks);
        Task InsertBookmarkAsync(Bookmark bookmark);
        Task UpdateBookmarkAsync(Bookmark bookmark);
    }
    public partial class BookmarkService : IBookmarkService
    {
        private readonly IRepository<Bookmark> _bookmarkRepository;

        public BookmarkService(
            IRepository<Bookmark> bookmarkRepository)
        {
            _bookmarkRepository = bookmarkRepository;
        }

        public virtual IQueryable<Bookmark> Table => _bookmarkRepository.Table;

        public virtual async Task<Bookmark> GetBookmarkByIdAsync(int bookmarkId)
        {
            return await _bookmarkRepository.GetByIdAsync(bookmarkId);
        }

        public virtual async Task<IList<Bookmark>> GetBookmarksByIdsAsync(int[] bookmarkIds)
        {
            return await _bookmarkRepository.GetByIdsAsync(bookmarkIds);
        }

        public virtual async Task<IList<Bookmark>> GetAllBookmarksAsync()
        {
            var entities = await _bookmarkRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteBookmarkAsync(Bookmark bookmark)
        {
            await _bookmarkRepository.DeleteAsync(bookmark);
        }

        public virtual async Task DeleteBookmarkAsync(IList<Bookmark> bookmarks)
        {
            await _bookmarkRepository.DeleteAsync(bookmarks);
        }

        public virtual async Task InsertBookmarkAsync(Bookmark bookmark)
        {
            await _bookmarkRepository.InsertAsync(bookmark);
        }

        public virtual async Task UpdateBookmarkAsync(Bookmark bookmark)
        {
            await _bookmarkRepository.UpdateAsync(bookmark);
        }
    }
}