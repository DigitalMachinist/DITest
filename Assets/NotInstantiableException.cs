using System;

public class NotInstantiableException : Exception
{
    public NotInstantiableException(Type type, Exception inner = null)
        : base(
            string.Format(
                "Unable to instantiate {0}! It is either is not bound to the ServiceContainer, is non-instantiable, or has no defined constructor.",
                type
            ),
            inner
        )
    {
    }
}
