using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common;
using IdentityControl.API.Data;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    public class GetApiResourceScopes : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResourceScope> _apiResourceScopeRepository;
        private readonly IIdentityRepository<ApiScope> _scopeRepository;

        public GetApiResourceScopes(IIdentityRepository<ApiResourceScope> apiResourceScopeRepository,
            IIdentityRepository<ApiScope> scopeRepository)
        {
            _apiResourceScopeRepository = apiResourceScopeRepository;
            _scopeRepository = scopeRepository;
        }

        [HttpGet("api-scope/api-resource/{id}")]
        [SwaggerOperation(Summary = "Gets all the scopes of a certain API Resource", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<List<ApiScopeDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = new List<ApiScopeDto>();

            var apiResourceScopes = await _apiResourceScopeRepository.Query()
                .Where(e => e.ApiResourceId == id)
                .Select(e => e.Scope)
                .ToListAsync(cancellationToken);

            var apiScopes = await _scopeRepository.Query()
                .Where(x => apiResourceScopes.Contains(x.Name))
                .Select(x => new
                {
                    x.Id,
                    x.Enabled,
                    x.DisplayName,
                    x.Name
                })
                .ToArrayAsync(cancellationToken);

            foreach (var apiResourceScope in apiResourceScopes)
            {
                var apiScope = apiScopes.FirstOrDefault(x => x.Name == apiResourceScope);

                response.Add(new ApiScopeDto
                {
                    Id = apiScope.Id,
                    Name = apiScope.Name,
                    Enabled = apiScope.Enabled,
                    DisplayName = apiScope.DisplayName,
                    IsReadOnly = AppConstants.ReadOnlyEntities.IdentityControlApiScope.Contains(apiScope.Name)
                });
            }

            return response;
        }
    }
}