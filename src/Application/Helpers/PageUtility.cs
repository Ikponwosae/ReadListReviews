﻿using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Reflection;

namespace Application.Helpers
{
    public class PageUtility<T>
    {
        public static Pagination CreateResourcePageUrl(ResourceParameter parameters, string name, PagedList<T> pageData, IUrlHelper helper)
        {

            var prevLink = pageData.HasPrevious
                ? CreateResourceUri(parameters, name, ResourceUriType.PreviousPage, helper)
                : null;
            var nextLink = pageData.HasNext
                ? CreateResourceUri(parameters, name, ResourceUriType.NextPage, helper)
                : null;

            var pagination = new Pagination
            {
                NextPage = nextLink,
                PreviousPage = prevLink,
                Total = pageData.TotalCount,
                PageSize = pageData.PageSize,
                TotalPages = pageData.TotalPages
            };

            return pagination;
        }

        private static string CreateResourceUri(ResourceParameter parameter, string name, ResourceUriType type, IUrlHelper url)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return url.Link(name,
                        new
                        {
                            PageNumber = parameter.PageNumber - 1,
                            parameter.PageSize,
                            parameter.Search,
                            parameter.Sort
                        });

                case ResourceUriType.NextPage:
                    return url.Link(name,
                        new
                        {
                            PageNumber = parameter.PageNumber + 1,
                            parameter.PageSize,
                            parameter.Search,
                            parameter.Sort
                        });

                default:
                    return url.Link(name,
                        new
                        {
                            parameter.PageNumber,
                            parameter.PageSize,
                            parameter.Search,
                            parameter.Sort
                        });
            }

        }

        public static Pagination CreateResourcePageUrl(IDictionary<string, object> parameters, string name, PagedList<T> pageData, IUrlHelper helper)
        {

            var prevLink = pageData.HasPrevious
                ? CreateResourceUri(parameters, name, ResourceUriType.PreviousPage, helper)
                : null;
            var nextLink = pageData.HasNext
                ? CreateResourceUri(parameters, name, ResourceUriType.NextPage, helper)
                : null;

            var pagination = new Pagination
            {
                NextPage = nextLink,
                PreviousPage = prevLink,
                Total = pageData.TotalCount,
                PageSize = pageData.PageSize,
                TotalPages = pageData.TotalPages
            };

            return pagination;
        }

        private static string CreateResourceUri(IDictionary<string, object> parameters, string name, ResourceUriType type, IUrlHelper url)
        {
            return type switch
            {
                ResourceUriType.PreviousPage => url.Link(name, parameters),
                ResourceUriType.NextPage => url.Link(name, parameters),
                _ => url.Link(name, parameters),
            };
        }

        public static IDictionary<string, object> GenerateResourceParameters(dynamic parameter, PagedList<T> events)
        {
            var pageNumber = "PageNumber";
            var dynamicParameter = new ExpandoObject() as IDictionary<string, object>;

            foreach (PropertyInfo param in parameter.GetType().GetProperties())
            {
                if (events.HasNext && param.Name.Equals(pageNumber, StringComparison.OrdinalIgnoreCase))
                {
                    dynamicParameter.Add(param.Name, (int)param.GetValue(parameter, null) + 1);
                }
                else if (events.HasPrevious && param.Name.Equals(pageNumber, StringComparison.OrdinalIgnoreCase))
                {
                    dynamicParameter.Add(param.Name, (int)param.GetValue(parameter, null) - 1);
                }
                else
                {
                    dynamicParameter.Add(param.Name, param.GetValue(parameter, null));
                }
            }

            return dynamicParameter;
        }
    }
}
