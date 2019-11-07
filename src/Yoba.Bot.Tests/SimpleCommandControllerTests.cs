using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telegram.Bot.Types;
using Xunit;
using Xunit.Abstractions;
using Yoba.Bot.Telegram.Controllers;

namespace Yoba.Bot.Tests
{
    public class SimpleCommandControllerTests
    {
        readonly ITestOutputHelper _output;
        readonly SimpleCommandController _controller;

        public SimpleCommandControllerTests(ITestOutputHelper output)
        {
            this._output = output;
            _controller = Setup.GetService<SimpleCommandController>();
        }


        [Theory]
        [InlineData("1", "  1  ")]
        [InlineData("pong", "  пинг  ")]
        [InlineData("pong", "  ping  ")]
        [InlineData("1.0.0.0", "  ёба версия  ")]
        public async Task Bot_Response_ShouldBeExpected(string expected, string request)
        {
            var req = Setup.Message(request);
            var res1 =  await _controller.Handle(req, CancellationToken.None);
            if (res1.Status == Status.Fail)
                _output.WriteLine(res1.Exception.ToString());
            res1.Status.Should().Be(Status.Success);
            var res2 = (Result<Message>) res1;
            res2.Response.Text.Should().Be(expected);
        }
      
    }
}