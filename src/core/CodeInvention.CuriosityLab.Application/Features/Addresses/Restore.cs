using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Addresses
{
    public class Restore
    {
        public record Command(int id) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var address = await _context.Addresses.IgnoreQueryFilters().FirstOrDefaultAsync(address => address.Id == request.id);

                if (address == null)
                {
                    throw new ArgumentException("Address doesn't exists");
                }

                address.IsDeleted = false;
                _context.Addresses.Update(address);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
