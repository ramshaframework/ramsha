using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAppDemo
{
    public class SimpleMessageHandler
    {
        public void Handle(SimpleMessage message, ILogger<SimpleMessageHandler> logger)
        {
            logger.LogInformation("I got a message and the text is {Text}", message.Text);
        }
    }
}