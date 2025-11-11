using System.Collections.Generic;

namespace GoodnightComputer.Constants;

public static class Constants
{
    public static readonly string DB_NAME = "GoodnightComputer.db";

    public static readonly string DISCORD_URL = "https://discord.gg/8JAzymSXsj";

    public static readonly List<string> EXCLUSION_APPS = new()
    {
        // System Core Processes
        "system",
        "registry",
        "smss",                  // Windows Session Manager
        "csrss",                 // Client Server Runtime Process
        "wininit",              // Windows Start-Up Process
        "services",             // Services Control Manager
        "lsass",                // Windows Security
        "winlogon",             // Windows Logon
        "explorer",             // Windows Explorer
        "svchost",              // Service Host
    
        // System Services
        "taskmgr",              // Task Manager
        "dwm",                  // Desktop Window Manager
        "runtimeBroker",
        "fontdrvhost",
        "spoolsv",              // Print Spooler
        "searchHost",           // Windows Search
        "sihost",               // Shell Infrastructure Host
    
        // Security Related
        "msmpeng",             // Windows Defender
        "securityhealthservice",
        "nissrv",              // Network Inspection Service
    
        // Your Application
        "goodnightcomputer",    // Protect your own application
    
        // Critical UI Components
        "shellexperiencehost",
        "startmenuexperiencehost",
        "taskmgr",             // Task Manager
    
        // Device Related
        "audiodg",             // Audio Device Graph
        "dasHost",             // Device Association Framework
    
        // System Utilities
        "conhost",             // Console Window Host
        "dllhost",             // COM Surrogate
        "wmiprvse"             // WMI Provider Host
    };
}
