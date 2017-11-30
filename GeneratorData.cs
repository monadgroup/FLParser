namespace Monad.FLParser
{
    public class GeneratorData : IChannelData
    {
        public byte[] PluginSettings { get; set; } = null;
        public Plugin Plugin { get; set; } = new Plugin();
        public string GeneratorName { get; set; } = "";
        public double Volume { get; set; } = 100;
        public double Panning { get; set; } = 0;
        public uint BaseNote { get; set; } = 57;
        public int Insert { get; set; } = -1;
        public int LayerParent { get; set; } = -1;

        public string SampleFileName { get; set; } = "";
        public int SampleAmp { get; set; } = 100;
        public bool SampleReversed { get; set; } = false;
        public bool SampleReverseStereo { get; set; } = false;
        public bool SampleUseLoopPoints { get; set; } = false;

        public Enums.ArpDirection ArpDir { get; set; } = Enums.ArpDirection.Off;
        public int ArpRange { get; set; } = 0;
        public int ArpChord { get; set; } = 0;
        public int ArpRepeat { get; set; } = 0;
        public double ArpTime { get; set; } = 100;
        public double ArpGate { get; set; } = 100;
        public bool ArpSlide { get; set; } = false;
    }
}
