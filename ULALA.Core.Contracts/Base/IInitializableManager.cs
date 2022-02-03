using System;
using System.Threading.Tasks;

namespace ULALA.Core.Contracts.Base
{
    public interface IInitializableManager
    {
        Task Initialize();
    }
}
