using LMS.Data.Models;
using LMS.Data;
using LMS.Services.ViewModels;
using Microsoft.EntityFrameworkCore;
using LMS.Data.Enum;
using LMS.Services.Interfaces;
using LMS.Services.Common;
using LMS.Services.Constants;
using LMS.Services.Helpers;
using MapsterMapper;
using File = LMS.Data.Models.File;

namespace LMS.Services
{
    public class FileService(
        ApplicationDbContext appDbContext,
        IMapper mapper,
        IAccountService accountService
        ) : IFileService
    {
        private readonly ApplicationDbContext _appDbContext = appDbContext;
        private readonly IMapper _mapper = mapper;
        private readonly IAccountService _accountService = accountService;

        public async Task<FileViewModel> CreateFile(FileViewModel model)
        {
            try
            {
                var fileToBeCreated = _mapper.Map<FileViewModel, File>(model);
                fileToBeCreated.CreatedBy = _accountService.GetCurrentLoggedInUserId();
                var result = await _appDbContext.Files.AddAsync(fileToBeCreated);
                _appDbContext.SaveChanges();
                return _mapper.Map<File, FileViewModel>(result.Entity);
            } catch (Exception)
            {
                throw;
            }
        }
        public async Task<FileViewModel> UpdateFile(FileViewModel model)
        {
            try
            {
                var fileToBeUpdated = await _appDbContext.Files
                    .FirstOrDefaultAsync(x => x.Id == model.Id) ?? throw new Exception("File not found!");

                fileToBeUpdated.Name = model.Name;
                fileToBeUpdated.Size = model.Size;
                fileToBeUpdated.Category = model.Category;
                fileToBeUpdated.Description = model.Description;
                fileToBeUpdated.Url = model.Url;
                fileToBeUpdated.FileStatus = model.FileStatus;
                fileToBeUpdated.ModifiedBy = _accountService.GetCurrentLoggedInUserId();
                fileToBeUpdated.ModifiedDate = DateTime.UtcNow;
                var result = _appDbContext.Files.Update(fileToBeUpdated);
                await _appDbContext.SaveChangesAsync();
                return _mapper.Map<File, FileViewModel>(result.Entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<FileViewModel>> GetAllFiles()
        {
            try
            {
                var files = await _appDbContext.Files
                    .ToListAsync();
                return _mapper.Map<List<File>, List<FileViewModel>>(files);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<FileViewModel>> GetFilesForEmployee(int id)
        {
            try
            {
                var files = await _appDbContext.Files
                    .Include(x => x.UploadedBy)
                    .Where(x => x.UploadedById == id)
                    .ToListAsync();
                return _mapper.Map<List<File>, List<FileViewModel>>(files);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataTableResult<FileViewModel>> GetAllFilesSsr(DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var files = await _appDbContext.Files
                    .ToListAsync();
                var fileViewModels = _mapper.Map<List<File>, List<FileViewModel>>(files);
                return DataTableResultHandler<FileViewModel>.ResultToSsr(fileViewModels, dataTableConfiguration, DataTableConfigurationOptions.All);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<FileViewModel>> GetFilesByName(string fileName)
        {
            try
            {
                var files = await _appDbContext.Files
                    .Where(x => x.Name.Contains(fileName))
                    .ToListAsync();
                return _mapper.Map<List<File>, List<FileViewModel>>(files);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<FileViewModel> GetFileById(int id)
        {
            try
            {
                var file = await _appDbContext.Files
                    .Include(x => x.UploadedBy  )
                    .FirstOrDefaultAsync(x => x.Id == id);
                return file is null
                    ? throw new Exception("File not found!")
                    : _mapper.Map<File, FileViewModel>(file);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<FileViewModel> DeleteFileById(int id)
        {
            try
            {
                var file = await _appDbContext.Files
                    .FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("File not found!");

                file.Status = DataRecordStatus.Deleted;
                file.DeletedBy = _accountService.GetCurrentLoggedInUserId();
                file.DeletedDate = DateTime.UtcNow;
                _appDbContext.Files.Update(file);
                await _appDbContext.SaveChangesAsync();
                return _mapper.Map<File, FileViewModel>(file);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
