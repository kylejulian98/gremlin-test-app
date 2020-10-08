using GremlinTestApp.Extensions;
using GremlinTestApp.Interfaces;
using GremlinTestApp.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GremlinTestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>(optional: true, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging(options =>
                {
                    options.AddConsole();
                })
                .Configure<GremlinOptions>(configuration.GetSection(GremlinOptions.APPSETTINGS_KEY))
                .AddGremlinClient()
                .AddTransient<IPersonRepository, PersonGremlinRepository>()
                .BuildServiceProvider();

            var personRepository = serviceProvider.GetService<IPersonRepository>();
            var logger = serviceProvider.GetService<ILogger<Program>>();
            
            // Read
            var personId = Guid.Parse("a45e4333-e9eb-4a13-89b3-07f289edf449");
            var person = await personRepository.GetPersonAsync(personId);

            if (person != null)
            {
                logger.LogInformation("Person [{Id}] on Partition [{Pk}] has Name [{Name}]", person.Id, person.Pk, person.Name);
            }

            await foreach(var personKnows in personRepository.GetPersonKnowsAsync(personId))
            {
                logger.LogInformation("Edge [{Id}] for Person [{OutVertex}] Knows [{InVertex}] Since [{KnownSince}]", personKnows.Id, personKnows.OutVertex, personKnows.InVertex, personKnows.KnownSince);
            }

            // Update
            var newPersonName = string.Format("Kyle-{0}", DateTime.UtcNow.Millisecond);
            var updatedPerson = await personRepository.UpdatePersonAsync(personId, newPersonName);

            if (updatedPerson != null)
            {
                logger.LogInformation("Person [{Id}] on Partition [{Pk}] has been updated. New Name [{Name}]", updatedPerson.Id, updatedPerson.Pk, updatedPerson.Name);
            }

            // Create
            var newPerson = await personRepository.CreatePersonAsync("engineering", string.Format("Kyle-{0}", DateTime.UtcNow.Millisecond), new[] { personId });

            if (newPerson != null)
            {
                logger.LogInformation("Person [{Id}] on Partition [{Pk}] has been created with Name [{Name}]", newPerson.Id, newPerson.Pk, newPerson.Name);
            }

            // Delete
            if (newPerson != null)
            {
                await personRepository.DeletePersonAsync(newPerson.Id);
                logger.LogInformation("Person [{Id}] has been deleted", newPerson.Id);
            }

            Console.ReadLine();
        }
    }
}
