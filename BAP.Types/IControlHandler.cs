using BAP.Types;
using System;
using System.Collections.Generic;

namespace BAP.Types
{
    public interface IControlHandler
    {
        IButtonProvider CurrentButtonProvider { get; set; }
        List<(Type type, string Name)> TinkerProviderTypes { get; init; }

        bool ChangeButtonProvider(Type type);
    }
}