using System.Collections.Generic;
using System.Drawing;

namespace XWingBuilder
{
    public class Pilot
    {
        public string Name;
        public int Skill;
        public int Points;
        public string ShipName;
        public List<XWingUpgrades> slots = new List<XWingUpgrades>();
        public bool unique;
        public XWingFaction faction;
        public string ImageSource;
        public Bitmap Image;
        public string Text;

        public Ship ship;
    }
}
