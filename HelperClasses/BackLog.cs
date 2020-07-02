using System.Collections;

/// <summary>
/// Collection for a backlog of a certain object (often one added every Update frame)
/// with a fixed maximum size. Older entries get overwritten with newer ones 
/// when the backlog is full.
/// </summary>
public class BackLog<T> : IEnumerator {

    /* The objects contained in a full back log, in chronological order
     * first, first + 1, first + 2 ... Count - 1, 0, 1, 2, 3 ... first - 1
     * The state of the backlog will evolve as in the following example
     * [-, -, -] f = 0, c = 0; l = -1, isEmpty
     * [0, -, -] f = 0, c = 1; l = 0
     * [0, 1, -] f = 0, c = 2; l = 1
     * [0, 1, 2] f = 0, c = 3; l = 2, isFull
     * [3, 1, 2] f = 1, c = 3; l = 0, isFull
     * [3, 4, 2] f = 2, c = 3; l = 1, isFull
     * [3, 4, 5] f = 0, c = 3; l = 2, isFull
     * ...
     */

    //backlog contents
    private T[] array;

    //backlog state variables
    private int first;
    public int Count { get; private set; }

    //iteration state
    private int iterator;

    //basic getters
    private int last { get {
            if(isFull && first != 0)
                return first - 1;
            return Count - 1;
        } }

    public bool isEmpty { get { return Count == 0; } }
    public bool isFull { get { return array.Length == Count; } }
    public int capacity { get { return array.Length; } }

    //constructors
    public BackLog(int capacity){
        array = new T[capacity];
        first = 0;
        Count = 0;
    }

    


    /// <summary>
    /// Add new element to backlog. Overwrites an existing element if the backlog is full
    /// </summary>
    /// <param name="element"></param>
    public void Add(T element) {
        if(isFull) {
            array[first] = element;
            if(++first == Count)
                first = 0;
        }
        else {
            array[Count++] = element;

        }
           
    }

    /// <summary>
    /// Return contents of this backlog in chronological order
    /// </summary>
    public T[] ToArray() {
        T[] toReturn = new T[Count];
        int index = first;
        for(int i = 0; i < Count; i++) {
            toReturn[i] = array[index];
            if(++index == Count)
                index = 0;
        }
        return toReturn;
    }

    /// <summary>
    /// Directly get/set a value in this backlog
    /// </summary>
    /// <param name="i">the number of indices backwards, e.g. 0 is the last element, 2 is the third last element etc.</param>
    public T this[int i] {
        get { return array[RealIndex(i)]; }
        set { array[RealIndex(i)] = value; }
    }

    private int RealIndex(int i) {
        if(i < 0)
            throw new System.IndexOutOfRangeException("Negative backlog index");
        else if(i >= array.Length)
            throw new System.IndexOutOfRangeException("Index exceeds backlog capacity");
        else if(i >= Count)
            throw new System.IndexOutOfRangeException("Index exceeds backlog contents");

        int index = last - i;
        if(index < 0)
            index += Count;
        return index;
    }

    /// <summary>
    /// Return lastly added element. Throws exception if backlog is empty
    /// </summary>
    public T GetLast() {
        return array[last];
    }

    /// <summary>
    /// Return oldest element in the backlog. Throws exception if backlog is empty
    /// </summary>
    public T GetFirst() {
        return array[first];
    }

    /// <summary>
    /// Mark backlog as empty. Note that values in the backlog are not automatically removed 
    /// and won't be immediately eligable for garbage collection.
    /// </summary>
    public void Clear() {
        first = 0;
        Count = 0;
    }

    //iterator
    public bool MoveNext() {
        if(iterator == -1) {
            iterator = first;
            return Count > 0;
        }

        iterator++;
        if(iterator == Count) {
            if(!isFull)
                return false;
            iterator = 0;
        }
        return iterator != first;
    }
    public void Reset() {
        iterator = -1;
    }
    public IEnumerator GetEnumerator() {
        this.Reset();
        return this;
    }
    public T Current => array[iterator];
    object IEnumerator.Current => Current;
}
