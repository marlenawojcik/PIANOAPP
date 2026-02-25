using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIANOAPP
{
    internal class Pause:MusicElement
    {
        public Pause(int duration, string name="pause", string type="pause"):base(0,name,type,duration)
        {
        }
        public override MusicElement Clone()
        {
            return (MusicElement)this.MemberwiseClone();
        }
    }
}
