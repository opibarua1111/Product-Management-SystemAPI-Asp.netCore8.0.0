using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductManagementAPI.Data;
using ProductManagementAPI.IServices;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.RequestModels;
using ProductManagementAPI.Models.ResponseModels;

namespace ProductManagementAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly SubContext _context;
        public ProductService(SubContext context)
        {
            _context = context;
        }
        public async Task<CustomResponse> GetProductList(ListRequestModel model)
        {
            try
            {
                int pg = model.page;
                int sl = model.length;
                string sv = model.searchValue;
                // Calculate the number of items to skip
                int skip = (pg - 1) * sl;

                // Retrieve the products and include variants where IsDeleted is false
                var products = await _context.Products
                                    .Where(p => !p.IsDeleted &&
                                                (
                                                  string.IsNullOrEmpty(sv) || p.ProductName.Contains(sv) ||
                                                  p.Type.Contains(sv) || p.Brand.Contains(sv)
                                                )
                                           )
                                    .Include(p => p.Variants.Where(v => !v.IsDeleted))
                                    .Select(p => new
                                    {
                                        p.ProductId,
                                        p.ProductName,
                                        p.Type,
                                        p.Brand,
                                        p.Description,
                                        p.Price,
                                        p.Stock,
                                        p.Variants,
                                        p.IsDeleted
                                    })
                                    .OrderBy(p => p.ProductName).ThenBy(p => p.Type)
                                    .Skip(skip)
                                    .Take(sl)
                                    .ToListAsync();

                var totalProducts = await _context.Products
                    .Where(p => !p.IsDeleted)
                    .CountAsync();

                return new CustomResponse
                {
                    ReponseCode = 200,
                    data = JsonConvert.SerializeObject(new
                    {
                        Products = products,
                        TotalItems = totalProducts,
                    })
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<CustomResponse> GetProductById(Guid ProductId)
        {
            try
            {
                // Retrieve the product and include variants where IsDeleted is false
                var product = await _context.Products
                    .Where(p => p.ProductId == ProductId && !p.IsDeleted)
                    .Include(p => p.Variants.Where(v => !v.IsDeleted))
                    .Select(p => new
                    {
                        p.ProductId,
                        p.ProductName,
                        p.Type,
                        p.Brand,
                        p.Description,
                        p.Price,
                        p.Stock,
                        p.Variants,
                        p.IsDeleted
                    }).FirstOrDefaultAsync();

                if (product == null)
                {
                    return new CustomResponse
                    {
                        Success = false,
                        Message = "Product not found"
                    };
                }
                return new CustomResponse
                {
                    Success = true,
                    ReponseCode = 200,
                    data = JsonConvert.SerializeObject(new
                    {
                        product = product,
                    })
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task SaveProduct(ProductRequestModel data, string userId)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region CREATE
                    if (data.ProductId == Guid.Empty) // If data.ProductId == null mean Product not created
                    {
                        var product = new Product();
                        List<Variant> nVariants = new List<Variant>();

                        Guid NewProductId = Guid.NewGuid();
                        product.ProductId = NewProductId;
                        product.ProductName = data.ProductName;
                        product.Brand = data.Brand;
                        product.Type = data.Type;
                        product.Description = data.Description;
                        product.Price = data.Price;
                        product.Image = data.Image;
                        product.Stock = data.Stock;
                        product.IsDeleted = false;
                        product.CreatedDate = DateTime.Now;
                        product.CreatedById = Guid.Parse(userId);
                        #region Add multiple Variant
                        foreach (var variant in data.Variants)
                        {
                            var nvariant = new Variant();
                            nvariant.VariantId = Guid.NewGuid();
                            nvariant.ProductId = NewProductId;
                            nvariant.Color = variant.Color;
                            nvariant.Size = variant.Size;
                            nvariant.Description = variant.Description;
                            nvariant.Specification = variant.Specification;
                            nvariant.Stock = variant.Stock;
                            nvariant.Price = variant.Price;
                            nvariant.Image = variant.Image;
                            nvariant.Stock = variant.Stock;
                            nvariant.IsDeleted = false;
                            nvariant.CreatedDate = DateTime.Now;
                            nvariant.CreatedById = Guid.Parse(userId);
                            nVariants.Add(nvariant);
                        }
                        #endregion
                        product.Variants = nVariants;
                        _context.Add(product);
                    }
                    #endregion
                    #region UPDATE
                    else
                    {
                        // Retrieve the existing product
                        var existingProduct = await _context.Products
                            .Where(p => p.ProductId == data.ProductId && !p.IsDeleted)
                            .Include(p => p.Variants)
                            .FirstOrDefaultAsync();

                        if (existingProduct == null)
                        {
                            throw new ArgumentException("Product not found");
                        }

                        // Update product data
                        existingProduct.ProductName = data.ProductName;
                        existingProduct.Description = data.Description;
                        existingProduct.ModifiedDate = DateTime.Now;
                        existingProduct.ModifiedById = Guid.Parse(userId);
                        existingProduct.IsDeleted = data.IsDeleted;

                        // Update variants
                        foreach (var variant in data.Variants)
                        {
                            var existingVariant = existingProduct.Variants
                                .Where(v => v.VariantId == variant.VariantId)
                                .FirstOrDefault();

                            if (existingVariant != null)
                            {
                                // Update existing variant
                                existingVariant.Color = variant.Color;
                                existingVariant.Size = variant.Size;
                                existingVariant.Specification = variant.Specification;
                                existingVariant.Price = variant.Price;
                                existingVariant.ModifiedDate = DateTime.Now;
                                existingVariant.ModifiedById = Guid.Parse(userId);
                                existingVariant.IsDeleted = variant.IsDeleted;
                            }
                            else
                            {
                                // Add new variant
                                Variant nVariant = new Variant();
                                nVariant.VariantId = Guid.NewGuid();
                                nVariant.ProductId = data.ProductId;
                                nVariant.Color = variant.Color;
                                nVariant.Size = variant.Size;
                                nVariant.Specification = variant.Specification;
                                nVariant.Stock = variant.Stock;
                                nVariant.Description = variant.Description;
                                nVariant.CreatedDate = DateTime.Now;
                                nVariant.CreatedById = Guid.Parse(userId);
                                nVariant.IsDeleted = false;
                                existingProduct.Variants.Add(nVariant);
                            }
                        }

                        _context.Update(existingProduct);
                    }
                    #endregion
                    await _context.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw;
                }
            }
        }
        public async Task DeleteProductById(Guid ProductId, string userId)
        {
            try
            {
                var product = await _context.Products
                                .Where(p => p.ProductId == ProductId && !p.IsDeleted)
                                .FirstOrDefaultAsync();
                if(product == null)
                {
                    throw new Exception("Product not found!");
                }
                product.IsDeleted = true;
                product.DeletedDate = DateTime.Now;
                product.DeletedById = Guid.Parse(userId);
                _context.Add(product);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
