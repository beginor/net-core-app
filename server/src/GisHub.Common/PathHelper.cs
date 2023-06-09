using System;
using System.IO;

namespace Beginor.GisHub.Common;

public static class PathHelper {

    public static string TrimStartDirectorySeparatorChar(this string path) {
        if (path == null) {
            throw new ArgumentNullException(nameof(path));
        }
        return path.TrimStart(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar
        );
    }

}
