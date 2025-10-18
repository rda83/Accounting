using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace Accounting.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VersionController : ControllerBase
{
    private readonly ILogger<VersionController> _logger;

    public VersionController(ILogger<VersionController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public VersionInfo Get()
    {
        var assembly = GetType().Assembly;
        var assemblyName = assembly.GetName();

        var result = new VersionInfo(

            Version: assemblyName.Version?.ToString() ?? "0.0.0.0",
            InformationalVersion: assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown",
            AssemblyName: assemblyName.Name ?? "Unknown",
            BuildDate: System.IO.File.GetLastWriteTime(assembly.Location)
        );
        return result;
    }

}

public record VersionInfo(
    string Version,
    string InformationalVersion,
    string AssemblyName,
    DateTime BuildDate
);
