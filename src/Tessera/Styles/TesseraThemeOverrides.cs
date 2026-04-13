namespace Tessera.Styles;

/// <summary>
///     Stores hierarchical theme overrides and resolves effective themes for controls.
/// </summary>
/// <remarks>
///     Precedence from lowest to highest:
///     global theme, global state, control-type theme, control-type state, control-instance theme, control-instance state.
/// </remarks>
public sealed class TesseraThemeOverrides
{
    private readonly Dictionary<object, Dictionary<TesseraThemeVisualState, TesseraTheme>> _controlInstanceStateThemes =
        new(ReferenceEqualityComparer.Instance);

    private readonly Dictionary<object, TesseraTheme> _controlInstanceThemes = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<Type, Dictionary<TesseraThemeVisualState, TesseraTheme>> _controlTypeStateThemes = [];
    private readonly Dictionary<Type, TesseraTheme> _controlTypeThemes = [];
    private readonly Dictionary<TesseraThemeVisualState, TesseraTheme> _globalStateThemes = [];

    /// <summary>
    ///     Gets or sets the global theme override.
    /// </summary>
    public TesseraTheme? GlobalTheme { get; set; }

    /// <summary>
    ///     Assigns a control-type override.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="theme">The theme override.</param>
    public void SetControlType<TControl>(TesseraTheme theme)
        where TControl : class
    {
        _controlTypeThemes[typeof(TControl)] = theme ?? throw new ArgumentNullException(nameof(theme));
    }

    /// <summary>
    ///     Assigns a control-instance override.
    /// </summary>
    /// <param name="control">The control instance.</param>
    /// <param name="theme">The theme override.</param>
    public void SetControlInstance(object control, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        _controlInstanceThemes[control] = theme ?? throw new ArgumentNullException(nameof(theme));
    }

    /// <summary>
    ///     Assigns a global state override.
    /// </summary>
    /// <param name="state">The visual state.</param>
    /// <param name="theme">The theme override.</param>
    public void SetState(TesseraThemeVisualState state, TesseraTheme theme)
    {
        _globalStateThemes[state] = theme ?? throw new ArgumentNullException(nameof(theme));
    }

    /// <summary>
    ///     Assigns a control-type state override.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="state">The visual state.</param>
    /// <param name="theme">The theme override.</param>
    public void SetControlTypeState<TControl>(TesseraThemeVisualState state, TesseraTheme theme)
        where TControl : class
    {
        var controlType = typeof(TControl);
        if (!_controlTypeStateThemes.TryGetValue(controlType, out var map))
        {
            map = [];
            _controlTypeStateThemes[controlType] = map;
        }

        map[state] = theme ?? throw new ArgumentNullException(nameof(theme));
    }

    /// <summary>
    ///     Assigns a control-instance state override.
    /// </summary>
    /// <param name="control">The control instance.</param>
    /// <param name="state">The visual state.</param>
    /// <param name="theme">The theme override.</param>
    public void SetControlInstanceState(object control, TesseraThemeVisualState state, TesseraTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        if (!_controlInstanceStateThemes.TryGetValue(control, out var map))
        {
            map = [];
            _controlInstanceStateThemes[control] = map;
        }

        map[state] = theme ?? throw new ArgumentNullException(nameof(theme));
    }

    /// <summary>
    ///     Resolves the effective theme for a control and optional visual state.
    /// </summary>
    /// <param name="control">The control instance.</param>
    /// <param name="baseTheme">The base theme.</param>
    /// <param name="state">The visual state.</param>
    /// <returns>The merged effective theme.</returns>
    public TesseraTheme Resolve(object control, TesseraTheme baseTheme,
        TesseraThemeVisualState state = TesseraThemeVisualState.Default)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(baseTheme);

        if (GlobalTheme is null
            && _globalStateThemes.Count == 0
            && _controlTypeThemes.Count == 0
            && _controlTypeStateThemes.Count == 0
            && _controlInstanceThemes.Count == 0
            && _controlInstanceStateThemes.Count == 0)
        {
            return baseTheme;
        }

        var resolved = baseTheme;
        if (GlobalTheme is not null)
        {
            resolved = TesseraThemeMerge.Merge(resolved, GlobalTheme);
        }

        if (_globalStateThemes.TryGetValue(state, out var globalStateTheme))
        {
            resolved = TesseraThemeMerge.Merge(resolved, globalStateTheme);
        }

        var controlType = control.GetType();
        if (_controlTypeThemes.TryGetValue(controlType, out var controlTypeTheme))
        {
            resolved = TesseraThemeMerge.Merge(resolved, controlTypeTheme);
        }

        if (_controlTypeStateThemes.TryGetValue(controlType, out var typeStateMap)
            && typeStateMap.TryGetValue(state, out var controlTypeStateTheme))
        {
            resolved = TesseraThemeMerge.Merge(resolved, controlTypeStateTheme);
        }

        if (_controlInstanceThemes.TryGetValue(control, out var controlInstanceTheme))
        {
            resolved = TesseraThemeMerge.Merge(resolved, controlInstanceTheme);
        }

        if (_controlInstanceStateThemes.TryGetValue(control, out var instanceStateMap)
            && instanceStateMap.TryGetValue(state, out var controlInstanceStateTheme))
        {
            resolved = TesseraThemeMerge.Merge(resolved, controlInstanceStateTheme);
        }

        return resolved;
    }
}
