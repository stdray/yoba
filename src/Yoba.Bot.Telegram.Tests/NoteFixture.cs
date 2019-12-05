using System;
using Yoba.Bot.Entities;

namespace Yoba.Bot.Tests
{
    public class NoteFixture : ServiceScopeFixture
    {
        public YobaNote Note { get; } = new YobaNote
        {
            Name = YobaNote.MakePkName("Display note name"),
            DisplayName = "Display note name",
            Content = "line1" + Environment.NewLine + "line2",
        };
    }
}