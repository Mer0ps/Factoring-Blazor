using Mx.NET.SDK.Core.Domain.Values;

namespace Domain.Models.Structs;
public class CompanyStruct
{

    public CompanyStruct(long id_offchain, IEnumerable<Address> administrators, bool is_kyc)
    {
        IdOffChain = id_offchain;
        IsKyc = is_kyc;
        Administrators = administrators.Select(x => x.Bech32);
    }
    public long IdOffChain { get; set; }
    public IEnumerable<string> Administrators { get; set; }
    public bool IsKyc { get; set; }
}
