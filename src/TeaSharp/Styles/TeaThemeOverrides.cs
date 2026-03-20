namespace TeaSharp.Styles;

/// <summary>
/// Stores hierarchical theme overrides and resolves effective themes for controls.
/// </summary>
/// <remarks>
/// Precedence from lowest to highest:
/// global theme, global state, control-type theme, control-type state, control-instance theme, control-instance state.
/// </remarks>
public sealed class TeaThemeOverrides
{
    private readonly Dictionary<Type, TeaTheme> _controlTypeThemes = [];
    private readonly Dictionary<object, TeaTheme> _controlInstanceThemes = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<TeaThemeVisualState, TeaTheme> _globalStateThemes = [];
    private readonly Dictionary<Type, Dictionary<TeaThemeVisualState, TeaTheme>> _controlTypeStateThemes = [];
    private readonly Dictionary<object, Dictionary<TeaThemeVisualState, TeaTheme>> _controlInstanceStateThemes = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Gets or sets the global theme override.
    /// </summary>
    public TeaTheme? GlobalTheme { get; set; }

    /// <summary>
    /// Assigns a control-type override.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="theme">The theme override.</param>
    public void SetControlType<TControl>(TeaTheme theme)
        where TControl : class
    {
        _controlTypeThemes[typeof(TControl)] = theme ?? throw new ArgumentNullException(nameof(theme));
    }

    /// <summary>
    /// Assigns a control-instance override.
    /// </summary>
    /// <param name="control">The control instance.</param>
    /// <param name="theme">The theme override.</param>
    public void SetControlInstance(object control, TeaTheme theme)
    {
        ArgumentNullException.ThrowIfNull(control);
        _controlInstanceThemes[control] = theme ?? throw new ArgumentNullException(nameof(theme));
    }

    /// <summary>
    /// Assigns a global state override.
    /// </summary>
    /// <param name="state">The visual state.</param>
    /// <param name="theme">The theme override.</param>
    public void SetState(TeaThemeVisualState state, TeaTheme theme)
    {
        _globalStateThemes[state] = theme ?? throw new ArgumentNullException(nameof(theme));
    }

    /// <summary>
    /// Assigns a control-type state override.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="state">The visual state.</param>
    /// <param name="theme">The theme override.</param>
    public void SetControlTypeState<TControl>(TeaThemeVisualState state, TeaTheme theme)
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
    /// Assigns a control-instance state override.
    /// </summary>
    /// <param name="control">The control instance.</param>
    /// <param name="state">The visual state.</param>
    /// <param name="theme">The theme override.</param>
    public void SetControlInstanceState(object control, TeaThemeVisualState state, TeaTheme theme)
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
    /// Resolves the effective theme for a control and optional visual state.
    /// </summary>
    /// <param name="control">The control instance.</param>
    /// <param name="baseTheme">The base theme.</param>
    /// <param name="state">The visual state.</param>
    /// <returns>The merged effective theme.</returns>
    public TeaTheme Resolve(object control, TeaTheme baseTheme, TeaThemeVisualState state = TeaThemeVisualState.Default)
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
            resolved = TeaThemeMerge.Merge(resolved, GlobalTheme);
        }

        if (_globalStateThemes.TryGetValue(state, out var globalStateTheme))
        {
            resolved = TeaThemeMerge.Merge(resolved, globalStateTheme);
        }

        var controlType = control.GetType();
        if (_controlTypeThemes.TryGetValue(controlType, out var controlTypeTheme))
        {
            resolved = TeaThemeMerge.Merge(resolved, controlTypeTheme);
        }

        if (_controlTypeStateThemes.TryGetValue(controlType, out var typeStateMap)
            && typeStateMap.TryGetValue(state, out var controlTypeStateTheme))
        {
            resolved = TeaThemeMerge.Merge(resolved, controlTypeStateTheme);
        }

        if (_controlInstanceThemes.TryGetValue(control, out var controlInstanceTheme))
        {
            resolved = TeaThemeMerge.Merge(resolved, controlInstanceTheme);
        }

        if (_controlInstanceStateThemes.TryGetValue(control, out var instanceStateMap)
            && instanceStateMap.TryGetValue(state, out var controlInstanceStateTheme))
        {
            resolved = TeaThemeMerge.Merge(resolved, controlInstanceStateTheme);
        }

        return resolved;
    }
}
