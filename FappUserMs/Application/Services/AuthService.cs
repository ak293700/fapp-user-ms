using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Application.Common.Dtos.AuthDtos;
using Domain.Entities.UserEntities;
using FappCommon.Exceptions.ApplicationExceptions;
using FappCommon.Exceptions.ApplicationExceptions.UserExceptions;
using FappCommon.Exceptions.DomainExceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace Application.Services;

public class AuthService
{
    private readonly string _authToken;
    private readonly IApplicationDbContext _context;
    public const string AuthTokenLocation = "Auth:Token";

    public AuthService(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _authToken = configuration.GetValue<string>(AuthTokenLocation)
                     ?? throw new Exception("Unable to create the token");
    }

    public async Task Register(RegisterDto request, CancellationToken cancellationToken = default)
    {
        ThrowIfEmailIsInvalid(request.Email);
        ThrowIfUsernameIsInvalid(request.UserName);


        if (request.UserName.Length is < 3 or > 20)
            throw new DataValidationException("Le nom d'utilisateur doit faire entre 3 et 20 caractères");

        // ift he email address is already used
        if (_context.Users.AsQueryable().Any(u => u.Email == request.Email))
            throw new AlreadyExistDomainException("Cette adresse email est déjà utilisée");

        if (_context.Users.AsQueryable().Any(u => u.UserName == request.UserName))
            throw new AlreadyExistDomainException("Ce nom d'utilisateur est déjà utilisé");


        CreatePasswordHashWithChecking(request.Password, out var passwordHash, out var passwordSalt);

        // Set password information
        User user = new()
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
        };

        await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Check that the password is okay, then call the method to create the hash and salt.
    /// If the password is too weak it throws an exception. 
    /// </summary>
    private static void CreatePasswordHashWithChecking(string password, out byte[] passwordHash,
        out byte[] passwordSalt)
    {
        if (!IsPasswordValid(password))
            throw new DataValidationException("Mot de passe trop faible");

        // Next we need to check if the user already exists
        CreatePasswordHash(password, out byte[] passwordHashTmp, out byte[] passwordSaltTmp);

        passwordHash = passwordHashTmp;
        passwordSalt = passwordSaltTmp;
    }

    /// <summary>
    /// Create a combo of password hash and salt
    /// </summary>
    /// <param name="password">The password you want to hash</param>
    /// <param name="passwordHash">The password will be stored in it</param>
    /// <param name="passwordSalt">The password salt will be stored in it</param>
    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (HMACSHA512 hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key; // Already generate a random salt for us
            // Hash the password with the salt
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    public ValueTask<string> LogIn(LogInDto registerUser, CancellationToken cancellationToken = default)
    {
        // Check if the user exists
        User user =
            _context.Users
                .AsQueryable()
                .Where(u => u.Email == registerUser.Email)
                .Select(u => new User
                {
                    Id = u.Id,
                    PasswordHash = u.PasswordHash,
                    PasswordSalt = u.PasswordSalt
                })
                .FirstOrDefault()
            ?? throw new NotAuthorizedApplicationException("Mot de passe ou email incorrect");

        // Check if the password is correct
        if (!VerifyPasswordHash(registerUser.Password, user.PasswordHash, user.PasswordSalt))
            throw new NotAuthorizedApplicationException("Mot de passe ou email incorrect");

        return new ValueTask<string>(CreateToken(user));
    }

    /// <summary>
    /// Check id the given password correspond to the password hash and salt
    /// </summary>
    /// <param name="password">Password you wan to check</param>
    /// <param name="passwordHash">The reference one (hashed)</param>
    /// <param name="passwordSalt">the corresponding salt of passwordHash</param>
    /// <returns>True if the password is correct, else false</returns>
    private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        // Init hmac with the same key as our password salt
        using (HMACSHA512 hmac = new HMACSHA512(passwordSalt))
        {
            // Hash the password with the salt
            byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            // Compare the two hashes
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    /// <summary>
    /// Create a JWT token for the given user
    /// </summary>
    /// <returns>return the token as a string</returns>
    private string CreateToken(User user)
    {
        List<Claim> claims = GetClaims(user);

        SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_authToken));

        // Credentials
        SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        DateTime now = DateTime.Today;
        DateTime expire = now.AddDays(1).AddSeconds(-1); // expires at 23:59:59, available for the day
        if (expire.AddHours(-1) < now) // If the token expires in less than an hour
            expire = now.AddDays(1); // the token will be valid for the net day too

        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            expires: expire,
            signingCredentials: cred
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static List<Claim> GetClaims(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
        };

        return claims;
    }

    private static readonly ImmutableArray<char> SpecialCharacters = new[]
    {
        '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '[', ']',
        '{', '}', '|', '\\', ':', ';', '"', '\'', '<', '>', ',', '.', '?', '/'
    }.ToImmutableArray();

    private static bool IsPasswordValid(string password)
    {
        if (password.Length < 8)
            return false;

        return !password.Any(char.IsWhiteSpace)
               && password.Any(char.IsUpper)
               && password.Any(char.IsLower)
               && password.Any(char.IsDigit)
               && password.Any(c => SpecialCharacters.Contains(c));
    }

    private static void ThrowIfEmailIsInvalid(string email)
    {
        try
        {
            MailAddress address = new MailAddress(email);
            if (string.IsNullOrEmpty(address.DisplayName))
                return;
        }
        catch (FormatException)
        {
        }

        throw new DataValidationException("L'email n'est pas valide");
    }

    private static void ThrowIfUsernameIsInvalid(string username)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length is < 3 or > 20)
            throw new DataValidationException("Le nom d'utilisateur doit faire entre 3 et 20 caractères");

        Regex regex = new Regex("^[a-zA-Z][a-zA-Z0-9_]*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled,
            TimeSpan.FromSeconds(1));

        try
        {
            if (regex.IsMatch(username))
                return;
        }
        catch (RegexMatchTimeoutException)
        {
        }

        throw new DataValidationException(
            "Le nom d'utilisateur doit commencer par une lettre puit ne contenir que des lettres, chiffres ou '_'");
    }
}