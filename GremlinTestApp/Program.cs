using GremlinTestApp.Extensions;
using GremlinTestApp.Gremlin.Edges;
using GremlinTestApp.Gremlin.Vertex;
using GremlinTestApp.Interfaces;
using GremlinTestApp.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GremlinTestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
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


            while (true)
            {
                Console.WriteLine("1 - GetPersonAsync");
                Console.WriteLine("2 - GetPersonKnowsAsync");
                Console.WriteLine("3 - UpdatePersonAsync");
                Console.WriteLine("4 - CreatePersonAsync");
                Console.WriteLine("5 - GetPeopleAsync");
                Console.WriteLine("6 - DeletePersonAsync");
                Console.WriteLine("X - Exit");

                Console.WriteLine("Enter your desired option:");

                var key = Console.ReadKey();
                var keyInfo = key.KeyChar;
                if (char.IsWhiteSpace(keyInfo) || keyInfo.Equals('X'))
                    break;

                string? personIdInput;
                Guid personId;
                PersonVertex? person = null;
                string? personName;

                Console.WriteLine(Environment.NewLine);

                switch (keyInfo)
                {
                    case '1':
                        Console.WriteLine("Enter your desired person Id (Guid):"); // Kyle - a45e4333-e9eb-4a13-89b3-07f289edf449

                        personIdInput = Console.ReadLine();
                        if (!Guid.TryParse(personIdInput, out personId))
                        {
                            logger.LogWarning("You need to enter a valid Id");
                            break;
                        }

                        person = await personRepository.GetPersonAsync(personId);

                        if (person != null)
                        {
                            Console.WriteLine("\nPerson [{0}] on Partition [{1}] has Name [{2}]", person.Id, person.Pk, person.Name);
                        }

                        break;
                    case '2':
                        Console.WriteLine("Enter your desired person Id (Guid):"); // Kyle - a45e4333-e9eb-4a13-89b3-07f289edf449

                        personIdInput = Console.ReadLine();
                        if (!Guid.TryParse(personIdInput, out personId))
                        {
                            logger.LogWarning("You need to enter a valid Id");
                            break;
                        }

                        await foreach (var personKnowsTemp in personRepository.GetPersonKnowsAsync(personId))
                        {
                            Console.WriteLine("\nEdge [{0}] for Person [{1}] Knows [{2}] Since [{3}]", personKnowsTemp.Id, personKnowsTemp.OutVertex, personKnowsTemp.InVertex, personKnowsTemp.KnownSince);
                        }

                        break;
                    case '3':
                        Console.WriteLine("Enter your desired person Id (Guid):"); // Kyle - a45e4333-e9eb-4a13-89b3-07f289edf449

                        personIdInput = Console.ReadLine();
                        if (!Guid.TryParse(personIdInput, out personId))
                        {
                            logger.LogWarning("You need to enter a valid Id");
                            break;
                        }

                        Console.WriteLine("Enter your desired person name (string):");

                        personName = Console.ReadLine();
                        if (string.IsNullOrEmpty(personName))
                        {
                            logger.LogWarning("You need to enter a valid name");
                            break;
                        }

                        var updatedPerson = await personRepository.UpdatePersonAsync(personId, personName);

                        if (updatedPerson != null)
                        {
                            Console.WriteLine("\nPerson [{0}] on Partition [{1}] has been updated. New Name [{2}]", updatedPerson.Id, updatedPerson.Pk, updatedPerson.Name);
                        }

                        break;
                    case '4':
                        Console.WriteLine("Enter your desired partition key (string):");

                        var partitionKey = Console.ReadLine();
                        if (string.IsNullOrEmpty(partitionKey))
                        {
                            logger.LogWarning("You need to enter a valid partition key");
                            break;
                        }

                        Console.WriteLine("Enter your desired person name (string):");

                        personName = Console.ReadLine();
                        if (string.IsNullOrEmpty(personName))
                        {
                            logger.LogWarning("You need to enter a valid name");
                            break;
                        }

                        Console.WriteLine("Enter your the ids this new person knows (Guid,Guid):");

                        var personKnows = Console.ReadLine();
                        PersonVertex? newPerson = null;
                        if (string.IsNullOrEmpty(personKnows))
                        {
                            newPerson = await personRepository.CreatePersonAsync(partitionKey, personName);
                        }
                        else
                        {
                            var personKnowsList = new List<Guid>();
                            var personKnowsSplit = personKnows.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var personKnowsSplitItem in personKnowsSplit)
                            {
                                if (Guid.TryParse(personKnowsSplitItem, out var personKnowsId))
                                {
                                    personKnowsList.Add(personKnowsId);
                                }
                                else
                                {
                                    logger.LogWarning("\nCannot add [{0}] to the new person", personKnowsSplitItem);
                                }
                            }

                            newPerson = await personRepository.CreatePersonAsync(partitionKey, personName, personKnowsList);
                        }

                        if (newPerson != null)
                        {
                            Console.WriteLine("\nPerson [{0}] on Partition [{1}] has been created with Name [{2}]", newPerson.Id, newPerson.Pk, newPerson.Name);
                        }

                        break;
                    case '5':
                        await foreach (var listPerson in personRepository.GetPeopleAsync(0, 4))
                        {
                            Console.WriteLine("\nPerson [{0}] on Partition [{1}] has Name [{2}]", listPerson.Id, listPerson.Pk, listPerson.Name);
                        }

                        break;
                    case '6':
                        Console.WriteLine("Enter your desired person Id (Guid):"); // Kyle - a45e4333-e9eb-4a13-89b3-07f289edf449

                        personIdInput = Console.ReadLine();
                        if (!Guid.TryParse(personIdInput, out personId))
                        {
                            logger.LogWarning("You need to enter a valid Id");
                            break;
                        }

                        await personRepository.DeletePersonAsync(personId);
                        Console.WriteLine("\nPerson [{0}] has been deleted", personId);

                        break;
                    default:
                        break;
                }

                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}
