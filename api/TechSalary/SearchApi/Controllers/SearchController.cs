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

        /// <summary>
        /// Get a distinct list of all company names in the index.
        /// </summary>
        [HttpGet("companies")]
        public async Task<ActionResult<IEnumerable<string>>> GetCompanies(CancellationToken cancellationToken)
        {
            var companies = await searchService.GetCompaniesAsync(cancellationToken);
            return Ok(companies);
        }

        /// <summary>
        /// Get a distinct list of all designations in the index.
        /// </summary>
        [HttpGet("designations")]
        public async Task<ActionResult<IEnumerable<string>>> GetDesignations(CancellationToken cancellationToken)
        {
            var designations = await searchService.GetDesignationsAsync(cancellationToken);
            return Ok(designations);
        }
    }
}
