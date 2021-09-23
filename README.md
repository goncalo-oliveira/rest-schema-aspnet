# RESTful Schema Extensions for ASP.NET

An ASP.NET implementation for the [REST Schema](https://github.com/goncalo-oliveira/rest-schema-spec) Spec. This is still a work in progress and it currently implements v0.2 of the spec.

Features:
- [x] Schema-Mapping 
- [x] Schema-Include
- [x] Headers
- [x] Query string parameters
- [x] JSON schema
- [x] Plain text schema
- [x] Schema filters

## Getting Started

The fastest way is to add the NuGet package to your ASP.NET project.

```shell
$ dotnet add package RestSchema --version 0.2.0-preview-1
```

Alternatively, you can checkout this repository as a submodule and then add a reference to the project.

```shell
$ git submodule add git@github.com:goncalo-oliveira/rest-schema-aspnet.git
$ dotnet add reference rest-schema-aspnet/src/rest-schema-aspnet.csproj
```

In the project's `Startup` class, change the MVC Controllers configuration replacing `AddControllers` with `AddSchemaControllers`.

```csharp
public void ConfigureServices( IServiceCollection services )
{
    services.AddSchemaControllers();
    ...
}
```

## Schema-Include

To handle with the `Schema-Include` you can use the extensions on the `HttpRequest` to verify if a property is to be included. Here's an example to retrieve a user's details and include the user's teams if included in the schema.

```csharp
public class UserController : ControllerBase
{
    ...

    public IActionResult GetUser( int id )
    {
        // retrieve the user's details
        var user = ExampleUserRepository.GetUser( id );

        // include the user's teams
        if ( Request.SchemaIncludes( "teams" ) )
        {
            user.Teams = ExampleTeamRepository.GetUserTeams( id );
        }

        return Ok( user );
    }
}
```
