namespace Beginor.GisHub.DataServices {

    public interface IConnectionFactory {

        IConnectionProvider CreateProvider(string databaseType);

    }

}
