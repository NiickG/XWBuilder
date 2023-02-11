using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace XWingBuilder
{
    /// <summary>
    /// Interaktionslogik für XWingSquadBuilder.xaml
    /// </summary>
    public class DataContextXWingSquadBuilder
    {
        public string InputFieldTag { get; set; }
    }
    public partial class XWingSquadBuilder : Window
    {

        public System.Windows.Media.Brush BorderColor { get; set; }

        public XWingFaction Currentfaction;
        public static XWingSquadBuilder instance;
        private string NameBefore;
        public List<int> Ids = new List<int>();
        public XWingSquadBuilder()
        {
            DataContext = this;
            InitializeComponent();
            DataContext = new DataContextXWingSquadBuilder() {InputFieldTag = "Name"};
            instance = this;
            HullStatImage.Content = "&";
            Fleet.Items.Clear();
            //ShipComboBox.Items.Add(new ShipDataContext { Ship = "a", ShipName = "test"});
        }
        public void SetPoints()
        {
            int points = 0;
            foreach (ShipCreatorDataContext item in Fleet.Items)
            {
                points += item.pilot.Points;
                foreach (var i in item.UpgradeSlots)
                {
                    if (i.shipUpgrade != null)
                    {
                        points += int.Parse(i.shipUpgrade.Points);
                    }
                }
            }
            Points.Content = points;
        }
        public void SetStats(XWingFaction faction, bool isnull, string Namebefore = "")
        {
            NameOfFleet.Text = Namebefore;
            if (NameBefore != "")
            {
                DataContext = new DataContextXWingSquadBuilder() { InputFieldTag = Namebefore };
            }
            NameBefore = Namebefore;
            dynamic dynamicships = JsonConvert.DeserializeObject(Data.Ships);

            foreach (Ship ship in MainWindow.instance.Ships)
            {
                if (ship.factions.Contains(faction))
                {
                    string mark = Data.GetMarkFromShip(ship.Name);
                    ship.ShipImage = mark;
                    ShipComboBox.Items.Add(new ShipDataContext { Ship = mark, ShipName = ship.Name, ShipSource = ship });
                }
            }
            var converter = new System.Windows.Media.BrushConverter();
            if (faction == XWingFaction.scum_and_villainy)
            {
                BorderColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFF3C257");
            }
            else if (faction == XWingFaction.galactic_empire)
            {
                BorderColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FF4875FF");
            }
            else if (faction == XWingFaction.rebel_alliance)
            {
                BorderColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFF3D3D");
            }
            BorderBrush = BorderColor;

            Rec1.Fill = BorderColor;
            Rec2.Fill = BorderColor;
            Rec3.Fill = BorderColor;

            Currentfaction = faction;
        }

        private void Window_Close()
        {
            MainWindow.instance.RefreshSaves();
            MainWindow.instance.squadBuilder = null;
            MainWindow.instance.Show();
            Close();
        }
        private void SaveFleet()
        {
            if (XWingSquadBuilder.instance.Fleet.Items.Count > 0)
            {
                string text = "";
                int Cost = 0;
                foreach (ShipCreatorDataContext item in XWingSquadBuilder.instance.Fleet.Items)
                {
                    string content = $"{item.ShipName}&{item.PilotName}|";
                    foreach (var i in item.UpgradeSlots)
                    {
                        if (i.shipUpgrade != null)
                        {
                            content += $"{i.Upgrade}&{i.shipUpgrade.Name}/";
                        }
                    }


                    Cost += item.pilot.Points;
                    for (int i = 0; i < item.UpgradeSlots.Count; i++)
                    {
                        if (item.UpgradeSlots[i].shipUpgrade != null)
                        {
                            try
                            {
                                Cost += int.Parse(item.UpgradeSlots[i].shipUpgrade.Points);
                            }
                            catch (Exception x)
                            {
                                Debug.WriteLine(x);
                            }
                        }
                    }
                    text += content + "§";
                }
                string nameFleet = NameOfFleet.Text;

                string dirPath = $"{AppDomain.CurrentDomain.BaseDirectory}/Saves/{Currentfaction}";

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Saves");
                    Directory.CreateDirectory(dirPath);                    
                }
                if (File.Exists(dirPath+$"/{NameBefore}.XWFleet"))
                {
                    File.Delete(dirPath + $"/{NameBefore}.XWFleet");
                }
                System.IO.File.WriteAllText($"{dirPath}/{nameFleet}.XWFleet", text);
            }
        }
        private string GetUniqueName(string name, string folderPath)
        {
            string validatedName = name;
            int tries = 1;
            while (System.IO.File.Exists(folderPath + validatedName))
            {
                validatedName = string.Format("{0} [{1}]", name, tries++);
            }
            validatedName = validatedName.Replace(".XWFleet", "");
            validatedName += ".XWFleet";
            return validatedName;
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window_Close();
        }

        private void TopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ShipComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShipDataContext shipData = (ShipDataContext)ShipComboBox.SelectedItem;

            attackLabel.Content = shipData.ShipSource.attack;
            agilityLabel.Content = shipData.ShipSource.agility;
            hullLabel.Content = shipData.ShipSource.hull;
            shieldLabel.Content = shipData.ShipSource.shields;

            if (shipData.ShipSource.energy > 0)
            {
                energyImage.Visibility = Visibility.Visible;
                energyLabel.Content = shipData.ShipSource.energy;
            }
            else
            {
                energyImage.Visibility = Visibility.Hidden;
                energyLabel.Content = "";
            }
            UpgradesOfPilot.Text = "";
            PilotComboBox.Items.Clear();
            foreach (Pilot item in shipData.ShipSource.Pilots)
            {
                if (item.faction == Currentfaction)
                {
                    PilotComboBox.Items.Add(new PilotDataContext(item));
                }
            }
        }

        private void PilotComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PilotComboBox.SelectedItem != null)
            {
                PilotDataContext pilot = (PilotDataContext)PilotComboBox.SelectedItem;

                UpgradesOfPilot.Text = "";
                foreach (XWingUpgrades item in pilot.source.slots)
                {
                    UpgradesOfPilot.Text += Data.GetMarkFromUpgrade(item);
                }
            }
        }

        private void AddShipToFleet(object sender, MouseButtonEventArgs e)
        {
            if (PilotComboBox.SelectedItem != null)
            {
                PilotDataContext pilotData = (PilotDataContext)PilotComboBox.SelectedItem;
                ShipCreatorDataContext ship = new ShipCreatorDataContext(pilotData.source, GenerateShipID());
                ship.RefreshCost();
                Fleet.Items.Add(ship);
                SetPoints();
            }
        }
        public int GenerateShipID()
        {
            Random rnd = new Random();
            int Id = rnd.Next(0, 10000000);

            while (Ids.Contains(Id))
            {
                Id = rnd.Next(0, 10000000);
            }

            Ids.Add(Id);

            return Id;
        }
        private void ImportShip_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "XWingFiles (*.XWShip; *.XWFleet)|*.XWShip; *.XWFleet";

            if (openFileDialog.ShowDialog() == true)
            {
                Import(openFileDialog.FileNames);
            }
        }
        public void Import(string[] files)
        {
            foreach (string filename in files)
            {
                string[] type = filename.Split('.');
                if (type[1] == "XWShip")
                {
                    string Text = System.IO.File.ReadAllText(filename);

                    string[] one = Text.Split('|');

                    string[] Ship_Pilot = one[0].Split('&');

                    string ShipName = Ship_Pilot[0];
                    string PilotName = Ship_Pilot[1];

                    string[] Upgrades = one[1].Split('/');

                    Pilot pilot = new Pilot();

                    int Id = GenerateShipID();
                    foreach (var item in MainWindow.instance.Pilots)
                    {
                        if (item.Name == PilotName)
                        {
                            pilot = item;
                        }
                    }
                    ShipCreatorDataContext Ship = new ShipCreatorDataContext(pilot, Id);
                    Debug.WriteLine(Ship.ShipID);
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
                            Ship.UpgradeSlots.Insert(x, new ShipCreatorDataContext.ShipUpgradeDataContext(item, pilot, Id));
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
                                    shipUpgrade.SetStats(item, Id, new CardSelectWindow());
                                    item.SetUpgrade(shipUpgrade);
                                    break;
                                }
                            }

                        }
                    }
                    Ship.RefreshCost();
                    XWingSquadBuilder.instance.Fleet.Items.Add(Ship);
                }
                if (type[1] == "XWFleet")
                {
                    string Text = System.IO.File.ReadAllText(filename);
                    string[] ships = Text.Split('§');


                    foreach (var Xitem in ships)
                    {
                        if (!string.IsNullOrEmpty(Xitem))
                        {
                            Debug.WriteLine(Xitem);
                            string[] one = Xitem.Split('|');

                            string[] Ship_Pilot = one[0].Split('&');

                            string ShipName = Ship_Pilot[0];
                            string PilotName = Ship_Pilot[1];

                            string[] Upgrades = one[1].Split('/');

                            Pilot pilot = new Pilot();

                            int id = GenerateShipID();

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
                            Ship.RefreshCost();
                            XWingSquadBuilder.instance.Fleet.Items.Add(Ship);
                        }
                    }
                }
                SetPoints();
            }
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

        private void SaveShips_Click(object sender, RoutedEventArgs e)
        {
            if (XWingSquadBuilder.instance.Fleet.Items.Count > 0)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "XWingFleet (*.XWFleet)|*.XWFleet";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName += ".XWFleet";
                string text = "";
                int Cost = 0;
                foreach (ShipCreatorDataContext item in XWingSquadBuilder.instance.Fleet.Items)
                {
                    string content = $"{item.ShipName}&{item.PilotName}|";
                    foreach (var i in item.UpgradeSlots)
                    {
                        if (i.shipUpgrade != null)
                        {
                            content += $"{i.Upgrade}&{i.shipUpgrade.Name}/";
                        }
                    }


                    Cost += item.pilot.Points;
                    for (int i = 0; i < item.UpgradeSlots.Count; i++)
                    {
                        if (item.UpgradeSlots[i].shipUpgrade != null)
                        {
                            try
                            {
                                Cost += int.Parse(item.UpgradeSlots[i].shipUpgrade.Points);
                            }
                            catch (Exception x)
                            {
                                Debug.WriteLine(x);
                            }
                        }
                    }
                    text += content + "§";
                }
                saveFileDialog.FileName = $"Fleet-${Cost}.XWFleet";
                if (saveFileDialog.ShowDialog() == true)
                {

                    System.IO.File.WriteAllText(saveFileDialog.FileName, text);
                }
            }
        }

        private void FileDroper_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                Import(files);
            }
        }

        private void SaveButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(NameOfFleet.Text))
            {
                System.Windows.Forms.MessageBox.Show("The Save File Cannot be Saved! You Need To Change The Name", "The Save File Cannot be Saved!", MessageBoxButtons.OK);
            }
            else
            {
                SaveFleet();
                Window_Close();
            }
        }
    }
}
