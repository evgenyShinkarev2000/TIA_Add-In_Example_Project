using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class PropertyWatcher
    {
        private readonly INotifyPropertyChangedFromAnywhere observable;
        private readonly Dictionary<string, string[]> propertyPairDependentProperties;
        public PropertyWatcher(INotifyPropertyChangedFromAnywhere observable)
        {
            this.observable = observable;
            observable.PropertyChanged += NotifyPropertyChangedHandler;
            propertyPairDependentProperties = InitDependencyDictionary(this.observable.GetType());
            CheckDictionaryHasntLoopReference();
        }

        ~PropertyWatcher()
        {
            observable.PropertyChanged -= NotifyPropertyChangedHandler;
        }

        private void NotifyPropertyChangedHandler(object sender, PropertyChangedEventArgs args)
        {
            if (!propertyPairDependentProperties.ContainsKey(args.PropertyName))
            {
                return;
            }

            var dependentProperties = new Queue<string>(propertyPairDependentProperties[args.PropertyName]);
            while (dependentProperties.Count() > 0)
            {
                var dependentProperty = dependentProperties.Dequeue();
                observable.NotifyPropertyChanged(dependentProperty);
                if (propertyPairDependentProperties.TryGetValue(dependentProperty, out var nextDependentProperties))
                {
                    dependentProperties.EnqueueRange(nextDependentProperties);
                }
            }
        }

        private Dictionary<string, string[]> InitDependencyDictionary(Type observableType)
        {
            var propertyInfos = observableType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propertyPairDependentProperties = new Dictionary<string, List<string>>();
            foreach (var propInfo in propertyInfos)
            {
                var observablePropertyAttribute = propInfo.GetCustomAttribute<DependentProperty>();
                if (observablePropertyAttribute == null)
                {
                    continue;
                }
                var propName = propInfo.Name;
                foreach (var observablePropertyName in observablePropertyAttribute.ObservablePropertyNames)
                {
                    ChechMemberIsPublicProperty(observableType, observablePropertyName);
                    if (!propertyPairDependentProperties.ContainsKey(observablePropertyName))
                    {
                        propertyPairDependentProperties.Add(observablePropertyName, new List<string>());
                    }

                    propertyPairDependentProperties[observablePropertyName].Add(propName);
                }
            }

            return propertyPairDependentProperties.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
        }

        private void CheckDictionaryHasntLoopReference()
        {
            foreach (var observablePropertyName in propertyPairDependentProperties.Keys)
            {
                var visited = new HashSet<string>() { observablePropertyName };
                var gonaVisit = new Queue<string>(propertyPairDependentProperties[observablePropertyName]);
                while (gonaVisit.Count() > 0)
                {
                    var currentVisit = gonaVisit.Dequeue();
                    if (visited.Contains(currentVisit))
                    {
                        throw new Exception("Circular dependence found");
                    }

                    visited.Add(currentVisit);
                    if (propertyPairDependentProperties.TryGetValue(currentVisit, out var nextVisits))
                    {
                        gonaVisit.EnqueueRange(nextVisits);
                    }
                }
            }
        }

        private void ChechMemberIsPublicProperty(Type observableType, string mebmerName)
        {
            var property = observableType.GetProperty(mebmerName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException($"member {mebmerName} isn't public instance property.");
            }
        }
    }
}
