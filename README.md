# FLParser

> A parser for FL Studio project files in .NET.

FLParser is a utility for parsing and structuring project files in [FL Studio](https://www.image-line.com/flstudio/) (FLP files). It is used by the MONAD Demogroup for converting FLP files to a format useable by the demo replayer.

FLParser is based on [andrewrk's node-flp](https://github.com/andrewrk/node-flp), but contains many improvements to clean up code and add compatability for newer versions of FL Studio.

Please note: FLParser is currently in beta. Only the minimal things that we need are currently implemented, and some of them might break - if they do, or you find something new, please submit an issue or pull request.

## install

In commandline:

```bash
$ cd my/project/directory
$ git clone https://github.com/monadgroup/flparser.git
```

In Visual Studio:

```
Solution > Add > Existing Project
    Find Monad.FLParser.csproj
Project References > Add Reference > Projects > Monad.FLParser
```

Nuget package coming soon!

## use

Load a project with `Project.Load`:

```cs
Project project1 = Project.Load("path/to/project.flp"); // load from file

Stream someStream = ...;
Project project2 = Project.Load(someStream); // load from stream

BinaryReader someReader = ...;
Project project3 = Project.Load(someReader); // load from binaryreader
```

## reference

### classes

#### `class Project`

Represents an FLP file. Has the following properties: (all `get; set;`)

```cs
const int MaxInsertCount;   // max number of inserts (as of FL 12, equal to 105)
const int MaxTrackCount;    // max number of tracks (as of FL 12, equal to 199)
int MainVolume;             // volume of project
int MainPitch;              // pitch of project
int Ppq;                    // pulses per quarter-beat
double Tempo;               // tempo of project
string ProjectTitle;        // title of project
string VersionString;       // xx.xx.xx-formatted string of FL version
int Version;                // int format of version
List<Channel> Channels;     // list of channels in project
Track[] Tracks;             // set of tracks in project (length is equal to Project.MaxTrackCount)
List<Pattern> Patterns;     // list of patterns in project
Insert[] Inserts;           // set of inserts in project (length is equal to Project.MaxInsertCount)
```

#### `class Channel`

Represents an individual channel (instrument) in a project. Has the following properties: (all `get; set;`)

```cs
int Id;             // channel ID
string Name;        // channel name
uint Color;         // 0x00RRGGBB-formatted channel color
IChannelData Data;  // channel data, will either be GeneratorData or AutomationData
```

#### `class GeneratorData : IChannelData`

Represents channel data for a generator channel (sampler or plugin). Has the following properties: (all `get; set;`)

```cs
byte[] PluginSettings;      // settings saved by the plugin
string GeneratorName;       // plugin generator name
double Volume;              // generator volume
double Panning;             // generator panning
uint BaseNote;              // generator base note
int Insert;                 // insert to send audio to
int LayerParent;            // ???

Enums.ArpDirection ArpDir;  // direction of arp
int ArpRange;               // range of arp
int ArpChord;               // chord of arp (mapping unknown)
int ArpRepeat;              // number of times to repeat arp
double ArpTime;             // speed of arp
double ArpGate;             // arp note duration
bool ArpSlide;              // slide arp?

// for samplers:
string SampleFileName;      // path to sample
int SampleAmp;              // sample volume
bool SampleReversed;        // play sample reversed?
bool SampleReverseStereo;   // swap stereo channels?
bool SampleUseLoopPoints;   // ???
```

#### `class AutomationData : IChannelData`

Represents channel data for an automation channel. Has the following properties: (all `get; set;`)

```cs
Channel Channel;                // channel that this automation is controlling
int Parameter;                  // parameter to control
AutomationKeyframe[] Keyframes; // automation keyframes
```

#### `class AutomationKeyframe`

Represents an individual keyframe in an automation track. Has the following properties: (all `get; set;`)

```cs
int Position;       // keyframe position in pulses
double Value;       // keyframe value
float Tension;      // keyframe tension
```

#### `class Track`

Represents a track in a project playlist. Has the following properties: (all `get; set;`)

```cs
string Name;                // track name
uint Color;                 // 0x00RRGGBB-formatted track color
List<IPlaylistItem> Items;  // list of playlist items in track, will either be ChannelPlaylistItem or PatternPlaylistItem
```

#### `class IPlaylistItem`

Represents an item in a track. Has the following properties: (all `get; set;`)

```cs
int Position;       // position of the item in pulses
int Length;         // length of the item in pulses
int StartOffset;    // position to start in data
int EndOffset;      // position to end in data
```

#### `class ChannelPlaylistItem : IPlaylistItem`

Represents a sample or automation item in a track. Has the following additional properties: (all `get; set;`)

```cs

Channel Channel;    // channel to take sample or automation data from
```

#### `class PatternPlaylistItem : IPlaylistItem`

Represents a pattern in a track. Has the following additional properties: (all `get; set;`)

```cs
Pattern Pattern;    // pattern to take note data from
```

#### `class Pattern`

Represents a pattern in a project. Has the following properties: (all `get; set;`)

```cs
int Id;                                 // pattern ID
string Name;                            // pattern name
Dictionary<Channel, List<Note>> Notes;  // the notes that each channel plays in this pattern
```

#### `class Note`

Represents a note in a pattern. Has the following properties: (all `get; set;`)

```cs
int Position;       // note position in pulses
int Length;         // note length in pulses
byte Key;           // note key in MIDI format
ushort FinePitch;   // note pitch adjustment
ushort Release;     // note release time
byte Pan;           // note pan, 0 is left and 127 is right
byte Velocity;      // note velocity
```

#### `class Insert`

Represents an insert in a project. Has the following properties: (all `get; set;`)

```cs
const int MaxSlotCount;     // max number of effect slots (as of FL 12, equal to 10)
int Id;                     // insert ID
string Name;                // insert name
uint Color;                 // 0x00RRGGBB-formatted track color
ushort Icon;                // icon ID
Enums.InsertFlags Flags;    // insert flags
int Volume;                 // insert volume
int Pan;                    // insert pan
int StereoSep;              // stereo separation
int LowLevel;               // EQ lowpass level
int BandLevel;              // EQ bandpass level
int HighLevel;              // EQ highpass level
int LowFreq;                // EQ lowpass frequency
int BandFreq;               // EQ bandpass frequency
int HighFreq;               // EQ highpass frequency
int LowWidth;               // EQ lowpass width
int BandWidth;              // EQ bandpass width
int HighWidth;              // EQ highpass width
bool[] Routes;              // map of which inserts this insert routes to (length is equal to Project.MaxInsertCount)
InsertSlot[] Slots;         // set of slots in insert (length is equal to Insert.MaxSlotCount)
```

#### `class InsertSlot`

Represents an effect slot in an insert. Has the following propertie: (all `get; set;`)

```cs
int Volume;     // volume of slot
int State;      // slot state (0 = muted, 1 = enabled, 2 = solo)
```

### enums

All enums are in the `Enums` static class.

#### `enum Event`

List of FLP event types, used for parsing. Consult the source code for a list of values.

#### `enum InsertParam`

List of insert parameters, used for parsing. Consult the source code for a list of values.

#### `enum ArpDirection`

Potential directions for generator channel's arpeggiator. Values are:

 - `Off = 0` - no active arp
 - `Up = 1` - arp notes go up
 - `Down = 2` - arp notes go down
 - `UpDownBounce = 3` - arp notes bounce up and down
 - `UpDownSticky = 4` - arp notes stick up and down
 - `Random` - random arp notes

#### `[Flags] enum InsertFlags`

Flags for inserts to specify their states. Values are:

 - `ReversePolarity = 1` - reverse polarity of the insert
 - `SwapChannels = 1 << 1` - swap left and right channels
 - `Unmute = 1 << 3` - insert is audible
 - `DisableThreaded = 1 << 4` - threading is disabled on insert
 - `DockedMiddle = 1 << 6` - insert is docked to middle
 - `DockedRight = 1 << 7` - insert is docked to right
 - `Separator = 1 << 10` - insert has separator on left
 - `Lock = 1 << 11` - insert is locked
 - `Solo = 1 << 12` - insert is solo'd (all other inserts will be muted)

## license

Licensed under the MIT license. See the LICENSE file for more information.