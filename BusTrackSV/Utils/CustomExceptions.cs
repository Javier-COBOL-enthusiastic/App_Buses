using System;

public class CamposRequeridosException : Exception
{
    public CamposRequeridosException() : base("Faltan campos requeridos") {}
    public CamposRequeridosException(string message) : base(message) {}
    public CamposRequeridosException(string message, Exception inner) : base(message, inner) {}
}

public class UserInvalidado : Exception
{
    public UserInvalidado() : base("Usuario Invalido") {}
    public UserInvalidado(string message) : base(message) {}
    public UserInvalidado(string message, Exception inner) : base(message, inner) {}
}

public class NullValue : Exception
{
    public NullValue() : base("No existe registro de este objeto.") {}
    public NullValue(string message) : base(message) {}
    public NullValue(string message, Exception inner) : base(message, inner) {}
}


public class RutaNoRegistrada : Exception
{
    public RutaNoRegistrada() : base("La ruta no está registrada.") {}
    public RutaNoRegistrada(string message) : base(message) {}
    public RutaNoRegistrada(string message, Exception inner) : base(message, inner) {}
}