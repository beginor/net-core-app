using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Beginor.AppFx.Core;
using Beginor.GisHub.DynamicSql;
using Beginor.GisHub.DynamicSql.Data;
using Beginor.GisHub.DynamicSql.Models;
using NUnit.Framework;

namespace Beginor.GisHub.Test.DynamicSql;

[TestFixture]
public class ApiDocBuilderTest : BaseTest {

    [Test]
    public void _01_CanBuildMarkdownApiDoc() {
        var apis = GetAllDataApis();
        IApiDocBuilder builder = new MarkdownApiDocBuilder();
        var markdown = builder.BuildApiDoc(
            "GIS Hub API Doc Demo",
            "test description",
            "http://localhost:5000/gishub/api/dataapis",
            apis,
            "9fffa2bf2d30412c8977a42bd937e5ff",
            "http://localhost:3000/"
        );
        Assert.IsNotEmpty(markdown);
        Console.WriteLine(markdown);
    }

    [Test]
    public void _02_CanBuildJsonApiDocs() {
        var apis = GetAllDataApis();
        IApiDocBuilder builder = new JsonApiDocBuilder();
        var json = builder.BuildApiDoc(
            "GIS Hub API Doc Demo",
            "test description",
            "http://localhost:5000/gishub/api/dataapis",
            apis,
            "9fffa2bf2d30412c8977a42bd937e5ff",
            "http://localhost:3000/"
        );
        Assert.IsNotEmpty(json);
        Console.WriteLine(json);
    }


    private static IList<DataApiModel> GetAllDataApis() {
        var repo = ServiceProvider.GetService<IDataApiRepository>();
        Assert.IsNotNull(repo);
        var apis = repo.GetAll();
        return apis;
    }

}
