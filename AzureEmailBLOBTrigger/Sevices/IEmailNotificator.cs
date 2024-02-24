using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureEmailBLOBTrigger.Sevices
{
    public interface IEmailNotificator
    {
        public void SendEmail(string email, string title, string emailBody);
    }
}
