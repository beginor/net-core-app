using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace Beginor.GisHub.Gmap {

    public class IPAddressConverter : TypeConverter {

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) {
            return destinationType == typeof(string);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) {
            return IPAddress.TryParse(value.ToString(), out var ipAddress) ? ipAddress : null;
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) {
            return value?.ToString();
        }

    }

}
