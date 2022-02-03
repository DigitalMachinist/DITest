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
        // Non-singleton generic syntax
        var a1 = Container.Get<IA>();
        Debug.LogFormat("a1.LocalCount: {0}", a1.LocalCount);

        // Non-singleton object syntax (requires unboxing)
        var a2 = Container.Get(typeof(IA)) as IA;
        Debug.LogFormat("a2.LocalCount: {0}", a2.LocalCount);

        // Singleton generic syntax
        var b1 = Container.Get<IB>(); 
        Debug.LogFormat("b1.LocalCount: {0} (singleton)", b1.LocalCount);

        // Singleton object syntax (requires unboxing)
        var b2 = Container.Get(typeof(IB)) as IB;
        Debug.LogFormat("b2.LocalCount: {0} (singleton)", b2.LocalCount);

        // Requesting an unbound instance to be instantiated ad-hoc (constructor requires IB injected)
        var c1 = Container.Get<C>();
        Debug.LogFormat("c1.LocalCount: {0}", c1.LocalCount);
        Debug.LogFormat("c1.B.LocalCount: {0}", c1.B.LocalCount);
        var c2 = Container.Get<C>();
        Debug.LogFormat("c2.LocalCount: {0}", c2.LocalCount);
        Debug.LogFormat("c2.B.LocalCount: {0}", c2.B.LocalCount);

        // Requesting an unbound instance to be instantiated ad-hoc (constructor requires IA and IC)
        // Expect a NotInstantiableException caused by interface IC not being bound to the ServiceContainer (since isn't instantiable without being bound).
        // Uncomment the following line to bind IC and allow D to be instantiated successfully.
        //Container.Bind<IC>(() => new C(Container.Get<IB>()));
        var d1 = Container.Get<D>();
        Debug.LogFormat("d1.LocalCount: {0}", d1.LocalCount);
        Debug.LogFormat("d1.A.LocalCount: {0}", d1.A.LocalCount);
        Debug.LogFormat("d1.C.LocalCount: {0}", d1.C.LocalCount);
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

    public interface IC
    {
        IB B { get; set; }
        int LocalCount { get; set; }
    }
    public class C : IC
    {
        public static int Count = 0;
        public IB B { get; set; }
        public int LocalCount { get; set; }

	public C(IB b)
        {
            B = b;
            LocalCount = ++Count;
        }
    }

    public interface ID
    {
        IA A { get; set; }
        IC C { get; set; }
        int LocalCount { get; set; }
    }
    public class D : ID
    {
        public static int Count = 0;
        public IA A { get; set; }
        public IC C { get; set; }
        public int LocalCount { get; set; }

	public D(IA a, IC c)
        {
            A = a;
            C = c;
            LocalCount = ++Count;
        }
    }
}
