namespace Application.Common.Dtos.AuthDtos;

public record RegisterDto(
    string UserName,
    string Email,
    string Password
);