using SearchApi.DTOs;

namespace SearchApi.Services
{
    public interface ISearchService
    {
        Task<SearchResultDto> SearchAsync(SearchQueryDto query, CancellationToken cancellationToken = default);
    }
}
