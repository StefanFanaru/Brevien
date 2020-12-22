using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common;
using IdentityControl.API.Data;
using IdentityControl.API.Services.SignalR;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using static IdentityControl.API.Endpoints.ApiScopeEndpoint.ApiScopeValidators;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    public class Update : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiScope> _repository;
        private readonly IAspValidator _validator;

        public Update(IIdentityRepository<ApiScope> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPatch("api-scope/{id}")]
        [SwaggerOperation(Summary = "Updates an API scope", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<ActionResult<UpdateApiScopeResponse>> HandleAsync(int id, UpdateApiScopeRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Success, ToasterVerbs.Updated);
            var validation =
                await _validator.ValidateAsync<UpdateApiScopeRequest, UpdateApiScopeResponse, UpdateApiScopeValidator>
                    (request, toaster, cancellationToken);

            if (validation.Failed)
            {
                return validation.Response;
            }

            if (!_repository.Query().Any(e => e.Id == id && e.Name != AppConstants.ReadOnlyEntities.IdentityControlApiScope))
            {
                return NotFound(id);
            }

            if (_repository.Query().Any(e => e.Name == request.Name))
            {
                return AspExtensions.GetBadRequestWithError<UpdateApiScopeResponse>(
                    $"API Scope \"{request.Name}\" already exists.");
            }

            var entity = await _repository.Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.DisplayName = request.DisplayName;

            toaster.Identifier = entity.Name;
            await _repository.SaveAsync(toaster);
            return validation.Response;
        }
    }
}