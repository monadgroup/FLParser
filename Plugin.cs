namespace Monad.FLParser
{
    public class Plugin
    {
        public int MidiInPort { get; set; }
        public int MidiOutPort { get; set; }
        public byte PitchBendRange { get; set; }

        public uint Flags { get; set; }

        public int NumInputs { get; set; }
        public int NumOutputs { get; set; }

        public PluginIoInfo[] InputInfo { get; set; }
        public PluginIoInfo[] OutputInfo { get; set; }

        public int InfoKind { get; set; }

        public uint VstNumber { get; set; }
        public string VstId { get; set; }

        public byte[] Guid { get; set; }

        public byte[] State { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string VendorName { get; set; }
    }
}
