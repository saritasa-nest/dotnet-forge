﻿using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Saritasa.NetForge.Domain.Entities.Metadata;
using Saritasa.NetForge.Domain.Enums;
using Saritasa.NetForge.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Infrastructure.EfCore.Extensions;

namespace Saritasa.NetForge.Infrastructure.EfCore.Services;

/// <summary>
/// Data service for EF core.
/// </summary>
public class EfCoreDataService : IOrmDataService
{
    private const string Entity = "entity";

    private readonly EfCoreOptions efCoreOptions;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Constructor.
    /// </summary>
    public EfCoreDataService(EfCoreOptions efCoreOptions, IServiceProvider serviceProvider)
    {
        this.efCoreOptions = efCoreOptions;
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public IQueryable<object> GetQuery(Type clrType)
    {
        foreach (var dbContextType in efCoreOptions.DbContexts)
        {
            var dbContextService = serviceProvider.GetService(dbContextType);

            if (dbContextService == null)
            {
                continue;
            }

            var dbContext = (DbContext)dbContextService;
            return dbContext.Set(clrType).OfType<object>();
        }

        throw new ArgumentException("Database entity with given type was not found", nameof(clrType));
    }

    /// <inheritdoc />
    public IQueryable<object> Search(
        IQueryable<object> query, string? searchString, Type entityType, ICollection<PropertyMetadata> properties)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return query;
        }

        Expression? combinedIsMatchExpressions = null;

        // entity => entity
        var entity = Expression.Parameter(typeof(object), Entity);

        // entity => (entityType)entity
        var converted = Expression.Convert(entity, entityType);

        var searchWords = searchString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        foreach (var searchWord in searchWords)
        {
            var searchFor = searchWord;

            var isExactMatch = false;
            const char quote = '"';
            if (searchWord.StartsWith(quote) && searchWord.EndsWith(quote))
            {
                const int startIndex = 1;
                var endIndex = searchFor.LastIndexOf(quote);

                searchFor = searchFor[startIndex..endIndex];

                isExactMatch = true;
            }

            var searchConstant = Expression.Constant(searchFor);

            Expression? isMatchExpressions = null;
            foreach (var property in properties)
            {
                var searchType = property.SearchType;
                if (!searchType.HasValue)
                {
                    continue;
                }

                // entity => ((entityType)entity).propertyName
                var propertyExpression = Expression.Property(converted, property.Name);

                if (isExactMatch)
                {
                    searchType = SearchType.ExactMatch;
                }

                var methodCall = searchType switch
                {
                    SearchType.CaseInsensitiveContains
                        => GetCaseInsensitiveContainsMethodCall(propertyExpression, searchConstant),

                    SearchType.CaseSensitiveStartsWith
                        => GetCaseSensitiveStartsWithCall(propertyExpression, searchConstant),

                    SearchType.ExactMatch
                        => GetExactMatchCall(propertyExpression, searchConstant),

                    _ => throw new NotImplementedException("Unsupported search type was used.")
                };

                if (isMatchExpressions is null)
                {
                    isMatchExpressions = methodCall;
                }
                else
                {
                    // entity => Regex.IsMatch(((entityType)entity).propertyName, searchWord, RegexOptions.IgnoreCase) ||
                    //           Regex.IsMatch(((entityType)entity).propertyName2, searchWord, RegexOptions.IgnoreCase)
                    isMatchExpressions = Expression.OrElse(isMatchExpressions, methodCall!);
                }
            }

            if (combinedIsMatchExpressions is null)
            {
                combinedIsMatchExpressions = isMatchExpressions;
            }
            else
            {
                combinedIsMatchExpressions = Expression.And(combinedIsMatchExpressions, isMatchExpressions!);
            }
        }

        var predicate = Expression.Lambda<Func<object, bool>>(combinedIsMatchExpressions!, entity);
        return query.Where(predicate);
    }

    private MethodCallExpression GetCaseInsensitiveContainsMethodCall(
        MemberExpression propertyExpression, ConstantExpression searchConstant)
    {
        var isMatch = typeof(Regex).GetMethod(
            nameof(Regex.IsMatch),
            new[]
            {
                typeof(string), typeof(string), typeof(RegexOptions)
            });

        // entity => Regex.IsMatch(((entityType)entity).propertyName, searchWord, RegexOptions.IgnoreCase)
        return Expression.Call(
            isMatch!, propertyExpression, searchConstant, Expression.Constant(RegexOptions.IgnoreCase));
    }

    private MethodCallExpression GetCaseSensitiveStartsWithCall(
        MemberExpression propertyExpression, ConstantExpression searchConstant)
    {
        var startsWith = typeof(string).GetMethod(
            nameof(string.StartsWith),
            new[]
            {
                typeof(string)
            });

        var property = GetConvertedExpressionIfNumber(propertyExpression);

        // entity => ((entityType)entity).propertyName.StartsWith(searchConstant)
        return Expression.Call(property, startsWith!, searchConstant);
    }

    private MethodCallExpression GetExactMatchCall(
        MemberExpression propertyExpression, ConstantExpression searchConstant)
    {
        var equal = typeof(string).GetMethod(
            nameof(string.Equals),
            new[]
            {
                typeof(string)
            });

        var property = GetConvertedExpressionIfNumber(propertyExpression);

        // entity => ((entityType)entity).propertyName.StartsWith(searchConstant)
        return Expression.Call(property, equal!, searchConstant);
    }

    private Expression GetConvertedExpressionIfNumber(MemberExpression propertyExpression)
    {
        var propertyType = ((PropertyInfo)propertyExpression.Member).PropertyType;

        if (propertyType != typeof(string))
        {
            // When passed property expression does not represent string we call ToString()
            return Expression.Call(propertyExpression, typeof(object).GetMethod(nameof(ToString))!);
        }

        return propertyExpression;
    }
}
