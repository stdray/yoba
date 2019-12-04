using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yoba.Bot.Entities;
using Yoba.Bot.RegularExpressions;
using static Yoba.Bot.RegularExpressions.Dsl;
using static Yoba.Bot.Telegram.Shared;

namespace Yoba.Bot.Telegram
{
    public class NoteController : Controller<Message>
    {
        readonly ITelegramBotClient _telegram;
        readonly INoteDao _dao;

        public NoteController(IEnumerable<IProvider<Message>> providers, ITelegramBotClient telegram, INoteDao dao)
            : base(providers)
        {
            _telegram = telegram;
            _dao = dao;

            AddOrUpdateNote();
            ShowNote();
            DelNoteLine();
            ListNotes();
        }

        void AddOrUpdateNote()
        {
            MatchHandle<Message> MakeHandle(bool update) =>
                async (request, match, cancel) =>
                {
                    var data = match.Value("data");
                    var name = match.Value("name").Trim();
                    var note = await _dao.FindNote(name, cancel);
                    if (note == null)
                    {
                        note = new YobaNote
                        {
                            Content = data,
                            Created = DateTime.Now,
                            Name = name,
                            DisplayName = name
                        };
                    }
                    else
                    {
                        var content = update ? data : note.Content + Environment.NewLine + data;
                        note.Content = string.Join(Environment.NewLine, Lines(content));
                    }
                    await Save(note, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Ок", cancel));
                };

            this.AddReRule(
                bot + (s + "добавь").opt + s + "в" + s + "заметку" + phrase("name") + ":" + phrase("data"),
                MakeHandle(false));
            this.AddReRule(
                bot + s + "обнови" + s + "заметку" + phrase("name") + ":" + phrase("data"),
                MakeHandle(true));
        }

        void ShowNote()
        {
            this.AddReRule(
                bot + s + "покажи" + s + "заметку" + phrase("name"),
                async (request, match, cancel) =>
                {
                    var name = match.Value("name").Trim();
                    var note = await _dao.FindNote(name, cancel);
                    if (note == null)
                        return Ok(await _telegram.ReplyAsync(request, "Заметка не найдена", cancel));
                    var lines = Lines(note.Content).Select((x, i) => $"{i + 1}. {x}");
                    var text = string.Join(Environment.NewLine, lines);
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                });
        }

        void ListNotes()
        {
            this.AddReRule(
                bot + s + "покажи" + s + "список" + s + "заметок",
                async (request, match, cancel) =>
                {
                    var notes = await _dao.GetNotes(cancel);
                    if (!notes.Any())
                        return Ok(await _telegram.ReplyAsync(request, "Не заметок", cancel));
                    var text = string.Join(Environment.NewLine, notes.Select(x => x.DisplayName ?? x.Name));
                    return Ok(await _telegram.ReplyAsync(request, text, cancel));
                });
        }

        void DelNoteLine()
        {
            this.AddReRule(
                bot + s + "удали" + (s + "строку").opt + s + digit.oneOrMore.group("line") + s + "из" +
                (s + "заметки").opt + phrase("name"),
                async (request, match, cancel) =>
                {
                    var line = int.Parse(match.Value("line"));
                    var name = match.Value("name").Trim();
                    var note = await _dao.FindNote(name, cancel);
                    if (note == null)
                        return Ok(await _telegram.ReplyAsync(request, "Заметка не найдена", cancel));
                    var lines = Lines(note.Content)
                        .Select((x, i) => new {Line = i + 1, Data = x})
                        .Where(x => x.Line != line)
                        .Select(x => x.Data);
                    note.Content = string.Join(Environment.NewLine, lines);
                    await Save(note, cancel);
                    return Ok(await _telegram.ReplyAsync(request, "Ок", cancel));
                });
        }

        async Task Save(YobaNote note, CancellationToken cancel)
        {
            if (string.IsNullOrEmpty(note.Content))
                await _dao.DeleteNote(note, cancel);
            else
                await _dao.AddOrUpdateNote(note, cancel);
        }


        static IEnumerable<string> Lines(string str) => str
            .Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x));
    }
}