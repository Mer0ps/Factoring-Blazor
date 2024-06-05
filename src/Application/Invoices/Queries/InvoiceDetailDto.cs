namespace Application.Invoices.Queries;
public class InvoiceDetailDto
{
    public InvoiceDto Invoice { get; set; }
    public IEnumerable<InvoiceHistoryDto> Histories { get; set; }
}
