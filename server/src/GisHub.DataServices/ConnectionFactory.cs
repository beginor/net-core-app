using System;
using Microsoft.Extensions.DependencyInjection;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices {

    public class ConnectionFactory : Disposable, IConnectionFactory {

        private IServiceScope scope;

        public ConnectionFactory(IServiceProvider serviceProvider) {
            scope = serviceProvider.CreateScope();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                scope.Dispose();
                scope = null;
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
            var provider = scope.ServiceProvider.GetService(Type.GetType(typeName));
            return provider as IConnectionProvider;
        }
    }

}
