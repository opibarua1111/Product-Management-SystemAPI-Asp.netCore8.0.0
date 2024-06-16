using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductManagementAPI.Data;
using ProductManagementAPI.IServices;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.RequestModels;
using ProductManagementAPI.Models.ResponseModels;

namespace ProductManagementAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly SubContext _context;
        public OrderService(SubContext context)
        {
            _context = context;
        }

        public async Task<CustomResponse> GetOrders(string userId)
        {
            try
            {
                var orders = await _context.Orders
                               .Where(o => o.UserId == Guid.Parse(userId) && !o.IsDeleted)
                               .Select(o => new
                               {
                                   o.OrderId,
                                   o.Name,
                                   o.Address,
                                   o.Email,
                                   o.Description,
                                   o.Status,
                                   o.CreatedDate,
                                   o.IsDeleted,
                                   Orders = o.OrderRecords
                                   .Where(or => !or.IsDeleted)
                                   .Select(or => new
                                   {
                                       VariantId = or.VariantId,
                                       Color = or.Variant.Color,
                                       Size = or.Variant.Size,
                                       Image = or.Variant.Image,
                                       Specification = or.Variant.Specification,
                                       IsApproved = or.Variant.IsApproved,
                                       Price = or.Variant.Price,
                                       Stock = or.Variant.Stock,
                                       ProductName = or.Variant.Product.ProductName,
                                       Brand = or.Variant.Product.Brand,
                                       Type = or.Variant.Product.Type,
                                   }).ToList()
                               }).ToListAsync();
                return new CustomResponse
                {
                    ReponseCode = 200,
                    data = new
                    {
                        orders,
                        totalOrders = orders.Count
                    }
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CustomResponse> OrderProduct(ProductOrderRequest data, string userId)
        {
            var response = new CustomResponse();
            using(var DbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order();
                    Guid OrderId = Guid.NewGuid();
                    var orderRecords = new List<OrderRecord>();
                    foreach (var variant in data.Variants)
                    {
                        var orderRecord = new OrderRecord();
                        orderRecord.OrderRecordId = Guid.NewGuid();
                        orderRecord.OrderId = OrderId;
                        orderRecord.VariantId = variant.VariantId;
                        orderRecord.IsDeleted = false;
                        orderRecord.CreatedDate = DateTime.Now;
                        orderRecord.CreatedById = Guid.Parse(userId);
                        orderRecords.Add(orderRecord);
                    }
                    order.OrderId = OrderId;
                    order.Name = data.Name;
                    order.Address = data.Address;
                    order.Email = data.Email;
                    order.Description = data.Description;
                    order.Status = "Active";
                    order.UserId = Guid.Parse(userId);
                    order.CreatedDate = DateTime.Now;
                    order.CreatedById = Guid.Parse(userId);
                    order.IsDeleted = false;
                    if (orderRecords.Count>0)
                    {
                        order.OrderRecords = orderRecords;
                        _context.Add(order);
                        await _context.SaveChangesAsync();
                        response.Message = "Order save successfully!";
                        response.ReponseCode = 200;
                        response.Success = true;
                        DbContextTransaction.Commit();
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Must Select product variant";
                        DbContextTransaction.Rollback();
                    }
                    return response;
                }
                catch(Exception ex)
                {
                    DbContextTransaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<CustomResponse> UpdateOrderProduct(ProductOrderRequest data, string userId)
        {
            var response = new CustomResponse();
            using (var DbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region GetRecord By orderId
                    var order = await _context.Orders
                                        .Where(o => o.OrderId == data.OrderId && !o.IsDeleted)
                                        .FirstOrDefaultAsync();
                    if (order == null)
                    {
                        response.Message = "Order not found!";
                        response.ReponseCode = 404;
                        response.Success = false;
                        return response;
                    }
                    #endregion
                    #region Get OrderRecords By OrderId
                    var oldOrderRecords = await _context.OrderRecords
                                                .Where(or => or.OrderId ==data.OrderId && !or.IsDeleted)
                                                .ToListAsync();
                    #endregion
                    Guid OrderId = data.OrderId;
                    foreach (var variant in data.Variants)
                    {
                        var existOrderRecord = oldOrderRecords
                                                .FirstOrDefault(oldOR => oldOR.VariantId == variant.VariantId);
                        #region Create OrderRecord If Record not exist
                        if (existOrderRecord == null)
                        {
                            var newOrderRecord = new OrderRecord
                            {
                                OrderRecordId = Guid.NewGuid(),
                                OrderId = data.OrderId,
                                VariantId = variant.VariantId,
                                IsDeleted = false,
                                CreatedDate = DateTime.Now,
                                CreatedById = Guid.Parse(userId)
                            };
                            await _context.OrderRecords.AddAsync(newOrderRecord);
                        }
                        #endregion
                        #region Update OrderRecord If Record exist
                        else
                        {
                            existOrderRecord.IsDeleted = variant.IsDeleted == null ? false : variant.IsDeleted;
                            existOrderRecord.ModifiedDate = DateTime.Now;
                            existOrderRecord.ModifiedById = Guid.Parse(userId);
                            _context.OrderRecords.Update(existOrderRecord);
                        }
                        #endregion
                    }
                    order.OrderId = OrderId;
                    order.Name = data.Name;
                    order.Address = data.Address;
                    order.Email = data.Email;
                    order.Description = data.Description;
                    order.UserId = Guid.Parse(userId);
                    order.ModifiedDate = DateTime.Now;
                    order.ModifiedById = Guid.Parse(userId);

                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                    response.Message = "Order Update successfully!";
                    response.ReponseCode = 200;
                    response.Success = true;
                    DbContextTransaction.Commit();
                    return response;
                }
                catch (Exception ex)
                {
                    DbContextTransaction.Rollback();
                    throw;
                }
            }
        }

        public async Task DeleteOrderById(Guid OrderId, string userId)
        {
            try
            {
                var product = await _context.Orders
                                .Where(p => p.OrderId == OrderId && !p.IsDeleted)
                                .FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new Exception("Order not found!");
                }
                product.IsDeleted = true;
                product.DeletedDate = DateTime.Now;
                product.DeletedById = Guid.Parse(userId);
                _context.Orders.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
