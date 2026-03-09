using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public readonly record struct ComponentSlot(ICanvasComponent Component, Rect Bounds);
