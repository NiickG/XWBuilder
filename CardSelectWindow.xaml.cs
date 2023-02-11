using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XWingBuilder
{
    /// <summary>
    /// Interaktionslogik für CardSelectWindow.xaml
    /// </summary>
    public partial class CardSelectWindow : Window
    {
        private List<ShipUpgrade> shipUpgradeList;
        public CardSelectWindow()
        {
            InitializeComponent();
        }
        public void SetStats(XWingUpgrades type, XWingFaction faction, Pilot pilot, ShipCreatorDataContext.ShipUpgradeDataContext source, int shipIndex)
        {
            AddUpgrade(faction, pilot, type, source, shipIndex);
            /*
            switch (type)
            {
                case XWingUpgrades.Title : Title_Upgrade(faction, pilot, type); break;
                case XWingUpgrades.Modification : Modification_Upgrade(faction, pilot, type); break;
                case XWingUpgrades.Crew : Crew_Upgrade(faction, pilot, type); break;
                default:
                    break;
            } 
            */
        }
        #region Upgrades
        private void AddUpgrade(XWingFaction faction, Pilot pilot, XWingUpgrades type, ShipCreatorDataContext.ShipUpgradeDataContext source, int shipIndex)
        {
            Cards.Items.Clear();
            List<ShipUpgrade> list = new List<ShipUpgrade>();

            switch (type)
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
                if (item.ShipName.Contains(pilot.ship.Name) || item.ShipName.Count == 0)
                {
                    if (item.faction == XWingFaction.Null || item.faction == faction)
                    {
                        if (item.shipSize.Count == 0 || item.shipSize.Contains(pilot.ship.shipSize))
                        {
                            item.SetStats(source, shipIndex, this);
                            Cards.Items.Add(item);
                        }
                    }
                }
            }

        }
        #endregion
        private void Window_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void TopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (shipUpgradeList == null)
            {
                shipUpgradeList = new List<ShipUpgrade>();
                foreach (var item in Cards.Items)
                {
                    shipUpgradeList.Add((ShipUpgrade)item);
                }
            }
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                Cards.Items.Clear();
                foreach (ShipUpgrade item in shipUpgradeList)
                {
                    if (item.Name.StartsWith(SearchBox.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        Cards.Items.Add(item);
                    }
                }
            }
            else
            {
                Cards.Items.Clear();

                foreach (ShipUpgrade item in shipUpgradeList)
                {
                    Cards.Items.Add(item);
                }
            }
        }
    }
}
