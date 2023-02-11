using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace XWingBuilder
{
    public class ShipCreatorDataContext
    {
        public string ShipName { get; set; }
        public string PilotName { get; set; }
        public string PilotSkill { get; set; }
        public string ShipImage { get; set; }

        public string PilotImage { get; set; }
        public string ShipCost { get; set; }
        public List<ShipUpgradeDataContext> UpgradeSlots { get; set; }

        public Pilot pilot { get; set; }

        public int ShipID;

        public string attack { get; set; }
        public string agility { get; set; }
        public string hull { get; set; }
        public string shields { get; set; }

        public string energy { get; set; }

        private ICommand _Remove;
        public ICommand Remove
        {
            get
            {
                return _Remove ?? (_Remove = new CommandHandler(() => RemoveThis(), () => true));
            }
        }
        private ICommand _SaveShip;
        public ICommand SaveShip
        {
            get
            {
                return _SaveShip ?? (_SaveShip = new CommandHandler(() => OnSaveShip(), () => true));
            }
        }
        public void RemoveThis()
        {
            for (int i = 0; i < XWingSquadBuilder.instance.Fleet.Items.Count; i++)
            {
                XWingSquadBuilder.instance.Fleet.Items.Remove(this);
                XWingSquadBuilder.instance.Ids.Remove(ShipID);

                XWingSquadBuilder.instance.SetPoints();
            }
        }


        public void RefreshCost()
        {
            int cost = pilot.Points;
            foreach (var item in UpgradeSlots)
            {
                if (item.shipUpgrade != null)
                {
                    cost += int.Parse(item.shipUpgrade.Points);
                }
            }

            ShipCost = cost.ToString();
        }
        private void OnSaveShip()
        {
            string content = $"{ShipName}&{PilotName}|";
            foreach (var item in UpgradeSlots)
            {
                if (item.shipUpgrade != null)
                {
                    content += $"{item.Upgrade}&{item.shipUpgrade.Name}/";
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XWingShip (*.XWShip)|*.XWShip";
            saveFileDialog.RestoreDirectory = true;
            int Cost = 0;
            Cost += pilot.Points;
            for (int i = 0; i < UpgradeSlots.Count; i++)
            {
                if (UpgradeSlots[i].shipUpgrade != null)
                {
                    try
                    {
                        Cost += int.Parse(UpgradeSlots[i].shipUpgrade.Points);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }
            string pn = PilotName;
            pn = pn.Replace('"', ' ');
            Debug.WriteLine(pn);
            pn = pn.Trim();
            saveFileDialog.FileName = $"{pn}-${Cost}.XWShip";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, content);
        }

        public void Refresh()
        {
            List<ShipUpgradeDataContext> upgrades = new List<ShipUpgradeDataContext>();
            upgrades.AddRange(UpgradeSlots);

            UpgradeSlots.Clear();
            foreach (ShipUpgradeDataContext item in upgrades)
            {
                UpgradeSlots.Add(item);
            }
        }

        public ShipCreatorDataContext(Pilot _pilot, int index, [Optional] List<ShipUpgradeDataContext> _upgrades)
        {
            ShipName = _pilot.ship.Name;
            Debug.WriteLine(_pilot.ship.Name);
            PilotName = _pilot.Name;
            ShipImage = _pilot.ship.ShipImage;
            PilotSkill = _pilot.Skill.ToString();
            PilotImage = _pilot.ImageSource;
            ShipID = index;
            UpgradeSlots = new List<ShipUpgradeDataContext>();
            foreach (var item in _pilot.slots)
            {
                UpgradeSlots.Add(new ShipUpgradeDataContext(item, _pilot, index));
            }
            if (_upgrades != null)
            {
                UpgradeSlots = _upgrades;
            }
            attack = _pilot.ship.attack.ToString();
            agility = _pilot.ship.agility.ToString();
            hull = _pilot.ship.hull.ToString();
            shields = _pilot.ship.shields.ToString();
            energy = _pilot.ship.energy.ToString();

            if (energy == null)
            {
                energy = "00";
            }

            this.pilot = _pilot;
        }
        public class ShipUpgradeDataContext
        {
            public string UpgradeName { get; set; }
            public string UpgradeImage { get; set; }
            public string UpgradeTypeImage { get; set; }
            public XWingUpgrades Upgrade { get; set; }


            public Pilot MyPilot;

            private ICommand _SelectCard;
            public ICommand SelectCard
            {
                get
                {
                    return _SelectCard ?? (_SelectCard = new CommandHandler(() => NewCardSelect(Upgrade), () => true));
                }
            }

            private ICommand _RemoveCard;
            public ICommand RemoveCard
            {
                get
                {
                    return _RemoveCard ?? (_RemoveCard = new CommandHandler(() => NewRemove(), () => true));
                }
            }

            public ShipUpgrade shipUpgrade;
            public int ShipID;

            public void NewRemove()
            {
                if (shipUpgrade != null)
                {
                    shipUpgrade.SetStats(this, ShipID, new CardSelectWindow());
                    shipUpgrade.NewSelectCard(true);
                }
            }
            public void NewRemoveCard()
            {
                shipUpgrade = null;

                UpgradeName = "";
                UpgradeImage = "";

                Debug.WriteLine(ShipID);
            }
            private void NewCardSelect(XWingUpgrades upgrade)
            {
                CardSelectWindow window = new CardSelectWindow();
                window.SetStats(Upgrade, XWingSquadBuilder.instance.Currentfaction, MyPilot, this, ShipID);
                window.Show();
            }
            public void SetUpgrade(ShipUpgrade upgrade)
            {
                shipUpgrade = upgrade;
                UpgradeName = upgrade.Name;
                UpgradeImage = upgrade.ImageSource;
            }
            public ShipUpgradeDataContext(XWingUpgrades upgrade, Pilot _MyPilot, int _ShipID)
            {
                Upgrade = upgrade;
                UpgradeTypeImage = Data.GetMarkFromUpgrade(Upgrade);
                MyPilot = _MyPilot;
                ShipID = _ShipID;
            }
        }
    }
}
