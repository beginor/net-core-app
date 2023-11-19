using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data;

public static class AppUserExtensions {

    public static AppUserModel ToUserModel(
        this AppUser user,
        IMapper mapper,
        IList<Claim> claims
    ) {
        var model = mapper.Map<AppUserModel>(user);
        model.Surname = claims.FirstOrDefault(
            c => c.Type == ClaimTypes.Surname
        )?.Value;
        model.GivenName = claims.FirstOrDefault(
            c => c.Type == ClaimTypes.GivenName
        )?.Value;
        model.DateOfBirth = claims.FirstOrDefault(
            c => c.Type == ClaimTypes.DateOfBirth
        )?.Value;
        model.Gender = claims.FirstOrDefault(
            c => c.Type == ClaimTypes.Gender
        )?.Value ?? "保密";
        model.StreetAddress = claims.FirstOrDefault(
            c => c.Type == ClaimTypes.StreetAddress
        )?.Value;
        return model;
    }

    public static IList<Claim> UpdateFromUserModel(
        this AppUser user,
        IMapper mapper,
        AppUserModel model
    ) {
        mapper.Map(model, user);
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Surname, model.Surname ?? string.Empty));
        claims.Add(new Claim(ClaimTypes.GivenName, model.GivenName ?? string.Empty));
        claims.Add(new Claim(ClaimTypes.DateOfBirth, model.DateOfBirth ?? "1970-1-1"));
        claims.Add(new Claim(ClaimTypes.Gender, model.Gender ?? "保密"));
        claims.Add(new Claim(ClaimTypes.StreetAddress, model.StreetAddress ?? string.Empty));
        return claims;
    }

}
