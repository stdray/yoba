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
            using (var db = _factory.Create())
                return await YobaNotes(db).ToListAsync(cancel);
        }

        public async Task<YobaNote> FindNote(string name, CancellationToken cancel = default)
        {
            using (var db = _factory.Create())
                return await YobaNotes(db)
                    .SingleOrDefaultAsync(x => x.Name == name, cancel);
        }

        public Task DeleteNote(YobaNote note, CancellationToken cancel = default)
        {
            using (var db = _factory.Create())
                return db.Notes
                    .Where(x => x.NoteName == note.Name)
                    .DeleteAsync(cancel);
        }


        public Task UpdateNote(YobaNote note, CancellationToken cancel = default)
        {
            using (var db = _factory.Create())
                return db.Notes
                    .Where(x => x.NoteName == note.Name)
                    .Set(x => x.Content, _ => note.Content)
                    .UpdateAsync(cancel);
        }

        static IQueryable<YobaNote> YobaNotes(YobaDb db) =>
            db.Notes
                .Select(x => new YobaNote
                {
                    Content = x.Content,
                    Created = x.Added,
                    Name = x.NoteName,
                    Updated = x.Updated,
                    DisplayName = x.DisplayNoteName
                });
    }
}