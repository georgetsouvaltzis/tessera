namespace Tessera.Controls;

/// <summary>
/// Defines supported operators for <see cref="QueryRule"/>.
/// </summary>
public enum QueryOperator
{
    /// <summary>
    /// The equals value.
    /// </summary>
    Equals = 0,
    /// <summary>
    /// The not equals value.
    /// </summary>
    NotEquals = 1,
    /// <summary>
    /// The contains value.
    /// </summary>
    Contains = 2,
    /// <summary>
    /// The starts with value.
    /// </summary>
    StartsWith = 3,
    /// <summary>
    /// The ends with value.
    /// </summary>
    EndsWith = 4,
    /// <summary>
    /// The greater than value.
    /// </summary>
    GreaterThan = 5,
    /// <summary>
    /// The greater than or equal value.
    /// </summary>
    GreaterThanOrEqual = 6,
    /// <summary>
    /// The less than value.
    /// </summary>
    LessThan = 7,
    /// <summary>
    /// The less than or equal value.
    /// </summary>
    LessThanOrEqual = 8,
    /// <summary>
    /// The in value.
    /// </summary>
    In = 9,
    /// <summary>
    /// The not in value.
    /// </summary>
    NotIn = 10,
    /// <summary>
    /// The is empty value.
    /// </summary>
    IsEmpty = 11,
    /// <summary>
    /// The is not empty value.
    /// </summary>
    IsNotEmpty = 12,
}
