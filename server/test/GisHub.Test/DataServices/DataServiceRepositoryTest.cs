using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;
using NHibernate.Linq;

namespace Beginor.GisHub.Test.DataServices; 

/// <summary>数据源（数据表或视图）仓储测试</summary>
[TestFixture]
public class DataServiceRepositoryTest : BaseTest<IDataServiceRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new DataServiceSearchModel {
            Skip = 0,
            Take = 10
        };
        var result = await Target.SearchAsync(searchModel);
        Assert.GreaterOrEqual(result.Total, 0);
        Assert.GreaterOrEqual(result.Take, result.Data.Count);
    }

    [Test]
    public void _03_CanMapDataSource() {
        var entity = new DataService {
            Id = 1,
            DataSource = new DataSource {
                Id = 2,
                Name = "Test"
            },
            Fields = new [] {
                new DataServiceField {
                    Name = "id",
                    Type = "int",
                    Nullable = false,
                    Editable = false,
                }
            }
        };
        var mapper = ServiceProvider.GetService<AutoMapper.IMapper>();
        var model = mapper.Map<DataServiceModel>(entity);
        Assert.AreEqual(entity.DataSource.Id.ToString(), model.DataSource.Id);
        var entity2 = mapper.Map<Beginor.GisHub.DataServices.Data.DataService>(model);
        Assert.IsNotNull(entity2.DataSource);
        Assert.AreEqual(entity2.DataSource.Id.ToString(), model.DataSource.Id);
        Assert.IsNotEmpty(model.Fields);
    }

    [Test]
    public async Task _03_CanGetCacheItem() {
        var id = 1607411721075030142;
        var cacheItem = await Target.GetCacheItemByIdAsync(id);
        Assert.IsNotNull(cacheItem);
        Console.WriteLine(cacheItem.ToJson());
    }
    
    [Test]
    public async Task _04_CanQueryDataService() {
        var session = ServiceProvider.GetService<NHibernate.ISession>();
        var query = session.Query<DataService>();
        var data = await query.Select(x => new DataService {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Schema = x.Schema,
            TableName = x.TableName,
            PrimaryKeyColumn = x.PrimaryKeyColumn,
            DisplayColumn = x.DisplayColumn,
            GeometryColumn = x.GeometryColumn,
            SupportMvt = x.SupportMvt,
            // MvtMinZoom = x.MvtMinZoom,
            // MvtMaxZoom = x.MvtMaxZoom,
            // MvtCacheDuration = x.MvtCacheDuration
        }).ToListAsync();
        Assert.IsNotEmpty(data);
        Console.WriteLine(data.Count);
    }

}
