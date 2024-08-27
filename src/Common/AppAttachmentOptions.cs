namespace Beginor.NetCoreApp.Common;

public class AppAttachmentOptions {
    public string[] Forbidden { get; set; } = [];
    public long MaxSize { get; set; } = 0;
    public long MaxBlockSize { get; set; } = 0;
}
