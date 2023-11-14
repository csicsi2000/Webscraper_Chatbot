using Backend.Logic.Data;
using NTextCat;

namespace Backend.Logic.Components.Logic
{
    internal class LanguageIdentifier
    {
        public LanguageInfo GetLanguageInfo(string text)
        {
            var factory = new RankedLanguageIdentifierFactory();
            var identifier = factory.Load(Path.Combine(CommonValues.folderLoc, "Resources/TextCat/Wiki82.profile.xml")); // can be an absolute or relative path. Beware of 260 chars limitation of the path length in Windows. Linux allows 4096 chars.
            var languages = identifier.Identify(text);
            var mostCertainLanguage = languages.FirstOrDefault();

            return mostCertainLanguage.Item1;
        }
    }
}
