using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using System.ComponentModel;

namespace TeaSharp.Components.Composition;

/// <summary>
/// Coordinates dialog visibility, modal registration, and focus restoration for app workflows.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class DialogWorkflow
{
    private readonly Func<ScreenFocusSnapshot> _captureFocus;
    private readonly Func<ScreenFocusSnapshot, bool> _restoreFocus;
    private readonly Func<ScreenFocusSnapshot, ScreenFocusChain, bool> _restoreFocusWithFallback;
    private readonly Func<ScreenRegionKey, bool> _setFocus;
    private readonly Func<Rect, ScreenRegion> _addModal;
    private ScreenFocusSnapshot _focusSnapshot;
    private bool _focusRequested;

    internal DialogWorkflow(
        DialogComponent dialog,
        ScreenRegionKey regionKey,
        Func<ScreenFocusSnapshot> captureFocus,
        Func<ScreenFocusSnapshot, bool> restoreFocus,
        Func<ScreenFocusSnapshot, ScreenFocusChain, bool> restoreFocusWithFallback,
        Func<ScreenRegionKey, bool> setFocus,
        Func<Rect, ScreenRegion> addModal)
    {
        Dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        RegionKey = regionKey;
        _captureFocus = captureFocus ?? throw new ArgumentNullException(nameof(captureFocus));
        _restoreFocus = restoreFocus ?? throw new ArgumentNullException(nameof(restoreFocus));
        _restoreFocusWithFallback = restoreFocusWithFallback ?? throw new ArgumentNullException(nameof(restoreFocusWithFallback));
        _setFocus = setFocus ?? throw new ArgumentNullException(nameof(setFocus));
        _addModal = addModal ?? throw new ArgumentNullException(nameof(addModal));

        Dialog.Accepted += (_, _) => RestoreFocus();
        Dialog.Dismissed += (_, _) => RestoreFocus();
    }

    /// <summary>
    /// Gets the dialog controlled by the workflow.
    /// </summary>
    public DialogComponent Dialog { get; }

    /// <summary>
    /// Gets the screen region used when the dialog is composed as a modal overlay.
    /// </summary>
    public ScreenRegionKey RegionKey { get; }

    /// <summary>
    /// Gets or sets the fallback focus order used when the captured focus target no longer exists.
    /// </summary>
    public ScreenFocusChain? FallbackFocusChain { get; set; }

    /// <summary>
    /// Gets a value indicating whether the dialog is currently open.
    /// </summary>
    public bool IsOpen => Dialog.IsVisible;

    /// <summary>
    /// Opens the dialog and schedules focus for its modal region.
    /// </summary>
    public void Show()
    {
        if (!Dialog.IsVisible)
        {
            _focusSnapshot = _captureFocus();
        }

        Dialog.IsVisible = true;
        _focusRequested = true;
    }

    /// <summary>
    /// Opens the dialog with the provided title and content lines.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="lines">The dialog content lines.</param>
    public void Show(string title, params string[] lines)
    {
        Show(title, (IReadOnlyList<string>)lines);
    }

    /// <summary>
    /// Opens the dialog with the provided title and content lines.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="lines">The dialog content lines.</param>
    public void Show(string title, IReadOnlyList<string> lines)
    {
        Dialog.Title = title;
        Dialog.BodyLines = lines ?? throw new ArgumentNullException(nameof(lines));
        Show();
    }

    /// <summary>
    /// Hides the dialog and restores focus to the prior region when possible.
    /// </summary>
    public bool Hide()
    {
        if (!Dialog.IsVisible)
        {
            return false;
        }

        Dialog.IsVisible = false;
        _focusRequested = false;
        return RestoreFocus();
    }

    /// <summary>
    /// Registers the dialog as a modal overlay when it is open.
    /// </summary>
    /// <param name="bounds">The modal host bounds.</param>
    public ScreenRegion? Compose(Rect bounds)
    {
        if (!Dialog.IsVisible)
        {
            return null;
        }

        var region = _addModal(bounds);
        if (_focusRequested)
        {
            _setFocus(RegionKey);
            _focusRequested = false;
        }

        return region;
    }

    /// <summary>
    /// Restores focus captured before the dialog opened.
    /// </summary>
    public bool RestoreFocus()
    {
        _focusRequested = false;
        return FallbackFocusChain is { } fallbackFocusChain
            ? _restoreFocusWithFallback(_focusSnapshot, fallbackFocusChain)
            : _restoreFocus(_focusSnapshot);
    }
}
