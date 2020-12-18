namespace Beginor.GisHub.DataServices {

    public interface IConnectionStringReader {

        string[] GetSchemasAsync(long id);
    }

}
