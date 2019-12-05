using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Yoba.Bot.Entities;


namespace Yoba.Bot.Tests
{
    public class NoteDaoTests : IClassFixture<NoteFixture>
    {
        readonly INoteDao _dao;
        readonly YobaNote _note;

        public NoteDaoTests(NoteFixture fixture)
        {
            _dao = fixture.Scope.ServiceProvider.GetService<INoteDao>();
            _note = fixture.Note;
        }

        
        [Fact]
        public async Task Test()
        {
            //public async Task Note_ShouldBeCreated()
            {
                await _dao.AddOrUpdateNote(_note);
                var dst = await _dao.FindNote(_note.Name);
                var notes = await _dao.GetNotes();
                dst.Content.Should().Be(_note.Content);
                dst.Name.Should().Be(_note.Name);
                dst.DisplayName.Should().Be(_note.DisplayName);
                var list = await _dao.GetNotes();
                list.Count.Should().Be(1);
                list.Single().Name.Should().Be(_note.Name);
            }
            //public async Task Note_ShouldBeUpdated()
            {
                var src = await _dao.FindNote(_note.Name);
                src.Content += Environment.NewLine + "line3";
                await _dao.AddOrUpdateNote(src);
                var dst = await _dao.FindNote(src.Name);
                dst.Content.Should().Be(src.Content);
            }
            //public async Task Note_ShouldBeDeleted()
            {
                await _dao.DeleteNote(_note);
                var dst = await _dao.FindNote(_note.Name);
                dst.Should().BeNull();
                var list = await _dao.GetNotes();
                list.Count.Should().Be(0);
            }
        }
    }
}