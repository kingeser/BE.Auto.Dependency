# BE.Auto.Dependency

## Overview

The `BE.Auto.Dependency` module simplifies dependency injection in your application by automating the registration of services using the ASP.NET Core Dependency Injection framework.

## Usage

To integrate automatic dependency registration, add the following code in your application startup:

```csharp
builder.Services.AddAutoDependency();
```
Example Service Implementation
Consider the implementation of a service, UserAppService, as an example:
```csharp
public class UserAppService : IUserAppService
{
    public async Task<bool> UpdateProfilePictureAsync(IFormFile file)
    {
        if (file.Length == 0)
            return false;

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", file.FileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return true;
    }

    public async Task<string> GetUserNameAsync()
    {
        return await Task.FromResult("Burak ESER");
    }
}
```
Service Interfaces
Define service interfaces that extend IApplicationService:

```csharp
public interface IUserAppService : IApplicationService
{
    Task<bool> UpdateProfilePictureAsync(IFormFile file);
    Task<string> GetUserNameAsync();
}

public interface IApplicationService : ITransientDependency
{
}
```
Lifetimes and Attributes
The module supports the following lifetimes:

ITransientDependency
ISingletonDependency
IScopedDependency
Attributes for lifetimes include:

ScopedDependencyAttribute
SingletonDependencyAttribute
TransientDependencyAttribute






