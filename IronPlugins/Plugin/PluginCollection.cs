using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins.Plugin
{
    /// <summary>
    /// Represents a collection of plugins.
    /// </summary>
    public class PluginCollection : System.Collections.Generic.ICollection<IIronPlugin>
    {
        List<IIronPlugin> _list;
        DuplicatePluginMode _dupMode = DuplicatePluginMode.Replace;
        
        /// <summary>
        /// Get or set the way in which this collection handles duplicates.
        /// </summary>
        public DuplicatePluginMode DuplicateMode
        {
            get { return _dupMode; }
            set { _dupMode = value; }
        }

        /// <summary>
        /// Get the number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Get whether the collection is read-only or not.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Get the plugin at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IIronPlugin this[int index]
        {
            get { return _list[index]; }
        }

        /// <summary>
        /// Create a default instance of PluginCollection.
        /// </summary>
        public PluginCollection()
        {
            _list = new List<IIronPlugin>();
        }        

        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item"></param>
        public void Add(IIronPlugin item)
        {
            AddPlugin(item);
        }

        /// <summary>
        /// Add the specified item. Returns false if the
        /// plugin was not added, depending on the value
        /// of the DuplicateMode property.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public AddResult AddPlugin(IIronPlugin item)
        {
            AddResult result = AddResult.NotAdded;
            bool contained = Contains(item);
            if (contained)
            {
                switch (DuplicateMode)
                {
                    case DuplicatePluginMode.Allow:
                        _list.Add(item);
                        result = AddResult.Added;
                        break;
                    case DuplicatePluginMode.DontAllow:
                        result = AddResult.NotAdded;
                        break;
                    case DuplicatePluginMode.Replace:
                        int exIndex = _list.IndexOf(item);
                        if (exIndex >= 0)
                        {
                            IIronPlugin existing = _list[exIndex];
                            _list[exIndex] = item;
                            result = AddResult.OverWrote;
                            existing.Dispose();
                        }
                        break;
                }
            }
            else
            {
                _list.Add(item);
                result = AddResult.Added;
            }
            return result;
        }

        /// <summary>
        /// Clear all items from the collection.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Return true if the collection contains the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(IIronPlugin item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Returns the 0-based index of the first occurence 
        /// of the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(IIronPlugin item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Copy the collection to the specified array, starting at the
        /// specified index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(IIronPlugin[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove the first occurrence of the item from the collection. Returns false if the
        /// item was not removed.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(IIronPlugin item)
        {
            return _list.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator to iterate through the list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IIronPlugin> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator to iterate through the list.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Specifies the way that duplicate plugins are handled.
        /// </summary>
        public enum DuplicatePluginMode
        {
            /// <summary>
            /// Don't allow duplicates; the existing will be kept.
            /// </summary>
            DontAllow,
            /// <summary>
            /// Don't allow duplicates; the existing will be replaced.
            /// </summary>
            Replace,
            /// <summary>
            /// Allow duplicates.
            /// </summary>
            Allow
        }

        public enum AddResult
        {
            /// <summary>
            /// The plugin was appended to the collection.
            /// </summary>
            Added,
            /// <summary>
            /// The plugin replaced an existing equal plugin.
            /// </summary>
            OverWrote,
            /// <summary>
            /// The plugin was not added to the collection.
            /// </summary>
            NotAdded
        }
    }
}
