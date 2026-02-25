using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIANOAPP
{
    internal class ViolinStrategy: ISoundStrategy
    {
        public void SetInstrument(MidiOut midiOut)
        {
            midiOut.Send(MidiMessage.ChangePatch(41, 1).RawData);
        }
        public void PlayTone(MusicElement element,MidiOut midiOut)
        {
            if (element is Note note)
            {
                // Zmiana instrumentu na fortepian (Program Change, Acoustic Grand Piano = 1)
                midiOut.Send(MidiMessage.ChangePatch(41, 1).RawData);

                // Graj nutę o określonym numerze MIDI z maksymalną głośnością (velocity = 127)
                midiOut.Send(MidiMessage.StartNote(note.MidiNum, 127, 1).RawData);

                // Czekaj przez czas trwania nuty
                Thread.Sleep(note.Duration);

                // Zatrzymaj nutę (Note Off)
                midiOut.Send(MidiMessage.StopNote(note.MidiNum, 0, 1).RawData);
            }
            else if (element is Pause pause)
            {
                Thread.Sleep(pause.Duration);
            }
        }
        public void StartTone(Note note, MidiOut midiOut)

        { try
            {
                midiOut.Send(MidiMessage.StartNote(note.MidiNum, 127, 1).RawData);
               
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }
        public void StopTone(Note note, MidiOut midiOut)
        {
            try
            {
                midiOut.Send(MidiMessage.StopNote(note.MidiNum, 0, 1).RawData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");

            }
        }
    }
    
}
