using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using CodeInvention.CuriosityLab.Domain.Enums;
using FluentValidation;
using MediatR;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class Add
    {
        public record Command(string name, string email, int? age) : IRequest
        {
            public User ToEntity() => new User
            {
                Name = name,
                Age = age,
                CreatedAt = DateTime.UtcNow,
                Contacts =
                {
                    new Contact
                    {
                        ContactTypeId = (int)ContactTypeEnum.Email,
                        Value = email
                    }
                }
            };
        }
        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var user = request.ToEntity();
                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IApplicationDbContext _context;
            public CommandValidator(IApplicationDbContext context)
            {
                _context = context;
                RuleLevelCascadeMode = CascadeMode.Stop;
                //ClassLevelCascadeMode = CascadeMode.Stop;

                RuleFor(user => user.name)
                    .NotEmpty()
                    .WithMessage("Name is Required");

                RuleFor(user => user.email)
                   .NotEmpty()
                   .WithMessage("Email is Required")
                   .EmailAddress()
                   .WithMessage("Email is not valid")
                   .Must((email) =>
                   {
                       return !_context.Contacts.Any(u => u.Value == email && u.ContactTypeId == (int)ContactTypeEnum.Email);
                   })
                   .WithMessage("This email is used for another account");
            }
        }
    }
}
