namespace Monad.FLParser
{
    public class Note
    {
        public int Position { get; set; }
        public byte Color { get; set; }
        public int Length { get; set; }
        public byte Key { get; set; }
        public ushort FinePitch { get; set; }
        public ushort Release { get; set; }
        public byte Pan { get; set; }
        public byte Velocity { get; set; }
    }
}
