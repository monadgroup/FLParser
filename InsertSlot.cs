namespace Monad.FLParser
{
    public class InsertSlot
    {
        public int Volume { get; set; } = 100;
        public int State { get; set; } = 0;
        public int DryWet { get; set; } = -1;
        public byte[] PluginSettings { get; set; } = null;
        public Plugin Plugin { get; set; } = new Plugin();
    }
}
