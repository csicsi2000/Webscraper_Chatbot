using Backend.Logic.Data.Json;
using General.Interfaces.Data;
using log4net;
using System.Text.Json;

namespace Backend.Logic.Utils
{
    public class SettingsManager
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string SettingsFilePath = "webscrapeSettings.json";
        private IServerSettings serverSettings; // Add a field to store the loaded settings

        public SettingsManager()
        {
            LoadFile();
        }

        public IServerSettings GetServerSettings()
        {
            return serverSettings; // Return the loaded settings
        }

        public bool SetServerSettings(IServerSettings newSettings)
        {
            newSettings.ExcludedUrls = newSettings.ExcludedUrls.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            if (!newSettings.RootUrl.EndsWith("/"))
            {
                newSettings.RootUrl = newSettings.RootUrl + "/";
            }
            serverSettings = newSettings; // Update the settings field

            SaveFile(); // Save the updated settings
            return true; // Indicate success or failure based on your logic
        }

        private void SaveFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(serverSettings);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                // Handle exceptions that might occur during file saving
                // Log or throw the exception based on your error-handling strategy
            }
        }

        private void LoadFile()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    serverSettings = JsonSerializer.Deserialize<ServerSettings>(json);
                }
                else
                {
                    serverSettings = new ServerSettings();
                    _log4.Info("New server settings file was created.");
                }
            }
            catch (Exception ex)
            {
                serverSettings = new ServerSettings();
                _log4.Info("New server settings file was not in the correct format.");
            }
        }
    }
}
