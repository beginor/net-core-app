using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class RefererFilteringMiddleware {

    }

    public class RefererFilteringOptions {

        public IList<string> Origions { get; set; }

    }

}
