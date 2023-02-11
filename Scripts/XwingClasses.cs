using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using static XWingBuilder.ShipCreatorDataContext;

namespace XWingBuilder
{
    public class ShipUpgrade
    {
        public string Name { get; set; }
        public string ImageSource { get; set; }
        public XWingFaction faction { get; set; }

        public List<ShipSize> shipSize { get; set; }

        public List<string> ShipName = new List<string>();
        public string Text { get; set; }

        public string Points { get; set; }

        public List<XWingUpgrades> UpgradeGains = new List<XWingUpgrades>();
        public List<XWingUpgrades> UpgradeLoses = new List<XWingUpgrades>();
        private ICommand _SelectCard;
        public ICommand SelectCard
        {
            get
            {
                return _SelectCard ?? (_SelectCard = new CommandHandler(() => NewSelectCard(false), () => true));
            }
        }
        public int ShipID;
        private ShipUpgradeDataContext source;
        private CardSelectWindow window;

        public ShipUpgrade()
        {
            faction = XWingFaction.Null;
            shipSize = new List<ShipSize>();
        }
        public void SetStats(ShipUpgradeDataContext _source, int _ShipID, CardSelectWindow _window)
        {
            source = _source;
            ShipID = _ShipID;
            window = _window;
        }
        public void NewSelectCard(bool IsNull)
        {
            Debug.WriteLine(ShipID);
            List<ShipCreatorDataContext> ships = new List<ShipCreatorDataContext>();
            foreach (var item in XWingSquadBuilder.instance.Fleet.Items)
            {
                ships.Add((ShipCreatorDataContext)item);
            }
            XWingSquadBuilder.instance.Fleet.Items.Clear();
            XWingSquadBuilder.instance.SetPoints();
            for (int item = 0; item < ships.Count; item++)
            {
                if (ships[item].ShipID == ShipID)
                {
                    List<ShipUpgradeDataContext> upgrades = new List<ShipUpgradeDataContext>();
                    foreach (var i in (ships[item] as ShipCreatorDataContext).UpgradeSlots)
                    {
                        upgrades.Add(i);
                    }
                    (ships[item] as ShipCreatorDataContext).UpgradeSlots.Clear();
                    for (int i = 0; i < upgrades.Count; i++)
                    {
                        if (i == GetIndex(upgrades, source))
                        {
                            if (upgrades[i].shipUpgrade != null)
                            {
                                foreach (XWingUpgrades x in upgrades[i].shipUpgrade.UpgradeGains)
                                {
                                    List<ShipUpgradeDataContext> ups = upgrades;
                                    for (int Y = 0; Y < ships[item].UpgradeSlots.Count; Y++)
                                    {
                                        if (ships[item].UpgradeSlots[Y].Upgrade == x)
                                        {
                                            ships[item].UpgradeSlots.RemoveAt(upgrades.FindIndex(element => element == ups[Y]));
                                        }
                                    }
                                }
                                foreach (XWingUpgrades x in upgrades[i].shipUpgrade.UpgradeLoses)
                                {
                                    ships[item].UpgradeSlots.Add(new ShipUpgradeDataContext(x, upgrades[i].MyPilot, ShipID));
                                }
                            }
                            if (IsNull == false)
                            {
                                upgrades[i].SetUpgrade(this);
                            }
                            else
                            {
                                Debug.WriteLine(ShipID);
                                upgrades[i].NewRemoveCard();
                            }

                            if (IsNull == false)
                            {
                                if (upgrades[i].shipUpgrade != null)
                                {
                                    foreach (XWingUpgrades x in upgrades[i].shipUpgrade.UpgradeLoses)
                                    {
                                        for (int Y = 0; Y < ships[item].UpgradeSlots.Count; Y++)
                                        {
                                            if (ships[item].UpgradeSlots[Y].Upgrade == x)
                                            {
                                                ships[item].UpgradeSlots.RemoveAt(Y);
                                            }
                                        }
                                    }
                                    foreach (XWingUpgrades x in upgrades[i].shipUpgrade.UpgradeGains)
                                    {
                                        ships[item].UpgradeSlots.Add(new ShipUpgradeDataContext(x, upgrades[i].MyPilot, ShipID));
                                    }
                                }
                            }
                        }
                        (ships[item] as ShipCreatorDataContext).UpgradeSlots.Add(upgrades[i]);
                    }
                }
                ships[item].RefreshCost();
                XWingSquadBuilder.instance.Fleet.Items.Add(ships[item]);
                XWingSquadBuilder.instance.SetPoints();
            }
            try
            {
                if (window != null)
                {
                    window.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        private int GetIndex(List<ShipUpgradeDataContext> collection, object Object)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i] == Object)
                {
                    return i;
                }
            }
            return 0;
        }
    }
    public enum XWingFaction
    {
        Null,
        galactic_empire,
        rebel_alliance,
        scum_and_villainy
    }
    public enum XWingAbbilities
    {
        Focus,
        TargetLock,
        BarrelRoll,
        Recover,
        Reinforce,
        Coordinate,
        Jam,
        Evade,
        Cloak,
        Boost,
        SLAM,
        RotateArc,
        Reload,
    }
    public enum XWingUpgrades
    {
        Elite,
        Torpedo,
        Astromech,
        Turret,
        Missile,
        Crew,
        Bomb,
        System,
        Cannon,
        Cargo,
        Hardpoint,
        Team,
        Illicit,
        SalvagedAstromech,
        Tech,
        Modification,
        Title
    }
    public enum ShipSize
    {
        Null,
        Small,
        Large,
        huge
    }
}
