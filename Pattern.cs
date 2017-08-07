using System.Collections.Generic;

namespace Monad.FLParser
{
    public class Pattern
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";
        public Dictionary<Channel, List<Note>> Notes { get; set; } = new Dictionary<Channel, List<Note>>();
    }
}
