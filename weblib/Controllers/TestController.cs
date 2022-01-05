using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet(Name = "GetInts")]
    public IEnumerable<int> Get()
    {
        return Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(-20, 55));
    }

    [Authorize(Policy = "IsAuthenticated")]
    [HttpGet("secured", Name = "GetIntsSecured")]
    public IEnumerable<int> GetSecured()
    {
        return Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(-20, 55));
    }

    [Authorize] // Default policy = allow anonymous
    [HttpGet("me", Name = "Me")]
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
