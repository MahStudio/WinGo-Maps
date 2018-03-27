using System;
using System.Threading.Tasks;

namespace GoogleMapsUnofficial.Interfaces
{
    public interface IDispatcher
    {
        Task RunAsync(Action action);

        bool HasThreadAccess { get; }
    }
}