using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class Update
    {
        public record Command(int id, int contactTypeId, string value) : IRequest;
        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public CommandHandler(IApplicationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var contact = await _context.Contacts.FirstOrDefaultAsync(contact => contact.Id == request.id);

                if (contact == null)
                {
                    throw new ArgumentException("Contact not found!");
                }

                contact.ContactTypeId = request.contactTypeId;
                contact.Value = request.value.Trim().ToLower();
                contact.UpdatedAt = DateTime.UtcNow;

                _context.Contacts.Update(contact);
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

                RuleFor(contact => contact.contactTypeId)
                    .NotEmpty()
                    .WithMessage("Contact type is required");

                RuleFor(contact => contact.value)
                    .NotEmpty()
                    .WithMessage("Value is required");

                RuleFor(contact => contact.id)
                    .Must((id) =>
                    {
                        return _context.Contacts.Any(contact => contact.Id == id);
                    })
                    .WithMessage("No contact found with this id!");

                RuleFor(contactType => contactType.contactTypeId)
                    .Must((contactTypeId) =>
                    {
                        return _context.ContactTypes.Any(contactType => contactType.Id == contactTypeId);
                    })
                    .WithMessage("No contact type found with this id!");

                RuleFor(contact => contact.id)
                    .Must((command, id) =>
                    {
                        var value = command.value.Trim().ToLower();

                        var ownerId = _context.Contacts
                            .Where(c => c.Id == id)
                            .Select(c => c.UserId)
                            .FirstOrDefault();

                        return !_context.Contacts.Any(contact => 
                            contact.Id != command.id &&
                            contact.UserId == ownerId &&
                            command.contactTypeId == contact.ContactTypeId &&
                            contact.Value == value
                            );
                    })
                    .When(command => !string.IsNullOrWhiteSpace(command.value))
                    .WithMessage("This contact has already been added.");

                RuleFor(contact => contact.value)
                    .Must(value => new EmailAddressAttribute().IsValid(value?.Trim()))
                    .WithMessage("Email is not valid")
                    .When(contact => contact.contactTypeId == (int)ContactTypeEnum.Email);

                RuleFor(contact => contact.value)
                    .Must(value => Regex.IsMatch(
                        Regex.Replace(value ?? "", @"[\s\-()]", ""),
                        @"^\+[1-9]\d{6,14}$"))
                    .WithMessage("Phone or fax number is not valid. Use international format, e.g. +3612345678")
                    .When(contact =>
                        contact.contactTypeId == (int)ContactTypeEnum.PhoneNumber ||
                        contact.contactTypeId == (int)ContactTypeEnum.FaxNumber);
            }
        }
    }
}