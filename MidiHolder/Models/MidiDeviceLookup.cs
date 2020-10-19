using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiHolder.Models
{
    public class MidiDeviceLookup
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
