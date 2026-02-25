using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIANOAPP
{
    public class ScoringManager
    {
        public int Score { get; private set; }
        public List<Note> PressedNotes { get; private set; }
        public List<MusicElement> Notes { get; private set; }


        public ScoringManager(Song song)
        {
            Notes = song.Elements.Where(e => e.Type == "note").ToList();
            
            PressedNotes = new List<Note>();

        }
        public void AddPressedNote(Note note)
        {
            PressedNotes.Add(note);
        }

        public void ComparePressedNotes(Song song)
        {
            Score = 0;
            var songNotes = song.Elements.Where(e => e.Type == "note").Select(e => e.Name).ToList();

            foreach (var pressedNote in PressedNotes)
            {
                if (songNotes.Remove(pressedNote.Name))
                {
                    Score++;
                }
            }
        }
     
        public string CalculateMatch(Song song)
        {
            double percent = 0;
            
            if (PressedNotes.Count() > 0)
            {
                percent = (((double)Score/PressedNotes.Count())*((double)Score/Notes.Count()))*100;
                
                return "your Match is on " + percent.ToString("F2") + " %. Total numbers of notes:  score: "+ +Notes.Count+ ", Your correct clicks: "+Score+", and your total presses: "+PressedNotes.Count;
            }
            else { return "You haven't pressed any key yet."; }
        }
        public void Reset()
        {
            PressedNotes.Clear();
            Score= 0;
        }
    }
}
