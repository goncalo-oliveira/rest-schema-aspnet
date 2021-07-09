# RESTful Schema Extensions for ASP.NET

An ASP.NET implementation for the [REST Schema](https://github.com/goncalo-oliveira/rest-schema-spec). This is still a work in progress and it currently implements v0.1 of the spec.

Features:
- [x] Schema-Mapping 
- [x] Schema-Include
- [x] Headers
- [ ] Query string parameters
- [x] JSON schema
- [ ] YAML schema
- [x] Plain text schema

Future Spec Features:
- [ ] Schema-Filter (v0.2?)
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

In the project's `Startup` class, change the MVC Controllers configuration to include the schema filters and the JSON serialization options.

```csharp
public void ConfigureServices( IServiceCollection services )
{
    services.AddControllers( options =>
    {
        options.Filters.AddSchemaResultFilter();
    })
    .AddJsonOptions( options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    } );
    ...
}
```

The `DictionaryKeyPolicy` is important and should match the `PropertyNamingPolicy`. By default, `DictionaryKeyPolicy` is null and that can lead to mixed naming strategies in the results.
