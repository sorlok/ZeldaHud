using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeldaGui
{
    public class CustomItem
    {
        public byte[] iconsId;
        public byte level;
        public bool on;
        public string name;
        public bool loop = false;
        public bool bottle = false;
        public bool count = false;
        public bool dungeon = false;
        public string eval;
        public object co;
        public System.Reflection.MethodInfo mi;
        public byte counter = 0;
        public CustomItem(byte[] iconsId, string name, bool loop = false,bool bottle = false,bool count = false,bool dungeon = false, string eval = "", object co = null, System.Reflection.MethodInfo mi = null)
        {
            this.iconsId = iconsId;
            this.level = 0;
            this.on = false;
            this.name = name;
            this.bottle = bottle;
            this.loop = loop;
            this.count = count;
            this.dungeon = dungeon;
            this.eval = eval;
            this.co = co;
            this.mi = mi;
        }

    }
}
