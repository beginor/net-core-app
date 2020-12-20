using System.Threading.Tasks;
using Npgsql;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices.PostGIS {

    public class PostGISConnectionProvider : Disposable, IConnectionProvider {

        private IConnectionRepository repo;

        public PostGISConnectionProvider(IConnectionRepository repo) {
            this.repo = repo;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                repo = null;
            }
            base.Dispose(disposing);
        }

        public async Task<string> BuildConnectionStringAsync(long id) {
            var conn = await repo.GetByIdAsync(id);
            if (conn == null) {
                return string.Empty;
            }
            var builder = new NpgsqlConnectionStringBuilder {
                ApplicationName = "GisHub",
                Host = conn.ServerAddress,
                Port = conn.ServerPort,
                Database = conn.DatabaseName,
                Username = conn.Username,
                Password = conn.Password,
                CommandTimeout = conn.Timeout
            };
            return builder.ConnectionString;
        }

        public async Task<string[]> GetSchemasAsync(long id) {
            throw new System.Exception();
        }
    }

}
