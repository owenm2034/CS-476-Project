namespace Room2Room.Services.Observers
{
    public class ItemSubject : IItemSubject
    {
        public Item? OldItem { get; private set; }
        public Item? NewItem { get; private set; }

        private readonly List<IItemObserver> _observers = new List<IItemObserver>();

        // Constructor to initialize the subject with a list of observers
        public ItemSubject(IEnumerable<IItemObserver> observers)
        {
            foreach (var observer in observers)
            {
                Register(observer);
            }
        }

        public void Register(IItemObserver observer)
        {
            //Console.WriteLine($"Attaching observer: {observer.GetType().Name}");
            _observers.Add(observer);
        }

        public void Unregister(IItemObserver observer)
        {
            _observers.Remove(observer);
            //Console.WriteLine($"Detaching observer: {observer.GetType().Name}");
        }

        public void Notify()
        {
            //Console.WriteLine($"Notifying {_observers.Count} observers...");
            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }

        public void UpdateItem(Item oldItem, Item newItem)
        {
            //Console.WriteLine($"Message from Subject: Item has been updated.");
            OldItem = oldItem;
            NewItem = newItem;
            Notify();
        }

    }
}