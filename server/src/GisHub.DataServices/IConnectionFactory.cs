namespace Beginor.GisHub.DataServices {

    public interface IConnectionFactory {

        IMetaDataProvider CreateMetadataProvider(string databaseType);

    }

}
