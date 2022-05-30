﻿using Microsoft.Extensions.Configuration;
using System.IO;

namespace MeetingGrpc.Server.Tests.App_Infrastructure.Factories
{
    public static class ConfigurationFactory
    {
        public static IConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json", false, true)
                .Build();
            return config;
        }
    }
}
