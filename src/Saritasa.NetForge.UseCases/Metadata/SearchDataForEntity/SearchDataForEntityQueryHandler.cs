using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MediatR;
using Saritasa.NetForge.Domain.Attributes;
using Saritasa.NetForge.DomainServices.Extensions;
using Saritasa.NetForge.Infrastructure.Abstractions.Interfaces;
using Saritasa.Tools.Common.Extensions;
using Saritasa.Tools.Common.Pagination;
using Saritasa.Tools.Domain.Exceptions;

namespace Saritasa.NetForge.UseCases.Metadata.SearchDataForEntity;

/// <summary>
/// Handler for <see cref="SearchDataForEntityQuery"/>.
/// </summary>
internal class SearchDataForEntityQueryHandler : IRequestHandler<SearchDataForEntityQuery, PagedListMetadataDto<object>>
{
    private readonly IOrmDataService dataService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public SearchDataForEntityQueryHandler(IOrmDataService dataService)
    {
        this.dataService = dataService;
    }

    /// <inheritdoc/>
    public Task<PagedListMetadataDto<object>> Handle(
        SearchDataForEntityQuery request, CancellationToken cancellationToken)
    {
        if (request.EntityType is null)
        {
            throw new NotFoundException("Entity with given type was not found.");
        }

        var query = dataService.GetQuery(request.EntityType);

        query = query.SelectProperties(request.EntityType, request.Properties);

        var searchOptions = request.SearchOptions;

        if (!string.IsNullOrEmpty(searchOptions.SearchString) && request.Properties.Any(property => property.IsSearchable))
        {
            query = dataService
                .CaseInsensitiveSearch(query, searchOptions.SearchString, request.EntityType, request.Properties);
        }

        var pagedList = PagedListFactory.FromSource(query, searchOptions.Page, searchOptions.PageSize);

        return Task.FromResult(pagedList.ToMetadataObject());
    }
}
