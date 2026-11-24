using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using CodeInvention.CuriosityLab.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class BulkUpdate
    {
        public record Item(int id, int contactTypeId, string value);
        public record Command(int userId, List<Item> items) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var itemsById = request.items.ToDictionary(item => item.id);
                var ids = itemsById.Keys.ToList();

                var contacts = await _context.Contacts
                    .Where(contact => ids.Contains(contact.Id))
                    .ToListAsync(cancellationToken);

                var now = DateTime.UtcNow;

                foreach (var contact in contacts)
                {
                    var item = itemsById[contact.Id];

                    contact.ContactTypeId = item.contactTypeId;
                    contact.Value = item.value.Trim().ToLowerInvariant();
                    contact.UpdatedAt = now;
                }

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

                RuleFor(command => command.userId)
                    .GreaterThan(0)
                    .WithMessage("User id must be positive")
                    .Must((userId) =>
                    {
                        return _context.Users.Any(user => user.Id == userId);
                    })
                    .WithMessage("No user found with this id!");

                RuleFor(command => command.items)
                    .NotEmpty()
                    .WithMessage("At least one contact is required")
                    .Must((items) =>
                    {
                        return items.Select(item => item.id).Distinct().Count() == items.Count;
                    })
                    .WithMessage("The same contact id appears more than once")
                    .Must((items) =>
                    {
                        return items
                            .Select(item => (item.contactTypeId, value: item.value?.Trim().ToLowerInvariant()))
                            .Distinct()
                            .Count() == items.Count;
                    })
                    .WithMessage("The same contact type and value appear more than once");

                RuleForEach(command => command.items).ChildRules(item =>
                {
                    item.RuleFor(i => i.id)
                        .GreaterThan(0)
                        .WithMessage("Contact id must be positive");

                    item.RuleFor(i => i.value)
                        .NotEmpty()
                        .WithMessage("Value is required")
                        .MaximumLength(255)
                        .WithMessage("Value must be at most 255 characters");

                    item.RuleFor(i => i.contactTypeId)
                        .Must((contactTypeId) =>
                        {
                            return _context.ContactTypes.Any(contactType => contactType.Id == contactTypeId);
                        })
                        .WithMessage("No contact type found with this id!");

                    item.RuleFor(i => i.value)
                        .EmailAddress()
                        .When(i => i.contactTypeId == (int)ContactTypeEnum.Email)
                        .WithMessage("Email is not valid");

                    item.RuleFor(i => i.value)
                        .Matches(@"^\+[1-9]\d{6,14}$")
                        .When(i => i.contactTypeId == (int)ContactTypeEnum.PhoneNumber ||
                                   i.contactTypeId == (int)ContactTypeEnum.FaxNumber)
                        .WithMessage("Phone or fax number is not valid. Use international format, e.g. +3612345678");
                });

                RuleForEach(command => command.items)
                    .Must((command, item) =>
                    {
                        return _context.Contacts.Any(contact =>
                            contact.Id == item.id &&
                            contact.UserId == command.userId);
                    })
                    .WithMessage("No contact found with this id for this user!");
            }
        }
    }
}