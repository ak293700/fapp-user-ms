using System.Dynamic;
using Application.Common.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace WebApi.Controllers;

[Route("/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Create(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        return Ok(await _userService.CreateAsync(request, cancellationToken));
    }
}