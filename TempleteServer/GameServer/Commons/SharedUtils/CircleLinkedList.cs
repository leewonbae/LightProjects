namespace GameServer.Commons.SharedUtils
{
    public class Node<T>
    {
        public T Data { get; private set; }
        public Node<T> NextNode { get; set; }
        public Node(T data)
        {
            Data = data;
        }
    }
    public class CircleLinkedList<T>
    {
        private int _nodeCount;
        private Node<T> _head;
        private Node<T> _tail;
        private Node<T> _current;

        public CircleLinkedList()
        {
            _nodeCount = 0;
            _head = null;
            _tail = null;
            _current = null;

        }

        public void AddNode(T newData)
        {
            var newNode = new Node<T>(newData);

            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
                _current = newNode;
            }
            else
            {
                _tail.NextNode = newNode;
                newNode.NextNode = _head;
                _tail = newNode;

            }

            _nodeCount++;
        }

        public T GetCurrentNodeData()
        {
            return _current.Data;
        }

        public int GetNodeCount()
        {
            return _nodeCount;
        }

        public void NextNode()
        {
            _current = _current.NextNode;
        }
    }
}