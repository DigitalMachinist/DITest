using System;

public class ParamNotInstantiableException : Exception
{
    public ParamNotInstantiableException(Type parentType, Type paramType, string paramName, Exception inner = null)
        : base(
            string.Format(
                "Unable to instantiate {0} because constructor parameter {2} ({1}) cannot be instantiated.",
                parentType, paramType, paramName
            ),
            inner
        )
    {
    }
}
