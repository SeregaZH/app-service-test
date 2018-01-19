using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;

namespace ConfigurationService.Data
{
    public sealed class CosmosDBRepository<T, TId> : IDocumentRepository<T, TId> 
        where T : class
        where TId: struct 
    {
        private const string DefaultIdentityProperty = "Id";
        private readonly IDocumentClient _client;
        private readonly string _databaseId;
        private readonly string _collectionName;
        private readonly string _identityPropertyName;

        private readonly Uri _databaseUri;
        private readonly Uri _collectionUri;

        public CosmosDBRepository(IDocumentClient  client, string databaseId, Expression<Func<string, object>> identityProperty = null, Func<string> collectionNameResolver = null)
        {
            _client = client;
            _databaseId = databaseId;
            _identityPropertyName = identityProperty == null
                ? DefaultIdentityProperty
                : GetMemberExpression(identityProperty)?.Member.Name;
            _databaseUri = UriFactory.CreateDatabaseUri(databaseId);
            _collectionName = collectionNameResolver == null ? typeof(T).Name : collectionNameResolver();
            _collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, _collectionName);
        }

        public Task<bool> RemoveAsync(RequestOptions requestOptions = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(TId id, RequestOptions requestOptions = null)
        {
            return (await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionName,
                id.ToString()))).StatusCode == HttpStatusCode.OK;
        }

        public async Task<T> AddOrUpdateAsync(T entity, RequestOptions requestOptions = null)
        {
            var upsertEntity = await _client.UpsertDocumentAsync(_collectionUri, entity, requestOptions);
            return JsonConvert.DeserializeObject<T>(upsertEntity.Resource.ToString());
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(_client.CreateDocumentQuery<T>(_collectionUri).AsEnumerable());
        }

        public async Task<T> GetByIdAsync(TId id)
        {
            return await Task.FromResult((T)(dynamic)_client.CreateDocumentQuery<Document>(_collectionUri).Where(d => d.Id == id.ToString()).AsEnumerable().FirstOrDefault());
        }

        public Task<T> FirstOrDefaultAsync(Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<T>> QueryAsync()
        {
            throw new NotImplementedException();
           
        }

        private static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> expr)
        {
            var member = expr.Body as MemberExpression;
            var unary = expr.Body as UnaryExpression;
            return member ?? unary?.Operand as MemberExpression;
        }
    }
}
