using System.Security.Cryptography;
using System.Text;
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
            byte[] passwordHash, passwordKey;
            using (var hmc = new HMACSHA512())
            {
                passwordKey = hmc.Key;
                passwordHash = hmc.ComputeHash(Encoding.Unicode.GetBytes(password));
            }

            User user = new()
            {
                FirstName = firstname,
                LastName = lastname,
                UserName = username,
                Password = passwordHash,
                PasswordKey = passwordKey,
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

    private static bool MatchPasswordHash(string passwordText, byte[] password, byte[] passwordKey)
    {
        try
        {
            using var hmac = new HMACSHA512(passwordKey);
            var passwordHash = hmac.ComputeHash(Encoding.Unicode.GetBytes(passwordText));
            for (int i = 0; i < password.Length; i++)
            {
                if (passwordHash[i] != password[i])
                {
                    return false;
                }
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<UserResponseDTO?> AutenticateUserAsync(string username, string password)
    {
        try
        {
            var user = await _context.Users.FirstAsync(x => x.UserName == username);
            if (user ==null || user.Password == null)
            {
                return null;
            }
            if (!MatchPasswordHash(password, user.Password, user.PasswordKey))
            {
                return null;
            }
            
            TinyMapper.Bind<User, UserResponseDTO>();
            var res = TinyMapper.Map<UserResponseDTO>(user);
            return res;
        }
        catch(Exception)
        {
            return null;
        }
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


