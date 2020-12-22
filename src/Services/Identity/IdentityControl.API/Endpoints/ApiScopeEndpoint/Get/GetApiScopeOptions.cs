using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    public class GetApiScopeOptions : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiScope> _repository;

        public GetApiScopeOptions(IIdentityRepository<ApiScope> repository)
        {
            _repository = repository;
        }

        [HttpGet("api-scope/options")]
        [SwaggerOperation(Summary = "Gets all possible API Scopes options for selection", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<List<BaseOption<string>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.Query()
                .Select(e => new BaseOption<string>
                {
                    Value = e.Name,
                    Text = e.DisplayName
                }).ToListAsync(cancellationToken);
        }
    }
}