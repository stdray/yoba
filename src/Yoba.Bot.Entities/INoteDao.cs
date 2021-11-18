namespace Yoba.Bot.Entities;

public interface INoteDao 
{
    Task<IReadOnlyCollection<YobaNote>> GetNotes(CancellationToken cancel = default);
    Task<YobaNote> FindNote(string name,CancellationToken cancel = default);
    Task DeleteNote(YobaNote note,CancellationToken cancel = default);
    Task AddOrUpdateNote(YobaNote note,CancellationToken cancel = default);
}