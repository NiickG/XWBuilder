using System.Collections.Generic;

namespace XWingBuilder
{
    public class Ship
    {
        public string Name;
        public List<XWingFaction> factions = new List<XWingFaction>();

        public int attack;
        public int agility;
        public int hull;
        public int shields;

        public int energy;

        public ShipSize shipSize;

        public List<XWingAbbilities> abbilities = new List<XWingAbbilities>();
        public List<Pilot> Pilots = new List<Pilot>();

        public string ShipImage;
    }
}
