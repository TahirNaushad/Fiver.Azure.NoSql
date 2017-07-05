using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fiver.Azure.NoSql
{
    public sealed class AzureNoSqlRepository<T> : IAzureNoSqlRepository<T>
    {
        #region " Public "

        public AzureNoSqlRepository(AzureNoSqlSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException("Settings");
            Init();
        }

        public async Task<List<T>> GetList()
        {
            var query = this.client
                            .CreateDocumentQuery<T>(GetCollectionUri())
                            .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<List<T>> GetList(Expression<Func<T, bool>> predicate)
        {
            var query = this.client
                            .CreateDocumentQuery<T>(GetCollectionUri())
                            .Where(predicate)
                            .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<List<T>> GetList(string sql)
        {
            var query = this.client
                            .CreateDocumentQuery<T>(GetCollectionUri(), sql)
                            .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<T> GetItem(string id)
        {
            Document document = await this.client.ReadDocumentAsync(GetDocumentUri(id));
            return (T)(dynamic)document;
        }

        public async Task<Document> Insert(T item)
        {
            return await this.client.CreateDocumentAsync(GetCollectionUri(), item);
        }

        public async Task<Document> Update(string id, T item)
        {
            return await this.client.ReplaceDocumentAsync(GetDocumentUri(id), item);
        }

        public async Task<Document> InsertOrUpdate(T item)
        {
            return await this.client.UpsertDocumentAsync(GetCollectionUri(), item);
        }

        public async Task Delete(string id)
        {
            await this.client.DeleteDocumentAsync(GetDocumentUri(id));
        }

        #endregion

        #region " Private "

        private AzureNoSqlSettings settings;
        private DocumentClient client;

        private void Init()
        {
            try
            {
                client = new DocumentClient(new Uri(this.settings.Endpoint), this.settings.AuthKey);

                CreateDatabaseIfNotExistsAsync().Wait();
                CreateCollectionIfNotExistsAsync().Wait();
            }
            catch (Exception ex)
            {
                throw new Exception("Init failed for AzureNoSqlRepository");
            }
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(this.settings.DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = this.settings.DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(this.settings.DatabaseId, this.settings.CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(this.settings.DatabaseId),
                        new DocumentCollection { Id = this.settings.CollectionId },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }

        private Uri GetCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(
                        this.settings.DatabaseId, this.settings.CollectionId);
        }

        private Uri GetDocumentUri(string documentId)
        {
            return UriFactory.CreateDocumentUri(
                        this.settings.DatabaseId, this.settings.CollectionId, documentId);
        }

        #endregion
    }
}
