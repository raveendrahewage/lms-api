using LMS.Services.Common;
using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface IFileService
    {
        Task<FileViewModel> CreateFile(FileViewModel model);
        Task<FileViewModel> DeleteFileById(int id);
        Task<List<FileViewModel>> GetAllFiles();
        Task<List<FileViewModel>> GetFilesForEmployee(int id);
        Task<DataTableResult<FileViewModel>> GetAllFilesSsr(DataTableConfiguration dataTableConfiguration);
        Task<List<FileViewModel>> GetFilesByName(string fileName);
        Task<FileViewModel> GetFileById(int id);
        Task<FileViewModel> UpdateFile(FileViewModel model);
    }
}