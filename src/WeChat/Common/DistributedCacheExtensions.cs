using System;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace Beginor.NetCoreApp.WeChat;

public static class DistributedCacheExtensions {

    public static async Task<Claim[]> GetUserClaimsAsync(
        this IDistributedCache cache,
        string userId
    ) {
        var buffer = await cache.GetAsync(userId);
        if (buffer == null) {
            return Array.Empty<Claim>();
        }
        await using var stream = new MemoryStream(buffer);
        var reader = new BinaryReader(stream);
        var count = reader.ReadInt32();
        var claims = new Claim[count];
        for (var i = 0; i < count; i++) {
            claims[i] = new Claim(reader);
        }
        return claims;
    }

    public static async Task SetUserClaimsAsync(
        this IDistributedCache cache,
        string userId,
        Claim[] claims,
        TimeSpan slidingExpiration
    ) {
        await using var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        writer.Write(claims.Length);
        foreach (var claim in claims) {
            claim.WriteTo(writer);
        }
        writer.Flush();
        var buffer = stream.GetBuffer();
        await cache.SetAsync(
            userId,
            buffer,
            new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration }
        );
    }

    public static async Task<T?> GetObjectAsync<T>(this IDistributedCache cache, string key) {
        var strJson = await cache.GetStringAsync(key);
        if (string.IsNullOrWhiteSpace(strJson)) {
            return default;
        }
        var res = JsonSerializer.Deserialize<T>(strJson) ?? throw new ApplicationException($"Can not deserialize cache json : {strJson} to {typeof(T)}.");
        return res;
    }
}
