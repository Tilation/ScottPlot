using ScottPlot.Axis;
using SkiaSharp;

namespace ScottPlot.Plottables;

public struct Marker
{
    public Marker(Coordinates coordinates, string label)
    {
        Coordinates = coordinates;
        Label = label;
    }

    public Marker(Coordinates coordinates, string label, FontStyle fontStyle, MarkerStyle markerStyle)
    {
        Coordinates = coordinates;
        Label = label;
        FontStyle = fontStyle;
        MarkerStyle = markerStyle;
    }

    public Coordinates Coordinates { get; set; }
    public string Label { get; set; }
    public FontStyle FontStyle { get; set; } = new FontStyle();
    public MarkerStyle MarkerStyle { get; set; } = MarkerStyle.Default;
}

public class FloatingMarkers : IPlottable
{
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = Axis.Axes.Default;
    public string Label { get; set; } = "";
    public IEnumerable<LegendItem> LegendItems => EnumerableExtensions.One<LegendItem>(
        new LegendItem
        {
            Label = Label
        });
    private readonly List<Marker> markers = new List<Marker>();
    public List<Marker> Markers => markers;


    public AxisLimits GetAxisLimits()
    {
        return AxisLimits.NoLimits;
    }

    public void SetDataSource<T>(ICollection<T> datasource, Func<T, Marker> getMarkers)
    {
        Markers.Clear();
        Markers.AddRange(datasource.Select(x => getMarkers(x)));
    }


    public void Render(SKSurface surface)
    {
        using (var paint = new SKPaint())
        {
            for (int i = 0; i < Markers.Count; i++)
            {
                var marker = Markers[i];
                var pixel = Axes.GetPixel(marker.Coordinates);
                marker.MarkerStyle.Render(surface.Canvas, pixel);
                float offset = marker.MarkerStyle.Size;
                foreach(var line in marker.Label.Split('\n'))
                {
                    marker.FontStyle.ApplyToPaint(paint);
                    surface.Canvas.DrawText(line, pixel.X, pixel.Y + offset, paint);
                    SKRect bounds = SKRect.Empty;
                    paint.MeasureText(line, ref bounds);
                    offset += bounds.Height;
                }
            }
        }
    }
}
