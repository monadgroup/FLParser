namespace Monad.FLParser
{
    public class ChannelPlaylistItem : IPlaylistItem
    {
        public int Position { get; set; }
        public int Length { get; set; }
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }
        public Channel Channel { get; set; }
    }
}
