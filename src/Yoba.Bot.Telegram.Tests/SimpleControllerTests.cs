using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Telegram.Bot.Types;
using Xunit;
using Xunit.Abstractions;
using Yoba.Bot.Telegram;

namespace Yoba.Bot.Tests
{
    public class SimpleControllerTests
    {
        readonly ITestOutputHelper _output;
        readonly SimpleController _controller;

        public SimpleControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _controller = Setup.GetService<SimpleController>();
        }


        [Theory]
        [InlineData("1", "  1  ")]
        [InlineData("pong", "  пинг  ")]
        [InlineData("pong", "  ping  ")]
        [InlineData("1.0.0.0", "  ёба версия  ")]
        [InlineData("боксифаг", "  ёба вангуй кто лелка: зхц, боксифаг, котовский, или пес ?  ")]
        [InlineData("нет", "  yoba гадай математики = гуманитарии ?  ")]
        [InlineData("гуманитарии", "  yoba вангуй математики, гуманитарии или транскоалиция   ")]
        [InlineData("batmansy", "  yoba транслитом батмансы")]
        public async Task Bot_Response_ShouldBe_Expected(string expected, string request)
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