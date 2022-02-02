using System;
using System.Collections.Generic;
using UnityEngine;

public class DITest : MonoBehaviour
{
    public ServiceContainer Container;

    // Start is called before the first frame update
    void Start()
    {
        Container = new ServiceContainer();

        Registration();
        Usage();
    }

    private void Registration()
    {
        // When we Get() this type below, LocalCount will increment by 1 for each new instance returned.
        Container.Bind<IA>(() => new A());

        // When we Get() this type below, we expect LocalCount to always be 1, since only 1 instance is made ever.
        Container.Singleton<IB>(() => new B());
    }

    private void Usage()
    {
        var a = Container.Get<IA>();
        Debug.LogFormat("a.LocalCount: {0}", a.LocalCount);
        a = Container.Get<IA>();
        Debug.LogFormat("a.LocalCount: {0}", a.LocalCount);

        var b = Container.Get<IB>();
        Debug.LogFormat("b.LocalCount: {0} (singleton)", b.LocalCount);
        b = Container.Get<IB>();
        Debug.LogFormat("b.LocalCount: {0} (singleton)", b.LocalCount);
    }

    public interface IA
    {
        int LocalCount { get; set; }
    }
    public class A : IA
    {
        public static int Count = 0;
        public int LocalCount { get; set; }

	public A()
        {
            LocalCount = ++Count;
        }
    }

    public interface IB
    {
        int LocalCount { get; set; }
    }
    public class B : IB
    {
        public static int Count = 0;
        public int LocalCount { get; set; }

	public B()
        {
            LocalCount = ++Count;
        }
    }
}
