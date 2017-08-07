namespace Monad.FLParser
{
    public class PatternPlaylistItem : IPlaylistItem
    {
        public int Position { get; set; }
        public int Length { get; set; }
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }
        public Pattern Pattern { get; set; }
    }
}
