using System.Data.Common;

namespace Beginor.GisHub.DataServices;

public interface IDataServiceFactory {

    IMetaDataProvider? CreateMetadataProvider(string databaseType);

    IDataServiceReader? CreateDataSourceReader(string databaseType);

    IFeatureProvider? CreateFeatureProvider(string databaseType);

}
