using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIANOAPP
{
    public class Song
    {
        public string Title { get; set; }
        public int Length {  get; set; }
        public ISoundStrategy Strategy {  get; set; }
        public string Instrument {  get; set; }
        public List<MusicElement> Elements { get; set; }

       public Song(string title, List<MusicElement> elements)
        {
            Title = title;
            Elements = elements;
           
            Length = SetLength();
        }
        private int SetLength()
        {
            int length = 0;
            foreach (MusicElement element in Elements)
            {
                length += element.Duration;
            }
            return length;
        }
        public int StartingTime(int positionInSong)
        {
            if (positionInSong ==0)
            {
                return 0;
            }
            int starting = 0;
            for(int i=0;i<positionInSong;i++)
            {
                starting += Elements[i].Duration;
            }
            return starting;
        }
    }  
}
