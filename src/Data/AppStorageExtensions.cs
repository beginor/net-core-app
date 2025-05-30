using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data;

public static class AppStorageExtensions {

    public static AppStorage Clone(this AppStorage storage) {
        return new AppStorage {
            Id = storage.Id,
            AliasName = storage.AliasName,
            RootFolder = storage.RootFolder,
            Readonly = storage.Readonly,
            Roles = storage.Roles,
        };
    }
}
