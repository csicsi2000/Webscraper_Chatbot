using Backend.Logic.Data.Json;
using General.Interfaces.Data;
using log4net;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Backend.Logic.Utils
{
    public class SettingsManager
    {
        private readonly object lockObject = new object();
        private ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string SettingsFilePath = "webscrapeSettings.json";
        private IServerSettings serverSettings;

        public SettingsManager()
        {
            LoadFile();
        }

        public IServerSettings GetServerSettings()
        {
            lock (lockObject)
            {
                return serverSettings;
            }
        }

        public bool SetServerSettings(IServerSettings newSettings)
        {
            lock (lockObject)
            {
                try
                {
                    newSettings.ExcludedUrls = newSettings.ExcludedUrls.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                    if (!newSettings.RootUrl.EndsWith("/"))
                    {
                        newSettings.RootUrl = newSettings.RootUrl + "/";
                    }
                    serverSettings = newSettings;

                    SaveFile();
                    return true;
                }
                catch (Exception ex)
                {
                    _log4.Error(ex.Message);
                    return false;
                }
            }
        }

        private void SaveFile()
        {
            lock (lockObject)
            {
                try
                {
                    string json = JsonSerializer.Serialize(serverSettings);
                    File.WriteAllText(SettingsFilePath, json);
                }
                catch (Exception ex)
                {
                    _log4.Error(ex.Message);
                }
            }
        }

        private void LoadFile()
        {
            lock (lockObject)
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
                    _log4.Error("New server settings file was not in the correct format.");
                    _log4.Error(ex.Message);
                }
            }
        }
    }
}
