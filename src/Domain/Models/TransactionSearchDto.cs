namespace Domain.Models;

public class TransactionSearchDto
{
    //public int Took { get; set; }
    //public bool TimedOut { get; set; }
    //public Shards Shards { get; set; }
    public Hits Hits { get; set; }
}

public class Shards
{
    public int Total { get; set; }
    public int Successful { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
}

public class Hits
{
    //public Total Total { get; set; }
    //public object MaxScore { get; set; }
    public List<Hit> HitsList { get; set; }
}

public class Total
{
    public int Value { get; set; }
    public string Relation { get; set; }
}

public class Hit
{
    //public string Index { get; set; }
    //public string Type { get; set; }
    //public string Id { get; set; }
    //public object Score { get; set; }
    public Source Source { get; set; }
    //public List<long> Sort { get; set; }
}

public class Source
{
    public string MiniBlockHash { get; set; }
    public int Nonce { get; set; }
    public int Round { get; set; }
    public string Value { get; set; }
    public int ValueNum { get; set; }
    public string Receiver { get; set; }
    public string Sender { get; set; }
    public int ReceiverShard { get; set; }
    public int SenderShard { get; set; }
    public long GasPrice { get; set; }
    public long GasLimit { get; set; }
    public long GasUsed { get; set; }
    public string Fee { get; set; }
    public double FeeNum { get; set; }
    public string InitialPaidFee { get; set; }
    public string Data { get; set; }
    public string Signature { get; set; }
    public long Timestamp { get; set; }
    public string Status { get; set; }
    public int SearchOrder { get; set; }
    public bool HasScResults { get; set; }
    public bool IsScCall { get; set; }
    public bool HasOperations { get; set; }
    public bool HasLogs { get; set; }
    public string Operation { get; set; }
    public string Function { get; set; }
    public int Version { get; set; }
}
