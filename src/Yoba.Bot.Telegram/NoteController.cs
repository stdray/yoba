using System.Collections.Generic;
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

        protected NoteController(IEnumerable<IProvider<Message>> providers, ITelegramBotClient telegram, INoteDao dao) 
            : base(providers)
        {
            _telegram = telegram;
            _dao = dao;

            DelNoteLine();
            AddToNote();
            UpdateNote();
            ShowNote();
            ListNotes();
        }

        readonly Re note = re("заметку") + s.oneOrMore; 
        
        void AddToNote()
        {
            this.AddReRule(
                bot + add + "добавь" + s.oneOrMore ++ name("name"),
                async (request, match, cancel) =>
                {
                    var prof = await _dao.FindProfile(match.Value("name"), cancel);
                    return Ok(await _telegram.ReplyAsync(request, prof.ToString(), cancel));
                });
        }

        void UpdateNote()
        {
            this.AddReRule(
                bot + show + profile + name("name"),
                async (request, match, cancel) =>
                {
                    var prof = await _dao.FindProfile(match.Value("name"), cancel);
                    return Ok(await _telegram.ReplyAsync(request, prof.ToString(), cancel));
                });
        }

        void ShowNote()
        {
            this.AddReRule(
                bot + show + profile + name("name"),
                async (request, match, cancel) =>
                {
                    var prof = await _dao.FindProfile(match.Value("name"), cancel);
                    return Ok(await _telegram.ReplyAsync(request, prof.ToString(), cancel));
                });
        }

        void ListNotes()
        {
            this.AddReRule(
                bot + show + profile + name("name"),
                async (request, match, cancel) =>
                {
                    var prof = await _dao.FindProfile(match.Value("name"), cancel);
                    return Ok(await _telegram.ReplyAsync(request, prof.ToString(), cancel));
                });
        }

        void DelNoteLine()
        {
            this.AddReRule(
                bot + show + profile + name("name"),
                async (request, match, cancel) =>
                {
                    var prof = await _dao.FindProfile(match.Value("name"), cancel);
                    return Ok(await _telegram.ReplyAsync(request, prof.ToString(), cancel));
                });
        }
    }
}