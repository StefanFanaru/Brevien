using System;
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
using static IdentityControl.API.Endpoints.ClientEndpoint.ClientValidators;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    public class Update : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<Client> _repository;
        private readonly IAspValidator _validator;

        public Update(IIdentityRepository<Client> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPatch("client/{id}")]
        [SwaggerOperation(Summary = "Updates a Client", Tags = new[] {"ClientEndpoint"})]
        public async Task<ActionResult<UpdateClientResponse>> HandleAsync(int id, UpdateClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(Client), ToasterType.Success, ToasterVerbs.Updated);
            var validation =
                await _validator.ValidateAsync<UpdateClientRequest, UpdateClientResponse, UpdateClientValidator>
                    (request, toaster, cancellationToken);

            if (validation.Failed) return validation.Response;

            if (!_repository.Query()
                .Any(e => e.Id == id && e.ClientId != AppConstants.ReadOnlyEntities.AngularClient))
                return NotFound(id);

            var entity = await _repository.Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            entity.ClientId = request.Name;
            entity.ClientName = request.DisplayName;
            entity.Description = request.Description;
            entity.NonEditable = request.IsReadOnly;
            entity.Updated = DateTime.UtcNow;
            entity.RequirePkce = request.RequirePkce;
            entity.AccessTokenLifetime = request.AccessTokenLifetime * 60; // transform minutes in seconds;
            entity.ClientUri = request.ClientUri;
            entity.AllowOfflineAccess = request.AllowOfflineAccess;
            entity.RequireClientSecret = request.RequireClientSecret;
            entity.AllowAccessTokensViaBrowser = request.AllowAccessTokensViaBrowser;

            toaster.Identifier = entity.ClientName;
            await _repository.SaveAsync(toaster);
            return validation.Response;
        }
    }
}