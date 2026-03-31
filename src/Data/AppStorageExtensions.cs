using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data;

public static class AppStorageExtensions {

    public static AppStorageEntity Clone(this AppStorageEntity storage) {
        return new AppStorageEntity {
            Id = storage.Id,
            AliasName = storage.AliasName,
            RootFolder = storage.RootFolder,
            Readonly = storage.Readonly,
            Roles = storage.Roles,
        };
    }
}
