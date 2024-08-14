using Dawnsbury.Core;
using Dawnsbury.Modding;

namespace Inventor
{
    public class ModLoader
    {
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            Dawnsbury.Core.CharacterBuilder.FeatsDb.AllFeats.All.AddRange(Inventor.LoadAll());
        }
     }
}
