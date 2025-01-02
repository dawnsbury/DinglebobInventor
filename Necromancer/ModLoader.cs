using Dawnsbury.Modding;

namespace Necromancer
{
    public class ModLoader
    {
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            Necromancer.LoadSpells();
            Dawnsbury.Core.CharacterBuilder.FeatsDb.AllFeats.All.AddRange(Necromancer.LoadAll());
        }
     }
}
