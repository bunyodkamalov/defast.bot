using System.Linq.Expressions;
using System.Security.Authentication;
using System.Text;
using Defast.Bot.Domain.Common.Caching;
using Defast.Bot.Domain.Common.Entities;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Persistence.Caching.Brokers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Defast.Bot.Persistence.Repositories;

public abstract class EntityRepositoryBase<TEntity, TContext>(
    TContext dbContext,
    ICacheBroker cacheBroker,
    CacheEntryOptions? cacheEntryOptions = default)
    where TEntity : class, IEntity
    where TContext : DbContext
{
    protected TContext DbContext => dbContext;


    protected async ValueTask<TEntity?> GetEntityFromUrl(string url, string sessionId, CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        using var client = new HttpClient(clientHandler);
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=99999");
        client.DefaultRequestHeaders.Add("Cookie",
            $"B1SESSION={sessionId ?? throw new ArgumentNullException(nameof(sessionId))}; ROUTEID=.node1");

        HttpResponseMessage response = await client.GetAsync(url, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Response<TEntity> salesPersons = JsonConvert.DeserializeObject<Response<TEntity>>(responseContent)!;

            return salesPersons.Value!.FirstOrDefault();
        }
        else
            throw new InvalidCredentialException("Please, login first!");
    }

    protected async ValueTask<List<TEntity?>> GetEntitiesFromUrl(string url, string sessionId,CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        using var client = new HttpClient(clientHandler);
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=99999");
        client.DefaultRequestHeaders.Add("Cookie",
            $"B1SESSION={sessionId ?? throw new ArgumentNullException(nameof(sessionId))}; ROUTEID=.node1");
        client.DefaultRequestHeaders.Add("B1S-CaseInsensitive", "true");
            
        HttpResponseMessage response = await client.GetAsync(url, cancellationToken);


        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var salesPersons = JsonConvert.DeserializeObject<Response<TEntity>>(responseContent);

            return salesPersons!.Value;
        }
        else
            throw new InvalidCredentialException("BadRequest");
    }

    protected async ValueTask<List<TEntity?>> GetEntitiesFromUrlWithPagination(string url, string sessionId, CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        using var client = new HttpClient(clientHandler);
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=10");
        client.DefaultRequestHeaders.Add("Cookie",
            $"B1SESSION={sessionId ?? throw new ArgumentNullException(nameof(sessionId))}; ROUTEID=.node1");

        HttpResponseMessage response = await client.GetAsync(url, cancellationToken);


        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var salesPersons = JsonConvert.DeserializeObject<Response<TEntity>>(responseContent);

            return salesPersons!.Value;
        }
        else
            throw new InvalidCredentialException("BadRequest");
    }
    
    protected async ValueTask<List<TEntity?>> GetQueryResultWithPagination(string url, CancellationToken cancellationToken)
    {
        var clientHandler = new HttpClientHandler
        {
            UseCookies = false,
        };
        var client = new HttpClient(clientHandler);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            Headers =
            {
                { "cookie", "xsSecureId6C049AA384B2DCB82A71BD297B2BC100=5FF5976FCEF7F94894FD5158B50B2357; sapxslb=8E6AEE87C9A12E499045115995044A5D" },
                { "User-Agent", "insomnia/9.2.0" },
                { "Authorization", "Basic bGxjX3Jlc19zdTI2X2FkbTpLaXcxYkVXMFAzNTQ=" },
                { "Prefer", "odata.maxpagesize=20"}
            },
        };
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonConvert.DeserializeObject<Response<TEntity>>(responseContent);

        return result!.Value!;
    }
    
    protected async ValueTask<List<TEntity?>> GetQueryResultWithNoPagination(string url, CancellationToken cancellationToken)
    {
        var clientHandler = new HttpClientHandler
        {
            UseCookies = false,
        };
        var client = new HttpClient(clientHandler);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            Headers =
            {
                { "cookie", "xsSecureId6C049AA384B2DCB82A71BD297B2BC100=5FF5976FCEF7F94894FD5158B50B2357; sapxslb=8E6AEE87C9A12E499045115995044A5D" },
                { "User-Agent", "insomnia/9.2.0" },
                { "Authorization", "Basic bGxjX3Jlc19zdTI2X2FkbTpLaXcxYkVXMFAzNTQ=" },
                { "Prefer", "odata.maxpagesize=99999"}
            },
        };
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonConvert.DeserializeObject<Response<TEntity>>(responseContent);
        
        return result!.Value!;
    }
    
    protected async ValueTask<TEntity?> PostToSap(TEntity payload, string url, string sessionId, CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        var payloadJson = JsonConvert.SerializeObject(payload);

        using var client = new HttpClient(clientHandler);
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=99999");
        client.DefaultRequestHeaders.Add("Cookie",
            $"B1SESSION={sessionId ?? throw new ArgumentNullException(nameof(sessionId))}; ROUTEID=.node1");

        var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(url, content, cancellationToken);
            
        if (response.IsSuccessStatusCode)
            return payload;
        else
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception(responseContent);
        }
            
    }

    protected async ValueTask<TEntity?> UpdateFromSap(TEntity payload, string url, string sessionId, CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
        {
            return true;
        };

        var payloadJson = JsonConvert.SerializeObject(payload);

        using var client = new HttpClient(clientHandler);
        client.DefaultRequestHeaders.Add("Cookie",
            $"B1SESSION={sessionId ?? throw new ArgumentNullException(nameof(sessionId))}; ROUTEID=.node2");

        client.DefaultRequestHeaders.Add("B1S-ReplaceCollectionsOnPatch", "true");
            
        var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PatchAsync(url, content, cancellationToken);
            
        if (response.IsSuccessStatusCode)
            return payload;
        else
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        }
            
        return null;
    }
    
    
    protected async ValueTask<bool> PostToSapForAction(string url, string sessionId, CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
        {
            return true;
        };
        
        using (var client = new HttpClient(clientHandler))
        {
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=99999");
            client.DefaultRequestHeaders.Add("Cookie",
                $"B1SESSION={sessionId ?? throw new ArgumentNullException(nameof(sessionId))}; ROUTEID=.node1");

            HttpResponseMessage response =
                await client.PostAsync(url, new StringContent("", Encoding.UTF8, "application/json"), cancellationToken);
            
            if (response.IsSuccessStatusCode)
                return true;
            
            return false;
        }
    }
    
    protected IQueryable<TEntity> Get(Expression<Func<TEntity, bool>>? predicate = default, bool asNoTracking = false)
    {
        var initialQuery = DbContext.Set<TEntity>().Where(entity => true);

        if (predicate is not null)
            initialQuery = initialQuery.Where(predicate);

        if (asNoTracking)
            initialQuery = initialQuery.AsNoTracking();

        return initialQuery;
    }
    
    protected async ValueTask<TEntity?> GetByIdAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var foundEntity = default(TEntity?);

        if (cacheEntryOptions is null ||
            !await cacheBroker.TryGetAsync<TEntity>(id.ToString(), out var cachedEntity, cancellationToken))
        {
            var initialQuery = DbContext.Set<TEntity>().AsQueryable();

            if (asNoTracking)
                initialQuery = initialQuery.AsNoTracking();

            foundEntity = await initialQuery.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

            if (cacheEntryOptions is not null && foundEntity is not null)
                await cacheBroker.SetAsync(foundEntity.Id.ToString(), foundEntity, cacheEntryOptions,
                    cancellationToken);
        }
        else
            foundEntity = cachedEntity;

        return foundEntity;
    }

    protected async ValueTask<IList<TEntity>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var initialQuery = DbContext.Set<TEntity>().Where(entity => ids.Contains(entity.Id));

        if (asNoTracking)
            initialQuery = initialQuery.AsNoTracking();

        return await initialQuery.ToListAsync(cancellationToken: cancellationToken);
    }

    protected async ValueTask<TEntity?> CreateAsync(
        TEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default
    )
    {
        await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);

        if (cacheEntryOptions is not null)
            await cacheBroker.SetAsync(entity.Id.ToString(), entity, cacheEntryOptions, cancellationToken);
        
        if (saveChanges)
            await DbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    protected async ValueTask<TEntity> UpdateAsync(
        TEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default
    )
    {
        DbContext.Set<TEntity>().Update(entity);

        if (cacheEntryOptions is not null)
            await cacheBroker.SetAsync(entity.Id.ToString(), entity, cacheEntryOptions, cancellationToken);

        if (saveChanges)
            await DbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }


    protected async ValueTask<TEntity?> DeleteAsync(
        TEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default
    )
    {
        DbContext.Set<TEntity>().Remove(entity);

        if (cacheEntryOptions is not null)
            await cacheBroker.DeleteAsync(entity.Id.ToString(), cancellationToken);

        if (saveChanges)
            await DbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }


    protected async ValueTask<TEntity?> DeleteByIdAsync(
        Guid id,
        bool saveChanges = true,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await DbContext.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken) ??
                     throw new InvalidOperationException();

        DbContext.Set<TEntity>().Remove(entity);

        if (cacheEntryOptions is not null)
            await cacheBroker.DeleteAsync(entity.Id.ToString(), cancellationToken);

        if (saveChanges)
            await DbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    private string AddTypePrefix(CacheModel model)
    {
        return $"{typeof(TEntity).Name}_{model.CacheKey}";
    }
}