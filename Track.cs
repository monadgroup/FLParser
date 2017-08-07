using System.Collections.Generic;

namespace Monad.FLParser
{
    public class Track
    {
        public string Name { get; set; }
        public uint Color { get; set; }
        public List<IPlaylistItem> Items { get; set; } = new List<IPlaylistItem>();
    }
}
