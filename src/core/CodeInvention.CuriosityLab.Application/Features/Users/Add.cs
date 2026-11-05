using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using FluentValidation.Validators;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Users
{
    public class Add
    {
        public record Command(string name, string email, int? age) : IRequest
        {
            public User ToEntity() => new User
            {
                Name = name,
                Email = email,
                Age = age,
                CreatedAt = DateTime.UtcNow
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
            public CommandValidator()
            {
                RuleLevelCascadeMode = CascadeMode.Stop;
                //ClassLevelCascadeMode = CascadeMode.Stop;

                RuleFor(user => user.name)
                    .NotEmpty()
                    .WithMessage("Name is Required");

                RuleFor(user => user.email)
                   .NotEmpty()
                   .WithMessage("Email is Required")
                   .EmailAddress()
                   .WithMessage("Email is not valid");
            }
        }
    }
}
