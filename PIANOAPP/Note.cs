using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;

namespace PIANOAPP
{
    public class Note:MusicElement
    {
        public int MidiNum { get; set; }
        public Note(int midiNum, int position, string name, int duration = 2000, string type="note"):base(position,name,type,duration)
        {
           MidiNum = midiNum;

        }
        public override MusicElement Clone()
        {
            return (MusicElement)this.MemberwiseClone();
        }

    }
}
