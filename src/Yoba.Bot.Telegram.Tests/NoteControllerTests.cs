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
                await Handle($"ёба добавь в заметку {_note.Name} : {_note.Content}");
                var note = await _dao.FindNote(_note.Name);
                note.Content.Should().Be(_note.Content);
                //Append line to note
                var line = Guid.NewGuid().ToString();
                await Handle($"ёба добавь в заметку {_note.Name} : {line}");
                note = await _dao.FindNote(_note.Name);
                _note.Content.Should().ContainAll(_note.Content.Split(Environment.NewLine));
                note.Content.Should().Contain(line);
                //Reset note to original content
                await Handle($"ёба обнови заметку {_note.Name} : {_note.Content}");
                note = await _dao.FindNote(_note.Name);
                note.Content.Should().Be(_note.Content);
            }
            
            //ShowNote
            {
                var result = await Handle($"ёба покажи заметку {_note.Name}");
                result.Response.Text.Should().ContainAll(_note.Content.Split(Environment.NewLine));
            }

            var note2 = new YobaNote
            {
                Content = "aaaaa" + Environment.NewLine + "bbb",
                DisplayName = "baz",
                Name = "baz",
                Created = DateTime.Now,
                Updated = DateTime.Now
            };
            await _dao.AddOrUpdateNote(note2);
            
            //ListNotes
            {
                var result = await Handle($"ёба покажи список заметок");
                result.Response.Text.Should().ContainAll(_note.Name, note2.DisplayName);
            }
            
            //DelNoteLine
            {
                await Handle($"ёба удали 1 из {note2.Name}");
                var tmp = await _dao.FindNote(note2.Name);
                tmp.Content.Should().Contain("bbb");
                tmp.Content.Should().NotContain("aaaaa");
                //Empty note should be deleted
                await Handle($"ёба удали 1 из {note2.Name}");
                tmp = await _dao.FindNote(note2.Name);
                tmp.Should().BeNull();
            }
        }

        async Task<Result<Message>> Handle(string text, string username = null)
        {
            var message = Setup.Message(text, username);
            var result = await _controller.Handle(message);
            result.Status.Should().Be(Status.Success);
            return (Result<Message>) result;
        }
    }
}