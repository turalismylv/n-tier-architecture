﻿using Core.Entities;
using Core.Utilities.FileService;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Services.Abstract;
using Web.ViewModels.Product;
using Web.ViewModels.Product.ProductPhoto;

namespace Web.Services.Concrete
{
    public class ProductService :IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IActionContextAccessor actionContextAccessor;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductPhotoRepository _productPhotoRepository;
        private readonly ModelStateDictionary _modelState;

        public ProductService(IProductRepository productRepository, 
            IActionContextAccessor actionContextAccessor,
            IFileService fileService,
            IWebHostEnvironment webHostEnvironment,
            ICategoryRepository categoryRepository,
            IProductPhotoRepository productPhotoRepository)
        {
            _productRepository = productRepository;
            this.actionContextAccessor = actionContextAccessor;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
            _categoryRepository = categoryRepository;
            _productPhotoRepository = productPhotoRepository;
            _modelState = actionContextAccessor.ActionContext.ModelState;
        }



        public async Task<ProductIndexVM> GetAllAsync()
        {
            var model = new ProductIndexVM
            {
                Products = await _productRepository.GetAllAsync()
            };
            return model;

        }
        public async Task<ProductCreateVM> GetCreateModelAsync()
        {
            var categories=await _categoryRepository.GetAllAsync();
            var model = new ProductCreateVM
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToList()
            };

            return model;
        }

        public async Task<bool> CreateAsync(ProductCreateVM model)
        {
            if (!_modelState.IsValid) return false;

            var isExist = await _productRepository.AnyAsync(c => c.Title.Trim().ToLower() == model.Title.Trim().ToLower());
            if (isExist)
            {
                _modelState.AddModelError("Title", "Bu adda kateqoriya mövcuddur");
                return false;
            }

            if (!_fileService.IsImage(model.MainPhoto))
            {
                _modelState.AddModelError("MainPhoto", "File image formatinda deyil zehmet olmasa image formasinda secin!!");
                return false;
            }
            if (!_fileService.CheckSize(model.MainPhoto, 300))
            {
                _modelState.AddModelError("MainPhoto", "File olcusu 300 kbdan boyukdur");
                return false;
            }

            bool hasError = false;
            foreach (var photo in model.Photos)
            {
                if (!_fileService.IsImage(photo))
                {
                    _modelState.AddModelError("Photos", $"{photo.FileName} yuklediyiniz file sekil formatinda olmalidir");
                    hasError = true;

                }
                else if (!_fileService.CheckSize(photo, 300))
                {
                    _modelState.AddModelError("Photos", $"{photo.FileName} ci yuklediyiniz sekil 300 kb dan az olmalidir");
                    hasError = true;

                }

            }

            if (hasError) { return false; }

            var product = new Product
            {
                Title = model.Title,
                Price = model.Price,
                Description = model.Description,
                Quantity = model.Quantity,
                Weight = model.Weight,
                CreatedAt=DateTime.Now,
                CategoryId = model.CategoryId,
                Status = model.Status,
                MainPhotoPath = await _fileService.UploadAsync(model.MainPhoto, _webHostEnvironment.WebRootPath),


            };

            await _productRepository.CreateAsync(product);

            int order = 1;
            foreach (var photo in model.Photos)
            {
                var productPhoto = new ProductPhoto
                {
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,
                    ProductId = product.Id
                };
              await  _productPhotoRepository.CreateAsync(productPhoto);

                order++;
            }
            

            return true;
        }

        public async Task<ProductUpdateVM> GetUpdateModelAsync(int id)
        {


            var categories = await _categoryRepository.GetAllAsync();
            var product = await _productRepository.GetAsync(id);

            if (product == null) return null;

            var model = new ProductUpdateVM
            {
                Id = product.Id,
                Status=product.Status,
                Categories = categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToList(),
                CategoryId = product.CategoryId,
                Description = product.Description,
                Price = product.Price,
                Weight = product.Weight,
                Title = product.Title,
                Quantity = product.Quantity,
                MainPhotoPath=product.MainPhotoPath,
                ProductPhotos=await _productPhotoRepository.GetAllAsync(),

            };

            return model;

        }

        public async Task<bool> UpdateAsync(ProductUpdateVM model)
        {
            if (!_modelState.IsValid) return false;

            var isExist = await _productRepository.AnyAsync(c => c.Title.Trim().ToLower() == model.Title.Trim().ToLower()&& c.Id != model.Id);
            if (isExist)
            {
                _modelState.AddModelError("Title", "Bu adda kateqoriya mövcuddur");
                return false;
            }
            if (model.MainPhoto!=null)
            {
                if (!_fileService.IsImage(model.MainPhoto))
                {
                    _modelState.AddModelError("MainPhoto", "File image formatinda deyil zehmet olmasa image formasinda secin!!");
                    return false;
                }
                if (!_fileService.CheckSize(model.MainPhoto, 300))
                {
                    _modelState.AddModelError("MainPhoto", "File olcusu 300 kbdan boyukdur");
                    return false;
                }
            }

            var product = await _productRepository.GetWithPhotosAsync(model.Id);

            bool hasError = false;

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        _modelState.AddModelError("Photos", $"{photo.FileName} yuklediyiniz file sekil formatinda olmalidir");
                        hasError = true;
                    }
                    else if (!_fileService.CheckSize(photo, 300))
                    {
                        _modelState.AddModelError("Photos", $"{photo.FileName} ci yuklediyiniz sekil 300 kb dan az olmalidir");
                        hasError = true;
                    }
                }

                if (hasError) { return false; }

                int order = product.ProductPhotos.OrderByDescending(pp => pp.Order).FirstOrDefault().Order;
                foreach (var photo in model.Photos)
                {
                    var productPhoto = new ProductPhoto
                    {
                        Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                        Order = ++order,
                        ProductId = product.Id
                    };
                    await _productPhotoRepository.CreateAsync(productPhoto);
                   


                }
            }


            
            if (product != null)
            {
                product.Title = model.Title;
                product.ModifiedAt = DateTime.Now;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Quantity = model.Quantity;
                product.Weight = model.Weight;
                product.CategoryId = model.CategoryId; 
                product.Status = model.Status;

                if (model.MainPhoto!=null)
                {
                    product.MainPhotoPath =await _fileService.UploadAsync(model.MainPhoto,_webHostEnvironment.WebRootPath);
                }

                await _productRepository.UpdateAsync(product);

            }
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetAsync(id);
            if (product != null)
            {
                _fileService.Delete(product.MainPhotoPath, _webHostEnvironment.WebRootPath);


                foreach (var photo in await _productPhotoRepository.GetAllAsync())
                {
                    _fileService.Delete(photo.Name, _webHostEnvironment.WebRootPath);

                }

                await _productRepository.DeleteAsync(product);

                return true;

            }

            return false;
        }

        public async Task<bool> DeletePhotoAsync(int id)
        {
            var productPhoto = await _productPhotoRepository.GetAsync(id);
            if (productPhoto != null)
            {
                _fileService.Delete(productPhoto.Name, _webHostEnvironment.WebRootPath);


                

                await _productPhotoRepository.DeleteAsync(productPhoto);

                return true;

            }

            return false;
        }


        public async Task<ProductPhotoUpdateVM> GetPhotoUpdateModelAsync(int id)
        {


            
            var productPhoto = await _productPhotoRepository.GetAsync(id);

            if (productPhoto == null) return null;

            var model = new ProductPhotoUpdateVM
            {
                Id = id,
               Order=productPhoto.Order,
               ProductId = productPhoto.ProductId,
            };

            return model;

        }


        public async Task<bool> UpdatePhotoAsync(ProductPhotoUpdateVM model)
        {
            if (!_modelState.IsValid) return false;

            var productPhoto = await _productPhotoRepository.GetAsync(model.Id);

            if (productPhoto != null)
            {
                productPhoto.Order = model.Order;
                
                await _productPhotoRepository.UpdateAsync(productPhoto);

            }
            return true;
        }



    }
}
