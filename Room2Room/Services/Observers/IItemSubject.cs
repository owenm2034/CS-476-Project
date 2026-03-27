namespace Room2Room.Services.Observers
{
    public interface IItemSubject
    {
        void Register(IItemObserver observer);
        void Unregister(IItemObserver observer);
        void Notify();
        void UpdateItem(Item oldItem, Item newItem);
    }
}