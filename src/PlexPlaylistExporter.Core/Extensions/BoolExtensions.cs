namespace PlexPlaylistExporter.Core.Extensions
{
    public static class BoolExtensions
    {
        public static int ToInt(this bool value) => value ? 1 : 0;
        public static long ToLong(this bool value) => value ? 1 : 0;
        public static short ToShort(this bool value) => value ? (short)1 : (short)0;
    }
}