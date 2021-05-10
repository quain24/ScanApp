using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    /// <summary>
    /// When handled, edits data of user with provided <paramref name="Name"/>.
    /// </summary>
    /// <param name="Name">Name user that will have its data edited.</param>
    public record EditUserDataCommand(string Name, Version Version) : IRequest<Result<Version>>
    {
        /// <summary>
        /// Gets name of user that will have its data edited.
        /// </summary>
        /// <value>Name of user if set, otherwise <see langword="null"/></value>
        public string Name { get; } = Name;

        /// <summary>
        /// Gets new name for the edited user.
        /// </summary>
        /// <value>New user name if set, otherwise <see langword="null"/></value>
        public string NewName { get; init; }

        /// <summary>
        /// Gets new phone number for the edited user.
        /// </summary>
        /// <value>New phone number if set, otherwise <see langword="null"/></value>
        public string Phone { get; init; }

        /// <summary>
        /// Gets new Email address for the edited user.
        /// </summary>
        /// <value>New Email address if set, otherwise <see langword="null"/></value>
        public string Email { get; init; }

        /// <summary>
        /// Gets new <see cref="ScanApp.Domain.Entities.Location"/> for the edited user.
        /// </summary>
        /// <value>New <see cref="ScanApp.Domain.Entities.Location"/> if set, otherwise <see langword="null"/></value>
        public Location Location { get; init; }
    }

    internal class EditUserDataCommandHandler : IRequestHandler<EditUserDataCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public EditUserDataCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<Result<Version>> Handle(EditUserDataCommand request, CancellationToken cancellationToken)
        {
            var data = new EditUserDto(request.Name)
            {
                Phone = request.Phone,
                Email = request.Email,
                Version = request.Version,
                NewName = request.NewName,
                Location = request.Location
            };

            return await _userManager.EditUserData(data).ConfigureAwait(false);
        }
    }
}