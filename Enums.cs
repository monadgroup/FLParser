using System;

namespace Monad.FLParser
{
    public static class Enums
    {

        public enum Event
        {
            Byte = 0,
            ByteEnabled = 0,
            ByteNoteOn = 1,
            ByteVol = 2,
            BytePan = 3,
            ByteMidiChan = 4,
            ByteMidiNote = 5,
            ByteMidiPatch = 6,
            ByteMidiBank = 7,
            ByteLoopActive = 9,
            ByteShowInfo = 10,
            ByteShuffle = 11,
            ByteMainVol = 12,
            ByteStretch = 13,
            BytePitchable = 14,
            ByteZipped = 15,
            ByteDelayFlags = 16,
            BytePatLength = 17,
            ByteBlockLength = 18,
            ByteUseLoopPoints = 19,
            ByteLoopType = 20,
            ByteChanType = 21,
            ByteMixSliceNum = 22,
            ByteEffectChannelMuted = 27,

            Word = 64,
            WordNewChan = Word,
            WordNewPat = Word + 1,
            WordTempo = Word + 2,
            WordCurrentPatNum = Word + 3,
            WordPatData = Word + 4,
            WordFx = Word + 5,
            WordFadeStereo = Word + 6,
            WordCutOff = Word + 7,
            WordDotVol = Word + 8,
            WordDotPan = Word + 9,
            WordPreAmp = Word + 10,
            WordDecay = Word + 11,
            WordAttack = Word + 12,
            WordDotNote = Word + 13,
            WordDotPitch = Word + 14,
            WordDotMix = Word + 15,
            WordMainPitch = Word + 16,
            WordRandChan = Word + 17,
            WordMixChan = Word + 18,
            WordResonance = Word + 19,
            WordLoopBar = Word + 20,
            WordStDel = Word + 21,
            WordFx3 = Word + 22,
            WordDotReso = Word + 23,
            WordDotCutOff = Word + 24,
            WordShiftDelay = Word + 25,
            WordLoopEndBar = Word + 26,
            WordDot = Word + 27,
            WordDotShift = Word + 28,
            WordLayerChans = Word + 30,
            WordInsertIcon = Word + 31,
            WordCurrentSlotNum = Word + 34,

            Int = 128,
            DWordColor = Int,
            DWordPlayListItem = Int + 1,
            DWordEcho = Int + 2,
            DWordFxSine = Int + 3,
            DWordCutCutBy = Int + 4,
            DWordWindowH = Int + 5,
            DWordMiddleNote = Int + 7,
            DWordReserved = Int + 8,
            DWordMainResoCutOff = Int + 9,
            DWordDelayReso = Int + 10,
            DWordReverb = Int + 11,
            DWordIntStretch = Int + 12,
            DWordSsNote = Int + 13,
            DWordFineTune = Int + 14,
            DWordInsertColor = Int + 21,
            DWordFineTempo = Int + 28,

            Undef = 192,
            Text = Undef,
            TextChanName = Text,
            TextPatName = Text + 1,
            TextTitle = Text + 2,
            TextComment = Text + 3,
            TextSampleFileName = Text + 4,
            TextUrl = Text + 5,
            TextCommentRtf = Text + 6,
            TextVersion = Text + 7,
            TextPluginName = Text + 9,
            TextInsertName = Text + 12,
            TextMidiCtrls = Text + 16,
            TextDelay = Text + 17,

            Data = 210,
            DataTs404Params = Data,
            DataDelayLine = Data + 1,
            DataNewPlugin = Data + 2,
            DataPluginParams = Data + 3,
            DataChanParams = Data + 5,
            DataEnvLfoParams = Data + 8,
            DataBasicChanParams = Data + 9,
            DataOldFilterParams = Data + 10,
            DataOldAutomationData = Data + 13,
            DataPatternNotes = Data + 14,
            DataInsertParams = Data + 15,
            DataAutomationChannels = Data + 17,
            DataChanGroupName = Data + 21,
            DataPlayListItems = Data + 23,
            DataAutomationData = Data + 24,
            DataInsertRoutes = Data + 25,
            DataInsertFlags = Data + 26,
            DataSaveTimestamp = Data + 27
        }

        /*public enum FilterType
        {
            LowPass = 0,
            HiPass = 1,
            BandPassCsg = 2,
            BandPassCzpg = 3,
            Notch = 4,
            AllPass = 5,
            Moog = 6,
            DoubleLowPass = 7,
            LowPassRc12 = 8,
            BandPassRc12 = 9,
            HighPassRc12 = 10,
            LowPassRc24 = 11,
            BandPassRc24 = 12,
            HighPassRc24 = 13,
            FormantFilter = 14
        }*/

        public enum ArpDirection
        {
            Off = 0,
            Up = 1,
            Down = 2,
            UpDownBounce = 3,
            UpDownSticky = 4,
            Random = 5
        }

        /*public enum EnvelopeTarget
        {
            Volume = 0,
            Cut = 1,
            Resonance = 2,
            NumTargets = 3
        }*/

        /*public enum PluginChunkId
        {
            Midi = 1,
            Flags = 2,
            Io = 30,
            InputInfo = 31,
            OutputInfo = 32,
            PluginInfo = 50,
            VstPlugin = 51,
            Guid = 52,
            State = 53,
            Name = 54,
            Filename = 55,
            VendorName = 56
        }*/

        public enum InsertParam
        {
            SlotState = 0x00,
            SlotVolume = 0x01,
            Volume = 0xC0,
            Pan = 0xC1,
            StereoSep = 0xC2,
            LowLevel = 0xD0,
            BandLevel = 0xD1,
            HighLevel = 0xD2,
            LowFreq = 0xD8,
            BandFreq = 0xD9,
            HighFreq = 0xDA,
            LowWidth = 0xE0,
            BandWidth = 0xE1,
            HighWidth = 0xE2,
            Unknown1 = 0xA4,
            Unknown2 = 0xA5,
            Unknown3 = 0x06,
            Unknown4 = 0x07,
            Unknown5 = 0x08
        }

        [Flags]
        public enum InsertFlags
        {
            ReversePolarity = 1,
            SwapChannels = 1 << 1,
            Unknown3 = 1 << 2,
            Unmute = 1 << 3,
            DisableThreaded = 1 << 4,
            Unknown6 = 1 << 5,
            DockedMiddle = 1 << 6,
            DockedRight = 1 << 7,
            Unknown9 = 1 << 8,
            Unknown10 = 1 << 9,
            Separator = 1 << 10,
            Lock = 1 << 11,
            Solo = 1 << 12,
            Unknown14 = 1 << 13,
            Unknown15 = 1 << 14,
            Unknown16 = 1 << 15
        }
    }
}
