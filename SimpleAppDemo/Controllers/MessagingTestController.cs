using System;
using Microsoft.AspNetCore.Mvc;
using Ramsha;
using Ramsha.AspNetCore.Mvc;
using Ramsha.LocalMessaging;
using Ramsha.LocalMessaging.Abstractions;

namespace SimpleAppDemo.Controllers;

public class MessagingTestController : RamshaApiController
{
    public async Task<IActionResult> TestCommandWithRamshaResult()
    => await Command(new TestCommandWithRamshaResult());
}


public class TestCommandWithRamshaResult : IRamshaCommand
{

}

public class TestCommandHandlerWithRamshaResult : IRamshaCommandHandler<TestCommandWithRamshaResult>
{
    public Task<IRamshaResult> HandleAsync(TestCommandWithRamshaResult message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}