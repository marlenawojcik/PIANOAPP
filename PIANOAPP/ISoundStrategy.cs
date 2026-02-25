using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIANOAPP
{
    public interface ISoundStrategy
    {
        void PlayTone(MusicElement element,MidiOut mididOut);
        void StartTone(Note note,MidiOut midiOut);
        void StopTone(Note note, MidiOut midiOut);
        void SetInstrument(MidiOut midiOut);
    }
}
