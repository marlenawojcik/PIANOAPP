using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIANOAPP
{
    public static class MidiManager
    {
       private static MidiOut _midiOut;

        public static MidiOut GetMidiOut()
        {
            if (_midiOut == null)
            {
                _midiOut = new MidiOut(0); 
            }
            return _midiOut;
        }
        public static void DisposeMidiOut()
        {
            if (_midiOut != null)
            {
                _midiOut.Dispose();
                _midiOut = null;
            }
        }
    }
}
