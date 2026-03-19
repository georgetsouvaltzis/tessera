using TeaSharp;

await Tea.RunAsync(new HelloApp());

internal sealed class HelloApp : TeaApp
{
    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key && (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl)))
        {
            return TeaEffects.Quit;
        }

        return null;
    }

    public override Screen Build(ScreenContext context) =>
        Screen.From(
            $"""
             TeaSharp HelloWorld

             Press q or Ctrl+C to quit.
             Terminal: {context.Width}x{context.Height}
             Focused: {context.HasFocus}
             """);
}
