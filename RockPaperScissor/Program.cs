using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Documents;

using Microsoft.Extensions.Logging;
namespace RockPaperScissor
{
    //cosmos trigger exempel
    public static class CosmosTrigger
    {
        [FunctionName("CosmosTrigger")]
        public static void Run([CosmosDBTrigger(
                databaseName: "ToDoItems",
                collectionName: "Items",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> documents,
            ILogger log)
        {
            if (documents != null && documents.Count > 0)
            {
                log.LogInformation($"Documents modified: {documents.Count}");
                log.LogInformation($"First document Id: {documents[0].Id}");
            }
        }
    }
    class Program
    {
        private static readonly string EndpointUri = "https://localhost:8081";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "FamilyDatabase";
        private string containerId = "FamilyContainer";

        public static async Task Main(string[] args)
        {

            try
            {
                Console.WriteLine("Beginning operations...");
                Program p = new Program();
                await p.StartGame();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }
        public async Task StartGame()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await this.CreateDatabaseAsync();
            // skapar en container
            await this.CreateContainerAsync();
            //await this.AddItemsToContainerAsync();
            //await this.QueryItemsAsync();
            //await this.ReplaceFamilyItemAsync();
            //await this.DeleteFamilyItemAsync();
            //await this.DeleteDatabaseAndCleanupAsync();
        }
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/LastName");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }
        //private async Task AddItemsToContainerAsync()
        //{
        //    // Create a family object for the Andersen family
        //    Game andersenFamily = new Game
        //    {
        //        Id = "Andersen.1",
        //        LastName = "Andersen",
        //        Parents = new Parent[]
        //        {
        //        new Parent { FirstName = "Thomas" },
        //        new Parent { FirstName = "Mary Kay" }
        //        },
        //        Children = new Child[]
        //        {
        //        new Child
        //        {
        //            FirstName = "Henriette Thaulow",
        //            Gender = "female",
        //            Grade = 5,
        //            Pets = new Pet[]
        //            {
        //                new Pet { GivenName = "Fluffy" }
        //            }
        //        }
        //        },
        //        Address = new Address { State = "WA", County = "King", City = "Seattle" },
        //        IsRegistered = false
        //    };
        //
        //    try
        //    {
        //        // Read the item to see if it exists.  
        //        ItemResponse<Family> andersenFamilyResponse = await this.container.ReadItemAsync<Family>(andersenFamily.Id, new PartitionKey(andersenFamily.LastName));
        //        Console.WriteLine("Item in database with id: {0} already exists\n", andersenFamilyResponse.Resource.Id);
        //    }
        //    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        //    {
        //        // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
        //        ItemResponse<Family> andersenFamilyResponse = await this.container.CreateItemAsync<Family>(andersenFamily, new PartitionKey(andersenFamily.LastName));
        //
        //        // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
        //        Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", andersenFamilyResponse.Resource.Id, andersenFamilyResponse.RequestCharge);
        //    }
        //
        //    // Create a family object for the Wakefield family
        //    Family wakefieldFamily = new Family
            {
                Id = "Wakefield.7",
                LastName = "Wakefield",
                Parents = new Parent[]
                {
                new Parent { FamilyName = "Wakefield", FirstName = "Robin" },
                new Parent { FamilyName = "Miller", FirstName = "Ben" }
                },
                Children = new Child[]
                {
                new Child
                {
                    FamilyName = "Merriam",
                    FirstName = "Jesse",
                    Gender = "female",
                    Grade = 8,
                    Pets = new Pet[]
                    {
                        new Pet { GivenName = "Goofy" },
                        new Pet { GivenName = "Shadow" }
                    }
                },
                new Child
                {
                    FamilyName = "Miller",
                    FirstName = "Lisa",
                    Gender = "female",
                    Grade = 1
                }
                },
                Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
                IsRegistered = true
            };
        //
        //    try
        //    {
        //        // Read the item to see if it exists
        //        ItemResponse<Family> wakefieldFamilyResponse = await this.container.ReadItemAsync<Family>(wakefieldFamily.Id, new PartitionKey(wakefieldFamily.LastName));
        //        Console.WriteLine("Item in database with id: {0} already exists\n", wakefieldFamilyResponse.Resource.Id);
        //    }
        //    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        //    {
        //        // Create an item in the container representing the Wakefield family. Note we provide the value of the partition key for this item, which is "Wakefield"
        //        ItemResponse<Family> wakefieldFamilyResponse = await this.container.CreateItemAsync<Family>(wakefieldFamily, new PartitionKey(wakefieldFamily.LastName));
        //
        //        // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
        //        Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", wakefieldFamilyResponse.Resource.Id, wakefieldFamilyResponse.RequestCharge);
        //    }
        //}
        //private async Task QueryItemsAsync()
        //{
        //    var sqlQueryText = "SELECT * FROM c WHERE c.LastName = 'Andersen'";
        //
        //    Console.WriteLine("Running query: {0}\n", sqlQueryText);
        //
        //    QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
        //    FeedIterator<Family> queryResultSetIterator = this.container.GetItemQueryIterator<Family>(queryDefinition);
        //
        //    List<Family> families = new List<Family>();
        //
        //    while (queryResultSetIterator.HasMoreResults)
        //    {
        //        FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
        //        foreach (Family family in currentResultSet)
        //        {
        //            families.Add(family);
        //            Console.WriteLine("\tRead {0}\n", family);
        //        }
        //    }
        //}
        //private async Task ReplaceFamilyItemAsync()
        //{
        //    ItemResponse<Family> wakefieldFamilyResponse = await this.container.ReadItemAsync<Family>("Wakefield.7", new PartitionKey("Wakefield"));
        //    var itemBody = wakefieldFamilyResponse.Resource;
        //
        //    // update registration status from false to true
        //    itemBody.IsRegistered = true;
        //    // update grade of child
        //    itemBody.Children[0].Grade = 6;
        //
        //    // replace the item with the updated content
        //    wakefieldFamilyResponse = await this.container.ReplaceItemAsync<Family>(itemBody, itemBody.Id, new PartitionKey(itemBody.LastName));
        //    Console.WriteLine("Updated Family [{0},{1}].\n \tBody is now: {2}\n", itemBody.LastName, itemBody.Id, wakefieldFamilyResponse.Resource);
        //}
        //private async Task DeleteFamilyItemAsync()
        //{
        //    var partitionKeyValue = "Wakefield";
        //    var familyId = "Wakefield.7";
        //
        //    // Delete an item. Note we must provide the partition key value and id of the item to delete
        //    ItemResponse<Family> wakefieldFamilyResponse = await this.container.DeleteItemAsync<Family>(familyId, new PartitionKey(partitionKeyValue));
        //    Console.WriteLine("Deleted Family [{0},{1}]\n", partitionKeyValue, familyId);
        //}
        //private async Task DeleteDatabaseAndCleanupAsync()
        //{
        //    DatabaseResponse databaseResourceResponse = await this.database.DeleteAsync();
        //    // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();
        //
        //    Console.WriteLine("Deleted Database: {0}\n", this.databaseId);
        //
        //    //Dispose of CosmosClient
        //    this.cosmosClient.Dispose();
        //}
    }
}
