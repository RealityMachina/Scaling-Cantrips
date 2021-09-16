using UnityModManagerNet;

namespace ScalingCantrips
{
    public class Settings : UnityModManager.ModSettings
    {

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }



        int CasterLevelsReq = 2;

        int MaxDice = 6;


        int DisruptCasterLevelsReq = 3;


        int DisruptMaxDice = 4;

     int VirtueCasterLevelsReq = 2;

        int VirtueMaxDice = 10;


        bool IgnoreDivineZap = false;


        int JoltingGraspLevelsReq = 2;


        int JoltingGraspMaxDice = 7;


        int DisruptLifeLevelsReq = 2;


        int DisruptLifeMaxDice = 6;

        bool DontAddUnholyZap = false;


        bool StartImmediately = true;
    }

}
