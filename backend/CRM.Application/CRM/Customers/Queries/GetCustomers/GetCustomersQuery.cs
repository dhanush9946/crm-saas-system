using CRM.Application.Common.Models;
using CRM.Application.CRM.Customers.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Application.CRM.Customers.Queries.GetCustomers
{
    public sealed class GetCustomersQuery
    : IRequest<PagedResult<CustomerDto>>
    {
        public string? Search { get; init; }

        public string? SortBy { get; init; }

        public string? SortDirection { get; init; }

        public int Page { get; init; } = 1;

        public int PageSize { get; init; } = 10;
    }
}
