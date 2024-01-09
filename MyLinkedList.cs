using System.Collections;

namespace Kursova
{
    public class MyLinkedList<T> : IEnumerable
    {
        internal int Count { get; private set; }
        internal Node<T> Head { get; private set; }
        private Node<T> Last { get; set; }

        public void AddFirst(T value)
        {
            var newNode = new Node<T>(value);

            if(Head == null)
                Head = Last = newNode;
            else
            {
                newNode.Next = Head;
                Head = newNode;
            }

            Count++;
        }

        public void Add(T value)
        {
            var newNode = new Node<T>(value);
            if(Head == null)
                Head = Last = newNode;
            else
            {
                Last.Next = newNode;
                Last = newNode;
            }

            Count++;
        }

        public void Remove(T value)
        {
            if (Head != null && Head.Value.Equals(value))//remove first node
            {
                Head = Head.Next;
                Count--;
                return;
            }

            var node = Head;
            if (Last.Value.Equals(value))//remove last node
            {
                while (node.Next.Next != null)// get to predposleden node
                    node = node.Next;

                node.Next = null;
                Last = node;
                Count--;
                return;
            }

            while (node.Next != null && !node.Next.Value.Equals(value))//remove in between node
                node = node.Next;

            if (node.Next != null)
            {
                node.Next = node.Next.Next;
                Count--;
            }
        }

        public bool Contains(T value)
        {
            var node = Head;

            while (node != null)
            {
                if (node.Value.Equals(value))
                    return true;
                node = node.Next;
            }

            return false;
        }

        public IEnumerator GetEnumerator()
        {
            var node = Head;

            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        public void Reverse()
        {
            Node<T> prev = null;
            var current = Head;
            Node<T> next = null;

            while (current != null) {
                next = current.Next;
                current.Next = prev;
                prev = current;
                current = next;
            }

            Head = prev;
        }
    }
    public class Node<T>
    {
        public readonly T Value;
        public Node<T> Next;

        public Node(T value)
        {
            Value = value;
        }
    }
}
