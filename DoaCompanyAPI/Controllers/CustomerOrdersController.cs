using DoaCompany.Enums;
using DoaCompany.Models;
using DoaCompany.Persistence;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace DoaCompany.Controllers
{
    public class CustomerOrdersController : ApiController
    {
        //// GET api/<controller>
        [ResponseType(typeof(List<CustomerOrderModel>))]
        public JsonResult<List<CustomerOrderModel>> Get()
        {
            CustomerOrderService customerOrderService = new CustomerOrderService();
            return Json(customerOrderService.GetAllOrders());
        }

        // GET api/<controller>/5
        [ResponseType(typeof(CustomerOrderModel))]
        public JsonResult<CustomerOrderModel> Get(int id)
        {
            CustomerOrderService customerOrderService = new CustomerOrderService();
            return Json(customerOrderService.GetOrder(id));
        }

        // POST api/<controller>
        public JsonResult<string> Post([FromBody] OrderProduct order)
        {
            CustomerOrderService customerOrderService = new CustomerOrderService();
            return Json(customerOrderService.CreateOrder(order));
        }

        // PUT api/<controller>/5
        public JsonResult<string> Put(int id, [FromBody] OrderProduct order, ProductOrderUpdateType productOrderType)
        {
            CustomerOrderService customerOrderService = new CustomerOrderService();
            return Json(customerOrderService.ChangeOrder(id, order, productOrderType));
        }

        [Route("api/CustomerOrders/{id:int}/ChangeAddress")]
        public JsonResult<string> Put(int id, string address)
        {
            CustomerOrderService customerOrderService = new CustomerOrderService();
            return Json(customerOrderService.ChangeOrderAddress(id, address));
        }

        // DELETE api/<controller>/5
        public JsonResult<string> Delete(int id)
        {
            CustomerOrderService customerOrderService = new CustomerOrderService();
            return Json(customerOrderService.DeleteOrder(id));
        }
    }
}