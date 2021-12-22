using System;

namespace Shared.SeedWork;

public interface IScopeContext
{
    Guid CurrentAccountId { get; }
    string CurrentAccountName { get; }
    string CurrentAccountEmail { get; }
    string Role { get; }
}