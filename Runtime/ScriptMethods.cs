using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radkii.ScriptMethods
{
    public class ScriptMethodBase
    {
        private string _name;

        public string name { get { return _name; } }

        public ScriptMethodBase(string name)
        {
            _name = name;
        }

        public virtual ScriptMethodBase GetNewRenamed(string newName)
        {
            return new ScriptMethodBase(newName);
        }
    }

    public class ScriptMethod : ScriptMethodBase
    {
        public System.Action action;

        public ScriptMethod(string name, System.Action action) : base(name)
        {
            this.action = action;
        }

        public bool Invoke()
        {
            action.Invoke();
            return true;
        }
    }

    public class ScriptMethod<T> : ScriptMethodBase
    {
        public System.Action<T> action;

        public ScriptMethod(string name, System.Action<T> action) : base(name)
        {
            this.action = action;
        }

        public bool Invoke(T value)
        {
            action.Invoke(value);
            return true;
        }
    }

    public class ScriptConverter<T1, T2> : ScriptMethodBase
    {
        public System.Converter<T1, T2> converter;

        public ScriptConverter(string name, System.Converter<T1, T2> converter) : base(name)
        {
            this.converter = converter;
        }

        public T2 Invoke(T1 input)
        {
            return converter.Invoke(input);
        }

        public override ScriptMethodBase GetNewRenamed(string newName)
        {
            return new ScriptConverter<T1, T2>(newName, converter);
        }
    }
}
