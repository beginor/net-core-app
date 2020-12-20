using System;
using Microsoft.Extensions.DependencyInjection;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices {

    public class ConnectionFactory : Disposable, IConnectionFactory {

        private IServiceProvider serviceProvider;

        public ConnectionFactory(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                serviceProvider = null;
            }
            base.Dispose(disposing);
        }

        public IConnectionProvider CreateProvider(string databaseType) {
            string typeName;
            if (databaseType.Equals("postgis", StringComparison.OrdinalIgnoreCase)) {
                typeName = "Beginor.GisHub.DataServices.PostGIS.PostGISConnectionProvider,Beginor.GisHub.DataServices.PostGIS";
            }
            else {
                throw new NotSupportedException(
                    $"Unsupported database type {databaseType}!"
                );
            }
            var provider = this.serviceProvider.GetService(Type.GetType(typeName));
            return provider as IConnectionProvider;
        }
    }

}
