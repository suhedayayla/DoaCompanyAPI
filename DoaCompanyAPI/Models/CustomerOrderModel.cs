using DoaCompany.Persistence;
using DoaCompanyAPI.Persistence;
using System.Collections.Generic;

namespace DoaCompany.Models
{
    public class CustomerOrderModel
    {
        public Customer Customer { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public List<CustomerOrderProduct> CustomerOrderProducts { get; set; }
    }
}