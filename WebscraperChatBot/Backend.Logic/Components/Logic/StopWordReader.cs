using Backend.Logic.Data;
using Backend.Logic.Data.Json;
using General.Interfaces.Backend.Logic;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
