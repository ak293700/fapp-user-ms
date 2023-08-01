namespace Application.Common.Dtos.AuthDtos;

public record LogInDto(
    string Email,
    string Password
);