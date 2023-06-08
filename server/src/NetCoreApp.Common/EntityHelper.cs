using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NHibernate.Mapping.Attributes;

namespace Beginor.NetCoreApp.Common;

public static class EntityHelper {

    public static EntityMapping GetEntityMapping(Type entityType) {
        var mapping = new EntityMapping();
        var attr = entityType.GetCustomAttribute<ClassAttribute>();
        if (attr == null) {
            throw new InvalidOperationException($"Type {entityType} does not declared with [Class] attribute!");
        }
        mapping.Schema = attr.Schema;
        mapping.Table = attr.Table;

        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        mapping.Properties = properties
            .Where(prop => prop.GetCustomAttribute<PropertyAttribute>() != null)
            .Select(prop => prop.GetCustomAttribute<PropertyAttribute>())
            .Select(att => new PropertyMapping { Name = att!.Name, Column = att.Column })
            .ToList();

        mapping.Id = properties.Where(prop => prop.GetCustomAttribute<IdAttribute>() != null)
            .Select(prop => prop.GetCustomAttribute<IdAttribute>())
            .Select(att => new PropertyMapping { Name = att!.Name, Column = att.Column })
            .First();

        return mapping;
    }

    public static string GenerateInsertSql(Type entityType) {
        var mapping = GetEntityMapping(entityType);
        var sql = new StringBuilder();
        sql.Append("insert into ");
        if (!string.IsNullOrEmpty(mapping.Schema)) {
            sql.Append($"{mapping.Schema}.");
        }
        sql.Append($"{mapping.Table}");
        sql.AppendLine("(");
        var columns = string.Join(", ", mapping.Properties.Select(p => p.Column));
        sql.AppendLine($"  {columns}");
        sql.AppendLine(")");
        sql.AppendLine("values");
        sql.AppendLine("(");
        var parameters = string.Join(", ", mapping.Properties.Select(p => $"@{p.Name}"));
        sql.AppendLine($"  {parameters}");
        sql.AppendLine(")");
        return sql.ToString();
    }

}

public class EntityMapping {
    public string Schema { get; set; } = string.Empty;
    public string Table { get; set; } = string.Empty;
    public PropertyMapping Id { get; set; } = new ();
    public List<PropertyMapping> Properties { get; set; } = new ();
}

public class PropertyMapping {
    public string Name { get; set; } = string.Empty;
    public string Column { get; set; } = string.Empty;
}
