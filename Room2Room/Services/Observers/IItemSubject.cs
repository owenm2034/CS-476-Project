namespace Room2Room.Services.Observers
{
    public interface IItemSubject
    {
        void Attach(IItemObserver observer);
        void Detach(IItemObserver observer);
        void Notify();
        void UpdateItem(Item oldItem, Item newItem);
    }
}