using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Go
{
    public class go_mutex
    {
        protected enum lock_status
        {
            st_unique,
            st_shared,
            st_pess,
            st_upgrade,
        };

        protected struct wait_node
        {
            public Action _ntf;
            public long _id;
            public lock_status _status;
        };

        protected shared_strand _strand;
        protected LinkedList<wait_node> _waitQueue;
        protected long _lockID;
        protected int _recCount;
        protected bool _mustTick;

        public go_mutex(shared_strand strand)
        {
            _strand = strand;
            _waitQueue = new LinkedList<wait_node>();
            _lockID = 0;
            _recCount = 0;
            _mustTick = false;
        }

        public go_mutex() : this(shared_strand.default_strand()) { }

        protected virtual void async_lock_(long id, Action ntf)
        {
            Debug.Assert(_recCount >= 0);
            if (0 == _lockID || id == _lockID)
            {
                _lockID = id;
                _recCount++;
                ntf();
            }
            else
            {
                _waitQueue.AddLast(new wait_node() { _ntf = ntf, _id = id });
            }
        }

        protected virtual void async_try_lock_(long id, Action<bool> ntf)
        {
            Debug.Assert(_recCount >= 0);
            if (0 == _lockID || id == _lockID)
            {
                _lockID = id;
                _recCount++;
                ntf(true);
            }
            else
            {
                ntf(false);
            }
        }

        protected virtual void async_timed_lock_(long id, int ms, Action<bool> ntf)
        {
            Debug.Assert(_recCount >= 0);
            if (0 == _lockID || id == _lockID)
            {
                _lockID = id;
                _recCount++;
                ntf(true);
            }
            else if (ms >= 0)
            {
                async_timer timer = new async_timer(_strand);
                LinkedListNode<wait_node> node = _waitQueue.AddLast(new wait_node()
                {
                    _ntf = delegate ()
                    {
                        timer.cancel();
                        ntf(true);
                    },
                    _id = id
                });
                timer.timeout(ms, delegate ()
                {
                    _waitQueue.Remove(node);
                    ntf(false);
                });
            }
            else
            {
                _waitQueue.AddLast(new wait_node() { _ntf = () => ntf(true), _id = id });
            }
        }

        protected virtual void async_unlock_(long id, Action ntf)
        {
            Debug.Assert(_recCount > 0);
            Debug.Assert(id == _lockID);
            if (0 == --_recCount)
            {
                if (0 != _waitQueue.Count)
                {
                    _recCount = 1;
                    wait_node queueFront = _waitQueue.First.Value;
                    _waitQueue.RemoveFirst();
                    _lockID = queueFront._id;
                    queueFront._ntf();
                }
                else
                {
                    _lockID = 0;
                }
            }
            ntf();
        }

        protected virtual void async_cancel_(long id, bool isAll, Action ntf)
        {
            if (id == _lockID)
            {
                if (isAll)
                {
                    _recCount = 1;
                    async_unlock_(id, ntf);
                }
                else
                {
                    async_unlock_(id, ntf);
                }
            }
            else
            {
                for (LinkedListNode<wait_node> it = _waitQueue.Last; null != it; it = it.Previous)
                {
                    if (it.Value._id == id)
                    {
                        _waitQueue.Remove(it);
                        break;
                    }
                }
                ntf();
            }
        }

        public void async_lock(long id, Action ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_lock_(id, ntf);
                else _strand.add_last(() => async_lock_(id, ntf));
            else _strand.post(() => async_lock_(id, ntf));
        }

        public void async_try_lock(long id, Action<bool> ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_try_lock_(id, ntf);
                else _strand.add_last(() => async_try_lock_(id, ntf));
            else _strand.post(() => async_try_lock_(id, ntf));
        }

        public virtual void async_timed_lock(long id, int ms, Action<bool> ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_timed_lock_(id, ms, ntf);
                else _strand.add_last(() => async_timed_lock_(id, ms, ntf));
            else _strand.post(() => async_timed_lock_(id, ms, ntf));
        }

        public virtual void async_unlock(long id, Action ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_unlock_(id, ntf);
                else _strand.add_last(() => async_unlock_(id, ntf));
            else _strand.post(() => async_unlock_(id, ntf));
        }

        public virtual void async_cancel(long id, bool isAll, Action ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_cancel_(id, isAll, ntf);
                else _strand.add_last(() => async_cancel_(id, isAll, ntf));
            else _strand.post(() => async_cancel_(id, isAll, ntf));
        }

        public shared_strand strand
        {
            get
            {
                return _strand;
            }
        }
    }

    public class go_shared_mutex : go_mutex
    {
        class shared_count
        {
            int _count = 0;

            public void one()
            {
                _count = 1;
            }

            public int inc()
            {
                Debug.Assert(_count >= 0);
                return ++_count;
            }

            public int dec()
            {
                Debug.Assert(_count > 0);
                return --_count;
            }
        };

        Dictionary<long, shared_count> _sharedMap;

        public go_shared_mutex(shared_strand strand) : base(strand)
        {
            _sharedMap = new Dictionary<long, shared_count>();
        }

        public go_shared_mutex() : base()
        {
            _sharedMap = new Dictionary<long, shared_count>();
        }

        protected override void async_lock_(long id, Action ntf)
        {
            Debug.Assert(_recCount >= 0);
            Debug.Assert(!_sharedMap.ContainsKey(id));
            if (0 == _sharedMap.Count && (0 == _lockID || id == _lockID))
            {
                _lockID = id;
                _recCount++;
                ntf();
            }
            else
            {
                _waitQueue.AddLast(new wait_node() { _ntf = ntf, _id = id, _status = lock_status.st_unique });
            }
        }

        protected override void async_try_lock_(long id, Action<bool> ntf)
        {
            Debug.Assert(_recCount >= 0);
            if (0 == _sharedMap.Count && (0 == _lockID || id == _lockID))
            {
                _lockID = id;
                _recCount++;
                ntf(true);
            }
            else
            {
                ntf(false);
            }
        }

        protected override void async_timed_lock_(long id, int ms, Action<bool> ntf)
        {
            Debug.Assert(_recCount >= 0);
            if (0 == _sharedMap.Count && (0 == _lockID || id == _lockID))
            {
                _lockID = id;
                _recCount++;
                ntf(true);
            }
            else if (ms >= 0)
            {
                async_timer timer = new async_timer(_strand);
                LinkedListNode<wait_node> node = _waitQueue.AddLast(new wait_node()
                {
                    _ntf = delegate ()
                    {
                        timer.cancel();
                        ntf(true);
                    },
                    _id = id,
                    _status = lock_status.st_unique
                });
                timer.timeout(ms, delegate ()
                {
                    _waitQueue.Remove(node);
                    ntf(false);
                });
            }
            else
            {
                _waitQueue.AddLast(new wait_node() { _ntf = () => ntf(true), _id = id, _status = lock_status.st_unique });
            }
        }

        shared_count find_map(long id)
        {
            shared_count ct = null;
            if (!_sharedMap.TryGetValue(id, out ct))
            {
                ct = new shared_count();
                _sharedMap.Add(id, ct);
            }
            return ct;
        }

        bool try_map(long id, out shared_count ct)
        {
            return _sharedMap.TryGetValue(id, out ct);
        }

        private void async_lock_shared_(long id, Action ntf)
        {
            Debug.Assert(_recCount >= 0);
            if (0 == _lockID)
            {
                find_map(id).inc();
                ntf();
            }
            else if (try_map(id, out shared_count ct))
            {
                ct.inc();
                ntf();
            }
            else
            {
                _waitQueue.AddLast(new wait_node() { _ntf = ntf, _id = id, _status = lock_status.st_shared });
            }
        }

        private void async_lock_pess_shared_(long id, Action ntf)
        {
            Debug.Assert(_recCount >= 0);
            if (0 == _waitQueue.Count)
            {
                async_lock_shared_(id, ntf);
            }
            else
            {
                _waitQueue.AddLast(new wait_node() { _ntf = ntf, _id = id, _status = lock_status.st_pess });
            }
        }

        private void async_try_lock_shared_(long id, Action<bool> ntf)
        {
            Debug.Assert(_recCount >= 0);
            if (0 == _lockID)
            {
                find_map(id).inc();
                ntf(true);
            }
            else if (try_map(id, out shared_count ct))
            {
                ct.inc();
                ntf(true);
            }
            else
            {
                ntf(false);
            }
        }

        private void async_timed_lock_shared_(long id, int ms, Action<bool> ntf)
        {
            Debug.Assert(_recCount >= 0);
            if (0 == _lockID)
            {
                find_map(id).inc();
                ntf(true);
            }
            else if (try_map(id, out shared_count ct))
            {
                ct.inc();
                ntf(true);
            }
            else if (ms >= 0)
            {
                async_timer timer = new async_timer(_strand);
                LinkedListNode<wait_node> node = _waitQueue.AddLast(new wait_node()
                {
                    _ntf = delegate ()
                    {
                        timer.cancel();
                        ntf(true);
                    },
                    _id = id,
                    _status = lock_status.st_shared
                });
                timer.timeout(ms, delegate ()
                {
                    _waitQueue.Remove(node);
                    ntf(false);
                });
            }
            else
            {
                _waitQueue.AddLast(new wait_node() { _ntf = () => ntf(true), _id = id, _status = lock_status.st_shared });
            }
        }

        private void async_lock_upgrade_(long id, Action ntf)
        {
            Debug.Assert(_sharedMap.ContainsKey(id));
            if (1 == _sharedMap.Count)
            {
                Debug.Assert(0 == _lockID || id == _lockID);
                Debug.Assert(_recCount >= 0);
                _lockID = id;
                _recCount++;
                ntf();
            }
            else
            {
                _waitQueue.AddLast(new wait_node() { _ntf = ntf, _id = id, _status = lock_status.st_upgrade });
            }
        }

        private void async_try_lock_upgrade_(long id, Action<bool> ntf)
        {
            Debug.Assert(_sharedMap.ContainsKey(id));
            if (1 == _sharedMap.Count)
            {
                Debug.Assert(0 == _lockID || id == _lockID);
                Debug.Assert(_recCount >= 0);
                _lockID = id;
                _recCount++;
                ntf(true);
            }
            else
            {
                ntf(false);
            }
        }

        private void async_timed_lock_upgrade_(long id, int ms, Action<bool> ntf)
        {
            Debug.Assert(_sharedMap.ContainsKey(id));
            if (1 == _sharedMap.Count)
            {
                Debug.Assert(0 == _lockID || id == _lockID);
                Debug.Assert(_recCount >= 0);
                _lockID = id;
                _recCount++;
                ntf(true);
            }
            else if (ms >= 0)
            {
                async_timer timer = new async_timer(_strand);
                LinkedListNode<wait_node> node = _waitQueue.AddLast(new wait_node()
                {
                    _ntf = delegate ()
                    {
                        timer.cancel();
                        ntf(true);
                    },
                    _id = id,
                    _status = lock_status.st_upgrade
                });
                timer.timeout(ms, delegate ()
                {
                    _waitQueue.Remove(node);
                    ntf(false);
                });
            }
            else
            {
                _waitQueue.AddLast(new wait_node() { _ntf = () => ntf(true), _id = id, _status = lock_status.st_upgrade });
            }
        }

        protected override void async_unlock_(long id, Action ntf)
        {
            Debug.Assert(_recCount > 0);
            Debug.Assert(id == _lockID);
            if (0 == --_recCount)
            {
                _lockID = 0;
                if (0 != _waitQueue.Count)
                {
                    _mustTick = true;
                    wait_node queueFront = _waitQueue.First.Value;
                    switch (queueFront._status)
                    {
                        case lock_status.st_shared:
                        case lock_status.st_pess:
                            for (LinkedListNode<wait_node> it = _waitQueue.First; null != it;)
                            {
                                if (lock_status.st_shared == it.Value._status ||
                                    lock_status.st_pess == it.Value._status)
                                {
                                    find_map(it.Value._id).inc();
                                    it.Value._ntf();
                                    LinkedListNode<wait_node> oit = it;
                                    it = it.Next;
                                    _waitQueue.Remove(oit);
                                }
                                else
                                {
                                    it = it.Next;
                                }
                            }
                            break;
                        case lock_status.st_unique:
                            _waitQueue.RemoveFirst();
                            _lockID = queueFront._id;
                            _recCount++;
                            queueFront._ntf();
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }
                    _mustTick = false;
                }
            }
            ntf();
        }

        private void async_unlock_shared_(long id, Action ntf)
        {
            Debug.Assert(_recCount == 0);
            Debug.Assert(_lockID == 0);
            if (0 == find_map(id).dec())
            {
                _sharedMap.Remove(id);
                if (0 != _waitQueue.Count)
                {
                    _mustTick = true;
                    for (LinkedListNode<wait_node> it1 = _waitQueue.First; null != it1; it1 = it1?.Next)
                    {
                        wait_node queueFront = it1.Value;
                        switch (queueFront._status)
                        {
                            case lock_status.st_unique:
                                if (0 == _sharedMap.Count)
                                {
                                    _waitQueue.Remove(it1);
                                    _lockID = queueFront._id;
                                    _recCount++;
                                    queueFront._ntf();
                                    it1 = null;
                                }
                                break;
                            case lock_status.st_upgrade:
                                if (1 == _sharedMap.Count)
                                {
                                    Debug.Assert(_sharedMap.ContainsKey(queueFront._id));
                                    _waitQueue.Remove(it1);
                                    _lockID = queueFront._id;
                                    _recCount++;
                                    queueFront._ntf();
                                    it1 = null;
                                }
                                break;
                            case lock_status.st_pess:
                                for (LinkedListNode<wait_node> it2 = it1; null != it2;)
                                {
                                    if (lock_status.st_shared == it2.Value._status ||
                                        lock_status.st_pess == it2.Value._status)
                                    {
                                        find_map(it2.Value._id).inc();
                                        it2.Value._ntf();
                                        LinkedListNode<wait_node> oit = it2;
                                        it2 = it2.Next;
                                        _waitQueue.Remove(oit);
                                    }
                                    else
                                    {
                                        it2 = it2.Next;
                                    }
                                }
                                it1 = null;
                                break;
                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }
                    _mustTick = false;
                }
            }
            ntf();
        }

        private void async_unlock_upgrade_(long id, Action ntf)
        {
            Debug.Assert(1 == _sharedMap.Count && _sharedMap.ContainsKey(id));
            Debug.Assert(_recCount > 0);
            Debug.Assert(id == _lockID);
            if (0 == --_recCount)
            {
                _lockID = 0;
                if (0 != _waitQueue.Count)
                {
                    _mustTick = true;
                    for (LinkedListNode<wait_node> it1 = _waitQueue.First; null != it1; it1 = it1?.Next)
                    {
                        wait_node queueFront = it1.Value;
                        switch (queueFront._status)
                        {
                            case lock_status.st_shared:
                            case lock_status.st_pess:
                                for (LinkedListNode<wait_node> it2 = it1; null != it2;)
                                {
                                    if (lock_status.st_shared == it2.Value._status ||
                                        lock_status.st_pess == it2.Value._status)
                                    {
                                        find_map(it2.Value._id).inc();
                                        it2.Value._ntf();
                                        LinkedListNode<wait_node> oit = it2;
                                        it2 = it2.Next;
                                        _waitQueue.Remove(oit);
                                    }
                                    else
                                    {
                                        it2 = it2.Next;
                                    }
                                }
                                it1 = null;
                                break;
                            case lock_status.st_unique:
                                break;
                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }
                    _mustTick = false;
                }
            }
            ntf();
        }

        protected override void async_cancel_(long id, bool isAll, Action ntf)
        {
            if (try_map(id, out shared_count ct))
            {
                if (id == _lockID)
                {
                    if (isAll)
                    {
                        _recCount = 1;
                        async_unlock_upgrade_(id, ntf);
                    }
                    else
                    {
                        async_unlock_upgrade_(id, ntf);
                    }
                }
                else
                {
                    bool removed = false;
                    for (LinkedListNode<wait_node> it = _waitQueue.Last; null != it; it = it.Previous)
                    {
                        if (it.Value._id == id)
                        {
                            _waitQueue.Remove(it);
                            removed = true;
                            break;
                        }
                    }
                    if (!removed)
                    {
                        if (isAll)
                        {
                            ct.one();
                            async_unlock_shared_(id, ntf);
                        }
                        else
                        {
                            async_unlock_shared_(id, ntf);
                        }
                    }
                }
            }
            else if (id == _lockID)
            {
                if (isAll)
                {
                    _recCount = 1;
                    async_unlock_(id, ntf);
                }
                else
                {
                    async_unlock_(id, ntf);
                }
            }
            else
            {
                for (LinkedListNode<wait_node> it = _waitQueue.Last; null != it; it = it.Previous)
                {
                    if (it.Value._id == id)
                    {
                        _waitQueue.Remove(it);
                        break;
                    }
                }
                ntf();
            }
        }

        public void async_lock_shared(long id, Action ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_lock_shared_(id, ntf);
                else _strand.add_last(() => async_lock_shared_(id, ntf));
            else _strand.post(() => async_lock_shared_(id, ntf));
        }

        public void async_lock_pess_shared(long id, Action ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_lock_pess_shared_(id, ntf);
                else _strand.add_last(() => async_lock_pess_shared_(id, ntf));
            else _strand.post(() => async_lock_pess_shared_(id, ntf));
        }

        public void async_try_lock_shared(long id, Action<bool> ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_try_lock_shared_(id, ntf);
                else _strand.add_last(() => async_try_lock_shared_(id, ntf));
            else _strand.post(() => async_try_lock_shared_(id, ntf));
        }

        public void async_timed_lock_shared(long id, int ms, Action<bool> ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_timed_lock_shared_(id, ms, ntf);
                else _strand.add_last(() => async_timed_lock_shared_(id, ms, ntf));
            else _strand.post(() => async_timed_lock_shared_(id, ms, ntf));
        }

        public void async_lock_upgrade(long id, Action ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_lock_upgrade_(id, ntf);
                else _strand.add_last(() => async_lock_upgrade_(id, ntf));
            else _strand.post(() => async_lock_upgrade_(id, ntf));
        }

        public void async_try_lock_upgrade(long id, Action<bool> ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_try_lock_upgrade_(id, ntf);
                else _strand.add_last(() => async_try_lock_upgrade_(id, ntf));
            else _strand.post(() => async_try_lock_upgrade_(id, ntf));
        }

        public void async_timed_lock_upgrade(long id, int ms, Action<bool> ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_timed_lock_upgrade_(id, ms, ntf);
                else _strand.add_last(() => async_timed_lock_upgrade_(id, ms, ntf));
            else _strand.post(() => async_timed_lock_upgrade_(id, ms, ntf));
        }

        public void async_unlock_shared(long id, Action ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_unlock_shared_(id, ntf);
                else _strand.add_last(() => async_unlock_shared_(id, ntf));
            else _strand.post(() => async_unlock_shared_(id, ntf));
        }

        public void async_unlock_upgrade(long id, Action ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_unlock_upgrade_(id, ntf);
                else _strand.add_last(() => async_unlock_upgrade_(id, ntf));
            else _strand.post(() => async_unlock_upgrade_(id, ntf));
        }
    }

    public class go_condition_variable
    {
        shared_strand _strand;
        LinkedList<tuple<long, go_mutex, Action>> _waitQueue;
        bool _mustTick;

        public go_condition_variable(shared_strand strand)
        {
            _strand = strand;
            _waitQueue = new LinkedList<tuple<long, go_mutex, Action>>();
            _mustTick = false;
        }

        public go_condition_variable() : this(shared_strand.default_strand()) { }

        public void async_wait(long id, go_mutex mutex, Action ntf)
        {
            mutex.async_unlock(id, delegate ()
            {
                if (_strand.running_in_this_thread())
                    if (!_mustTick) _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, () => mutex.async_lock(id, ntf)));
                    else _strand.add_last(() => _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, () => mutex.async_lock(id, ntf))));
                else _strand.post(() => _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, () => mutex.async_lock(id, ntf))));
            });
        }

        private void async_timed_wait_(long id, int ms, go_mutex mutex, Action<bool> ntf)
        {
            if (ms >= 0)
            {
                async_timer timer = new async_timer(_strand);
                LinkedListNode<tuple<long, go_mutex, Action>> node = _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, delegate ()
                {
                    timer.cancel();
                    mutex.async_lock(id, () => ntf(true));
                }));
                timer.timeout(ms, delegate ()
                {
                    _waitQueue.Remove(node);
                    mutex.async_lock(id, () => ntf(false));
                });
            }
            else
            {
                _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, () => mutex.async_lock(id, () => ntf(true))));
            }
        }

        public void async_timed_wait(long id, int ms, go_mutex mutex, Action<bool> ntf)
        {
            mutex.async_unlock(id, delegate ()
            {
                if (_strand.running_in_this_thread())
                    if (!_mustTick) async_timed_wait_(id, ms, mutex, ntf);
                    else _strand.add_last(() => async_timed_wait_(id, ms, mutex, ntf));
                else _strand.post(() => async_timed_wait_(id, ms, mutex, ntf));
            });
        }

        private void notify_one_()
        {
            if (0 != _waitQueue.Count)
            {
                Action ntf = _waitQueue.First.Value.value3;
                _waitQueue.RemoveFirst();
                ntf();
            }
        }

        public void notify_one()
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) notify_one_();
                else _strand.add_last(() => notify_one_());
            else _strand.post(() => notify_one_());
        }

        private void notify_all_()
        {
            _mustTick = true;
            while (0 != _waitQueue.Count)
            {
                Action ntf = _waitQueue.First.Value.value3;
                _waitQueue.RemoveFirst();
                ntf();
            }
            _mustTick = false;
        }

        public void notify_all()
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) notify_all_();
                else _strand.add_last(() => notify_all_());
            else _strand.post(() => notify_all_());
        }

        private void async_cancel_(long id, Action ntf)
        {
            for (LinkedListNode<tuple<long, go_mutex, Action>> it = _waitQueue.First; null != it; it = it.Next)
            {
                if (id == it.Value.value1)
                {
                    go_mutex mtx = it.Value.value2;
                    mtx.async_cancel(id, true, ntf);
                    return;
                }
            }
            ntf();
        }

        public void async_cancel(long id, Action ntf)
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_cancel_(id, ntf);
                else _strand.add_last(() => async_cancel_(id, ntf));
            else _strand.post(() => async_cancel_(id, ntf));
        }

        public shared_strand strand
        {
            get
            {
                return _strand;
            }
        }
    }
}
