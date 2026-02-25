using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIANOAPP
{
    public abstract class MusicElement
    {
        public int Duration {  get; set; }
        public int Position { get; set; }
        public string Type { get; set; } // "note" lub "rest"
        public string Name { get; set; } // "note" lub "rest"


        public MusicElement(int position, string name, string type,int duration = 500 )
        {
            Position = position;
            Type = type;
            Name = name;
            Duration = duration;
        }
        public virtual MusicElement Clone()
        {
            return (MusicElement)this.MemberwiseClone();
        }
    }
}
