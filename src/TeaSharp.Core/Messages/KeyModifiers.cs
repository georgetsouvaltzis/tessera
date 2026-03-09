using System.Text;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Messages;

[Flags]
public enum KeyModifiers
{
    None = 0,
    Shift = 1 << 0,
    Alt = 1 << 1,
    Ctrl = 1 << 2,
    Meta = 1 << 3,
}

