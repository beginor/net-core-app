# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

- `dotnet restore NetCoreApp.slnx`
- `dotnet build NetCoreApp.slnx`
- `dotnet run --project src/Entry/Entry.csproj`
- `dotnet test test/Test/Test.csproj`
- `dotnet test test/Test/Test.csproj --filter FullyQualifiedName~Beginor.NetCoreApp.Test.Api.UsersControllerTest`
- `dotnet test test/Test/Test.csproj --filter FullyQualifiedName~Beginor.NetCoreApp.Test.SystemTest._01_CanInitSystem`

## Facts

- Backend repo only; Angular frontend is in separate repo `net-core-app-client`.
- `src/Entry` is the composition root; config is loaded from `src/Entry/config/`, and `Startup` is split into partial files.
- Main layers: `src/Api` (HTTP/auth/middleware), `src/Data` (NHibernate repositories/entities, with Dapper for SQL-heavy queries), `src/Models` (DTOs), `src/Common` (shared helpers/options), `test/Test` (NUnit tests booting real DI).
- Default DB is PostgreSQL via `src/Entry/config/hibernate.config`; many tests are integration-style, and system bootstrap is `Beginor.NetCoreApp.Test.SystemTest._01_CanInitSystem`.