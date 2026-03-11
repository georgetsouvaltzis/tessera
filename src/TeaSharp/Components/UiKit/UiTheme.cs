using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

public readonly record struct UiTheme(
    char StatusFill = ' ',
    char SkeletonEvenFill = '░',
    char SkeletonOddFill = '▒',
    char ModalBackdropFill = '·');

