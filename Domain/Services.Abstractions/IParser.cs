using Domain.Models;

namespace Domain.Services.Abstractions;

public interface IParser
{
    Task ParseAsync(CancellationToken cancellationToken = default);
}