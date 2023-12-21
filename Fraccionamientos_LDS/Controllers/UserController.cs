using Fraccionamientos_LDS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        try
        {
            var users = _userService.GetUsers();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al obtener usuarios: {ex.Message}" });
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        try
        {
            var user = _userService.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al obtener el usuario: {ex.Message}" });
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Crea un nuevo usuario.")]
    [SwaggerResponse(StatusCodes.Status201Created, "El usuario ha sido creado con éxito.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Datos de usuario no válidos.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error al crear el usuario.")]
    public IActionResult CreateUser([FromBody] User user)
    {
        try
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Datos de usuario no válidos");
            }

            _userService.CreateUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al crear el usuario: {ex.Message}" });
        }
    }

    [HttpPut("{Id}")]
    public IActionResult UpdateUser(int Id, [FromBody] User updatedUser)
    {
        try
        {
            if (updatedUser == null)
            {
                return BadRequest("Datos de usuario no válidos");
            }

            _userService.UpdateUser(Id, updatedUser);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error en la actualización del usuario: {ex.Message}" });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            _userService.DeleteUser(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al eliminar el usuario: {ex.Message}" });
        }
    }
}
