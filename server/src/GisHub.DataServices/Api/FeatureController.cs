using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Filters;

namespace Beginor.GisHub.DataServices.Api {

    /// <summary>要素服务接口</summary>
    [ApiController]
    [Route("rest/services/features")]
    public class FeatureController : Controller {

        private ILogger<DataSourceController> logger;
        private IDataSourceRepository repository;
        private IDataServiceFactory factory;
        private IAppJsonDataRepository jsonRepository;

        public FeatureController(
            ILogger<DataSourceController> logger,
            IDataSourceRepository repository,
            IDataServiceFactory factory,
            IAppJsonDataRepository jsonRepository
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.jsonRepository = jsonRepository ?? throw new ArgumentNullException(nameof(jsonRepository));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                repository = null;
                factory = null;
                jsonRepository = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>读取要素服务信息</summary>
        [HttpGet("{id:long}/MapServer/0")]
        [Authorize("features.get_layer_info")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult<AgsLayerDescription>> GetLayerDescriptionAsync(
            [FromRoute]long id,
            [FromQuery]AgsCommonParams param
        ) {
            try {
                var dataSource = await repository.GetCacheItemByIdAsync(id);
                if (dataSource == null) {
                    return NotFound($"Datasource {id} does not exist !");
                }
                var serializerOptions = JsonFactory.CreateAgsJsonSerializerOptions();
                var jsonElement = await jsonRepository.GetValueByIdAsync(id);
                if (jsonElement.ValueKind != JsonValueKind.Undefined) {
                    return Json(jsonElement, serializerOptions);
                }
                var featureProvider = factory.CreateFeatureProvider(dataSource.DatabaseType);
                var layerDesc = await featureProvider.GetLayerDescriptionAsync(dataSource);
                var json = layerDesc.ToJson(serializerOptions);
                jsonElement = JsonDocument.Parse(json).RootElement;
                await jsonRepository.SaveValueAsync(id, jsonElement);
                return Content(json, "application/json", Encoding.UTF8);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get layer description from datasource {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>查询空间要素</summary>
        [HttpGet("{id:long}/MapServer/0/query")]
        [Authorize("features.query")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult<AgsFeatureSet>> QueryFeaturesByGetAsync(
            [FromRoute]long id,
            [FromQuery]AgsQueryParam param
        ) {
            try {
                var featureSet = await QueryFeaturesAsync(id, param);
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
        [Authorize("features.query")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult<AgsFeatureSet>> QueryFeaturesByPostAsync(
            [FromRoute]long id,
            [ModelBinder(BinderType = typeof(AgsQueryParamsBinder))]AgsQueryParam param
        ) {
            try {
                var featureSet = await QueryFeaturesAsync(id, param);
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

        private async Task<AgsFeatureSet> QueryFeaturesAsync(
            long id,
            AgsQueryParam param
        ) {
            var dataSource = await repository.GetCacheItemByIdAsync(id);
            if (dataSource == null) {
                return null;
            }
            if (param.OutFields.Trim() == "*") {
                param.OutFields = string.Join(",", dataSource.Fields.Select(f => f.Name));
            }
            var featureProvider = factory.CreateFeatureProvider(dataSource.DatabaseType);
            var featureSet = await featureProvider.QueryAsync(dataSource, param);
            return featureSet;
        }
    }

}
