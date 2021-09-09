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
        int DisruptCasterLevelsReq = 5;

        [JsonProperty]
        int DisruptMaxDice = 4;
        
        [JsonProperty]
        int VirtueCasterLevelsReq = 2;
        [JsonProperty]
        int VirtueMaxDice = 5;

        [JsonProperty]
        bool IgnoreDivineZap = false;

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
