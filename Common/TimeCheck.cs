// $Id: $
// Author: Valeriy Onuchin   29.12.2011

using System;
using System.Collections;


namespace P
{
    /// <summary>
    /// Summary description for TimeCheck.
    /// </summary>
    public class TimeCheck
    {
        struct StartTime
        {
            public DateTime start;
            public string msg;
        }

        private static Stack stack = new Stack(1000);

        public static void Push()
        {
           Push(null);
        }

        public static void Push(string msg)
        {
            StartTime st;
            st.start = DateTime.Now;
            st.msg = msg;

            stack.Push(st);
        }

        public static void Pop()
        {
            StartTime st = (StartTime)stack.Pop();

            Console.WriteLine(((TimeSpan)(DateTime.Now - st.start)) + " " +  st.msg); 
        }
    }
}
