
using DoaCompany.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoaCompany.Models
{
    public class OrderProduct
    {
        public int CustomerId { get; set; }
        public List<OrderProductModel> Products { get; set; }
    }
}