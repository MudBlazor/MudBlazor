namespace MudBlazor.Extensions
{
    public static class OriginExtensions
    {
        public static Origin ConvertOrigin(this Origin origin, bool rightToLeft = false)
        {
            return origin switch
            {
                Origin.TopStart => rightToLeft ? Origin.TopRight : Origin.TopLeft,
                Origin.CenterStart => rightToLeft ? Origin.CenterRight : Origin.CenterLeft,
                Origin.BottomStart => rightToLeft ? Origin.BottomRight : Origin.BottomLeft,
                Origin.TopEnd => rightToLeft ? Origin.TopLeft : Origin.TopRight,
                Origin.CenterEnd => rightToLeft ? Origin.CenterLeft : Origin.CenterRight,
                Origin.BottomEnd => rightToLeft ? Origin.BottomLeft : Origin.BottomRight,
                _ => origin
            };
        }
    }
}
