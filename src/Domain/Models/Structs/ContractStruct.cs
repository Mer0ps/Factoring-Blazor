namespace Domain.Models.Structs;
public class ContractStruct
{
    public ContractStruct(long id_supplier, long id_client)
    {
        IdSupplier = id_supplier;
        IdClient = id_client;
    }

    public long IdSupplier { get; set; }
    public long IdClient { get; set; }
}
