using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    public T GetStrict<T>() where T : class
    {
        return this.GetStrict(typeof(T)) as T;
    }
    public object GetStrict(Type type)
    {
        return this.bindings[type].GetInstance();
    }

    public T Get<T>() where T : class
    {
        return this.Get(typeof(T)) as T;
    }
    public object Get(Type type)
    {
        return this.RecursiveGet(type);
    }
    private object RecursiveGet(Type type)
    {
        // If the requested type is explicitly bound, return it.
        if (this.bindings.ContainsKey(type)) {
            return this.GetStrict(type);
        }

        // If it isn't bound, we may be able to build an instance ad-hoc.
        // Find the constructor of the type being requested. We assume there is only 1 public constructor.
        ConstructorInfo[] constructorInfos = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        if (constructorInfos.Length == 0)
        {
            // Type is non-instantiable or has no defined constructor.
            throw new NotInstantiableException(type);
        }

        // Find the required constructor parameters and try to instantiate them from the service container recursively.
        ConstructorInfo constructorInfo = constructorInfos[0];
        ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
        object[] args = new object[parameterInfos.Length];
        for (int i = 0; i < parameterInfos.Length; i++) {
            ParameterInfo parameterInfo = parameterInfos[i];
            try {
                args[i] = this.RecursiveGet(parameterInfo.ParameterType);
            }
            catch (NotInstantiableException e) {
                throw new ParamNotInstantiableException(type, parameterInfo.ParameterType, parameterInfo.Name, e);
            }
        }

        // Call the constructor and return the instance.
        return constructorInfo.Invoke(args);
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
        public SingletonBinding(Func<T> recipe) : base(recipe) { }
    }
    public class SingletonBinding : Binding
    {
        public object Instance;

        public SingletonBinding(Func<object> recipe) : base(recipe) { }

        public override object GetInstance()
        {
            // If Instance is non-null, return it.
            // Otherwise, execute Recipe(), cache the instance, and then return it.
            return Instance ??= (Instance = Recipe());
        }
    }
}
