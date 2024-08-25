using Dawnsbury.Modding;

namespace Shifter
{
    public class ModLoader
    {
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            Dawnsbury.Core.CharacterBuilder.FeatsDb.AllFeats.All.AddRange(Shifter.LoadAll());
        }
     }
}
