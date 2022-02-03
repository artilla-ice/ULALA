using System.Collections.Generic;

namespace ULALA.UI.Core.Contracts.MVVM
{
    public interface IParametrizedView
    {
        IDictionary<string, object> Parameters { get; }
    }
}
