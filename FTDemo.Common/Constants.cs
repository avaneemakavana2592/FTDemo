using System;
using System.Collections.Generic;
using System.Text;

namespace FTDemo.Common
{
    public class Constants
    {
        public class EncryptionKeys
        {
            public const int keySize = 256;
            public const string hashAlgorithm = "SHA1";
            public const string passPhrase = "SkillTest";
            public const string saltValue = "LRT%YUR#VBNL@1";
            public const string initVector = "HR$2pIjHR$2pIj12";
        }
        public class Role
        {
            public const string Admin = "Admin";
            public const string ContentView = "ContentView";
        }
    }
}
