using Tessera.Layout;

namespace Tessera.Examples.TransitBoard;

internal sealed partial class TransitBoardApp
{
    private void ConfigureHeader(WindowBuilder window, ScreenContext context)
    {
        var headerHeight = context.Height < 28 ? 4 : 5;
        var stripHeight = context.Height < 28 ? 4 : 5;

        window.Header(
            headerHeight + stripHeight,
            header => header.Column(column =>
            {
                column.Fixed(headerHeight, _hero);
                column.Fill(filters => filters.Column(stack =>
                {
                    stack.Fixed(2, top => top.Row(row =>
                    {
                        row.Weighted(2, _modeStrip);
                        row.Weighted(1, _themeStrip);
                    }));
                    stack.Fill(_routeStrip);
                }));
            }));
    }

    private void ConfigureBody(ContentBuilder body, ScreenContext context)
    {
        var bottomHeight = context.Height < 28 ? 5 : 7;

        body.Column(column =>
        {
            column.Fill(_board);
            column.Fixed(bottomHeight, bottom => bottom.Row(row =>
            {
                row.Weighted(3, _notices);
                row.Weighted(2, _journey);
            }));
        });
    }
}
