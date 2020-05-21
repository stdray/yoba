using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Xunit;
using Yoba.Bot.Entities;
using Yoba.Bot.Telegram;

namespace Yoba.Bot.Tests
{
    public class NoteControllerTests : IClassFixture<NoteFixture>
    {
        readonly NoteController _controller;
        readonly INoteDao _dao;
        readonly YobaNote _note;

        public NoteControllerTests(NoteFixture fixture)
        {
            _controller = fixture.Scope.ServiceProvider.GetService<NoteController>();
            _dao = fixture.Scope.ServiceProvider.GetService<INoteDao>();
            _note = fixture.Note;
        }

        [Fact]
        public async Task Test()
        {
            //AddOrUpdateNote
            {
                //Create new note from line
                await Handle($"ёба добавь в заметку {_note.DisplayName} : {_note.Content}");
                var note = await _dao.FindNote(_note.Name);
                note.Content.Should().Be(_note.Content);
                //Append line to note
                var line = Guid.NewGuid().ToString();
                await Handle($"ёба добавь в заметку {_note.DisplayName} : {line}");
                note = await _dao.FindNote(_note.Name);
                _note.Content.Should().ContainAll(_note.Content.Split('\r', '\n'));
                note.Content.Should().Contain(line);
                //Reset note to original content
                await Handle($"ёба обнови заметку {_note.DisplayName} : {_note.Content}");
                note = await _dao.FindNote(_note.Name);
                note.Content.Should().Be(_note.Content);
            }

            //ShowNote
            {
                var result = await Handle($"ёба покажи заметку {_note.DisplayName}");
                result.Response.Text.Should().ContainAll(_note.Content.Split('\r', '\n'));
            }

            var note2 = new YobaNote
            {
                Content = "aaaaa" + Environment.NewLine + "bbb",
                DisplayName = "baz",
                Created = DateTime.Now,
                Updated = DateTime.Now
            };
            await _dao.AddOrUpdateNote(note2);

            //ListNotes
            {
                async Task checkList(string msg)
                {
                    var result = await Handle(msg);
                    result.Response.Text.Should().Contain(note2.DisplayName);
                }
                await checkList("ёба покажи список заметок");
                await checkList("ёба покажи заметки");
            }

            //DelNoteLine
            {
                await Handle($"ёба удали 1 из {note2.DisplayName}");
                var tmp = await _dao.FindNote(note2.Name);
                tmp.Content.Should().Contain("bbb");
                tmp.Content.Should().NotContain("aaaaa");
                //Empty note should be deleted
                await Handle($"ёба удали 1 из {note2.DisplayName}");
                tmp = await _dao.FindNote(note2.Name);
                tmp.Should().BeNull();
            }
        }

        async Task<Result<Message>> Handle(string text, string username = null)
        {
            var message = Setup.Message(text, username);
            var result = await _controller.Handle(message);
            result.Status.Should().Be(Status.Success);
            return (Result<Message>)result;
        }
    }
}