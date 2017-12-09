namespace Monad.FLParser
{
    public class Insert
    {
        public const int MaxSlotCount = 10;

        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";
        public uint Color { get; set; } = 0x000000;
        public ushort Icon { get; set; } = 0;
        public Enums.InsertFlags Flags { get; set; } = 0;
        public int Volume { get; set; } = 100;
        public int Pan { get; set; } = 0;
        public int StereoSep { get; set; } = 0;
        public int LowLevel { get; set; } = 0;
        public int BandLevel { get; set; } = 0;
        public int HighLevel { get; set; } = 0;
        public int LowFreq { get; set; } = 0;
        public int BandFreq { get; set; } = 0;
        public int HighFreq { get; set; } = 0;
        public int LowWidth { get; set; } = 0;
        public int BandWidth { get; set; } = 0;
        public int HighWidth { get; set; } = 0;
        public bool[] Routes { get; set; } = new bool[Project.MaxInsertCount];
        public int[] RouteVolumes { get; set; } = new int[Project.MaxInsertCount];
        public InsertSlot[] Slots { get; set; } = new InsertSlot[MaxSlotCount];

        public Insert()
        {
            for (var i = 0; i < MaxSlotCount; i++)
            {
                Slots[i] = new InsertSlot();
            }

            for (var i = 0; i < Project.MaxInsertCount; i++)
            {
                RouteVolumes[i] = 12800;
            }
        }
    }
}
