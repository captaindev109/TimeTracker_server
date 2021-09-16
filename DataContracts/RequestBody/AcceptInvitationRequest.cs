using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts.RequestBody
{
    public class AcceptInvitationRequest
    {
        public string senderEmail { get; set; }
        public string token { get; set; }
    }
}
