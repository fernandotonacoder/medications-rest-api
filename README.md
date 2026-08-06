# Medications REST API

A small REST API to list, create and delete medications, built with .NET 10 Minimal APIs.

## Tech stack

- **.NET 10 / C#** — ASP.NET Core [Minimal APIs]
- **[EF Core]** — [InMemory provider] for Unit Tests, SQL Server for the real app
- **[.NET Aspire]** — AppHost for local orchestration
- **OpenAPI + [Scalar]** — generated document with interactive UI (Development only)
- **[xunit v3][xunit]** — unit tests, with Coverlet coverage feeding SonarQube Cloud in CI

## Getting started

Requires the [.NET 10 SDK].

```bash
# Run via the Aspire AppHost (dashboard + Scalar link):
dotnet run --project src/Medications.AppHost

# Or run the API on its own:
dotnet run --project src/Medications.Api
```

Standalone, the API listens on `http://localhost:5122`. In Development, interactive docs are at `/scalar` and the OpenAPI document at `/openapi/v1.json`.

### Endpoints

| Method   | Route                   | Success          | Errors |
| -------- | ----------------------- | ---------------- | ------ |
| `GET`    | `/api/medications`      | `200` list       | —      |
| `GET`    | `/api/medications/{id}` | `200`            | `400` invalid id, `404` |
| `POST`   | `/api/medications`      | `201` + Location | `400` validation |
| `DELETE` | `/api/medications/{id}` | `204`            | `400` invalid id, `404` |

## Tests

```bash
dotnet test
```

Unit tests cover the endpoint handlers (invoked directly, with a fresh InMemory context per test and a fake `TimeProvider`), the mapping layer, and the request contract's validation attributes. Coverage is configured in `tests/test.runsettings` (cobertura, consumed by SonarQube in CI).

## Some Design notes

- Validation uses [DataAnnotations] on the request DTO, enforced by .NET 10's [`AddValidation()`]. The route `id` is validated the same way, through a [`[Range]`][range] attribute on the handler parameter, so every validation error has the same [`HttpValidationProblemDetails`] shape.
- `Name` is capped at 200 characters, defined once in `Medication.NameMaxLength` and used by both [`[StringLength]`][stringlength] on the DTO and [`HasMaxLength`] in the EF model.
- The DTO doesn't use the C# [`required`][required-keyword] keyword for `Name`: a body without `name` would then fail deserialization with a generic 400, instead of a normal `errors.Name` validation error.
- Errors are [Problem Details (RFC 9457)][rfc9457] on every path: [`AddProblemDetails`] + [`UseExceptionHandler`] (with [`StatusCodeSelector`]) + [`UseStatusCodePages`]. [`ThrowOnBadRequest`] is enabled for all environments so binding failures keep their `detail` outside Development, and [`SuppressDiagnosticsCallback`] keeps those client errors out of Error-level logs.
- Invalid ids (`0` or negative) return `400`; well-formed ids that don't exist return `404`.
- `CreationDate` is set by the server, using an injected [`TimeProvider`] ([`FakeTimeProvider`] in tests). The request and response DTOs are separate types, so `Id` and `CreationDate` are never accepted as input.
- `GET /api/medications/{id}` is not in the challenge spec; it exists as the target of the `Location` header returned by [`CreatedAtRoute`] on POST.

[Minimal APIs]: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview
[EF Core]: https://learn.microsoft.com/en-us/ef/core/
[InMemory provider]: https://learn.microsoft.com/en-us/ef/core/providers/in-memory/
[.NET Aspire]: https://learn.microsoft.com/en-us/dotnet/aspire/
[Scalar]: https://github.com/scalar/scalar
[xunit]: https://xunit.net/
[.NET 10 SDK]: https://dotnet.microsoft.com/download/dotnet/10.0
[DataAnnotations]: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations
[`AddValidation()`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.validationservicecollectionextensions.addvalidation
[range]: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.rangeattribute
[stringlength]: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.stringlengthattribute
[`HttpValidationProblemDetails`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpvalidationproblemdetails
[`HasMaxLength`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder.hasmaxlength
[required-keyword]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required
[rfc9457]: https://www.rfc-editor.org/rfc/rfc9457
[`AddProblemDetails`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.problemdetailsservicecollectionextensions.addproblemdetails
[`UseExceptionHandler`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.exceptionhandlerextensions.useexceptionhandler
[`StatusCodeSelector`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.exceptionhandleroptions.statuscodeselector
[`UseStatusCodePages`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.statuscodepagesextensions.usestatuscodepages
[`ThrowOnBadRequest`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routehandleroptions.throwonbadrequest
[`SuppressDiagnosticsCallback`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.exceptionhandleroptions.suppressdiagnosticscallback
[`TimeProvider`]: https://learn.microsoft.com/en-us/dotnet/api/system.timeprovider
[`FakeTimeProvider`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.time.testing.faketimeprovider
[`CreatedAtRoute`]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.typedresults.createdatroute
