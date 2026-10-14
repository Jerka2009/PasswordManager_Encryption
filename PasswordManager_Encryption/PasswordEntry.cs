using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordManager_Encryption
{
    public class PasswordEntry
    {
        public string Password { get; set; }
        public byte[] Hash { get; set; }
    }
}
