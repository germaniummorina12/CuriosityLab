using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using CodeInvention.CuriosityLab.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class BulkAdd
    {
        public record Command(int userId, List<int> contactTypeIds, List<string> values)
            : IRequest<IEnumerable<int>>
        {
            public IEnumerable<Contact> ToEntities()
            {
                var now = DateTime.UtcNow;

                return contactTypeIds.Zip(values, (typeId, value) => new Contact
                {
                    UserId = userId,
                    ContactTypeId = typeId,
                    Value = value.Trim().ToLowerInvariant(),
                    CreatedAt = now
                });
            }
        }

        public class CommandHandler : IRequestHandler<Command, IEnumerable<int>>
            {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

           public async Task<IEnumerable<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                var failures = new List<ValidationFailure>();

                var userExists = await _context.Users
                    .AnyAsync(user => user.Id == request.userId, cancellationToken);

                if (!userExists)
                {
                    failures.Add(new ValidationFailure(nameof(request.userId),
                        $"User {request.userId} does not exist"));
                }

                var requestedTypeIds = request.contactTypeIds.Distinct().ToList();

                var existingTypeIds = await _context.ContactTypes
                    .Where(contactType => requestedTypeIds.Contains(contactType.Id))
                    .Select(contactType => contactType.Id)
                    .ToListAsync(cancellationToken);

                foreach (var missingId in requestedTypeIds.Except(existingTypeIds))
                {
                    failures.Add(new ValidationFailure(nameof(request.contactTypeIds),
                        $"Contact type {missingId} does not exist"));
                }

                if (failures.Count > 0)
                {
                    throw new ValidationException(failures);
                }

                var contacts = request.ToEntities().ToList();

                _context.Contacts.AddRange(contacts);
                await _context.SaveChangesAsync(cancellationToken);

                return contacts.Select(contact => contact.Id).ToList();
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandValidator(IApplicationDbContext context)
            {
                _context = context;
                RuleLevelCascadeMode = CascadeMode.Stop;

                RuleFor(c => c.userId).GreaterThan(0);

                RuleFor(c => c.contactTypeIds)
                    .NotEmpty().WithMessage("At least one contact is required");

                RuleFor(c => c.values)
                    .NotEmpty();

                RuleFor(c => c)
                    .Must(c => c.contactTypeIds?.Count == c.values?.Count)
                    .WithMessage("contactTypeIds and values must have the same number of items");

                RuleForEach(c => c.contactTypeIds).GreaterThan(0);

                RuleForEach(c => c.values)
                    .NotEmpty()
                    .MaximumLength(255);

                RuleFor(user => user.userId)
                         .Must((userId) =>
                         {
                             return _context.Users.Any(user => user.Id == userId);
                         })
                         .WithMessage("No user found with this id!");

                RuleFor(contactType => contactType.contactTypeIds)
                    .MustAsync(async (ids, ct) =>
                    {
                        var distinct = ids.Distinct().ToList();
                        var found = await context.ContactTypes
                            .CountAsync(t => distinct.Contains(t.Id), ct);
                        return found == distinct.Count;
                    })
                    .WithMessage("No contact type found with this id!");

                RuleFor(c => c).Custom((command, ctx) =>
                {
                    for (var i = 0; i < command.values.Count; i++)
                    {
                        var typeId = command.contactTypeIds[i];
                        var value = command.values[i]?.Trim() ?? string.Empty;

                        if (typeId == (int)ContactTypeEnum.Email &&
                            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(value))
                        {
                            ctx.AddFailure($"values[{i}]", "Email is not valid");
                        }

                        if ((typeId == (int)ContactTypeEnum.PhoneNumber || typeId == (int)ContactTypeEnum.FaxNumber) &&
                            !System.Text.RegularExpressions.Regex.IsMatch(value, @"^\+[1-9]\d{6,14}$"))
                        {
                            ctx.AddFailure($"values[{i}]",
                                "Phone or fax number is not valid. Use international format, e.g. +3612345678");
                        }
                    }
                })
                .When(command => !string.IsNullOrWhiteSpace(command.value))
                    .WithMessage("This contact has already been added.");

                RuleFor(contact => contact.value)
                    .EmailAddress()
                    .When(contact => contact.contactTypeId == (int)ContactTypeEnum.Email)
                    .WithMessage("Email is not valid");

                RuleFor(contact => contact.value)
                    .Matches(@"^\+[1-9]\d{6,14}$")
                    .WithMessage("Phone or fax number is not valid. Use international format, e.g. +3612345678")
                    .When(contact =>
                        contact.contactTypeId == (int)ContactTypeEnum.PhoneNumber ||
                        contact.contactTypeId == (int)ContactTypeEnum.FaxNumber);
            }
        }
    }
}