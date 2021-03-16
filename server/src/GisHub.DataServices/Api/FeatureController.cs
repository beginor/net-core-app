using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices.Api {

    /// <summary>要素服务接口</summary>
    [ApiController]
    [Route("rest/services/features")]
    public class FeatureController : Controller {

        private ILogger<DataSourceController> logger;
        private IDataSourceRepository repository;
        private IDataServiceFactory factory;

        public FeatureController(
            ILogger<DataSourceController> logger,
            IDataSourceRepository repository,
            IDataServiceFactory factory
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                repository = null;
                factory = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>读取要素服务信息</summary>
        [HttpGet("{id:long}/MapServer/0")]
        public async Task<ActionResult<AgsLayerDescription>> GetLayerDescription(
            [FromRoute]long id,
            [FromQuery]AgsCommonParams param
        ) {
            try {
                var dataSource = await repository.GetCacheItemByIdAsync(id);
                if (dataSource == null) {
                    return NotFound($"Datasource {id} does not exist !");
                }
                var featureProvider = factory.CreateFeatureProvider(dataSource.DatabaseType);
                var layerDesc = await featureProvider.GetLayerDescriptionAsync(dataSource);
                return Json(layerDesc, JsonFactory.CreateAgsJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get layer description from datasource {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>查询空间要素</summary>
        [HttpGet("{id:long}/MapServer/0/query")]
        public async Task<ActionResult<AgsFeatureSet>> QueryFeaturesByGetAsync(
            [FromRoute]long id,
            [FromQuery]AgsQueryParam param
        ) {
            try {
                var featureSet = await QueryFeatures(id, param);
                if (featureSet == null) {
                    return NotFound($"Datasource {id} does not exist !");
                }
                return Json(featureSet, JsonFactory.CreateAgsJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not query features from datasource {id} with params {param.ToJson(JsonFactory.CreateJsonSerializerOptions())}");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>查询空间要素</summary>
        [HttpPost("{id:long}/MapServer/0/query")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<AgsFeatureSet>> QueryFeaturesByPostAsync(
            [FromRoute]long id,
            [ModelBinder(BinderType = typeof(AgsQueryParamsBinder))]AgsQueryParam param
        ) {
            try {
                var featureSet = await QueryFeatures(id, param);
                if (featureSet == null) {
                    return NotFound($"Datasource {id} does not exist !");
                }
                return Json(featureSet, JsonFactory.CreateAgsJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not query features from datasource {id} with params {param.ToJson(JsonFactory.CreateJsonSerializerOptions())}");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        private async Task<AgsFeatureSet> QueryFeatures(
            long id,
            AgsQueryParam param
        ) {
            var dataSource = await repository.GetCacheItemByIdAsync(id);
            if (dataSource == null) {
                return null;
            }
            var reader = factory.CreateFeatureProvider(dataSource.DatabaseType);
            var featureSet = await reader.QueryAsync(dataSource, param);
            return featureSet;
        }
    }

}
