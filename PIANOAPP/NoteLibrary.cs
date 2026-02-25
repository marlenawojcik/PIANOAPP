using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PIANOAPP
{
    public static class NoteLibrary
    {
        private static Dictionary<string, Note> notes = new Dictionary<string, Note>();

        static NoteLibrary()
        {
            InitializeNotes();
        }

        private static void InitializeNotes()
        {
            //var notes = new List<Note>();
            var baseNotes = new[]
           {
           new { MidiNum = 12,position=-30,Name = "C"},
                new { MidiNum = 13,position=-30,Name = "C#"},
                new { MidiNum = 14,position=-29,Name = "D"},
                new { MidiNum = 15,position=-29,Name = "D#" },
                new {MidiNum = 16,position=-28, Name = "E"},
                new {MidiNum = 17,position=-27, Name = "F"},
                new { MidiNum = 18,position=-27,Name = "F#" },
                new { MidiNum = 19,position=-26,Name = "G"},
                new {MidiNum = 20,position=-26, Name = "G#" },
                new { MidiNum = 21,position=-25,Name = "A"},
                new { MidiNum = 22,position=-25,Name = "A#"},
                new {MidiNum = 23,position=-24, Name = "B" }
        };

            for (int octave = 0; octave <= 9; octave++) // Zakres oktaw od 0 do 9
            {
                foreach (var baseNote in baseNotes)
                {
                    int midiNum = baseNote.MidiNum + 12 * octave;
                    string noteName = $"{baseNote.Name}{octave}";
                    int position = baseNote.position + 7 * octave;
                    Note newNote = new Note(midiNum, position, noteName);
                    notes.Add(newNote.Name, newNote);
                }
            }
        }
        public static Note GetNote(string name)
        {
            if (notes.TryGetValue(name, out var baseNote))
            {
                return new Note(baseNote.MidiNum, baseNote.Position, baseNote.Name);
            }

            throw new ArgumentException($"Nuta {name} nie istnieje w bibliotece.");
        }
        public static Note GetNote(int midi, int duration=1000)
        {
           Note baseNote = notes.Values.FirstOrDefault(n => n.MidiNum == midi);
            if (baseNote != null)
            {
                return new Note(baseNote.MidiNum, baseNote.Position, baseNote.Name, duration);
            }
            throw new ArgumentException($"Nuta o pozycji {midi} nie istnieje w bibliotece.");
        }

        public static Note GetNote(string name, int duration)
        {
            if (notes.TryGetValue(name, out var baseNote))
            {
                return new Note(baseNote.MidiNum, baseNote.Position, baseNote.Name, duration);
            }
            else
            {
                throw new KeyNotFoundException($"Note {name} not found in library.");
            }
        }
    }
}
