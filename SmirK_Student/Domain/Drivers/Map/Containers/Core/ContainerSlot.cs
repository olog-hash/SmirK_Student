using SmirK_Student.Engine.Constants;

namespace SmirK_Student.Domain.Drivers.Map.Containers.Core
{

    public abstract class ContainerSlot<T>
    {
        public int OccupatedSlots => _occupatedSlots;
        public bool IsEmpty => OccupatedSlots == 0;
        
        protected T? _singleSlot;
        protected List<T>? _multiSlot;

        private int _occupatedSlots;
        
        public bool TryAdd(T item)
        {
            if (IsFull())
            {
                return false;
            }
            
            if (IsSingleContainer())
            {
                _singleSlot = item;

            }
            else
            {
                _multiSlot ??= new List<T>();
                _multiSlot.Add(item);
            }
            
            _occupatedSlots++;
            return true;
        }

        public bool Remove(T item)
        {
            if (IsEmpty)
            {
                return false;
            }
            
            if (IsSingleContainer())
            {
                _singleSlot = default(T);
                _occupatedSlots--;
                return true;
            }
            else
            {
                if (_multiSlot != null && _multiSlot.Remove(item))
                {
                    if (_multiSlot.Count == 0)
                    {
                        _multiSlot = null;
                    }

                    _occupatedSlots--;
                    return true;
                }
            }
            
            return false;
        }
        
        public abstract T GetByID(int id);
        
        public abstract bool Contains(int id);

        public void Clear()
        {
            _singleSlot = default(T);
            _multiSlot = null;
            _occupatedSlots = 0;
        }

        public bool IsFull()
        {
            return SmirKConstants.MaxDriversAtSlot <= 0 || OccupatedSlots >= SmirKConstants.MaxDriversAtSlot;
        }

        public bool IsSingleContainer()
        {
            return SmirKConstants.MaxDriversAtSlot == 1;
        }
    }
}