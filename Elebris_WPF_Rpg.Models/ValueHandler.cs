using Elebris.Database.Manager;
using Elebris.Library.Units;
using Elebris.Library.Units.Core.Models;
using Elebris.Library.Units.Resources.Models;
using Elebris.Library.Units.Values.Enforcing.MutableValues;
using System;
using System.Collections.Generic;

namespace Elebris.Library.Units.Values.Handlers
{

    public class ValueHandler : IValueHandler
    {
        public Guid guid { get; set; }
        private Dictionary<string, StatValue> StoredStats { get; set; }

        //Track all of the characters various levels and carry-over accomplishments
        private Dictionary<string, IValueObserver> ValueObserversDict { get; set; } = new Dictionary<string, IValueObserver>();

        
        //https://stackoverflow.com/questions/3028724/why-do-we-need-the-event-keyword-while-defining-events

        public ValueHandler(Dictionary<string, StatValue> Stats)
        {
            StoredStats = Stats;
        }

        public StatValue RetrieveValue(string retrievableValue)
        {
            StatValue value;
            if (StoredStats.TryGetValue(retrievableValue, out value))
            {
                return value;
            }
            else
            {
                Console.WriteLine("No such key: {0}", retrievableValue);
                return null;
            }
        }

        public void AddValueObserver(string name, IValueObserver observer)
        {
            ValueObserversDict.Add(name, observer);
        }
        public void RemoveValueObserver(string name)
        {
            //Do I need to unsubscribe or will the garbage collector handle that?
            ValueObserversDict.Remove(name);
        }

    }
}
