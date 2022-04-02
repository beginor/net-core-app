using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices.Api;

/// <summary>数据服务 服务接口</summary>
[ApiController]
[Route(RouteTemplate)]
public partial class DataServiceController : Controller {

    private const string RouteTemplate = "api/dataservices";

    private ILogger<DataServiceController> logger;
    private IDataServiceRepository repository;
    private IDataServiceFactory factory;
    private IAppJsonDataRepository jsonRepository;
    private JsonSerializerOptionsFactory serializerOptionsFactory;
    private IFileCacheProvider fileCache;

    public DataServiceController(
        ILogger<DataServiceController> logger,
        IDataServiceRepository repository,
        IDataServiceFactory factory,
        IAppJsonDataRepository jsonRepository,
        JsonSerializerOptionsFactory serializerOptionsFactory,
        IFileCacheProvider fileCache
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        this.jsonRepository = jsonRepository ?? throw new ArgumentNullException(nameof(jsonRepository));
        this.serializerOptionsFactory = serializerOptionsFactory ?? throw new ArgumentNullException(nameof(serializerOptionsFactory));
        this.fileCache = fileCache ?? throw new ArgumentNullException(nameof(fileCache));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            logger = null;
            repository = null;
            factory = null;
            jsonRepository = null;
            serializerOptionsFactory = null;
            fileCache = null;
        }
        base.Dispose(disposing);
    }

    /// <summary>搜索 数据服务 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("data_services.read")]
    public async Task<ActionResult<PaginatedResponseModel<DataServiceModel>>> Search(
        [FromQuery]DataServiceSearchModel model
    ) {
        try {
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();
            var result = await repository.SearchAsync(model, roles);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search dataservice with {model.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary> 创建 数据服务 </summary>
    /// <response code="200">创建 数据服务 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("data_services.create")]
    public async Task<ActionResult<DataServiceModel>> Create(
        [FromBody]DataServiceModel model
    ) {
        try {
            await repository.SaveAsync(model);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to dataservice.");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>删除 数据服务 </summary>
    /// <response code="204">删除 数据服务 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("data_services.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            await repository.DeleteAsync(id);
            await jsonRepository.DeleteAsync(id);
            await fileCache.DeleteAsync(id.ToString());
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete dataservice by id {id} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>
    /// 获取指定的 数据服务
    /// </summary>
    /// <response code="200">返回 数据服务 信息</response>
    /// <response code="404"> 数据服务 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("data_services.read_by_id")]
    public async Task<ActionResult<DataServiceModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id);
            if (result == null) {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get dataservice by id {id}.");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>
    /// 更新 数据服务
    /// </summary>
    /// <response code="200">更新成功，返回 数据服务 信息</response>
    /// <response code="404"> 数据服务 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("data_services.update")]
    public async Task<ActionResult<DataServiceModel>> Update(
        [FromRoute]long id,
        [FromBody]DataServiceModel model
    ) {
        try {
            var exists = await repository.ExistAsync(id);
            if (!exists) {
                return NotFound();
            }
            await repository.UpdateAsync(id, model);
            await jsonRepository.DeleteAsync(id);
            var infoPath = Path.Combine(id.ToString(), "info.json");
            if (model.SupportMvt) {
                await fileCache.DeleteAsync(infoPath);
            }
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not update dataservice by id {id} with {model.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

}
