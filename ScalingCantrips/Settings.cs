using UnityModManagerNet;

namespace ScalingCantrips
{
    public class Settings : UnityModManager.ModSettings
    {

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }



        public int CasterLevelsReq = 2;

        public int MaxDice = 6;


        public int DisruptCasterLevelsReq = 3;


        public  int DisruptMaxDice = 4;

        public int VirtueCasterLevelsReq = 2;

        public  int VirtueMaxDice = 10;


        public bool IgnoreDivineZap = false;


        public int JoltingGraspLevelsReq = 2;


        public int JoltingGraspMaxDice = 7;


        public int DisruptLifeLevelsReq = 2;


        public int DisruptLifeMaxDice = 6;

        public  bool DontAddUnholyZap = false;


        public bool StartImmediately = true;
    }

}
