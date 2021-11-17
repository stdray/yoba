using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Yoba.Bot.Entities;

namespace Yoba.Bot.Db
{
    public class NoteDao : INoteDao
    {
        readonly IYobaDbFactory _factory;

        public NoteDao(IYobaDbFactory factory)
        {
            _factory = factory;
        }

        public async Task<IReadOnlyCollection<YobaNote>> GetNotes(CancellationToken cancel = default)
        {
            using var db = _factory.Create();
            return await YobaNotes(db).ToListAsync(cancel);
        }

        public async Task<YobaNote> FindNote(string name, CancellationToken cancel = default)
        {
            using var db = _factory.Create();
            return await YobaNotes(db)
                .SingleOrDefaultAsync(x => x.Name == name, cancel);
        }

        public Task DeleteNote(YobaNote note, CancellationToken cancel = default)
        {
            using var db = _factory.Create();
            return db.Notes
                .Where(x => x.NoteName == note.Name)
                .DeleteAsync(cancel);
        }


        public async Task AddOrUpdateNote(YobaNote note, CancellationToken cancel = default)
        {
            var now = DateTime.Now;
            using var db = _factory.Create();
            var count = await db.Notes
                .Where(x => x.NoteName == note.Name)
                .Set(x => x.Content, _ => note.Content)
                .Set(x => x.Updated, _ => now)
                .UpdateAsync(cancel);
            if (count == 1)
                return;
            if (count > 1)
                throw new InvalidOperationException("Conflict");
            await db.Notes.InsertAsync(() => new Note
            {
                NoteName = note.Name,
                DisplayNoteName = note.DisplayName,
                Content = note.Content,
                Added = now,
                Updated = now,
            }, cancel);
        }

        static IQueryable<YobaNote> YobaNotes(YobaDb db) =>
            db.Notes
                .Select(x => new YobaNote
                {
                    Content = x.Content,
                    Created = x.Added,
                    Updated = x.Updated,
                    Name = x.NoteName,
                    DisplayName = x.DisplayNoteName,
                });
    }
}