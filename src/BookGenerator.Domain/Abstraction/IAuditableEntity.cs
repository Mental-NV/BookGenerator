namespace BookGenerator.Domain.Abstraction;

/// <summary>
/// Represents an entity that supports auditing.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets created on date and time in UTC format.
    /// </summary>
    public DateTime CreateOnUtc { get; }

    /// <summary>
    /// Gets modified on date and time in UTC format.
    /// </summary>
    public DateTime? ModifiedOnUtc { get; }
}
