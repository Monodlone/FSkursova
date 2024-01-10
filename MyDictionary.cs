namespace Kursova
{
    internal class MyDictionary
    {
        private readonly MyLinkedList<KeyValuePair<string, long>> _items = new();

        internal void Add(string key, long value)
        {
            if (!Contains(key, value))
            {
                _items.Add(new KeyValuePair<string, long>(key, value));
            }
        }

        internal bool ContainsKey(string key)
        {
            foreach (KeyValuePair<string, long> pair in _items)
            {
                if (pair.Key.Equals(key))
                    return true;
            }

            return false;
        }

        internal void Remove(string key, long value)
        {
            var node = _items.Head;

            while (node != null)
            {
                if (node.Value.Key.Equals(key) && node.Value.Value.Equals(value))
                {
                    _items.Remove(node.Value);
                    return;
                }
                node = node.Next;
            }
        }

        internal long GetValue(string key)
        {
            foreach (KeyValuePair<string, long> pair in _items)
            {
                if (pair.Key.Equals(key))
                    return pair.Value;
            }

            return -1;
        }

        internal long GetValueBackwards(string key)
        {
            _items.Reverse();
            foreach (KeyValuePair<string, long> pair in _items)
            {
                if (pair.Key.Equals(key))
                {
                    _items.Reverse();
                    return pair.Value;
                }
            }

            _items.Reverse();
            return -1;
        }

        private bool Contains(string key, long value)
        {
            foreach (KeyValuePair<string, long> pair in _items)
            {
                if (pair.Key.Equals(key) && pair.Value.Equals(value))
                    return true;
            }

            return false;
        }
    }
}
