﻿using System;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public interface INavigationRequest
    {
        Func<object, Task> Observer { set; }
    }

    public interface INavigationRequest<out T> : INavigationRequest
    {
        new Func<T, Task> Observer { set; }
    }

}