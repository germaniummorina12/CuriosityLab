using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class Update
    {
        public record Command(int id, string name, string email, int? age) : IRequest;
        public class CommandHandler : IRequestHandler<Command>
        {
            private IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == request.id);
                
                if (user == null)
                {
                    throw new ArgumentException("User doesn't exists.", nameof(user));
                }
                
                user.Name = request.name;
                user.Email = request.email;
                user.Age = request.age;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleLevelCascadeMode = CascadeMode.Stop;

                RuleFor(user => user.name)
                    .NotEmpty()
                    .WithMessage("Name is Required");

                RuleFor(user => user.email)
                    .NotEmpty()
                    .WithMessage("Email is Required")
                    .EmailAddress()
                    .WithMessage("Email not Valid");
            }
        }
    }
}
