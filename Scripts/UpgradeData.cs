using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace XWingBuilder
{
    public class UpgradeData
    {
        public List<ShipUpgrade> Title_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Modification_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Crew_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Elite_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Tech_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> SalvagedAstromech_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Illicit_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Team_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Hardpoint_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Cargo_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Cannon_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> System_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Bomb_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Missile_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Turret_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Astromech_Upgrades = new List<ShipUpgrade>();
        public List<ShipUpgrade> Torpedo_Upgrades = new List<ShipUpgrade>();

        public UpgradeData()
        {
            string RawUpgrades = System.IO.File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/UpgradeCards.txt");

            dynamic upgrades = JsonConvert.DeserializeObject(RawUpgrades);
            foreach (dynamic item in upgrades)
            {
                Set_Upgrade(item);
            }
        }
        public void Set_Upgrade(dynamic item)
        {
            string s = item.slot;
            ShipUpgrade title = new ShipUpgrade();

            title.ImageSource = $"{AppDomain.CurrentDomain.BaseDirectory}/Images/{item.image}";
            title.Text = item.text;
            title.Name = item.name;
            title.Points = item.points;
            if (item.ship != null)
            {
                foreach (var i in item.ship)
                {
                    Debug.WriteLine(title.Name);
                    string x = i;
                    title.ShipName.Add(x);
                }
            }
            if (item.size != null)
            {
                foreach (var i in item.size)
                {
                    string x = i;
                    switch (x)
                    {
                        case "small": title.shipSize.Add(ShipSize.Small); break;
                        case "large": title.shipSize.Add(ShipSize.Large); break;
                        case "huge": title.shipSize.Add(ShipSize.huge); break;

                        default:
                            break;
                    }
                }
            }
            if (item.grants != null)
            {
                foreach (var i in item.grants)
                {
                    if (i.type == "slot")
                    {
                        string f = i.name;
                        title.UpgradeGains.Add(GetUpgradeTypeByString(f));
                    }
                }
            }
            switch (title.Name)
            {
                case "Havoc": title.UpgradeLoses.Add(XWingUpgrades.Crew); break;
                case "TIE Shuttle":
                    title.UpgradeLoses.Add(XWingUpgrades.Torpedo); title.UpgradeLoses.Add(XWingUpgrades.Missile);
                    title.UpgradeLoses.Add(XWingUpgrades.Missile); title.UpgradeLoses.Add(XWingUpgrades.Torpedo); title.UpgradeLoses.Add(XWingUpgrades.Bomb); break;

                default:
                    break;
            }
            string faction = item.faction;
            if (faction != null)
            {
                string x = item.faction;
                if (x == "Rebel Alliance" || x == "Resistance")
                {
                    title.faction = XWingFaction.rebel_alliance;
                }
                else if (x == "Galactic Empire" || x == "First Order")
                {
                    title.faction = XWingFaction.galactic_empire;
                }
                else if (x == "Scum and Villainy")
                {
                    title.faction = XWingFaction.scum_and_villainy;
                }
                else
                {
                    title.faction = XWingFaction.Null;
                }
            }
            string z = item.slot;
            switch (z)
            {
                case "Title": Title_Upgrades.Add(title); break;
                case "Modification": Modification_Upgrades.Add(title); break;
                case "Crew": Crew_Upgrades.Add(title); break;
                case "Elite": Elite_Upgrades.Add(title); break;
                case "Tech": Tech_Upgrades.Add(title); break;
                case "Salvaged Astromech": SalvagedAstromech_Upgrades.Add(title); break;
                case "Team": Team_Upgrades.Add(title); break;
                case "Illicit": Illicit_Upgrades.Add(title); break;
                case "Hardpoint": Hardpoint_Upgrades.Add(title); break;
                case "Cargo": Cargo_Upgrades.Add(title); break;
                case "Cannon": Cannon_Upgrades.Add(title); break;
                case "System": System_Upgrades.Add(title); break;
                case "Bomb": Bomb_Upgrades.Add(title); break;
                case "Missile": Missile_Upgrades.Add(title); break;
                case "Turret": Turret_Upgrades.Add(title); break;
                case "Astromech": Astromech_Upgrades.Add(title); break;
                case "Torpedo": Torpedo_Upgrades.Add(title); break;

                default:
                    break;
            }
        }
        public XWingUpgrades GetUpgradeTypeByString(string typeString)
        {
            switch (typeString)
            {
                case "Title": return XWingUpgrades.Title;
                case "Modification": return XWingUpgrades.Modification;
                case "Crew": return XWingUpgrades.Crew;
                case "Elite": return XWingUpgrades.Elite;
                case "Tech": return XWingUpgrades.Tech;
                case "Salvaged Astromech": return XWingUpgrades.SalvagedAstromech;
                case "Team": return XWingUpgrades.Team;
                case "Illicit": return XWingUpgrades.Illicit;
                case "Hardpoint": return XWingUpgrades.Hardpoint;
                case "Cargo": return XWingUpgrades.Cargo;
                case "Cannon": return XWingUpgrades.Cannon;
                case "System": return XWingUpgrades.System;
                case "Bomb": return XWingUpgrades.Bomb;
                case "Missile": return XWingUpgrades.Missile;
                case "Turret": return XWingUpgrades.Turret;
                case "Astromech": return XWingUpgrades.Astromech;
                case "Torpedo": return XWingUpgrades.Torpedo;

                default:
                    return XWingUpgrades.Elite;
            }
        }
    }
}
