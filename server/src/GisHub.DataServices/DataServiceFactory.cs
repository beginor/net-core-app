using System;
using Microsoft.Extensions.DependencyInjection;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices {

    public class DataServiceFactory : Disposable, IDataServiceFactory {

        private IServiceScope scope;

        public DataServiceFactory(IServiceProvider serviceProvider) {
            scope = serviceProvider.CreateScope();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                scope.Dispose();
                scope = null;
            }
            base.Dispose(disposing);
        }

        public IMetaDataProvider CreateMetadataProvider(string databaseType) {
            string typeName;
            if (databaseType.Equals("postgis", StringComparison.OrdinalIgnoreCase)) {
                typeName = "Beginor.GisHub.DataServices.PostGIS.PostGISMetaDataProvider,Beginor.GisHub.DataServices.PostGIS";
            }
            else {
                throw new NotSupportedException(
                    $"Unsupported database type {databaseType}!"
                );
            }
            var type = Type.GetType(typeName);
            if (type == null) {
                throw new InvalidOperationException($"Can not get type {type} !");
            }
            var provider = scope.ServiceProvider.GetService(type);
            return provider as IMetaDataProvider;
        }

        public IDataServiceReader CreateDataSourceReader(string databaseType) {
            string typeName;
            if (databaseType.Equals("postgis", StringComparison.OrdinalIgnoreCase)) {
                typeName = "Beginor.GisHub.DataServices.PostGIS.PostGISDataSourceReader,Beginor.GisHub.DataServices.PostGIS";
            }
            else {
                throw new NotSupportedException(
                    $"Unsupported database type {databaseType}!"
                );
            }
            var type = Type.GetType(typeName);
            if (type == null) {
                throw new InvalidOperationException($"Can not get type {type} !");
            }
            var provider = scope.ServiceProvider.GetService(type);
            return provider as IDataServiceReader;
        }

        public IFeatureProvider CreateFeatureProvider(string databaseType) {
            string typeName;
            if (databaseType.Equals("postgis", StringComparison.OrdinalIgnoreCase)) {
                typeName = "Beginor.GisHub.DataServices.PostGIS.PostGISFeatureProvider,Beginor.GisHub.DataServices.PostGIS";
            }
            else {
                throw new NotSupportedException(
                    $"Unsupported database type {databaseType}!"
                );
            }
            var type = Type.GetType(typeName);
            if (type == null) {
                throw new InvalidOperationException($"Can not get type {type} !");
            }
            var provider = scope.ServiceProvider.GetService(type);
            return provider as IFeatureProvider;
        }

    }

}
