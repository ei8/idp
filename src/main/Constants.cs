﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.IdP
{
    public class Constants
    {
        public struct Paths
        {
            public const string Logout = "/Account/LogoutCallback";
        }
        public struct EnvironmentVariableKeys
        {
            public const string ClientsXamarin = "CLIENTS_D23";
            public const string IssuerUri = "ISSUER_URI";
            public const string ConnectionStringsDefault = "CONNECTION_STRINGS_DEFAULT";
            public const string HostNameExpected = "HOST_NAME_EXPECTED";
            public const string HostNameReplacement = "HOST_NAME_REPLACEMENT";
        }
    }
}