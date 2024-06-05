using Mx.NET.SDK.Core.Domain.Codec;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;

namespace Infrastructure.Helpers;

public static class Decoder
{
    public static TOut ExtractResultScArg<TOut>(TypeValue type, string data)
    {
        var codec = new BinaryCodec();
        var responseBytes = Convert.FromBase64String(data);
        var decodedResponse = codec.DecodeTopLevel(responseBytes, type);
        return decodedResponse.ToObject<TOut>();
    }

    public static TOut ToEnum<TOut>(TypeValue type, string data)
    {
        var codec = new BinaryCodec();
        var responseBytes = Convert.FromBase64String(data);
        var decodedResponse = (EnumValue)codec.DecodeTopLevel(responseBytes, type);

        return JsonWrapper.Deserialize<TOut>(decodedResponse.Discriminant.ToString());
    }
}
