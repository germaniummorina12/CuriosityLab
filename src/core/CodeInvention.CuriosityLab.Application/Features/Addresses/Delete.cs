using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class Delete
    {
        public record Command(int id) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var address = await _context.Addresses.FirstOrDefaultAsync(address => address.Id == request.id);

                address.IsDeleted = true;
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

                RuleFor(address => address.id)
                    .Must((id) =>
                    {
                        return _context.Addresses.Any(address => address.Id == id);
                    })
                    .WithMessage("This address was not found!");
            }
        }
    }
}