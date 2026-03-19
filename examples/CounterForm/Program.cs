using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .ConfigureServices(static services => services.AddSingleton<CounterState>())
    .UseApp<CounterFormApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Counter Form",
        };
    })
    .Build();

await app.RunAsync();

internal sealed record IncrementRequested : Message;
internal sealed record StepUpdated(int Value) : Message;
internal sealed record CounterResetRequested : Message;

internal sealed class CounterFormApp : TeaApp
{
    private readonly CounterState _state;
    private readonly TextInput _stepInput = new()
    {
        Title = "Step Size",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Placeholder = "Enter an integer and press Enter",
    };

    private readonly Button _increment = new()
    {
        Text = "Increment",
        Description = "Enter to add current step",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Button _reset = new()
    {
        Text = "Reset",
        Description = "Clear count and step",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Label _summary = new()
    {
        Title = "Counter",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();
    private string _statusText = "Ready";

    public CounterFormApp(CounterState state)
    {
        _state = state;
        _stepInput.SetValue(_state.Step.ToString(CultureInfo.InvariantCulture));
        _stepInput.Submitted += (_, args) =>
        {
            var parsed = ParseStep(args.Value);
            Post(new StepUpdated(parsed));
        };

        _increment.Activated += (_, _) => Post(new IncrementRequested());
        _reset.Activated += (_, _) => Post(new CounterResetRequested());
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        switch (message)
        {
            case IncrementRequested:
                _state.Count += _state.Step;
                _statusText = $"Added {_state.Step}.";
                break;
            case StepUpdated step:
                _state.Step = step.Value;
                _statusText = $"Step set to {_state.Step}.";
                break;
            case CounterResetRequested:
                _state.Count = 0;
                _state.Step = 1;
                _stepInput.SetValue("1");
                _statusText = "Counter reset.";
                break;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _summary.Text =
            $"""
             Count: {_state.Count}
             Step: {_state.Step}
             Size: {context.Width}x{context.Height}
             """;

        _status.LeftText = "Enter updates step  Tab/Shift+Tab moves focus";
        _status.RightText = $"{_statusText}  Ctrl+C quits";

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(6, _summary);
            window.Footer(1, _status);
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(5, _stepInput);
                column.Fixed(5, _increment);
                column.Fixed(5, _reset);
            }));
        });
    }

    private static int ParseStep(string raw)
    {
        if (int.TryParse(raw.Trim(), out var value))
        {
            return Math.Clamp(value, 1, 100);
        }

        return 1;
    }
}

internal sealed class CounterState
{
    public int Count { get; set; }

    public int Step { get; set; } = 1;
}
