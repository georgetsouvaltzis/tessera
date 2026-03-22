using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using TeaSharp.Controls;

namespace TeaSharp.Styles;

/// <summary>
/// Provides helpers for applying a resolved <see cref="TeaTheme" /> across multiple controls.
/// </summary>
/// <remarks>
/// This helper keeps existing per-control <c>ApplyTheme</c> extension methods as the source of truth and
/// dispatches to them by runtime control type.
/// </remarks>
public static class ThemeScope
{
    private static readonly ConcurrentDictionary<Type, Action<Control, TeaTheme>?> ApplyThemeDispatchCache = new();

    /// <summary>
    /// Applies <paramref name="theme" /> across the provided controls.
    /// </summary>
    /// <param name="theme">Resolved theme to apply.</param>
    /// <param name="controls">Controls to update.</param>
    /// <returns>The number of controls that were successfully themed.</returns>
    public static int Apply(TeaTheme theme, params Control[] controls)
    {
        ArgumentNullException.ThrowIfNull(controls);
        return Apply(theme, (IEnumerable<Control?>)controls);
    }

    /// <summary>
    /// Applies <paramref name="theme" /> across a control sequence.
    /// </summary>
    /// <param name="theme">Resolved theme to apply.</param>
    /// <param name="controls">Controls to update. <see langword="null" /> elements are ignored.</param>
    /// <returns>The number of controls that were successfully themed.</returns>
    public static int Apply(TeaTheme theme, IEnumerable<Control?> controls)
    {
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(controls);

        var applied = 0;
        foreach (var control in controls)
        {
            if (control is null)
            {
                continue;
            }

            var dispatcher = ApplyThemeDispatchCache.GetOrAdd(control.GetType(), static type => BuildApplyThemeDispatcher(type));
            if (dispatcher is null)
            {
                continue;
            }

            dispatcher(control, theme);
            applied++;
        }

        return applied;
    }

    private static Action<Control, TeaTheme>? BuildApplyThemeDispatcher(Type controlType)
    {
        var applyTheme = ResolveApplyThemeMethod(controlType);
        if (applyTheme is null)
        {
            return null;
        }

        if (applyTheme.IsGenericMethodDefinition)
        {
            if (!controlType.IsGenericType)
            {
                return null;
            }

            applyTheme = applyTheme.MakeGenericMethod(controlType.GetGenericArguments());
        }

        var extensionTarget = applyTheme.GetParameters()[0].ParameterType;
        var control = Expression.Parameter(typeof(Control), "control");
        var theme = Expression.Parameter(typeof(TeaTheme), "theme");
        var body = Expression.Call(
            applyTheme,
            Expression.Convert(control, extensionTarget),
            theme);
        return Expression.Lambda<Action<Control, TeaTheme>>(body, control, theme).Compile();
    }

    private static MethodInfo? ResolveApplyThemeMethod(Type controlType)
    {
        var methods = typeof(TeaThemeControlExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);
        MethodInfo? assignableMatch = null;

        for (var index = 0; index < methods.Length; index++)
        {
            var method = methods[index];
            if (!string.Equals(method.Name, nameof(TeaThemeControlExtensions.ApplyTheme), StringComparison.Ordinal))
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 2 || parameters[1].ParameterType != typeof(TeaTheme))
            {
                continue;
            }

            var extensionTarget = parameters[0].ParameterType;
            if (extensionTarget == controlType)
            {
                return method;
            }

            if (method.IsGenericMethodDefinition
                && extensionTarget.IsGenericType
                && controlType.IsGenericType
                && extensionTarget.GetGenericTypeDefinition() == controlType.GetGenericTypeDefinition()
                && method.GetGenericArguments().Length == controlType.GetGenericArguments().Length)
            {
                return method;
            }

            if (!extensionTarget.ContainsGenericParameters && extensionTarget.IsAssignableFrom(controlType))
            {
                assignableMatch ??= method;
            }
        }

        return assignableMatch;
    }
}
