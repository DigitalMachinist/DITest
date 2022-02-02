using System;
using System.Collections.Generic;
using System.Linq;

public class ServiceContainer
{
    private readonly Dictionary<Type, Binding> bindings;

    public ServiceContainer()
    {
        this.bindings = new Dictionary<Type, Binding>();
    }

    public ServiceContainer Bind<T>(Func<T> recipe) where T : class
    {
        if (this.bindings.ContainsKey(typeof(T)))
        {
            this.bindings.Remove(typeof(T));
        }

        var newBinding = new Binding<T>(recipe);
        this.bindings.Add(typeof(T), newBinding);

        return this;
    }

    public ServiceContainer Singleton<T>(Func<T> recipe, bool lazy = true) where T : class
    {
        if (this.bindings.ContainsKey(typeof(T)))
        {
            this.bindings.Remove(typeof(T));
        }

        var newBinding = new SingletonBinding<T>(recipe);
        if (!lazy)
        {
            newBinding.GetInstance();
        }

        this.bindings.Add(typeof(T), newBinding);

        return this;
    }

    public T Get<T>() where T : class
    {
        return this.bindings[typeof(T)].GetInstance() as T;
    }

    public void GetBindings(ref List<string> list)
    {
        list.Clear();
        list.AddRange(this.bindings.Keys.Select(x => x.ToString()));
    }

    public class Binding<T> : Binding where T : class
    {
        public Binding(Func<T> recipe) : base(recipe) {}
    }
    public class Binding
    {
        public Func<object> Recipe;

        public Binding(Func<object> recipe)
        {
            Recipe = recipe;
        }

        public virtual object GetInstance()
        {
            return Recipe();
        }

        public bool IsSingleton()
        {
            return this is SingletonBinding;
        }
    }

    public class SingletonBinding<T> : SingletonBinding where T : class
    {
        public SingletonBinding(Func<T> recipe) : base(recipe) {}
    }
    public class SingletonBinding : Binding
    {
        public object Instance;

        public SingletonBinding(Func<object> recipe) : base(recipe) {}

        public override object GetInstance()
        {
            // If Instance is non-null, return it.
            // Otherwise, execute Recipe(), cache the instance, and then return it.
            return Instance ??= (Instance = Recipe());
        }
    }
}
