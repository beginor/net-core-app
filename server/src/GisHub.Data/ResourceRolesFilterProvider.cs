using System;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Microsoft.Extensions.Caching.Distributed;
using Beginor.GisHub.Common;
using Beginor.GisHub.Data.Repositories;

namespace Beginor.GisHub.Data;

public class ResourceRolesFilterProvider : Disposable, IRolesFilterProvider {

    private IDistributedCache cache;
    private IBaseResourceRepository repository;
    private CommonOption commonOption;

    private static readonly string cacheKeyFormat = "{0}_roles";

    public ResourceRolesFilterProvider(
        IBaseResourceRepository repository,
        IDistributedCache cache,
        CommonOption commonOption
    ) {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.commonOption = commonOption ?? throw new ArgumentNullException(nameof(commonOption));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // dispose managed resource here;
        }
        base.Dispose(disposing);
    }

    public async Task<string[]> GetRolesAsync(object id) {
        var cacheKey = GetCacheKey(id);
        var roles = await cache.GetAsync<string[]>(cacheKey);
        if (roles != null) {
            return roles;
        }
        roles = await repository.GetRolesByResourceIdAsync((long)id);
        await cache.SetAsync(cacheKey, roles, commonOption.Cache.MemoryExpiration);
        return roles;
    }

    public async Task ResetRolesAsync(object id) {
        var cacheKey = GetCacheKey(id);
        await cache.RemoveAsync(cacheKey);
    }

    private static string GetCacheKey(object id) {
        var resourceId = (long)id;
        var cacheKey = string.Format(cacheKeyFormat, resourceId);
        return cacheKey;
    }

}
