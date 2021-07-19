# RESTful Schema Extensions for ASP.NET

An ASP.NET implementation for the [REST Schema](https://github.com/goncalo-oliveira/rest-schema-spec) Spec. This is still a work in progress and it currently implements v0.1 of the spec.

Features:
- [x] Schema-Mapping 
- [x] Schema-Include
- [x] Headers
- [x] Query string parameters
- [x] JSON schema
- [ ] YAML schema
- [x] Plain text schema

Future Spec Features:
- [ ] Schema filters (v0.2?)
- [ ] Schema references (v0.2?)

## Getting Started

Create a new ASP.NET project and initialize the git repository. Then, checkout this repository as a submodule.

```shell
$ mkdir rest-schema-demo
$ cd rest-schema-demo
$ dotnet new webapi -o src --name rest-schema-demo
$ git init
$ git submodule add git@github.com:goncalo-oliveira/rest-schema-aspnet.git
```

Add a reference on the project and fire up VS Code or any other IDE you prefer.

```shell
$ dotnet add src/rest-schema-demo.csproj reference rest-schema-aspnet/src/rest-schema-aspnet.csproj
$ code .
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
