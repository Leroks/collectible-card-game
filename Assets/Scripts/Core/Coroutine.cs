using System;
using System.Collections;
using UnityEngine;

namespace core
{
    public class Coroutine
    {

        public class Job
        {
            public int Counter;
            public bool Busy => Counter > 0;

            public System.Collections.IEnumerable Wait(int until = 0)
            {
                while (Counter > until) yield return null;
            }

        }

        System.Collections.Generic.List<IEnumerator> _stack = new System.Collections.Generic.List<IEnumerator>();
        float _wakeTime;
        GameObject _parent;
        Job _job;

        static System.Collections.Generic.List<Coroutine> _running = new System.Collections.Generic.List<Coroutine>();

        static Timer _tickTimer = new Timer(0)
        {
            run = Coroutine.Tick
        };
        static Coroutine _current;
        static _Wait _w = new _Wait();

        IEnumerator Routine => _stack.Count > 0 ? _stack[^1] : null;

        public static void Start(GameObject parent, Func<IEnumerable> routine, Job job = null)
        {
            var co = new Coroutine
            {
                _parent = parent,
                _job = job
            };
            co._stack.Add(routine.Invoke().GetEnumerator());
            _running.Add(co);
            if (job != null) job.Counter++;
        }

        public static object Wait(float duration)
        {
            _w.millis = duration;
            return _w;
        }


        internal static void Tick()
        {
            var now = Time.frameCount;
            var index = _running.Count - 1;
            while (index >= 0)
            {
                var ptr = _running[index];
                if (ptr._wakeTime > 0)
                {
                    if (now < ptr._wakeTime)
                    {
                        index--;
                        continue;
                    }
                    ptr._wakeTime = 0;
                }

                var routine = ptr.Routine;
                var parentGone = ptr._parent == null;
                var done = routine == null || parentGone || !routine.MoveNext();
                if (done)
                {
                    if (!parentGone && ptr._stack.Count > 0)
                        ptr._stack.RemoveAt(ptr._stack.Count - 1);
                    else
                    {
                        ptr._parent = null;
                        _running.RemoveAt(index);
                        if (ptr._job != null) ptr._job.Counter--;
                    }
                }
                else
                {
                    var current = routine.Current;
                    if (current == _w)
                    {
                        ptr._wakeTime = now + _w.millis;
                    }
                    else if (current is IEnumerable enm)
                    {
                        ptr._stack.Add(enm.GetEnumerator());
                    }
                }
                index--;
            }
        }
        class _Wait
        {
            internal float millis;
        }
    }
}