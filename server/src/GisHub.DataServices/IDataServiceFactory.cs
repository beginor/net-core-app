namespace Beginor.GisHub.DataServices {

    public interface IDataServiceFactory {

        IMetaDataProvider CreateMetadataProvider(string databaseType);

        IDataSourceReader CreateDataSourceReader(string databaseType);

    }

}
