using CodeInvention.CuriosityLab.Common.Extensions;
using CodeInvention.CuriosityLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeInvention.CuriosityLab.Persistence
{
    public class ApplicationDbContextInitializer
    {
        public void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            Seed(context);
        }

        private void Seed(ApplicationDbContext context)
        {
            SeedContactTypes(context);
            SeedRoles(context);
        }

        private void SeedContactTypes(ApplicationDbContext context)
        {
            if (!context.ContactTypes.Any())
            {
                var contactTypes = ReadJson("contactTypes.json").Deserialize<IEnumerable<ContactType>>();
                context.ContactTypes.AddRange(contactTypes);
                context.SaveChanges();
            }
        }

        private void SeedRoles(ApplicationDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = ReadJson("roles.json").Deserialize<IEnumerable<Role>>();
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }
        }

        private string ReadJson(string fileName)
        {
            var assembly = typeof(ApplicationDbContext).Assembly;

            var resources = assembly.GetManifestResourceNames();

            using (Stream strem = assembly.GetManifestResourceStream(resources.First(x => x.Contains(fileName))))
            {
                using (StreamReader sr = new StreamReader(strem))
                {
                    var result = sr.ReadToEnd();
                    return result;
                }
            }
        }
    }
}