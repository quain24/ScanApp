using Microsoft.AspNetCore.Components.Authorization;
using ScanApp.Application.Admin;
using ScanApp.Common.Exceptions;
using ScanApp.Common.Extensions;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScanApp.Application.Common.Interfaces;

namespace ScanApp.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly AuthenticationStateProvider _provider;

        public CurrentUserService(AuthenticationStateProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider), $"DI could not resolve proper {nameof(AuthenticationStateProvider)} instance");
        }

        public async Task<bool> IsItMe(string userName) => (await GetState().ConfigureAwait(false)).IsItMe(userName);

        public async Task<string> Name() => (await GetState().ConfigureAwait(false)).Name();

        public async Task<string> LocationId() => (await GetState().ConfigureAwait(false)).LocationId();

        public async Task<bool> IsAuthenticated() => (await GetState().ConfigureAwait(false))?.User?.Identity?.IsAuthenticated ?? false;

        public async Task<bool> IsInRole(string roleName) => (await GetState().ConfigureAwait(false)).User.IsInRole(roleName);

        public async Task<bool> HasClaim(ClaimModel claim) => (await GetState().ConfigureAwait(false)).HasClaim(claim);

        public async Task<bool> HasClaim(Claim claim) => (await GetState().ConfigureAwait(false)).HasClaim(claim);

        public async Task<bool> HasClaim(string claimType) => (await GetState().ConfigureAwait(false)).HasClaim(claimType);

        public async Task<bool> HasClaim(string claimType, string claimValue) => (await GetState().ConfigureAwait(false)).HasClaim(claimType, claimValue);

        public async Task<List<ClaimModel>> AllClaims() => (await GetState().ConfigureAwait(false)).User.Claims.Select(c => new ClaimModel(c.Type, c.Value)).ToList();

        public async Task<List<ClaimModel>> AllClaims(string claimType) => (await GetState().ConfigureAwait(false)).User.FindAll(claimType).Select(c => new ClaimModel(c.Type, c.Value)).ToList();

        public async Task<ClaimModel> FindFirstClaim(string claimType)
        {
            var claim = (await GetState().ConfigureAwait(false)).User.FindFirst(claimType);
            return claim is null ? null : new ClaimModel(claim.Type, claim.Value);
        }

        private async Task<AuthenticationState> GetState()
        {
            try
            {
                return await _provider.GetAuthenticationStateAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                throw new WrongScopeException($"{nameof(CurrentUserService)} has been probably instantiated from wrong scope (using scope factory, sendScoped?) and cannot resolve Authentication state Task", ex);
            }
        }
    }
}