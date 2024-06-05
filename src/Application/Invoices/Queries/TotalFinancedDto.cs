using Mx.NET.SDK.Core.Domain;
using System.Numerics;

namespace Application.Invoices.Queries;
public class TotalFinancedDto
{
    public string Month { get; set; }
    public BigInteger Amount { get; set; }
    public string Identifier { get; set; }
    public ESDT Esdt { get; set; }
}


