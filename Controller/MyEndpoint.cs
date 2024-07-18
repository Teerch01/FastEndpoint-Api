using FastEndpoints;
using FastEndpoints.Security;
using WebApi.Models.RequestDTO;
using WebApi.Models.ResponseDTO;
using WebApi.Service;

namespace WebApi.Controller;

public class MyEndpoint(UserService service) : Endpoint<UserRequestDTO, UserResponseDTO>
{
    readonly UserService _service = service;
    public override void Configure()
    {
        Post("/api/user/create");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UserRequestDTO newuser, CancellationToken ct)
    {
        await _service.CreateUserAsync(newuser.FirstName, newuser.LastName, newuser.UserName, newuser.Password, newuser.Email);
        await SendOkAsync(ct);
    }
}

public class GetUserAsync(UserService service) : EndpointWithoutRequest
{
    readonly UserService _service = service;
    public override void Configure()
    {
        Get("/api/user");
        AllowAnonymous();

    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = await _service.GetUserAsync();
        if (user == null)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await SendOkAsync(user, ct);
    }
}
public class UserLoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/api/user/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var jwtToken = JwtBearer.CreateToken(
            o =>
            {
                o.SigningKey = "@/admin@/signing_key/[a-zA-Z0-9]";
                o.ExpireAt = DateTime.UtcNow.AddDays(1);
                o.User.Roles.Add("Manager", "Auditor");
                o.User.Claims.Add(("UserName", req.UserName));
                o.User["UserId"] = "001";
            }
        );
        await SendAsync(new()
        {
            UserName = req.UserName,
            Token = jwtToken
        });

    }

}

public class AutenticateUserEndpoint(UserService service) : Endpoint<AutenticateUser, UserResponseDTO>
{
    readonly UserService _service = service;
    public override void Configure()
    {
        Get("/api/user/authenticate");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AutenticateUser User, CancellationToken ct)
    {
        var user = await _service.AutenticateUserAsync(User.username, User.password);
        if (user == null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendAsync(new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
        }, cancellation: ct);
    }
}

public class UserbyEmailEndpoint(UserService service) : Endpoint<GetUserByEmail, UserResponseDTO>
{
    readonly UserService _service = service;
    public override void Configure()
    {
        Get("/api/user/getuser/email");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUserByEmail User, CancellationToken ct)
    {
        var user = await _service.GetUserByEmailAsync(User.Email);
        await SendAsync(new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
        }, cancellation: ct);
    }
}

