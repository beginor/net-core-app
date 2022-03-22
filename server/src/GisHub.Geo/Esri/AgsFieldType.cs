using System;
using System.Collections.Generic;

namespace Beginor.GisHub.Geo.Esri; 

public static class AgsFieldType {

    public readonly static Dictionary<Type, Func<string>> FieldTypeMap =
        new Dictionary<Type, Func<string>> {
            { typeof(string), () => AgsFieldType.EsriString },
            { typeof(int), () => AgsFieldType.EsriInteger },
            { typeof(short), () => AgsFieldType.EsriSmallInteger },
            { typeof(long), () => AgsFieldType.EsriInteger },
            { typeof(decimal), () => AgsFieldType.EsriDouble },
            { typeof(double), () => AgsFieldType.EsriDouble },
            { typeof(float), () => AgsFieldType.EsriSingle },
            { typeof(DateTime), () => AgsFieldType.EsriDate },
            { typeof(bool), () => AgsFieldType.EsriString }
        };

    public const string EsriString = "esriFieldTypeString";
    public const string EsriInteger = "esriFieldTypeInteger";
    public const string EsriDouble = "esriFieldTypeDouble";
    public const string EsriDate = "esriFieldTypeDate";
    public const string EsriOID = "esriFieldTypeOID";
    public const string EsriSingle = "esriFieldTypeSingle";
    public const string EsriSmallInteger = "esriFieldTypeSmallInteger";
    public const string EsriGeometry = "esriFieldTypeGeometry";

}