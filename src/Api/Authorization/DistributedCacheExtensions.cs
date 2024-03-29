using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Beginor.NetCoreApp.Api.Authorization;

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
}
