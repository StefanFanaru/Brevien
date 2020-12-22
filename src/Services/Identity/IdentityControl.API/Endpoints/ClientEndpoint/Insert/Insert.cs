using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Services.SignalR;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static IdentityControl.API.Endpoints.ClientEndpoint.ClientValidators;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Insert
{
    [Authorize(Policy = "AdminOnly")]
    public class Insert : BaseAsyncEndpoint<InsertClientRequest, InsertClientResponse>
    {
        private readonly IIdentityRepository<Client> _repository;
        private readonly IAspValidator _validator;

        public Insert(IIdentityRepository<Client> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPost("client")]
        [SwaggerOperation(Summary = "Creates a new Client", Tags = new[] {"ClientEndpoint"})]
        public override async Task<ActionResult<InsertClientResponse>> HandleAsync(InsertClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(Client), ToasterType.Success, ToasterVerbs.Created);
            var validation =
                await _validator.ValidateAsync<InsertClientRequest, InsertClientResponse, InsertClientValidator>
                    (request, toaster, cancellationToken);
            if (validation.Failed) return validation.Response;

            if (_repository.Query().Any(e => e.ClientId == request.Name))
                return AspExtensions.GetBadRequestWithError<InsertClientResponse>($"Client \"{request.Name}\" already exists.");

            var entity = new Client
            {
                ClientId = request.Name,
                ClientName = request.DisplayName,
                Description = request.Description,
                NonEditable = request.IsReadOnly,
                Created = DateTime.UtcNow,
                RequirePkce = request.RequirePkce,
                AccessTokenLifetime = request.AccessTokenLifetime * 60, // transform minutes in seconds
                ClientUri = request.ClientUri,
                AllowOfflineAccess = request.AllowOfflineAccess,
                RequireClientSecret = request.RequireClientSecret,
                AllowAccessTokensViaBrowser = request.AllowAccessTokensViaBrowser
            };

            toaster.Identifier = entity.ClientName;
            await _repository.InsertAsync(entity);
            await _repository.SaveAsync(toaster);

            return validation.Response;
        }
    }
}