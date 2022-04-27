using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;
using Beginor.GisHub.Data.Repositories;

namespace Beginor.GisHub.Api.Controllers;

/// <summary>数据资源的基类 服务接口</summary>
[ApiController]
[Route("api/resources")]
[Authorize]
public class BaseResourceController : Controller {

    private ILogger<BaseResourceController> logger;
    private IBaseResourceRepository repository;

    public BaseResourceController(
        ILogger<BaseResourceController> logger,
        IBaseResourceRepository repository
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            logger = null;
            repository = null;
        }
        base.Dispose(disposing);
    }

    /// <summary>搜索 数据资源的基类 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResponseModel<BaseResourceModel>>> Search(
        [FromQuery]BaseResourceSearchModel model
    ) {
        try {
            var result = await repository.SearchAsync(model);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search base_resources with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }
    
    /// <summary>按类别统计资源的个数</summary>
    /// <response code="200">成功, 返回统计结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("count/category")]
    public async Task<ActionResult<PaginatedResponseModel<CategoryCountModel>>> CountByCategory([FromQuery]BaseResourceStatisticRequestModel model) {
        try {
            var result = await repository.CountByCategoryAsync(model);
            return Ok(result);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not count base_resources by category with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

}
