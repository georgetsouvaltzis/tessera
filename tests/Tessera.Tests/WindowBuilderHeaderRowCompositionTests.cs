using NUnit.Framework;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class WindowBuilderHeaderRowCompositionTests
{
    [Test]
    public void WindowBuilderHeaderRowComposesConfiguredRowContent()
    {
        var output = Render(new HeaderLayoutApp(useHeaderRowApi: true), width: 56, height: 8);
        var firstLine = output.Split('\n', 2)[0];

        Assert.That(firstLine.Contains("Status", StringComparison.Ordinal), Is.True);
        Assert.That(firstLine.Contains("Actions", StringComparison.Ordinal), Is.True);
        Assert.That(firstLine.IndexOf("Status", StringComparison.Ordinal), Is.LessThan(firstLine.IndexOf("Actions", StringComparison.Ordinal)));
    }

    [Test]
    public void WindowBuilderHeaderRowRemainsCompatibleWithExistingHeaderContentBuilderUsage()
    {
        var headerRowOutput = Render(new HeaderLayoutApp(useHeaderRowApi: true), width: 56, height: 8);
        var legacyHeaderOutput = Render(new HeaderLayoutApp(useHeaderRowApi: false), width: 56, height: 8);

        Assert.That(headerRowOutput, Is.EqualTo(legacyHeaderOutput));
    }

    private static string Render(TesseraApp app, int width, int height)
    {
        _ = app.UpdateRuntime(new WindowResized(width, height));
        return app.RenderRuntime().Output.Frame.Content;
    }

    private sealed class HeaderLayoutApp(bool useHeaderRowApi) : TesseraApp
    {
        private readonly Label _status = new()
        {
            Border = BorderStyle.None,
            Text = "Status",
        };

        private readonly Label _actions = new()
        {
            Border = BorderStyle.None,
            Text = "Actions",
        };

        private readonly Label _body = new()
        {
            Border = BorderStyle.None,
            Text = "Body",
        };

        public override TesseraEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context)
        {
            return Screen.Build(window =>
            {
                if (useHeaderRowApi)
                {
                    window.HeaderRow(
                        1,
                        row =>
                        {
                            row.Gap(2);
                            row.Auto(_status);
                            row.Auto(_actions);
                        });
                }
                else
                {
                    window.Header(
                        1,
                        header => header.Row(
                            row =>
                            {
                                row.Gap(2);
                                row.Auto(_status);
                                row.Auto(_actions);
                            }));
                }

                window.Body(_body);
            });
        }
    }
}
