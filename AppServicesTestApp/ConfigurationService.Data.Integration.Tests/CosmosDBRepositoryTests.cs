using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationService.Exceptions;
using ConfigurationService.Exctensions;
using ConfigurationService.Models;
using ConfigurationService.Services.Exceptions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConfigurationService.Data.Integration.Tests
{
    public class CosmosDBRepositoryTests
    {
        private IDocumentClient _client;
        private IDocumentRepository<Device, Guid> _sut;
        private string _databaseId;
        private string _collectionName;

        public CosmosDBRepositoryTests()
        {
            InitializeEnvironment();
        }

        [Fact]
        public async Task UpdateDeviceConcurrently_ShouldUpdate_Async()
        {
            var entity = new Device
            {
                Id = Guid.NewGuid(),
                Type = "D1",
                Config = new Configuration
                {
                    FirmwareVersion = "v.1.0.0"
                }
            };
            await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName), entity);

            var newEntity = new Device
            {

                Id = entity.Id,
                Type = "D1",
                Config = new Configuration
                {
                    FirmwareVersion = "v.1.0.2"
                }
            };

            var updatedEntity = await _sut.UpdateAsync(newEntity.Id, newEntity);
            Assert.Equal(updatedEntity.Config.FirmwareVersion, newEntity.Config.FirmwareVersion);
            Assert.NotNull(updatedEntity.ETag);
        }

        [Fact]
        public async Task UpdateDeviceConcurrently_ShouldDetectConflict_Async()
        {
            var entity = new Device
            {
                Id = Guid.NewGuid(),
                Type = "D1",
                Config = new Configuration
                {
                    FirmwareVersion = "v.1.0.0"
                }
            };
            ResourceResponse<Document> insertedEntity = await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName), entity);
            var id = insertedEntity.Resource.Id;
            var conflict = _client
                .CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName))
                .Where(x => x.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            conflict.SetPropertyValue(nameof(entity.Type), "D2");
            await _client.ReplaceDocumentAsync(conflict);

            var newEntity = new Device
            {

                Id = Guid.Parse(insertedEntity.Resource.Id),
                Type = "D1",
                Config = new Configuration
                {
                    FirmwareVersion = "v.1.0.2"
                },
                ETag = insertedEntity.Resource.ETag 
            };

            await Assert.ThrowsAsync<ConflictException>(async () => await _sut.UpdateAsync(newEntity.Id, newEntity));
        }

        [Fact]
        public async Task UpdateDeviceConcurrently_ShouldNotFoundDocument_Async()
        {
            var entity = new Device
            {
                Id = Guid.NewGuid(),
                Type = "D1",
                Config = new Configuration
                {
                    FirmwareVersion = "v.1.0.0"
                }
            };
            Document insertedEntity = await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName), entity);
            await _client.DeleteDocumentAsync(insertedEntity.SelfLink, new RequestOptions { PartitionKey = new PartitionKey(insertedEntity.Id)});

            var newEntity = new Device
            {

                Id = Guid.Parse(insertedEntity.Id),
                Type = "D1",
                Config = new Configuration
                {
                    FirmwareVersion = "v.1.0.2"
                },
                ETag = insertedEntity.ETag
            };

            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.UpdateAsync(newEntity.Id, newEntity));
        }

        [Fact]
        public async Task GetDeviceById_ShouldReturnDevice_Async()
        {
            var entity = new Device
            {
                Id = Guid.NewGuid(),
                Type = "D1",
                Config = new Configuration
                {
                    FirmwareVersion = "v.1.0.0"
                }
            };
            ResourceResponse<Document> insertedEntity = await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName), entity);
            var device = await _sut.GetByIdAsync(entity.Id);
            Assert.NotNull(device);
            Assert.Equal(device.Id, entity.Id);
        }

        private void InitializeEnvironment()
        {
            var appConfig = new ConfigurationBuilder().AddJsonFile("appsettings.Tests.json").Build();
            var connectionString = appConfig.GetConnectionString("documentDBConnectionString");
            var authKey = appConfig.GetSection("AuthKey").Value;
            _client = new DocumentClient(new Uri(connectionString), authKey);
            _databaseId = appConfig.GetSection("DatabaseId").Value;
            _collectionName = typeof(Device).Name;
            _sut = new CosmosDBRepository<Device, Guid>(_client, _databaseId);
            _client.CreateDatabaseIfNotExistsAsync(_databaseId).GetAwaiter().GetResult();
            RecreateCollection<Device>().GetAwaiter().GetResult();
        }


        private static Action<Device, Device> CreateDeviceFullMap()
        {
            return (src, dst) =>
            {
                dst.Config = src.Config;
                dst.Id = src.Id;
                dst.Instructions = src.Instructions;
                dst.Type = src.Type;
            };
        }

        private async Task RecreateCollection<T>(RequestOptions options = null,
            PartitionKeyDefinition keyDefinition = null)
        {
            var opt = CreateOptions(options, keyDefinition);
            try
            {
                await _client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName));
                await _client.DeleteDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName));
                await _client.CreateDocumentCollectionAsync(opt.dbUri, opt.col, opt.opt);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentCollectionAsync(opt.dbUri, opt.col, opt.opt);
                }
                else
                {
                    throw;
                }
            }
        }

        private (Uri dbUri, DocumentCollection col, RequestOptions opt) CreateOptions(RequestOptions options = null,
            PartitionKeyDefinition keyDefinition = null)
        {
            return (UriFactory.CreateDatabaseUri(_databaseId), new DocumentCollection
            {
                Id = _collectionName,
                PartitionKey =
                    keyDefinition ??
                    new PartitionKeyDefinition {Paths = new Collection<string>(new[] {"/id"})}
            }, options ?? new RequestOptions {OfferThroughput = 1000});
        }
    }
}
