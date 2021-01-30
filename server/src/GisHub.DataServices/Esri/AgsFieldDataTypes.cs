using System;
using System.Collections.Generic;

namespace Beginor.GisHub.DataServices.Esri {

    public static class AgsFieldDataTypes {

        public readonly static Dictionary<Type, Func<string>> FieldDataTypeMap =
            new Dictionary<Type, Func<string>> {
                {typeof(string), () => AgsFieldDataTypes.EsriString},
                {typeof(int), () => AgsFieldDataTypes.EsriInteger},
                {typeof(short), () => AgsFieldDataTypes.EsriSmallInteger},
                {typeof(long), () => AgsFieldDataTypes.EsriInteger},
                {typeof(decimal), () => AgsFieldDataTypes.EsriDouble},
                {typeof(double), () => AgsFieldDataTypes.EsriDouble},
                {typeof(float), () => AgsFieldDataTypes.EsriSingle},
                {typeof(DateTime), () => AgsFieldDataTypes.EsriDate},
                {typeof(bool), () => AgsFieldDataTypes.EsriString}
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

}
