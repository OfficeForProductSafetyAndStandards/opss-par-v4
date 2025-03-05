using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Helpers;

public class QueryHelper : IQueryHelper
{
    /// <summary>
    /// Parse a query string into its component key and value parts.
    /// </summary>
    /// <param name="queryString">The raw query string value, with or without the leading '?'.</param>
    /// <returns>A collection of parsed keys and values.</returns>
    public Dictionary<string, StringValues> ParseQuery(string? queryString)
    {
        return QueryHelpers.ParseQuery(queryString);
    }

    /// <summary>
    /// Parse a query string into its component key and value parts.
    /// </summary>
    /// <param name="queryString">The raw query string value, with or without the leading '?'.</param>
    /// <returns>A collection of parsed keys and values, null if there are no entries.</returns>
    public Dictionary<string, StringValues>? ParseNullableQuery(string? queryString)
    {
        return QueryHelpers.ParseNullableQuery(queryString);
    }
}
