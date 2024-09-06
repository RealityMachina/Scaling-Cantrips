using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System.IO;
namespace ScalingCantrips.Config
{
    public class Scaling : IUpdatableSettings
    {
        [JsonProperty]
        int CasterLevelsReq = 2;
        [JsonProperty]
        int MaxDice = 6;

        [JsonProperty]
        int DisruptCasterLevelsReq = 3;

        [JsonProperty]
        int DisruptMaxDice = 4;
        
        [JsonProperty]
        int VirtueCasterLevelsReq = 2;
        [JsonProperty]
        int VirtueMaxDice = 10;

        [JsonProperty]
        bool IgnoreDivineZap = false;

        [JsonProperty]
        int JoltingGraspLevelsReq = 2;

        [JsonProperty]
        int JoltingGraspMaxDice = 7;

        [JsonProperty]
        int DisruptLifeLevelsReq = 2;

        [JsonProperty]
        int DisruptLifeMaxDice = 6;

        [JsonProperty]
        bool DontAddUnholyZap = false;

        [JsonProperty]
        bool StartImmediately = true;

        [JsonProperty]
        bool DontAddFirebolt = false;
        public void OverrideSettings(IUpdatableSettings userSettings)
        {
            var loadedSettings = userSettings as Scaling;


            CasterLevelsReq = Math.Max(loadedSettings.CasterLevelsReq, 1); //let's not see what happens when the game divides by zero
            MaxDice = Math.Max(loadedSettings.MaxDice, 1); //always at least one

            VirtueCasterLevelsReq = Math.Max(loadedSettings.VirtueCasterLevelsReq, 1);
            VirtueMaxDice = Math.Max(loadedSettings.VirtueMaxDice, 1);
            DisruptMaxDice = Math.Max(loadedSettings.DisruptMaxDice, 1);
            DisruptCasterLevelsReq = Math.Max(loadedSettings.DisruptCasterLevelsReq, 1);
            IgnoreDivineZap = loadedSettings.IgnoreDivineZap; //either it's false or not
            JoltingGraspLevelsReq = Math.Max(loadedSettings.JoltingGraspLevelsReq, 1);
            JoltingGraspMaxDice = Math.Max(loadedSettings.JoltingGraspMaxDice, 1); ; //either it's false or not
            DisruptLifeLevelsReq = Math.Max(loadedSettings.DisruptLifeLevelsReq, 1);
            DisruptLifeMaxDice = Math.Max(loadedSettings.DisruptLifeMaxDice, 1);
            DontAddUnholyZap = loadedSettings.DontAddUnholyZap;
            DontAddFirebolt = loadedSettings.DontAddFirebolt;
            StartImmediately = loadedSettings.StartImmediately;
        }

        public bool UseOnePlusDivStep()
        {
            return StartImmediately;
        }
        public bool UnholyZapUnavailable()
        {
            return DontAddUnholyZap;
        }
        public int GetDisruptLifeMaxDice()
        {
            return DisruptLifeMaxDice;
        }

        public int GetDisruptLifeLevelsReq()
        {
            return DisruptLifeLevelsReq;
        }

        public int GetJoltingGraspMaxDice()
        {
            return JoltingGraspMaxDice;
        }

        public int GetJoltingGraspLevelsReq()
        {
            return JoltingGraspLevelsReq;
        }


        public int GetMaxDice()
        {
            return MaxDice;
        }

        public int GetCasterLevelsReq()
        {
            return CasterLevelsReq;
        }

        public int GetDisruptMaxDice()
        {
            return DisruptMaxDice;
        }

        public int GetDisruptCasterLevelsReq()
        {
            return DisruptCasterLevelsReq;
        }

        public int GetVirtueMaxDice()
        {
            return VirtueMaxDice;
        }

        public int GetVirtueCasterLevelsReq()
        {
            return VirtueCasterLevelsReq;
        }

        public bool GetIgnoreDivineZap()
        {
            return IgnoreDivineZap;
        }
    }
}
