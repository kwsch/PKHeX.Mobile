using PKHeX.Core;

namespace PKHeX.ViewModels
{
    public class FeatureFlags
    {
        public FeatureFlags(PKM pkm) => Update(pkm);
        public bool Nature { get; private set; }
        public bool HeldItem { get; private set; }
        public bool Version { get; private set; }
        public bool Ball { get; private set; }
        public bool EggMet { get; private set; }
        public bool Met { get; private set; }
        public bool Dates { get; private set; }
        public bool Handler { get; private set; }
        public bool SID { get; private set; }
        public bool Ability { get; private set; }
        public bool Form { get; private set; }

        public void Update(PKM pkm)
        {
            Nature = pkm.Format >= 3;
            HeldItem = pkm.Format >= 2;
            Version = pkm.Format >= 3;
            Ball = pkm.Format >= 3;
            EggMet = pkm.Format >= 4;
            Met = pkm.Format >= 2;
            Dates = pkm.Format >= 4;
            Handler = pkm.Format >= 6;
            SID = pkm.Format >= 2;
            Form = pkm.Format >= 3;
            Ability = pkm.Format >= 3;
        }
    }
}