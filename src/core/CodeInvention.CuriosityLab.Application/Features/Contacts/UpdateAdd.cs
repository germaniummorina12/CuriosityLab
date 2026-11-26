using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CodeInvention.CuriosityLab.Application.Features.Contacts
{
    public class UpdateAdd
    {
        //public record Add(int contactTypeId, string value);

        // Databaze i kemi 3 kontakte me id: 1, id: 2, id: 3

        // Requesti na ka ardh prej frontit: id: null, id: 1, id: 2
        // kur id eshte null duhet te krijohet e re, kur id ka vlere duhet te behet update, kur id ekziston ne databaze por nuk vjen nga fronti duhet te fshihet
        public record ContactCommand(int? id, int contactTypeId, string value);
        //public record Update(int id, int contactTypeId, string value);
        //public record Command(int userId, List<Update>? updates, List<Add>? adds, List<int>? deleteIds) : IRequest
        public record Command(int userId, List<ContactCommand> contacts) : IRequest
        {
            public IEnumerable<Contact> ToEntities()
            {
                var now = DateTime.UtcNow;

                return contacts.Where(contact => contact.id == null).Select(add => new Contact
                {
                    UserId = userId,
                    ContactTypeId = add.contactTypeId,
                    Value = add.value.Trim().ToLower(),
                    CreatedAt = now
                });
            }
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
                var ids = request.contacts.Where(t => t.id != null).Select(update => update.id).ToList();

                var currentContacts = _context.Contacts
                    .Where(contact => ids.Contains(contact.Id))
                    .ToList();

                foreach (var update in request.contacts.Where(contact => contact.id != null))
                {
                    var currentContact = currentContacts.FirstOrDefault(contact => contact.Id == update.id);
                    if (currentContact == null)
                        continue;

                    currentContact.ContactTypeId = update.contactTypeId;
                    currentContact.Value = update.value;
                    currentContact.UpdatedAt = DateTime.UtcNow;
                }

                var contacts = request.ToEntities().ToList();

                _context.Contacts.AddRange(contacts);

                // Mi gjet kontaktet te cilat jane ne databaze dhe qe nuk na kan ardh ne request, ato kontakte duhet mi fshi

                var currentIds = _context.Contacts
                    .Where(contact => contact.UserId == request.userId)
                    .Select(contact => contact.Id)
                    .ToList();

                var sentIds = request.contacts
                    .Where(contact => contact.id.HasValue)
                    .Select(contact => contact.id!.Value)
                    .ToList();

                foreach (var contact in currentContacts.Where(contact => !sentIds.Contains(contact.Id)))
                {
                    contact.IsDeleted = true;
                    contact.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
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

                RuleFor(command => command.contacts)
                    .NotEmpty()
                    .WithMessage("At least one contact is required")
                    .Must((contacts) =>
                    {
                        return contacts.Select(item => item.id).Distinct().Count() == contacts.Count;
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

                RuleForEach(command => command.contacts)
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