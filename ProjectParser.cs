using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Monad.FLParser
{
    internal class ProjectParser
    {
        private readonly Project _project = new Project();
        private Pattern _curPattern;
        private Channel _curChannel;
        private Insert _curInsert;
        private InsertSlot _curSlot;
        private readonly bool _verbose;
        private int _versionMajor;

        public ProjectParser(bool verbose)
        {
            _verbose = verbose;
        }

        public Project Parse(BinaryReader reader)
        {
            _curInsert = _project.Inserts[0];

            ParseHeader(reader);
            ParseFldt(reader);

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                ParseEvent(reader);
            }

            return _project;
        }

        private void ParseHeader(BinaryReader reader)
        {
            if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "FLhd")
                throw new FlParseException("Invalid magic number", reader.BaseStream.Position);

            // header + type
            var headerLength = reader.ReadInt32();
            if (headerLength != 6)
                throw new FlParseException($"Expected header length 6, not {headerLength}", reader.BaseStream.Position);

            var type = reader.ReadInt16();
            if (type != 0) throw new FlParseException($"Type {type} is not supported", reader.BaseStream.Position);

            // channels
            var channelCount = reader.ReadInt16();
            if (channelCount < 1 || channelCount > 1000)
                throw new FlParseException($"Invalid number of channels: {channelCount}", reader.BaseStream.Position);
            for (var i = 0; i < channelCount; i++)
            {
                _project.Channels.Add(new Channel { Id = i, Data = new GeneratorData() });
            }

            // ppq
            _project.Ppq = reader.ReadInt16();
            if (_project.Ppq < 0) throw new Exception($"Invalid PPQ: {_project.Ppq}");
        }

        private void ParseFldt(BinaryReader reader)
        {
            string id;
            var len = 0;

            do
            {
                reader.ReadBytes(len);

                id = Encoding.ASCII.GetString(reader.ReadBytes(4));
                len = reader.ReadInt32();

                // sanity check
                //if (len < 0 || len > 0x10000000)
                //    throw new FlParseException($"Invalid chunk length: {len}", reader.BaseStream.Position);

            } while (id != "FLdt");
        }

        private void ParseEvent(BinaryReader reader)
        {
            var startPos = reader.BaseStream.Position;
            var eventId = (Enums.Event)reader.ReadByte();
            Output($"{eventId} ({(int)eventId:X2}) at {startPos:X} ");

            if (eventId < Enums.Event.Word) ParseByteEvent(eventId, reader);
            else if (eventId < Enums.Event.Int) ParseWordEvent(eventId, reader);
            else if (eventId < Enums.Event.Text) ParseDwordEvent(eventId, reader);
            else if (eventId < Enums.Event.Data) ParseTextEvent(eventId, reader);
            else ParseDataEvent(eventId, reader);
        }

        private void ParseByteEvent(Enums.Event eventId, BinaryReader reader)
        {
            var data = reader.ReadByte();
            if (_verbose) OutputLine($"byte: {data:X2}");

            var genData = _curChannel?.Data as GeneratorData;

            switch (eventId)
            {
                case Enums.Event.ByteMainVol:
                    _project.MainVolume = data;
                    break;
                case Enums.Event.ByteUseLoopPoints:
                    if (genData != null) genData.SampleUseLoopPoints = true;
                    break;
                case Enums.Event.ByteMixSliceNum:
                    if (genData != null) genData.Insert = data;
                    break;
                case Enums.Event.BytePlayTruncatedNotes:
                    _project.PlayTruncatedNotes = Convert.ToBoolean(data);
                    break;
            }
        }

        private void ParseWordEvent(Enums.Event eventId, BinaryReader reader)
        {
            var data = reader.ReadUInt16();
            OutputLine($"word: {data:X4}");

            var genData = _curChannel?.Data as GeneratorData;

            switch (eventId)
            {
                case Enums.Event.WordNewChan:
                    _curChannel = _project.Channels[data];
                    break;
                case Enums.Event.WordNewPat:
                    while (_project.Patterns.Count < data) _project.Patterns.Add(new Pattern { Id = _project.Patterns.Count });
                    _curPattern = _project.Patterns[data - 1];
                    break;
                case Enums.Event.WordTempo:
                    _project.Tempo = data;
                    break;
                case Enums.Event.WordFadeStereo:
                    if (genData == null) break;
                    if ((data & 0x02) != 0) genData.SampleReversed = true;
                    else if ((data & 0x100) != 0) genData.SampleReverseStereo = true;
                    break;
                case Enums.Event.WordPreAmp:
                    if (genData == null) break;
                    genData.SampleAmp = data;
                    break;
                case Enums.Event.WordMainPitch:
                    _project.MainPitch = data;
                    break;
                case Enums.Event.WordInsertIcon:
                    _curInsert.Icon = data;
                    break;
                case Enums.Event.WordCurrentSlotNum:
                    if (_curSlot != null) // Current slot after plugin event, now re-arranged.
                    {
                        _curInsert.Slots[data] = _curSlot;
                        _curSlot = new InsertSlot();
                    }
                    _curChannel = null;
                    break;
            }
        }

        private void ParseDwordEvent(Enums.Event eventId, BinaryReader reader)
        {
            var data = reader.ReadUInt32();
            OutputLine($"int: {data:X8}");

            switch (eventId)
            {
                case Enums.Event.DWordColor:
                    if (_curChannel != null) _curChannel.Color = data;
                    break;
                case Enums.Event.DWordMiddleNote:
                    if (_curChannel != null && _curChannel.Data is GeneratorData genData) genData.BaseNote = data + 9;
                    break;
                case Enums.Event.DWordInsertColor:
                    _curInsert.Color = data;
                    break;
                case Enums.Event.DWordFineTempo:
                    _project.Tempo = data / 1000.0;
                    break;
            }
        }

        private static int GetBufferLen(BinaryReader reader)
        {
            var data = reader.ReadByte();
            var dataLen = data & 0x7F;
            var shift = 0;
            while ((data & 0x80) != 0)
            {
                data = reader.ReadByte();
                dataLen = dataLen | ((data & 0x7F) << (shift += 7));
            }
            return dataLen;
        }

        private void ParseTextEvent(Enums.Event eventId, BinaryReader reader)
        {
            var dataLen = GetBufferLen(reader);
            var dataBytes = reader.ReadBytes(dataLen);
            var unicodeString = Encoding.Unicode.GetString(dataBytes);
            if (unicodeString.EndsWith("\0")) unicodeString = unicodeString.Substring(0, unicodeString.Length - 1);

            OutputLine($"text: {unicodeString}");

            var genData = _curChannel?.Data as GeneratorData;

            switch (eventId)
            {
                case Enums.Event.TextChanName:
                    if (_curChannel != null) _curChannel.Name = unicodeString;
                    break;
                case Enums.Event.TextPatName:
                    if (_curPattern != null) _curPattern.Name = unicodeString;
                    break;
                case Enums.Event.TextTitle:
                    _project.ProjectTitle = unicodeString;
                    break;
                case Enums.Event.TextAuthor:
                    _project.Author = unicodeString;
                    break;
                case Enums.Event.TextComment:
                    _project.Comment = unicodeString;
                    break;
                case Enums.Event.TextGenre:
                    _project.Genre = unicodeString;
                    break;
                case Enums.Event.TextSampleFileName:
                    if (genData == null) break;
                    genData.SampleFileName = unicodeString;
                    genData.GeneratorName = "Sampler";
                    break;
                case Enums.Event.TextVersion:
                    _project.VersionString = Encoding.UTF8.GetString(dataBytes);
                    if (_project.VersionString.EndsWith("\0")) _project.VersionString = _project.VersionString.Substring(0, _project.VersionString.Length - 1);
                    var numbers = _project.VersionString.Split('.');
                    _versionMajor = int.Parse(numbers[0]);
                    _project.Version = (int.Parse(numbers[0]) << 8) +
                                       (int.Parse(numbers[1]) << 4) +
                                       (int.Parse(numbers[2]) << 0);

                    var trackCount = _versionMajor <= 12 ? 199 : 499;
                    if(_project.Tracks.Length != trackCount)
                        _project.InitTracks(trackCount);
                    break;
                case Enums.Event.GeneratorName:
                    if (genData != null) genData.GeneratorName = unicodeString;
                    break;
                case Enums.Event.TextInsertName:
                    _curInsert.Name = unicodeString;
                    break;
                case Enums.Event.TextDelay:
                  if(genData != null)
                  {
                    genData.Echo = dataBytes[12];
                    genData.EchoFeed = (uint)((dataBytes[1] << 8) | dataBytes[0]);
                    genData.EchoTime = (uint)((dataBytes[17] << 8) | dataBytes[16]);
                  }
                  break;
            }
        }

        private void ParseDataEvent(Enums.Event eventId, BinaryReader reader)
        {
            var dataLen = GetBufferLen(reader);
            var dataStart = reader.BaseStream.Position;
            var dataEnd = dataStart + dataLen;

            var genData = _curChannel?.Data as GeneratorData;
            var autData = _curChannel?.Data as AutomationData;
            var slotData = _curSlot;

            switch (eventId)
            {
                case Enums.Event.DataPluginParams:
                    if (slotData != null)
                    {
                        OutputLine($"Found plugin settings for insert (id {_curInsert.Id})");
                        _curSlot.PluginSettings = reader.ReadBytes(dataLen);
                        _curSlot.Plugin = ParsePluginChunk(slotData.PluginSettings);
                    }
                    else
                    {
                        if (genData == null) break;
                        if (genData.PluginSettings != null)
                            throw new Exception("Attempted to overwrite plugin");

                        OutputLine($"Found plugin settings for {genData.GeneratorName} (id {_curChannel.Id})");
                        genData.PluginSettings = reader.ReadBytes(dataLen);
                        genData.Plugin = ParsePluginChunk(genData.PluginSettings);
                    }
                    break;
                case Enums.Event.DataChanParams:
                    {
                        if (genData == null) break;
                        //Console.WriteLine(reader.BaseStream.Position);
                        var unknown1 = reader.ReadBytes(40);
                        genData.ArpDir = (Enums.ArpDirection)reader.ReadInt32();
                        genData.ArpRange = reader.ReadInt32();
                        genData.ArpChord = reader.ReadInt32();
                        genData.ArpTime = reader.ReadInt32() + 1;
                        genData.ArpGate = reader.ReadInt32();
                        genData.ArpSlide = reader.ReadBoolean();
                        var unknown2 = reader.ReadBytes(31);
                        genData.ArpRepeat = reader.ReadInt32();
                        var unknown3 = reader.ReadBytes(29);
                    }
                    break;
                case Enums.Event.DataBasicChanParams:
                    if (genData == null) break;
                    genData.Panning = reader.ReadInt32();
                    genData.Volume = reader.ReadInt32();
                    break;
                case Enums.Event.DataPatternNotes:
                    while (reader.BaseStream.Position < dataEnd)
                    {
                        var pos = reader.ReadInt32();
                        var unknown1 = reader.ReadInt16();
                        var ch = reader.ReadByte();
                        var unknown2 = reader.ReadByte();
                        var length = reader.ReadInt32();
                        var key = reader.ReadByte();
                        var unknown3 = reader.ReadInt16();
                        var unknown4 = reader.ReadByte();
                        var finePitch = reader.ReadUInt16();
                        var release = reader.ReadByte(); //Its actually a byte, next byte after is note channel
                        var color = reader.ReadByte(); //Used for map note color to midi channel
                        var pan = reader.ReadByte();
                        var velocity = reader.ReadByte();
                        var x1 = reader.ReadByte();
                        var x2 = reader.ReadByte();

                        var channel = _project.Channels[ch];
                        if (!_curPattern.Notes.ContainsKey(channel)) _curPattern.Notes.Add(channel, new List<Note>());
                        _curPattern.Notes[channel].Add(new Note
                        {
                            Position = pos,
                            Color = color,
                            Length = length,
                            Key = key,
                            FinePitch = finePitch,
                            Release = release,
                            Pan = pan,
                            Velocity = velocity
                        });
                    }
                    break;
                case Enums.Event.DataInsertParams:
                    while (reader.BaseStream.Position < dataEnd)
                    {
                        var startPos = reader.BaseStream.Position;
                        var unknown1 = reader.ReadInt32();
                        var messageId = (Enums.InsertParam)reader.ReadByte();
                        var unknown2 = reader.ReadByte();
                        var channelData = reader.ReadUInt16();
                        var messageData = reader.ReadInt32();

                        var slotId = channelData & 0x3F;
                        var insertId = (channelData >> 6) & 0x7F;
                        var insertType = channelData >> 13;

                        var insert = _project.Inserts[insertId];

                        switch (messageId)
                        {
                            case Enums.InsertParam.SlotState:
                                insert.Slots[slotId].State = messageData;
                                break;
                            case Enums.InsertParam.SlotVolume:
                                insert.Slots[slotId].Volume = messageData;
                                break;
                            case Enums.InsertParam.Volume:
                                insert.Volume = messageData;
                                break;
                            case Enums.InsertParam.Pan:
                                insert.Pan = messageData;
                                break;
                            case Enums.InsertParam.StereoSep:
                                insert.StereoSep = messageData;
                                break;
                            case Enums.InsertParam.LowLevel:
                                insert.LowLevel = messageData;
                                break;
                            case Enums.InsertParam.BandLevel:
                                insert.BandLevel = messageData;
                                break;
                            case Enums.InsertParam.HighLevel:
                                insert.HighLevel = messageData;
                                break;
                            case Enums.InsertParam.LowFreq:
                                insert.LowFreq = messageData;
                                break;
                            case Enums.InsertParam.BandFreq:
                                insert.BandFreq = messageData;
                                break;
                            case Enums.InsertParam.HighFreq:
                                insert.HighFreq = messageData;
                                break;
                            case Enums.InsertParam.LowWidth:
                                insert.LowWidth = messageData;
                                break;
                            case Enums.InsertParam.BandWidth:
                                insert.BandWidth = messageData;
                                break;
                            case Enums.InsertParam.HighWidth:
                                insert.HighWidth = messageData;
                                break;
                            default:
                                if ((int)messageId >= 64 && (int)messageId <= 64 + 104)  // any value 64 or above appears to be the desination insert
                                {
                                    var insertDest = (int)messageId - 64;
                                    insert.RouteVolumes[insertDest] = messageData;
                                    OutputLine($"{startPos:X4} insert send from {insertId} to {insertDest} volume: {messageData:X8}");
                                }
                                else
                                {
                                    OutputLine($"{startPos:X4} insert param: {messageId} {insertId}-{slotId}, data: {messageData:X8}");
                                }
                                break;
                        }
                    }
                    break;
                case Enums.Event.DataAutomationChannels:
                    while (reader.BaseStream.Position < dataEnd)
                    {
                        var unknown1 = reader.ReadUInt16();
                        var automationChannel = reader.ReadByte();
                        var unknown2 = reader.ReadUInt32();
                        var unknown3 = reader.ReadByte();
                        var param = reader.ReadUInt16();
                        var paramDestination = reader.ReadUInt16();
                        var unknown4 = reader.ReadUInt64();

                        var channel = _project.Channels[automationChannel];

                        if(paramDestination > _project.Channels.Count)
                        {
                          //Console.WriteLine("Tempo Automation Track found, but is currently not supported. Please use SAFC to merge the tempo back");
                          //break;
                          channel.Data = new AutomationData // automation on insert slot
                          {
                            Parameter = param & 0x7fff
                            //InsertId = (paramDestination & 0x0FF0) >> 6,  // seems to be out by one
                            //SlotId = paramDestination & 0x003F
                          };
                        }

                        if ((paramDestination & 0x2000) == 0 && paramDestination < _project.Channels.Count)  // Automation on channel
                        {
                            channel.Data = new AutomationData
                            {
                                Channel = _project.Channels[paramDestination],
                                Parameter = param & 0x7fff,
                                VstParameter = (param & 0x8000) > 0 ? true : false      // switch determines if automation is on channel or vst
                            };
                        }
                        else
                        {
                            channel.Data = new AutomationData // automation on insert slot
                            {
                                Parameter = param & 0x7fff,
                                InsertId = (paramDestination & 0x0FF0) >> 6,  // seems to be out by one
                                SlotId = paramDestination & 0x003F
                            };
                        }
                    }
                    break;
                case Enums.Event.DataPlayListItems:
                    while (reader.BaseStream.Position < dataEnd)
                    {
                        var startTime = reader.ReadInt32();
                        var patternBase = reader.ReadUInt16();
                        var patternId = reader.ReadUInt16();
                        var length = reader.ReadInt32();
                        var track = reader.ReadInt32();
                        if (_versionMajor == 20)
                            track = 499 - track;
                        else
                            track = 198 - track;
                        var unknown1 = reader.ReadUInt16();
                        var itemFlags = reader.ReadUInt16();
                        var unknown3 = reader.ReadUInt32();
                        bool muted = (itemFlags & 0x2000) > 0 ? true : false;   // flag determines if item is muted

                        // id of 0-patternBase is samples or automation, after is pattern
                        if (patternId <= patternBase)
                        {
                            var startOffset = (int)(reader.ReadSingle() * (float)_project.Ppq);
                            var endOffset = (int)(reader.ReadSingle() * (float)_project.Ppq);

                            _project.Tracks[track].Items.Add(new ChannelPlaylistItem
                            {
                                Position = startTime,
                                Length = length,
                                StartOffset = startOffset,
                                EndOffset = endOffset,
                                Channel = _project.Channels[patternId],
                                Muted = muted
                            });
                        }
                        else
                        {
                            var startOffset = reader.ReadInt32();
                            var endOffset = reader.ReadInt32();

                            if((patternId - patternBase - 1) > _project.Patterns.Count)
                            {
                              Console.WriteLine("Pattern found on track " + (track) + ", but it seems to be empty. Skipping...");
                              break;
                            }

                            try
                            {
                              _project.Tracks[track].Items.Add(new PatternPlaylistItem
                              {
                                Position = startTime,
                                Length = length,
                                StartOffset = startOffset,
                                EndOffset = endOffset,
                                Pattern = _project.Patterns[patternId - patternBase - 1],
                                Muted = muted
                              });
                            } catch(Exception e)
                            {
                              Console.WriteLine("Pattern found on track " + (track) + ", but it seems to be empty. Skipping...");
                              break;
                            }
                        }
                    }
                    break;
                case Enums.Event.DataAutomationData:
                    {
                        var unknown1 = reader.ReadUInt32(); // always 1?
                        var unknown2 = reader.ReadUInt32(); // always 64?
                        var unknown3 = reader.ReadByte();
                        var unknown4 = reader.ReadUInt16();
                        var unknown5 = reader.ReadUInt16(); // always 0?
                        var unknown6 = reader.ReadUInt32();
                        var keyCount = reader.ReadUInt32();

                        if (autData == null) break;
                        autData.Keyframes = new AutomationKeyframe[keyCount];

                        for (var i = 0; i < keyCount; i++)
                        {
                            reader.ReadBytes(3);
                            var startPos = reader.BaseStream.Position;

                            var keyPos = reader.ReadUInt32(); //This is a UInt32
                            reader.ReadUInt32(); //Skip 4 bytes
                            var keyVal = reader.ReadUInt32(); //This is a UInt32
                            reader.ReadByte(); //Skip a byte, since it's useless
                            var keyTension = reader.ReadSingle(); // Tension is a single
                            var mode = reader.ReadByte(); // Seems to be the mode of the keyframe

                            var endPos = reader.BaseStream.Position;
                            reader.BaseStream.Position = startPos;
                            var byteData = reader.ReadBytes((int)(endPos - startPos));
                            OutputLine($"Key {i} data: {string.Join(" ", byteData.Select(x => x.ToString("X2")))}");

                            autData.Keyframes[i] = new AutomationKeyframe
                            {
                                Position = (int)(keyPos * _project.Ppq),
                                Tension = keyTension,
                                Value = keyVal,
                                Mode = mode
                            };
                            reader.ReadBytes(3);
                        }

                        // remaining data is unknown
                    }
                    break;
                case Enums.Event.DataInsertRoutes:
                    for (var i = 0; i < Project.MaxInsertCount; i++)
                    {
                        _curInsert.Routes[i] = reader.ReadBoolean();
                    }

                    var newIndex = _curInsert.Id + 1;
                    if (newIndex < _project.Inserts.Length) _curInsert = _project.Inserts[newIndex];

                    break;
                case Enums.Event.DataInsertFlags:
                    reader.ReadUInt32();
                    var flags = (Enums.InsertFlags)reader.ReadUInt32();
                    _curInsert.Flags = flags;
                    _curSlot = new InsertSlot();  // New insert route, create new slot
                    break;
            }

            // make sure cursor is at end of data
            reader.BaseStream.Position = dataEnd;
        }

        private Plugin ParsePluginChunk(byte[] chunk)
        {
            var plugin = new Plugin();

            using (var reader = new BinaryReader(new MemoryStream(chunk)))
            {
                var pluginType = (Enums.PluginType)reader.ReadInt32();

                if (pluginType != Enums.PluginType.Vst)
                {
                  return null;
                }

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var eventId = (Enums.PluginChunkId)reader.ReadInt32();
                    var length = (int)reader.ReadInt64();

                    switch (eventId)
                    {
                        case Enums.PluginChunkId.VendorName:
                            plugin.VendorName = Encoding.ASCII.GetString(reader.ReadBytes(length));
                            break;
                        case Enums.PluginChunkId.Filename:
                            plugin.FileName = Encoding.ASCII.GetString(reader.ReadBytes(length));
                            break;
                        case Enums.PluginChunkId.Name:
                            plugin.Name = Encoding.ASCII.GetString(reader.ReadBytes(length));
                            break;
                        case Enums.PluginChunkId.State:
                            plugin.State = reader.ReadBytes(length);
                            break;
                        default:
                            OutputLine($"Event {eventId}, data: {string.Join(" ", reader.ReadBytes(length).Select(x => x.ToString("X2")))}");
                            break;
                    }
                }

                //Console.WriteLine(reader.BaseStream.Position);
                return plugin;
            }
        }

        private void Output(string value)
        {
            if (_verbose)
                Console.Write(value);
        }

        private void OutputLine(string value)
        {
            if (_verbose)
                Console.WriteLine(value);
        }
    }
}
