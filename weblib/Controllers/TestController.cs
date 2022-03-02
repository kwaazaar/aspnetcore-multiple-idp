using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using web.auth;
using weblib.Models;
using System.Runtime.Caching;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

//[CustomApiVersion("1-core")]
[ApiController]
[ApiAuthorisationFilter]
[ApiExplorerSettings(IgnoreApi =false)]
[Route("[controller]")]
public class TestController : ControllerBase
{
    public TestController(ILogger<TestController> logger, IApiVersionDescriptionProvider provider, IApiDescriptionGroupCollectionProvider apiExplorer)
    {
        this._logger = logger;
        this.provider = provider;
        this.apiExplorer = apiExplorer;
    }

    private static readonly MemoryCache _theCache = new MemoryCache("X");
    private readonly ILogger<TestController> _logger;
    private readonly IApiVersionDescriptionProvider provider;
    private readonly IApiDescriptionGroupCollectionProvider apiExplorer;

    [HttpGet(Name = "GetInts")]
    [AllowAnonymous]
    public IEnumerable<int> Get()
    {
        if (_theCache.Contains("numbers"))
        {
            _logger.LogInformation("Loading numbers from cache");
        }
        else
        {
            _logger.LogInformation("Generating new numbers");
        }

        List<int> numbers = (List<int>)_theCache["numbers"];

        if (numbers == null)
        {
            numbers = Enumerable.Range(1, 5)
                .Select(_ => Random.Shared.Next(-20, 55))
                .ToList();
            _theCache.Add("numbers", numbers, DateTimeOffset.Now.AddMinutes(5));
        }

        return numbers;
    }

    [HttpGet("get/{max}")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<int>> GetMinMax([FromRoute] int max)
    {
        var minString = Request.Query["MIN"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(minString)) return BadRequest();
        var min = int.Parse(minString);

        return Ok(Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(min, max)));
    }

    [HttpPut("get/minmax")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<int>> GetMinMaxComplex([Required]MinMax minMax) // [Required] makes Swagger mark it as required too. Without it, validation still fails, indicating an empty body is not allowed
    {
        return Ok(Enumerable.Range(1,5).Select(_ => Random.Shared.Next(minMax.Min.Value, minMax.Max.Value)));
    }

    [HttpGet("secureduser", Name = "GetIntsForUserSecured")]
    //[ApiAuthorisationFilter] // Already specified on controller: no need to execute twice
    [UserAuthorisationFilter] // Adds additional policy: Must be user (NB: all policies must be satisfied!)
    public IEnumerable<int> GetSecuredForUser()
    {
        return Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(-20, 55));
    }

    [HttpGet("secured", Name = "GetIntsSecured")]
    public IEnumerable<int> GetSecured()
    {
        return Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(-20, 55));
    }

    [HttpGet("info")]
    [AllowAnonymous]
    public ActionResult GetInfo()
    {
        return Ok(new
        {
            AppContext.BaseDirectory,
            Environment.ProcessPath,
            Environment.CurrentDirectory,
            Assembly.GetExecutingAssembly().Location,
            provider.ApiVersionDescriptions,
            ApiDescriptionGroups = apiExplorer.ApiDescriptionGroups.Items.SelectMany(g => g.Items.Select(i => new { i.GroupName, i.HttpMethod, i.RelativePath })),
        });
    }

    [HttpGet("info2/{*rest}")]
    [AllowAnonymous]
    public ActionResult GetInfo2(string rest = null)
    {
        return Ok(new
        {
            DisplayUrl = Request.GetDisplayUrl(),
            EncodedUrl = Request.GetEncodedUrl(),
            BasePath = new Uri(Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority),
            Path = Request.Path,
            Query = Request.QueryString,
            Rest = rest,
        });
    }

    [HttpGet("me", Name = "Me")]
    [AllowAnonymous]
    public object Me()
    {
        if (User.Identity.IsAuthenticated)
        {
            return new
            {
                Username = User.Identity.Name,
                User.Identity.AuthenticationType,
                Claims = User.Claims.Select(c => new
                {
                    c.Type,
                    c.Value
                })
            };
        }
        else
            return NotFound("Not logged in");
    }
}
