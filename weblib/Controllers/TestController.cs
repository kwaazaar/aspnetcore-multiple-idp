using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using web.auth;

[ApiController]
[ApiAuthorisationFilter]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet(Name = "GetInts")]
    [AllowAnonymous]
    public IEnumerable<int> Get()
    {
        return Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(-20, 55));
    }

    [HttpGet("get/{min}/{max}")]
    [AllowAnonymous]
    public IEnumerable<int> GetMinMax([FromRoute]int min, [FromRoute] int max)
    {
        return Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(min, max));
    }

    [HttpGet("secureduser", Name = "GetIntsForUserSecured")]
    [UserAuthorisationFilter] // Must be user
    public IEnumerable<int> GetSecuredForUser()
    {
        return Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(-20, 55));
    }

    [HttpGet("secured", Name = "GetIntsSecured")]
    public IEnumerable<int> GetSecured()
    {
        return Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(-20, 55));
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
