using System.Collections.Generic;

namespace Utils
{
    public class CircularEnumerable<T>
    {
        private readonly List<T> _items; 
        private int _index = -1;

        public CircularEnumerable(IEnumerable<T> items)
        {
            _items = new List<T>(items);
        }

        public CircularEnumerable()
        {
            _items = new List<T>();
        }

        public bool CanCycle()
        {
            return _items.Count > 0;
        }
        
        public T Next() {
            _index = (_index + 1) % _items.Count;

            return _items[_index];
        }

        public T Previous() {
            _index = (_items.Count + _index-1) % _items.Count;

            return _items[_index];
        }
    }
}