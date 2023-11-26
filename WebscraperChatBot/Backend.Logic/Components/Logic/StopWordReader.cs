using Backend.Logic.Data;
using Backend.Logic.Data.Json;
using log4net;
using System.Text.Json;

namespace Backend.Logic.Components.Logic
{
    public class StopWordReader
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IList<string> GetStopwords()
        {
            var files = Directory.GetFiles(Path.Combine(CommonValues.folderLoc,"Resources/StopWords"));
            var stopWords = new List<string>();
            foreach (var file in files)
            {
                string text = File.ReadAllText(file);
                var words = JsonSerializer.Deserialize<StopWords>(text);
                if (words == null)
                {
                    _log4.Warn("Stopword File was not parsed. " + file);
                    continue;
                }
                stopWords.AddRange(words.Words);
            }
            return stopWords;
        }
    }
}
