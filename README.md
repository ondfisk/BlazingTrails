# Blazing Trails

Blazing Trails app built while studying [Blazor in Action](https://www.manning.com/books/blazor-in-action).

## Initialize SQLite

```bash
dotnet ef migrations add Initial --project src/BlazingTrails/ --output-dir Persistence/Data/Migrations
dotnet ef database update --project src/BlazingTrails/
```

## Run

```bash
dotnet watch --project src/BlazingTrails/
```
