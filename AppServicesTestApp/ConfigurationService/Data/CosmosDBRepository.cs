using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using ConfigurationService.Exceptions;
using ConfigurationService.Models;
using ConfigurationService.Services.Exceptions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ConfigurationService.Data
{
    public sealed class CosmosDBRepository<T, TId> : IDocumentRepository<T, TId> 
        where T : DocumentEntity
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

        public async Task<T> RemoveAsync(TId id, string ETag, RequestOptions requestOptions = null)
        {
            Document result = null;
            var ac = new AccessCondition {Condition = ETag, Type = AccessConditionType.IfMatch};

            try
            {
                result = await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionName,
                    id.ToString()), new RequestOptions {AccessCondition = ac});
            }
            catch (DocumentClientException e)
            {
                switch (e.StatusCode)
                {
                    case HttpStatusCode.PreconditionFailed:
                        throw new ConflictException();
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException();
                    default: throw;
                }
            }


            return (T) (dynamic) result;
        }

        public async Task<T> CreateAsync(T entity, RequestOptions requestOptions = null)
        {
            Document upsertEntity = await _client.UpsertDocumentAsync(_collectionUri, entity, requestOptions);
            return (T)(dynamic)upsertEntity;
        }

        public async Task<T> UpdateAsync(TId id, T entity, RequestOptions requestOptions = null)
        {
            var ac = new AccessCondition { Condition = entity.ETag, Type = AccessConditionType.IfMatch };

            Document updated;
            try
            {
                updated = await _client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(_databaseId, _collectionName, id.ToString()), entity, new RequestOptions { AccessCondition = ac });
            }
            catch (DocumentClientException e)
            {
                switch (e.StatusCode)
                {
                    case HttpStatusCode.PreconditionFailed:
                        throw new ConflictException($"Conflict old etag:{entity.ETag}");
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException();
                    default: throw;
                }
            }

            return (T) (dynamic) updated;
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
            var res = (T) (dynamic) _client.CreateDocumentQuery<Document>(_collectionUri)
                .Where(d => d.Id == id.ToString()).AsEnumerable().FirstOrDefault();
            return await Task.FromResult(res);
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
