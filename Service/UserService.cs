using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;
using WebApi.Data;
using WebApi.Models;
using WebApi.Models.ResponseDTO;

namespace WebApi.Service;

public class UserService(UserContext context)
{
    private readonly UserContext _context = context;

    public async Task<IEnumerable<UserResponseDTO>?> GetUserAsync()
    {
        try
        {
            var users = await _context.Users.ToListAsync();
            TinyMapper.Bind<List<User>, List<UserResponseDTO>>();
            var res = TinyMapper.Map<List<UserResponseDTO>>(users);

            return res;
        }
        catch (Exception)
        {
            return null;
        }

    }

    public async Task<User?> CreateUserAsync(string firstname, string lastname, string username, string password, string email)
    {
        try
        {
            User user = new()
            {
                FirstName = firstname,
                LastName = lastname,
                UserName = username,
                Password = password,
                Email = email
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }
        catch (Exception)
        {
            return null;
        };
    }

    public async Task<UserResponseDTO?> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }
            TinyMapper.Bind<User, UserResponseDTO>();
            var res = TinyMapper.Map<UserResponseDTO>(user);
            return res;
        }
        catch (Exception)
        {
            return null;
        }
    }
}


