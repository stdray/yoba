using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Yoba.Bot.Entities
{
    public interface INoteDao 
    {
        Task<IReadOnlyCollection<YobaNote>> GetNotes(CancellationToken cancel = default);
        Task<YobaNote> FindNote(string name,CancellationToken cancel = default);
        Task DeleteNote(YobaNote note,CancellationToken cancel = default);
        Task UpdateNote(YobaNote note,CancellationToken cancel = default);
    }
}