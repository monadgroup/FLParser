namespace Monad.FLParser
{
    public interface IPlaylistItem
    {
        int Position { get; set; }
        int Length { get; set; }
        int StartOffset { get; set; }
        int EndOffset { get; set; }
    }
}
