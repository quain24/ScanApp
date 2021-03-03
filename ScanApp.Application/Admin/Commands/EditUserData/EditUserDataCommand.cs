﻿using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommand : IRequest<Result<ConcurrencyStamp>>
    {
        public string Name { get; }
        public string NewName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public ConcurrencyStamp ConcurrencyStamp { get; set; }

        public EditUserDataCommand(string name)
        {
            Name = name;
        }
    }

    public class EditUserDataCommandHandler : IRequestHandler<EditUserDataCommand, Result<ConcurrencyStamp>>
    {
        private readonly IUserManager _userManager;

        public EditUserDataCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<ConcurrencyStamp>> Handle(EditUserDataCommand request, CancellationToken cancellationToken)
        {
            var data = new EditUserDto(request.Name)
            {
                Phone = request.Phone,
                Email = request.Email,
                Location = request.Location,
                ConcurrencyStamp = request.ConcurrencyStamp,
                NewName = request.NewName
            };

            return await _userManager.EditUserData(data).ConfigureAwait(false);
        }
    }
}