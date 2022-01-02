namespace IcedTea.Api.UseCases.LogError;

public record LogErrorDto(long Id, string Message, string Level, DateTimeOffset TimeStamp,
    string LogEvent, string? Properties);