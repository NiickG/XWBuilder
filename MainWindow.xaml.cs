using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace XWingBuilder
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public class ShipDataList
    {
        public string Name { get; set; }
        public List<ShipCreatorDataContext> ShipList { get; set; }
        public List<int> ShipIds { get; set; }
        public int FullCost { get; set; }
        private ICommand _OpenFleet;
        public ICommand OpenFleet
        {
            get
            {
                return _OpenFleet ?? (_OpenFleet = new CommandHandler(() => StartOpenFleet(), () => true));
            }
        }
        private ICommand _Remove;
        public ICommand Remove
        {
            get
            {
                return _Remove ?? (_Remove = new CommandHandler(() => RemoveThis(), () => true));
            }
        }
        public void StartOpenFleet()
        {
            if (MainWindow.instance.squadBuilder == null)
            {
                XWingSquadBuilder sb = new XWingSquadBuilder();

                if (MainWindow.instance.factionTabControl.SelectedIndex == 0)
                {
                    sb.SetStats(XWingFaction.rebel_alliance, MainWindow.instance.listebesetzt, Name);
                }
                else if (MainWindow.instance.factionTabControl.SelectedIndex == 1)
                {
                    sb.SetStats(XWingFaction.galactic_empire, MainWindow.instance.listebesetzt, Name);
                }
                else if (MainWindow.instance.factionTabControl.SelectedIndex == 2)
                {
                    sb.SetStats(XWingFaction.scum_and_villainy, MainWindow.instance.listebesetzt, Name);
                }
                MainWindow.instance.squadBuilder = sb;
                MainWindow.instance.squadBuilder.Show();
                MainWindow.instance.listebesetzt = true;
                MainWindow.instance.Hide();

                foreach (var item in ShipList)
                {
                    ShipCreatorDataContext SCDC = new ShipCreatorDataContext(item.pilot, item.ShipID, item.UpgradeSlots);
                    SCDC.RefreshCost();

                    sb.Fleet.Items.Add(SCDC);
                }
                sb.SetPoints();
                sb.Ids = ShipIds;

            }
        }
        public void RemoveThis()
        {
            string dirPath = $"{AppDomain.CurrentDomain.BaseDirectory}/Saves";
            if (MainWindow.instance.factionTabControl.SelectedIndex == 0)
            {
                File.Delete($"{dirPath}/rebel_alliance/{Name}.XWFleet");
            }
            else if (MainWindow.instance.factionTabControl.SelectedIndex == 1)
            {
                File.Delete($"{dirPath}/galactic_empire/{Name}.XWFleet");
            }
            else if (MainWindow.instance.factionTabControl.SelectedIndex == 2)
            {
                File.Delete($"{dirPath}/scum_and_villainy/{Name}.XWFleet");
            }
            MainWindow.instance.RefreshSaves();
        }
    }
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        public List<Ship> Ships = new List<Ship>();
        public List<Pilot> Pilots = new List<Pilot>();
        public UpgradeData upgradeData;

        public List<ShipDataList> galactic_empire_List = new List<ShipDataList>();
        public List<ShipDataList> rebel_alliance_List = new List<ShipDataList>();
        public List<ShipDataList> scum_and_villainy_List = new List<ShipDataList>();

        public XWingSquadBuilder squadBuilder;

        public bool listebesetzt = false;

        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            GetData();

            RefreshSaves();
        }
        public void RefreshSaves()
        {
            galactic_empire_List.Clear();
            rebel_alliance_List.Clear();
            scum_and_villainy_List.Clear();

            galactic_empire_ListOfShips.Items.Clear();
            rebel_alliance_ListOfShips.Items.Clear();
            scum_and_villainy_ListOfShips.Items.Clear();

            string dirpath = $"{AppDomain.CurrentDomain.BaseDirectory}/Saves/";
            if (Directory.Exists(dirpath + "galactic_empire"))
            {
                galactic_empire_List = Import(Directory.GetFiles(dirpath + "galactic_empire"));
            }
            if (Directory.Exists(dirpath + "rebel_alliance"))
            {
                rebel_alliance_List = Import(Directory.GetFiles(dirpath + "rebel_alliance"));
            }
            if (Directory.Exists(dirpath + "scum_and_villainy"))
            {
                scum_and_villainy_List = Import(Directory.GetFiles(dirpath + "scum_and_villainy"));
            }
            foreach (var item in galactic_empire_List)
            {
                galactic_empire_ListOfShips.Items.Add(item);
            }
            foreach (var item in rebel_alliance_List)
            {
                rebel_alliance_ListOfShips.Items.Add(item);
            }
            foreach (var item in scum_and_villainy_List)
            {
                scum_and_villainy_ListOfShips.Items.Add(item);
            }
        }
        private void GetData()
        {
            upgradeData = new UpgradeData();
            dynamic dynamicships = JsonConvert.DeserializeObject(Data.Ships);

            dynamic dynamicpilots = JsonConvert.DeserializeObject(Data.Pilot);
            foreach (dynamic item in dynamicpilots)
            {
                Pilot pilot = new Pilot();

                pilot.Name = item.name;
                if (item.unique == "false")
                {
                    pilot.unique = false;
                }
                else
                {
                    pilot.unique = true;
                }
                pilot.Skill = item.skill;
                pilot.Points = item.points;

                foreach (dynamic i in item.slots)
                {
                    XWingUpgrades upgrade;
                    string Text = i;
                    switch (Text)
                    {
                        case "Elite": upgrade = XWingUpgrades.Elite; break;
                        case "Torpedo": upgrade = XWingUpgrades.Torpedo; break;
                        case "Turret": upgrade = XWingUpgrades.Turret; break;
                        case "Missile": upgrade = XWingUpgrades.Missile; break;
                        case "Crew": upgrade = XWingUpgrades.Crew; break;
                        case "Bomb": upgrade = XWingUpgrades.Bomb; break;
                        case "System": upgrade = XWingUpgrades.System; break;
                        case "Cannon": upgrade = XWingUpgrades.Cannon; break;
                        case "Cargo": upgrade = XWingUpgrades.Cargo; break;
                        case "Hardpoint": upgrade = XWingUpgrades.Hardpoint; break;
                        case "Team": upgrade = XWingUpgrades.Team; break;
                        case "Illicit": upgrade = XWingUpgrades.Illicit; break;
                        case "Salvaged Astromech": upgrade = XWingUpgrades.SalvagedAstromech; break;
                        case "Tech": upgrade = XWingUpgrades.Tech; break;
                        case "Astromech": upgrade = XWingUpgrades.Astromech; break;

                        case "Modification": upgrade = XWingUpgrades.Modification; break;
                        case "Title": upgrade = XWingUpgrades.Title; break;
                        default:
                            upgrade = XWingUpgrades.Elite; break;
                    }

                    pilot.slots.Add(upgrade);
                }

                pilot.slots.Add(XWingUpgrades.Modification);
                pilot.slots.Add(XWingUpgrades.Title);

                pilot.ShipName = item.ship;
                pilot.Text = item.text;
                pilot.ImageSource = AppDomain.CurrentDomain.BaseDirectory + "Images/" + item.image;

                pilot.Image = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Images/" + item.image);
                if (item.faction == "Rebel Alliance" || item.faction == "Resistance")
                {
                    pilot.faction = XWingFaction.rebel_alliance;
                }
                else if (item.faction == "Galactic Empire" || item.faction == "First Order")
                {
                    pilot.faction = XWingFaction.galactic_empire;
                }
                else if (item.faction == "Scum and Villainy")
                {
                    pilot.faction = XWingFaction.scum_and_villainy;
                }

                MainWindow.instance.Pilots.Add(pilot);
            }
            foreach (dynamic item in dynamicships)
            {
                Ship ship = new Ship();

                ship.Name = item.name;
                foreach (dynamic i in item.faction)
                {
                    if (i == "Rebel Alliance" || i == "Resistance")
                    {
                        ship.factions.Add(XWingFaction.rebel_alliance);
                    }
                    else if (i == "Galactic Empire" || i == "First Order")
                    {
                        ship.factions.Add(XWingFaction.galactic_empire);
                    }
                    else if (i == "Scum and Villainy")
                    {
                        ship.factions.Add(XWingFaction.scum_and_villainy);
                    }
                }
                if (item.attack == null)
                {
                    ship.attack = 0;
                    ship.energy = item.energy;
                }
                else
                {
                    ship.attack = item.attack;
                }
                ship.agility = item.agility;
                ship.hull = item.hull;
                ship.shields = item.shields;

                if (item.size == "small")
                {
                    ship.shipSize = ShipSize.Small;
                }
                else if (item.size == "large")
                {
                    ship.shipSize = ShipSize.Large;
                }
                else if (item.size == "huge")
                {
                    ship.shipSize = ShipSize.huge;
                }
                else
                {
                    Debug.WriteLine("Wrong Size!");
                }
                foreach (dynamic i in item.actions)
                {
                    string Text = i;
                    switch (Text)
                    {
                        case "Focus": ship.abbilities.Add(XWingAbbilities.Focus); break;
                        case "Target Lock": ship.abbilities.Add(XWingAbbilities.TargetLock); break;
                        case "Barrel Roll": ship.abbilities.Add(XWingAbbilities.BarrelRoll); break;
                        case "Recover": ship.abbilities.Add(XWingAbbilities.Recover); break;
                        case "Reinforce": ship.abbilities.Add(XWingAbbilities.Reinforce); break;
                        case "Coordinate": ship.abbilities.Add(XWingAbbilities.Coordinate); break;
                        case "Jam": ship.abbilities.Add(XWingAbbilities.Jam); break;
                        case "Evade": ship.abbilities.Add(XWingAbbilities.Evade); break;
                        case "Cloak": ship.abbilities.Add(XWingAbbilities.Cloak); break;
                        case "Boost": ship.abbilities.Add(XWingAbbilities.Boost); break;
                        case "SLAM": ship.abbilities.Add(XWingAbbilities.SLAM); break;
                        case "Rotate Arc": ship.abbilities.Add(XWingAbbilities.RotateArc); break;
                        case "Reload": ship.abbilities.Add(XWingAbbilities.Reload); break;

                        default:
                            break;
                    }
                }
                ship.Pilots = GetPilotsfromShip(ship);
                MainWindow.instance.Ships.Add(ship);
            }
        }
        private List<Pilot> GetPilotsfromShip(Ship ship)
        {
            List<Pilot> pilots = new List<Pilot>();
            foreach (Pilot item in MainWindow.instance.Pilots)
            {
                if (item.ShipName == ship.Name)
                {
                    item.ship = ship;
                    pilots.Add(item);
                }
            }
            return pilots;
        }
        private void TopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void NewSquad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (squadBuilder == null)
            {
                XWingSquadBuilder sb = new XWingSquadBuilder();

                if (factionTabControl.SelectedIndex == 0)
                {
                    sb.SetStats(XWingFaction.rebel_alliance, listebesetzt);
                }
                else if (factionTabControl.SelectedIndex == 1)
                {
                    sb.SetStats(XWingFaction.galactic_empire, listebesetzt);
                }
                else if (factionTabControl.SelectedIndex == 2)
                {
                    sb.SetStats(XWingFaction.scum_and_villainy, listebesetzt);
                }
                squadBuilder = sb;
                squadBuilder.Show();
                listebesetzt = true;
                Hide();
            }
        }
        public int GenerateShipID(List<int> Ids, out List<int> idsOut)
        {
            Random rnd = new Random();
            int Id = rnd.Next(0, 10000000);

            while (Ids.Contains(Id))
            {
                Id = rnd.Next(0, 10000000);
            }

            Ids.Add(Id);
            idsOut = Ids;
            return Id;
        }

        public List<ShipDataList> Import(string[] files)
        {
            List<ShipDataList> tempListOfShipData = new List<ShipDataList>();
            foreach (string filename in files)
            {
                string[] type = filename.Split('.');
                if (type[1] == "XWFleet")
                {
                    string Text = System.IO.File.ReadAllText(filename);
                    string[] ships = Text.Split('§');

                    List<ShipCreatorDataContext> shipData = new List<ShipCreatorDataContext>();
                    List<int> idsfromFleet = new List<int>();
                    foreach (var Xitem in ships)
                    {
                        if (!string.IsNullOrEmpty(Xitem))
                        {
                            string[] one = Xitem.Split('|');

                            string[] Ship_Pilot = one[0].Split('&');

                            string ShipName = Ship_Pilot[0];
                            string PilotName = Ship_Pilot[1];

                            string[] Upgrades = one[1].Split('/');

                            Pilot pilot = new Pilot();


                            int id = GenerateShipID(idsfromFleet, out List<int> idsOut);

                            idsfromFleet = idsOut;

                            foreach (var item in MainWindow.instance.Ships)
                            {
                                if (item.Name == ShipName)
                                {
                                    pilot.ship = item;
                                }
                            }
                            foreach (var item in MainWindow.instance.Pilots)
                            {
                                if (item.Name == PilotName)
                                {
                                    pilot = item;
                                }
                            }
                            ShipCreatorDataContext Ship = new ShipCreatorDataContext(pilot, id);
                            for (int i = 0; i < Upgrades.Length - 1; i++)
                            {
                                string[] split = Upgrades[i].Split('&');

                                XWingUpgrades UpgradeType = GetUpgradeTypeByString(split[0]);
                                string UpgradeName = split[1];


                                ShipUpgrade shipUpgrade = new ShipUpgrade();

                                List<ShipUpgrade> list = new List<ShipUpgrade>();
                                switch (UpgradeType)
                                {
                                    case XWingUpgrades.Elite:
                                        list = MainWindow.instance.upgradeData.Elite_Upgrades;
                                        break;
                                    case XWingUpgrades.Torpedo:
                                        list = MainWindow.instance.upgradeData.Torpedo_Upgrades;
                                        break;
                                    case XWingUpgrades.Astromech:
                                        list = MainWindow.instance.upgradeData.Astromech_Upgrades;
                                        break;
                                    case XWingUpgrades.Turret:
                                        list = MainWindow.instance.upgradeData.Turret_Upgrades;
                                        break;
                                    case XWingUpgrades.Missile:
                                        list = MainWindow.instance.upgradeData.Missile_Upgrades;
                                        break;
                                    case XWingUpgrades.Crew:
                                        list = MainWindow.instance.upgradeData.Crew_Upgrades;
                                        break;
                                    case XWingUpgrades.Bomb:
                                        list = MainWindow.instance.upgradeData.Bomb_Upgrades;
                                        break;
                                    case XWingUpgrades.System:
                                        list = MainWindow.instance.upgradeData.System_Upgrades;
                                        break;
                                    case XWingUpgrades.Cannon:
                                        list = MainWindow.instance.upgradeData.Cannon_Upgrades;
                                        break;
                                    case XWingUpgrades.Cargo:
                                        list = MainWindow.instance.upgradeData.Cargo_Upgrades;
                                        break;
                                    case XWingUpgrades.Hardpoint:
                                        list = MainWindow.instance.upgradeData.Hardpoint_Upgrades;
                                        break;
                                    case XWingUpgrades.Team:
                                        list = MainWindow.instance.upgradeData.Team_Upgrades;
                                        break;
                                    case XWingUpgrades.Illicit:
                                        list = MainWindow.instance.upgradeData.Illicit_Upgrades;
                                        break;
                                    case XWingUpgrades.SalvagedAstromech:
                                        list = MainWindow.instance.upgradeData.SalvagedAstromech_Upgrades;
                                        break;
                                    case XWingUpgrades.Tech:
                                        list = MainWindow.instance.upgradeData.Tech_Upgrades;
                                        break;
                                    case XWingUpgrades.Modification:
                                        list = MainWindow.instance.upgradeData.Modification_Upgrades;
                                        break;
                                    case XWingUpgrades.Title:
                                        list = MainWindow.instance.upgradeData.Title_Upgrades;
                                        break;
                                    default:
                                        break;
                                }

                                foreach (var item in list)
                                {
                                    if (item.Name == UpgradeName)
                                    {
                                        shipUpgrade = item;
                                    }
                                }
                                foreach (XWingUpgrades item in shipUpgrade.UpgradeGains)
                                {
                                    int x = 0;
                                    for (int xxx = 0; xxx < Ship.UpgradeSlots.Count; xxx++)
                                    {
                                        if (Ship.UpgradeSlots[xxx].Upgrade == UpgradeType)
                                        {
                                            x = xxx;
                                        }
                                    }
                                    Ship.UpgradeSlots.Insert(x, new ShipCreatorDataContext.ShipUpgradeDataContext(item, pilot, id));
                                }
                                foreach (XWingUpgrades item in shipUpgrade.UpgradeLoses)
                                {
                                    for (int Y = 0; Y < Ship.UpgradeSlots.Count; Y++)
                                    {
                                        if (Ship.UpgradeSlots[Y].Upgrade == item)
                                        {
                                            Ship.UpgradeSlots.RemoveAt(Y);
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < Upgrades.Length - 1; i++)
                            {
                                string[] split = Upgrades[i].Split('&');

                                XWingUpgrades UpgradeType = GetUpgradeTypeByString(split[0]);
                                string UpgradeName = split[1];


                                ShipUpgrade shipUpgrade = new ShipUpgrade();



                                List<ShipUpgrade> list = new List<ShipUpgrade>();
                                switch (UpgradeType)
                                {
                                    case XWingUpgrades.Elite:
                                        list = MainWindow.instance.upgradeData.Elite_Upgrades;
                                        break;
                                    case XWingUpgrades.Torpedo:
                                        list = MainWindow.instance.upgradeData.Torpedo_Upgrades;
                                        break;
                                    case XWingUpgrades.Astromech:
                                        list = MainWindow.instance.upgradeData.Astromech_Upgrades;
                                        break;
                                    case XWingUpgrades.Turret:
                                        list = MainWindow.instance.upgradeData.Turret_Upgrades;
                                        break;
                                    case XWingUpgrades.Missile:
                                        list = MainWindow.instance.upgradeData.Missile_Upgrades;
                                        break;
                                    case XWingUpgrades.Crew:
                                        list = MainWindow.instance.upgradeData.Crew_Upgrades;
                                        break;
                                    case XWingUpgrades.Bomb:
                                        list = MainWindow.instance.upgradeData.Bomb_Upgrades;
                                        break;
                                    case XWingUpgrades.System:
                                        list = MainWindow.instance.upgradeData.System_Upgrades;
                                        break;
                                    case XWingUpgrades.Cannon:
                                        list = MainWindow.instance.upgradeData.Cannon_Upgrades;
                                        break;
                                    case XWingUpgrades.Cargo:
                                        list = MainWindow.instance.upgradeData.Cargo_Upgrades;
                                        break;
                                    case XWingUpgrades.Hardpoint:
                                        list = MainWindow.instance.upgradeData.Hardpoint_Upgrades;
                                        break;
                                    case XWingUpgrades.Team:
                                        list = MainWindow.instance.upgradeData.Team_Upgrades;
                                        break;
                                    case XWingUpgrades.Illicit:
                                        list = MainWindow.instance.upgradeData.Illicit_Upgrades;
                                        break;
                                    case XWingUpgrades.SalvagedAstromech:
                                        list = MainWindow.instance.upgradeData.SalvagedAstromech_Upgrades;
                                        break;
                                    case XWingUpgrades.Tech:
                                        list = MainWindow.instance.upgradeData.Tech_Upgrades;
                                        break;
                                    case XWingUpgrades.Modification:
                                        list = MainWindow.instance.upgradeData.Modification_Upgrades;
                                        break;
                                    case XWingUpgrades.Title:
                                        list = MainWindow.instance.upgradeData.Title_Upgrades;
                                        break;
                                    default:
                                        break;
                                }

                                foreach (var item in list)
                                {
                                    if (item.Name == UpgradeName)
                                    {
                                        shipUpgrade = item;
                                    }
                                }
                                foreach (var item in Ship.UpgradeSlots)
                                {
                                    if (item.Upgrade == UpgradeType)
                                    {
                                        if (item.shipUpgrade == null)
                                        {
                                            shipUpgrade.SetStats(item, id, new CardSelectWindow());
                                            item.SetUpgrade(shipUpgrade);
                                            break;
                                        }
                                    }

                                }
                            }

                            Ship.UpgradeSlots = Ship.UpgradeSlots;
                            Ship.RefreshCost();
                            shipData.Add(Ship);

                        }
                    }
                    string[] nameOfFile = type[0].Split('\\');
                    int cost = 0;
                    foreach (var item in shipData)
                    {
                        cost += item.pilot.Points;
                        foreach (var i in item.UpgradeSlots)
                        {
                            if (i.shipUpgrade != null)
                            {
                                cost += int.Parse(i.shipUpgrade.Points);
                            }
                        }
                    }
                    tempListOfShipData.Add(new ShipDataList() { Name = nameOfFile.Last(), ShipList = shipData, ShipIds = idsfromFleet, FullCost = cost });
                }
            }
            return tempListOfShipData;
        }
        private XWingUpgrades GetUpgradeTypeByString(string typeString)
        {
            switch (typeString)
            {
                case "Title": return XWingUpgrades.Title;
                case "Modification": return XWingUpgrades.Modification;
                case "Crew": return XWingUpgrades.Crew;
                case "Elite": return XWingUpgrades.Elite;
                case "Tech": return XWingUpgrades.Tech;
                case "SalvagedAstromech": return XWingUpgrades.SalvagedAstromech;
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

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
