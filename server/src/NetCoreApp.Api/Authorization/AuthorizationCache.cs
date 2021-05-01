using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Beginor.NetCoreApp.Api.Authorization {

    // public interface IAuthorizationCache {
    //     Task<Claim[]> GetUserClaimsAsync(string userId);
    //     Task SetUserClaimsAsync(string userId, Claim[] claims);
    // }

    // public class AuthorizationCache : IAuthorizationCache {

    //     public AuthorizationCache(IDistributedCache cache) {

    //     }

    //     private IDictionary<string, Claim[]> userRoles = new Dictionary<string, Claim[]>();

    //     public Task<Claim[]> GetUserClaimsAsync(string userId) {
    //         IDistributedCache cache = new MemoryDistributedCache(null);
    //         if (userRoles.ContainsKey(userId)) {
    //             return Task.FromResult(userRoles[userId]);
    //         }
    //         return Task.FromResult(new Claim[0]);
    //     }

    //     public Task SetUserClaimsAsync(string userId, Claim[] roles) {
    //         userRoles[userId] = roles;
    //         return Task.CompletedTask;
    //     }

    // }

}
