using System.Collections.Generic;
using System.IO;

namespace Monad.FLParser
{
    public class Project
    {
        public const int MaxInsertCount = 105;
        public const int MaxTrackCount = 199;

        public int MainVolume { get; set; } = 300;
        public int MainPitch { get; set; } = 0;
        public int Ppq { get; set; } = 0;
        public double Tempo { get; set; } = 140;
        public string ProjectTitle { get; set; } = "";
        public string VersionString { get; set; } = "";
        public int Version { get; set; } = 0x100;
        public List<Channel> Channels { get; set; } = new List<Channel>();
        public Track[] Tracks { get; set; } = new Track[MaxTrackCount];
        public List<Pattern> Patterns = new List<Pattern>();
        public Insert[] Inserts { get; set; } = new Insert[MaxInsertCount];

        public Project()
        {
            for (var i = 0; i < MaxTrackCount; i++)
            {
                Tracks[i] = new Track();
            }

            for (var i = 0; i < MaxInsertCount; i++)
            {
                Inserts[i] = new Insert {Id = i};
            }
        }

        public static Project Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return Load(stream);
            }
        }

        public static Project Load(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                return Load(reader);
            }
        }

        public static Project Load(BinaryReader reader)
        {
            var parser = new ProjectParser();
            return parser.Parse(reader);
        }
    }
}
