// $Id: $
// Author: Valeriy Onuchin   29.12.2010

using System;
using System.Collections;


namespace P
{
    /// <summary>
    /// There is a nasty bug in framework 1.1
    /// If you Enqueue some items, and then Dequeue an item, followed by cloning the Q, the clone
    /// will not contain the correct data, so we have to clone a Q manually until the next version.
    /// </summary>
    public class PQueue
    {
        public static Queue Clone(Queue orig)
        {
            Queue clone = new Queue(orig.Count);

            foreach(object o in orig)
            {
                clone.Enqueue(o);
            }

            return clone;
        }
    }
}
