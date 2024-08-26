namespace Beginor.NetCoreApp.Common;

public class AppAttachmentOptions {
    public string[] Forbidden { get; set; } = [];
    public long MaxLength { get; set; } = 0;
}
