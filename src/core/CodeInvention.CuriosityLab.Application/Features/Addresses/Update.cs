using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class Update
    {
        public record Command(int id, string city, string street, string postalCode) : IRequest;
        public class CommandHandler : IRequestHandler<Command>
        {
            private IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var address = await _context.Addresses.FirstOrDefaultAsync(address => address.Id == request.id);

                address.City = request.city;
                address.Street = request.street;
                address.PostalCode = request.postalCode;
                address.UpdatedAt = DateTime.UtcNow;

                _context.Addresses.Update(address);
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

                RuleFor(address => address.id)
                    .Must((id) =>
                    {
                        return _context.Addresses.Any(address => address.Id == id);
                    })
                    .WithMessage("This address do not exist!");
            }
        }
    }
}