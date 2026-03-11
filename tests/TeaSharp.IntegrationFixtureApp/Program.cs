using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp;
using TeaSharp.TestFixtures;

var program = Tea.NewProgram(new CounterFixtureModel(), new TeaProgramOptions
{
    UseConsoleKeyEvents = false,
});

await program.RunAsync();
