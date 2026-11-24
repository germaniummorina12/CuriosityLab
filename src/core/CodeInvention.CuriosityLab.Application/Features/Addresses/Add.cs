using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class Add
    {
        public record Command(int userId, string city, string street, string postalCode) : IRequest
        {
            public Address toEntity() => new Address
            {
                UserId = userId,
                City = city,
                Street = street,
                PostalCode = postalCode,
                CreatedAt = DateTime.UtcNow
            };
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var address = request.toEntity();

                _context.Addresses.Add(address);
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

                RuleFor(address => address.city)
                    .NotEmpty()
                    .WithMessage("City is required");

                RuleFor(address => address.street)
                    .NotEmpty()
                    .WithMessage("Street is required");

                RuleFor(address => address.postalCode)
                    .NotEmpty()
                    .WithMessage("Postal Code is required");

                RuleFor(user => user.userId)
                    .Must((id) =>
                    {
                        return _context.Users.Any(user => user.Id == id);
                    })
                    .WithMessage("No user found with this id!");

                RuleFor(user => user.userId)
                    .Must((command, userId) =>
                    {
                        return !_context.Addresses.Any(user => user.UserId == userId && 
                            command.city == user.City && 
                            command.street == user.Street && 
                            command.postalCode == user.PostalCode);
                    })
                    .WithMessage("This address has already been added.");
            }
        }
    }
}