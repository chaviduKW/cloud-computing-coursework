using Microsoft.AspNetCore.Mvc;
using SearchApi.DTOs;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController(ISearchService searchService) : ControllerBase
    {
        /// <summary>
        /// Search salary records with optional filters, sorting, and pagination.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<SearchResultDto>> Search([FromQuery] SearchQueryDto query, CancellationToken cancellationToken)
        {
            var result = await searchService.SearchAsync(query, cancellationToken);
            return Ok(result);
        }

    }
}
