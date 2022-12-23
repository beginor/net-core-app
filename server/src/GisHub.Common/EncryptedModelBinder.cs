using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace Beginor.GisHub.Common;

public class EncryptedModelBinder : IModelBinder {

    private static readonly string EncryptedParamName = "$encrypted";

    public Task BindModelAsync(ModelBindingContext bindingContext) {
        var request = bindingContext.HttpContext.Request;
        object? model = null;
        if (request.Method == HttpMethods.Get) {
            model = BindModelFromQuery(request.Query, bindingContext.ModelType);
        }
        if (request.Method == HttpMethods.Post) {
            model = BindModelFromForm(request.Form, bindingContext.ModelType);
        }
        if (model != null) {
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
        bindingContext.Result = ModelBindingResult.Failed();
        return Task.CompletedTask;
    }

    private static bool IsTrueValue(string? value) {
        if (value.IsNullOrEmpty()) {
            return false;
        }
        if (bool.TryParse(value, out var result)) {
            return result;
        }
        return false;
    }

    private static object? BindModelFromQuery(IQueryCollection query, Type modelType) {
        var encrypted = IsTrueValue(query[EncryptedParamName]);
        return BindModel(query, modelType, encrypted);
    }

    private static object? BindModelFromForm(IFormCollection form, Type modelType) {
        var encrypted = IsTrueValue(form[EncryptedParamName]);
        return BindModel(form, modelType, encrypted);
    }

    private static object? BindModel(IEnumerable<KeyValuePair<string, StringValues>> paires, Type modelType, bool encrypted) {
        var model = Activator.CreateInstance(modelType);
        var props = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<FromQueryAttribute>() != null);
        foreach (var prop in props) {
            var attr = prop.GetCustomAttribute<FromQueryAttribute>()!;
            if (paires.Any(a => a.Key == attr.Name)) {
                var pair = paires.First(a => a.Key.Equals(attr.Name, StringComparison.OrdinalIgnoreCase));
                var val = pair.Value.ToString();
                if (encrypted) {
                    val = Base64UrlEncoder.Decode(val);
                }
                var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                if (converter != null && converter.CanConvertFrom(typeof(string))) {
                    prop.SetValue(model, converter.ConvertFromString(val));
                }
            }
        }
        return model;
    }

}
