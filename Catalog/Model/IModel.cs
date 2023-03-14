namespace Catalog.Model
{
    public interface IModel : ITimestamps
    {
        int Id { get; }

        bool IsNew { get; }
    }
}
