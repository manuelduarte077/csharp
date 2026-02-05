using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Models.Dtos;
using MovieApi.Repository.IRepository;

namespace MovieApi.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    protected APIResponse _apiResponse;

    public UsersController(IUserRepository userRepo, IMapper mapper)
    {
        _userRepo = userRepo;
        _mapper = mapper;
        _apiResponse = new();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetUsers()
    {
        var userList = _userRepo.GetAll();
        var userDto = new List<UserDto>();

        foreach (var list in userList)
        {
            userDto.Add(_mapper.Map<UserDto>(list));
        }

        return Ok(userDto);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:int}", Name = "GetUser")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetUser(int id)
    {
        var userItem = _userRepo.GetById(id);
        if (userItem == null)
        {
            return NotFound();
        }

        var itemUserDto = _mapper.Map<UserDto>(userItem);
        return Ok(itemUserDto);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        bool userNameUnique = _userRepo.IsUniqueUsername(registerUserDto.Username);
        if (!userNameUnique)
        {
            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.Success = false;
            _apiResponse.ErrorMessages.Add("Username already exists");
            return BadRequest(_apiResponse);
        }

        var user = await _userRepo.Register(registerUserDto);
        if (user == null)
        {
            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.Success = false;
            _apiResponse.ErrorMessages.Add("Error occured while registering");
            return BadRequest(_apiResponse);
        }

        _apiResponse.StatusCode = HttpStatusCode.OK;
        _apiResponse.Success = true;
        return Ok(_apiResponse);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        var loginResponse = await _userRepo.Login(loginUserDto);
        if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
        {
            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.Success = false;
            _apiResponse.ErrorMessages.Add("Username or password is incorrect");
            return BadRequest(_apiResponse);
        }

        _apiResponse.StatusCode = HttpStatusCode.OK;
        _apiResponse.Success = true;
        _apiResponse.Result = loginResponse;
        return Ok(_apiResponse);
    }
}