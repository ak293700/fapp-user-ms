namespace Application.Common.Dtos.AuthDtos;

public record RegisterDto(
    string FirstName,
    string LastName,
    string Email,
    string Password
);