﻿using General.Interfaces.Backend.Components;

namespace General.Interfaces.Data
{
    public interface IServerSettings
    {
        string DbPath { get; set; }
        IList<string> ExcludedUrls { get; set; }
        string RootUrl { get; set; }
        string WaitedClassName { get; set; }
    }
}