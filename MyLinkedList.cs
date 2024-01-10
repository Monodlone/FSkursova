using System.Collections;

namespace Kursova
{
    public class MyLinkedList<T> : IEnumerable
    {
        internal Node<T> Head { get; private set; }
        private Node<T> Last { get; set; }

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
        }

        public void Remove(T value)
        {
            if (Head != null && Head.Value.Equals(value))//remove first node
            {
                Head = Head.Next;
                return;
            }

            var node = Head;
            if (Last.Value.Equals(value))//remove last node
            {
                while (node.Next.Next != null)// get to predposleden node
                    node = node.Next;

                node.Next = null;
                Last = node;
                return;
            }

            while (node.Next != null && !node.Next.Value.Equals(value))//remove in between node
                node = node.Next;

            if (node.Next != null)
            {
                node.Next = node.Next.Next;
            }
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
