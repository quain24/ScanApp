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

        /// <summary>
        /// Creates new instance of <see cref="CurrentUserService"/>
        /// </summary>
        /// <param name="provider">Provides information about currently logged-in user</param>
        public CurrentUserService(AuthenticationStateProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider), $"DI could not resolve proper {nameof(AuthenticationStateProvider)} instance");
        }

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="userName"/> is <see langword="null"/></exception>
        public async Task<bool> IsItMe(string userName) => (await GetState().ConfigureAwait(false)).IsItMe(userName);

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        /// <exception cref="ArgumentNullException">Underlying <see cref="AuthenticationState"/> does not contain user identity or identity has no name</exception>
        public async Task<string> Name() => (await GetState().ConfigureAwait(false)).Name();

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        public async Task<string> LocationId() => (await GetState().ConfigureAwait(false)).LocationId();

        /// <inheritdoc />
        /// <remarks>If underlying <seealso cref="AuthenticationState"/> is null or does not contain identity data this method will return <see langword="false"/> </remarks>
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        public async Task<bool> IsAuthenticated() => (await GetState().ConfigureAwait(false))?.User?.Identity?.IsAuthenticated ?? false;

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        public async Task<bool> IsInRole(string roleName) => (await GetState().ConfigureAwait(false)).User.IsInRole(roleName);

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="claim"/> is <see langword="null"/></exception>
        public async Task<bool> HasClaim(ClaimModel claim) => (await GetState().ConfigureAwait(false)).HasClaim(claim);

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="claim"/> is <see langword="null"/></exception>
        public async Task<bool> HasClaim(Claim claim) => (await GetState().ConfigureAwait(false)).HasClaim(claim);

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="claimType"/> is <see langword="null"/></exception>
        public async Task<bool> HasClaim(string claimType) => (await GetState().ConfigureAwait(false)).HasClaim(claimType);

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="claimType"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException">When <paramref name="claimValue"/> is <see langword="null"/></exception>
        public async Task<bool> HasClaim(string claimType, string claimValue) => (await GetState().ConfigureAwait(false)).HasClaim(claimType, claimValue);

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        public async Task<List<ClaimModel>> AllClaims() => (await GetState().ConfigureAwait(false)).User.Claims.Select(c => new ClaimModel(c.Type, c.Value)).ToList();

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        public async Task<List<ClaimModel>> AllClaims(string claimType) => (await GetState().ConfigureAwait(false)).User.FindAll(claimType).Select(c => new ClaimModel(c.Type, c.Value)).ToList();

        /// <inheritdoc />
        /// <exception cref="WrongScopeException">When method is called from different then original scope</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="claimType"/> is <see langword="null"/></exception>
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