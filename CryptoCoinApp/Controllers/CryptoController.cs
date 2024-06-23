using CryptoCoinApp.Common;
using CryptoCoinApp.Helpers.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CryptoCoinApp.Controllers;
[ApiController]
[Route("api/v1/cryptocurrency")]
public class CryptoCurrencyController : ControllerBase
{
    readonly IHttpConnectionHelper _httpConnectionHelpers;
    string? url;
    public CryptoCurrencyController(IConfiguration configuration, IHttpConnectionHelper httpConnectionHelpers)
    {
        _httpConnectionHelpers = httpConnectionHelpers;
        url = configuration.GetValue<string>("AppOptions:url");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCrytoCurrency(int pageNumber = 1, int pageSize = 5)
    {
        var apiResponse = await _httpConnectionHelpers.AppRequest<ApiResponse>(url, "get");
        if (apiResponse.data.Any())
        {
            var totalCount = apiResponse.data.Count();
            var pagedData = apiResponse.data
                .OrderBy(x => x.id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResponse = new PaginatedResponse<CryptoRecord>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = pagedData
            };

            return Ok(paginatedResponse);
        }

        return Ok("No record found");
    }

    [HttpGet("by-rank")]
    public async Task<IActionResult> GetById([FromQuery] string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("id value can not be null or empty");

        var response = await _httpConnectionHelpers.AppRequest<ApiResponse>(url, "get");
        if (response.Code.Equals("00"))
        {
            return Ok(response.data.Where(x => x.id.ToLower().Equals(id.ToLower())).FirstOrDefault());
        }
        return BadRequest($"No response for id : {id}. Please try another");
    }
}