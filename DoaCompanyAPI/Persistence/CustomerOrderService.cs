using System;
using System.Collections.Generic;
using System.Linq;
using DoaCompany.Models;
using DoaCompanyAPI.Persistence;

namespace DoaCompany.Persistence
{
    /// <summary>
    /// Müşteri siparişleri servis classı
    /// </summary>
    public class CustomerOrderService
    {
        private readonly doaDBEntities _context = new doaDBEntities();

        /// <summary>
        /// Sipariş oluşturma
        /// </summary>
        /// <param name="order"></param>
        public string CreateOrder(OrderProduct order)
        {
            //önce bu Id li bir müşteri varmı onu kontrol ediyorum. Eğer yoksa zaten hiç sipariş oluşmuyor
            var customer = _context.Customer.Where(x => x.Id == order.CustomerId).FirstOrDefault();
            if (customer != null)
            {
                CustomerOrder customerOrder = new CustomerOrder();
                customerOrder.CustomerId = order.CustomerId;
                customerOrder.Address = customer.Address;
                _context.CustomerOrder.Add(customerOrder);
                _context.SaveChanges();
                var customerOrderId = customerOrder.Id;
                List<CustomerOrderProduct> customerOrderProducts = new List<CustomerOrderProduct>();
                //Sipariş edilmiş ürünleri dönüm her birini tek tek sipariş detay tablosuna ekleniyor
                foreach (var item in order.Products)
                {
                    CustomerOrderProduct customerOrderProduct = new CustomerOrderProduct();
                    customerOrderProduct.CustomerOrderId = customerOrderId;
                    customerOrderProduct.ProductId = item.ProductId;
                    customerOrderProduct.Quantity = Convert.ToInt32(item.Quantity);
                    customerOrderProducts.Add(customerOrderProduct);
                }
                _context.CustomerOrderProduct.AddRange(customerOrderProducts);
                _context.SaveChanges();
                return "Sipariş oluşturulmuştur";
            }
            return "Böyle bir müşteri bulunamadı";
        }

        /// <summary>
        /// Tüm siparişeri getirir
        /// </summary>
        /// <returns></returns>
        public List<CustomerOrderModel> GetAllOrders()
        {
            List<CustomerOrderModel> customerOrderModels = new List<CustomerOrderModel>();
            //Tüm siparişleri dönüp topluyorum
            foreach (var item in _context.CustomerOrder.ToList())
            {
                CustomerOrderModel customerOrderModel = new CustomerOrderModel();
                customerOrderModel.CustomerOrder = item;
                //sipariş tablosunda bu hangi müşteri onu buluyorum.
                customerOrderModel.Customer = _context.Customer.Where(x => x.Id == customerOrderModel.CustomerOrder.CustomerId).FirstOrDefault();
                //daha sonra bu siparişe ait tüm sipariş detaylarını çekiyorum
                customerOrderModel.CustomerOrderProducts = _context.CustomerOrderProduct.Where(x => x.CustomerOrderId == customerOrderModel.CustomerOrder.Id).ToList();
                customerOrderModels.Add(customerOrderModel);
            }
            return customerOrderModels;
        }

        /// <summary>
        /// Belirli siparişi getirir
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CustomerOrderModel GetOrder(int id)
        {
            CustomerOrderModel customerOrderModel = new CustomerOrderModel();
            //önce böyle bir sipariş varmı  o kontrol ediliyor
            var customerOrder = _context.CustomerOrder.Where(x => x.Id == id).FirstOrDefault();
            if (customerOrder != null)
            {
                customerOrderModel.CustomerOrder = customerOrder;
                //sipariş tablosunda bu hangi müşteri onu buluyorum.
                customerOrderModel.Customer = _context.Customer.Where(x => x.Id == customerOrderModel.CustomerOrder.CustomerId).FirstOrDefault();
                //daha sonra bu siparişe ait tüm sipariş detaylarını çekiyorum
                customerOrderModel.CustomerOrderProducts = _context.CustomerOrderProduct.Where(x => x.CustomerOrderId == customerOrderModel.CustomerOrder.Id).ToList();
            }
            return customerOrderModel;
        }

        /// <summary>
        /// Sipariş değiştirme,ekleme,silme
        /// </summary>
        /// <param name="id"></param>
        /// <param name="order"></param>
        /// <param name="productOrderType"></param>
        /// <returns></returns>
        public string ChangeOrder(int id, OrderProduct order, Enums.ProductOrderUpdateType productOrderType)
        {
            //önce böyle bir sipariş varmı o kontrol ediliyor
            var customerOrderProducts = _context.CustomerOrderProduct.Where(x => x.CustomerOrderId == id).ToList();
            if (customerOrderProducts != null)
            {
                switch (productOrderType)
                {
                    //yeni ürün ekleme veya mevcut ürün adet değiştirme
                    case Enums.ProductOrderUpdateType.AddProductOrChangeQuantitiy:
                        foreach (var item in order.Products)
                        {
                            var selectedProduct = customerOrderProducts.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                            //eğer gönderilen ürün mevcut siparişte var ise sadece adedi değiştiriliyor
                            if (selectedProduct != null)
                            {
                                selectedProduct.Quantity = Convert.ToInt32(item.Quantity);
                            }
                            //yok ise gönderilen ürün sipariş detaylara ekleniyor
                            else
                            {
                                CustomerOrderProduct customerOrder = new CustomerOrderProduct();
                                customerOrder.CustomerOrderId = id;
                                customerOrder.ProductId = item.ProductId;
                                customerOrder.Quantity = Convert.ToInt32(item.Quantity);
                                _context.CustomerOrderProduct.Add(customerOrder);
                            }
                        }
                        _context.SaveChanges();
                        return "Sipariş başarılı bir şekilde değiştirilmiştir";
                    //siparişten ürün silme
                    case Enums.ProductOrderUpdateType.RemoveProduct:
                        foreach (var item in order.Products)
                        {
                            var selectedProduct = customerOrderProducts.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                            //önce o siparişt detaydaki ürün siliniyor
                            if (selectedProduct != null)
                            {
                                _context.CustomerOrderProduct.Remove(selectedProduct);
                                customerOrderProducts.Remove(selectedProduct);
                            }
                            //eğer sipariş detaylarında başka bir ürün kalmadı ise o sipariş komple siliniyor
                            else if (customerOrderProducts.Count < 1)
                            {
                                var customerOrder = _context.CustomerOrder.Where(x => x.Id == id).FirstOrDefault();
                                if (customerOrder != null)
                                {
                                    _context.CustomerOrder.Remove(customerOrder);
                                }
                            }
                        }
                        _context.SaveChanges();
                        return "Sipariş başarılı bir şekilde silinmiştir.";
                }
            }
            return "Id li sipariş bulunamamıştır";
        }

        /// <summary>
        /// Sipariş adresini değiştirir
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public string ChangeOrderAddress(int id, string address)
        {
            //önce böyle bir sipariş varmı yok mu onun kontrolü
            var customerOrder = _context.CustomerOrder.Where(x => x.Id == id).FirstOrDefault();
            if (customerOrder != null)
            {
                customerOrder.Address = address;
                _context.SaveChanges();
                return "Sipariş adresi değişmiştir.";
            }
            return "Id li sipariş bulunamamıştır";
        }

        /// <summary>
        /// Müşteri siparişini siler
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteOrder(int id)
        {
            //Parametre olarak gelen CustomerOrderId yi önce sipariş detaylarını tuttuğumuz tablodan temizliyoruz.
            var customerOrderProducts = _context.CustomerOrderProduct.Where(x => x.CustomerOrderId == id).ToList();
            if (customerOrderProducts != null)
            {
                _context.CustomerOrderProduct.RemoveRange(customerOrderProducts);
            }

            //Daha sonra sipariş tablosunda siliyoruz.
            var customerOrder = _context.CustomerOrder.Where(x => x.Id == id).FirstOrDefault();
            if (customerOrder != null)
            {
                _context.CustomerOrder.Remove(customerOrder);
                _context.SaveChanges();
                return "Sipariş Silinmiştir";
            }
            return "Id li sipariş bulunamamıştır";
        }
    }
}