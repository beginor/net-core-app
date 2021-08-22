using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Beginor.GisHub.Geo.Esri {

    public class AgsQueryParamsBinder : IModelBinder {
        public Task BindModelAsync(ModelBindingContext bindingContext) {
            var http = bindingContext.HttpContext;
            if (bindingContext.HttpContext.Request.Method.ToLowerInvariant() == "get") {
                var model = BindQuery(http.Request.Query);
                bindingContext.Result = ModelBindingResult.Success(model);
                return Task.CompletedTask;
            }
            if (bindingContext.HttpContext.Request.Method.ToLowerInvariant() == "post") {
                var model = BindQuery(http.Request.Form);
                bindingContext.Result = ModelBindingResult.Success(model);
                return Task.CompletedTask;
            }
            return Task.FromException(new Exception($"Unsupported request method {bindingContext.HttpContext.Request.Method} !"));
        }
        private static AgsQueryParam BindQuery(IEnumerable<KeyValuePair<string, StringValues>> paires) {
            var type = typeof(AgsQueryParam);
            var model = new AgsQueryParam();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<FromQueryAttribute>() != null);
            foreach (var prop in props) {
                var attr = prop.GetCustomAttribute<FromQueryAttribute>();
                if (paires.Any(a => a.Key == attr.Name)) {
                    var pair = paires.First(a => a.Key.Equals(attr.Name, StringComparison.OrdinalIgnoreCase));
                    var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                    if (converter != null && converter.CanConvertFrom(typeof(string))) {
                        prop.SetValue(model, converter.ConvertFromString(pair.Value.ToString()));
                    }
                }
            }
            return model;
        }
    }
}
