using Core.Entities;
using Core.Utilities.FileService;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Services.Abstract;
using Web.ViewModels.Expert;

namespace Web.Services.Concrete
{
    public class ExpertService:IExpertService
    {

        private readonly ModelStateDictionary _modelState;
        private readonly IExpertRepository _expertRepository;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExpertService(IExpertRepository expertRepository, IActionContextAccessor actionContextAccessor, IFileService fileService,
            IWebHostEnvironment webHostEnvironment)
        {
            _expertRepository = expertRepository;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
            _modelState = actionContextAccessor.ActionContext.ModelState;
            
        }

        public async Task<ExpertIndexVM> GetAllAsync()
        {
            var model = new ExpertIndexVM
            {
               Experts= await _expertRepository.GetAllAsync()
            };
            return model;

        }


        public async Task<bool> CreateAsync(ExpertCreateVM model)
        {
            if (!_modelState.IsValid) return false;

            var isExist = await _expertRepository.AnyAsync(c => c.Name.Trim().ToLower() == model.Name.Trim().ToLower());
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

           

            var expert = new Expert
            {
                Name = model.Name,
                Surname = model.Surname,
                CreatedAt=DateTime.Now,
                Position = model.Position,
                PhotoName = await _fileService.UploadAsync(model.MainPhoto, _webHostEnvironment.WebRootPath),



            };

            await _expertRepository.CreateAsync(expert);
            return true;
        }

        public async Task<ExpertUpdateVM> GetUpdateModelAsync(int id)
        {


            var expert = await _expertRepository.GetAsync(id);

            if (expert == null) return null;

            var model = new ExpertUpdateVM
            {
                Id = expert.Id,
                Name = expert.Name,
                Surname=expert.Surname,
                MainPhotoName=expert.PhotoName,
                Position=expert.Position,
            };

            return model;

        }

        public async Task<bool> UpdateAsync(ExpertUpdateVM model)
        {
            if (!_modelState.IsValid) return false;

            //var isExist = await _expertRepository.AnyAsync(c => c..Trim().ToLower() == model.Title.Trim().ToLower() && c.Id != model.Id);
            //if (isExist)
            //{
            //    _modelState.AddModelError("Title", "Bu adda kateqoriya mövcuddur");
            //    return false;
            //}
            if (model.MainPhoto != null)
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

            var expert = await _expertRepository.GetAsync(model.Id);

           


            if (expert != null)
            {
                expert.Name = model.Name;
                expert.ModifiedAt = DateTime.Now;
                expert.Surname = model.Surname;
                expert.Position = model.Position;
                

                if (model.MainPhoto != null)
                {
                    expert.PhotoName = await _fileService.UploadAsync(model.MainPhoto, _webHostEnvironment.WebRootPath);
                }

                await _expertRepository.UpdateAsync(expert);

            }
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var expert = await _expertRepository.GetAsync(id);
            if (expert != null)
            {
                _fileService.Delete(expert.PhotoName, _webHostEnvironment.WebRootPath);


                

                await _expertRepository.DeleteAsync(expert);

                return true;

            }

            return false;
        }


    }
}
