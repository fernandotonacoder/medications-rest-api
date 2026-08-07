# Medications REST API

[![CI](https://github.com/fernandotonacoder/medications-rest-api/actions/workflows/ci.yml/badge.svg)](https://github.com/fernandotonacoder/medications-rest-api/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/aspire/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927)](https://www.microsoft.com/sql-server)
[![Scalar](https://img.shields.io/badge/Scalar-API%20Reference-1F2937)](https://scalar.com/)

[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=fernandotonacoder_medications-rest-api)](https://sonarcloud.io/summary/overall?id=fernandotonacoder_medications-rest-api)

A small REST API to list, create and delete medications, built with .NET 10 Minimal APIs.

## Tech stack

- **.NET 10 / C#** — ASP.NET Core [Minimal APIs]
- **EF Core** — SQL Server for the app, InMemory provider for unit tests
- **[.NET Aspire]** — AppHost for local orchestration
- **OpenAPI + Scalar** — generated document with interactive UI (Development only)
- **xunit v3** — unit tests, with Coverlet coverage feeding SonarQube Cloud
- **[GitHub Actions]** — builds and tests every push and pull request to `main`

## Getting started

Requires the [.NET 10 SDK] and a container runtime (Docker or Podman).

```powershell
# Run via the Aspire AppHost (dashboard + Scalar link):
dotnet run --project src/Medications.AppHost

# Or, with the Aspire CLI installed:
aspire run
```

The dashboard URL is printed on the console; the API and its Scalar UI are listed there. The AppHost runs SQL Server in a container pulled by Aspire automatically if non-existent, and passes the connection string to the API, so there is nothing to configure. The container and its data are kept between runs.

If running the solution directly from Visual Studio, Rider or VS Code, the IDE launches the browser with the Aspire Dashboard.

### Using your own SQL Server instead

No container runtime needed. If a `medications` connection string is configured, the AppHost uses
it and starts no container:

```powershell
dotnet user-secrets --project src/Medications.AppHost set "ConnectionStrings:medications" "Server=localhost;Database=Medications;Trusted_Connection=True;TrustServerCertificate=True"
```

The API can also run without the AppHost, reading the same connection string from its own
configuration ([appsettings.json](src/Medications.Api/appsettings.json), already set to
`localhost` with Windows authentication):

```powershell
dotnet run --project src/Medications.Api
```

Standalone, the API listens on `http://localhost:5122`; interactive docs at `/scalar` (Development).

### Endpoints

| Method   | Route                   | Success | Errors |
| -------- | ----------------------- | ------- | ------ |
| `GET`    | `/api/medications`      | `200`   | —      |
| `GET`    | `/api/medications/{id}` | `200`   | `400`, `404` |
| `POST`   | `/api/medications`      | `201`   | `400` |
| `DELETE` | `/api/medications/{id}` | `204`   | `400`, `404` |

## Tests

```powershell
dotnet test
```

Unit tests cover the endpoint handlers, the mapping layer and the request contract's validation attributes, using the EF InMemory provider and a `FakeTimeProvider`, so no database is needed.

## Database

The schema is managed with EF Core migrations, applied when the API starts. To work with them,
use the [EF CLI][dotnet-ef]:

```powershell
dotnet tool restore
dotnet ef migrations add <Name> --project src/Medications.Api   # after changing the model
dotnet ef database update --project src/Medications.Api         # apply without running the API
```

## Some Design notes

- Validation uses DataAnnotations on the request DTO, enforced by .NET 10's built-in minimal API validation ([`AddValidation()`]). The route `id` is validated the same way, so every validation error has the same response shape.
- `Name` is capped at 200 characters, defined once in `Medication.NameMaxLength` and used by both the DTO and the EF model.
- The DTO doesn't use the C# `required` keyword for `Name`: a missing `name` then returns a normal field-level validation error instead of a generic 400.
- Every error response uses the same JSON shape (ASP.NET Core's Problem Details), whether it is a validation error, malformed JSON, an invalid route value or a 404. This is wired once in Program.cs, not per endpoint.
- Binding failures keep their explanatory `detail` message in Production too (by default ASP.NET Core only includes it in Development), and client mistakes like malformed JSON are not logged as server errors.
- Invalid ids (`0` or negative) return `400`; well-formed ids that don't exist return `404`.
- `CreationDate` is set by the server (injected `TimeProvider`, faked in tests); `Id` and `CreationDate` are never accepted as input.
- `GET /api/medications/{id}` is not in the challenge spec; it exists as the target of POST's `Location` header.
- The DbContext is registered with Aspire's [`AddSqlServerDbContext`], which adds retries, health checks and telemetry on top of `AddDbContext`.
- The [SQL Server container] is pinned to a specific image tag instead of `2022-latest` (which changes over time), is kept between runs with its data in a volume, and its generated password lives in the AppHost's user secrets.
- Migrations run at startup to keep the demo to a single command; a real deployment would apply them as a separate step.

---

<div align="center">

Made with ❤️ by Fernando Tona
[Website](https://fernandotonacoder.github.io) • [LinkedIn](https://www.linkedin.com/in/fernandotona/) • [GitHub](https://github.com/fernandotonacoder)

</div>

[Minimal APIs]: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview
[.NET Aspire]: https://learn.microsoft.com/en-us/dotnet/aspire/
[.NET 10 SDK]: https://dotnet.microsoft.com/download/dotnet/10.0
[`AddValidation()`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.validationservicecollectionextensions.addvalidation
[`AddSqlServerDbContext`]: https://aspire.dev/integrations/databases/efcore/sql-server/sql-server-connect/
[SQL Server container]: https://aspire.dev/integrations/databases/sql-server/sql-server-host/
[dotnet-ef]: https://learn.microsoft.com/en-us/ef/core/cli/dotnet
[GitHub Actions]: .github/workflows/ci.yml
