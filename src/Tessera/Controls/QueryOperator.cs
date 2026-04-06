namespace Tessera.Controls;

/// <summary>
/// Defines supported operators for <see cref="QueryRule"/>.
/// </summary>
public enum QueryOperator
{
    Equals = 0,
    NotEquals = 1,
    Contains = 2,
    StartsWith = 3,
    EndsWith = 4,
    GreaterThan = 5,
    GreaterThanOrEqual = 6,
    LessThan = 7,
    LessThanOrEqual = 8,
    In = 9,
    NotIn = 10,
    IsEmpty = 11,
    IsNotEmpty = 12,
}
