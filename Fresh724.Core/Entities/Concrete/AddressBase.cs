using System.ComponentModel.DataAnnotations;

namespace Fresh724.Core.Entities;

public class AddressBase:IAddressBase
{
    [Key]
    public Guid Id { get; set; }
    public string Street1 { get; set; }
    public string Street2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    
    public string ZipCode { get; set; }
    public string Country { get; set; }
}

                                                                                                                                                         