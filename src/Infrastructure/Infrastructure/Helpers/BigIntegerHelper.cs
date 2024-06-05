using Mx.NET.SDK.Core.Domain;
using System.Numerics;

namespace Infrastructure.Helpers;
public static class BigIntegerHelper
{
    public static double AmountToDouble(this BigInteger amount, ESDT esdt)
    {
        return esdt != null ? ESDTAmount.From(amount, esdt).ToDouble() : 0;
    }
}
