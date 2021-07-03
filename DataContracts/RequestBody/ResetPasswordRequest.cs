using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts.RequestBody
{
    public class ResetPasswordRequest
    {
        public string password { get; set; }
        public string token { get; set; }
    }
}
