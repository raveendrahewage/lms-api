using LMS.Api.Helpers.Interfaces;
using LMS.Services;
using LMS.Services.Common;
using LMS.Services.Interfaces;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Api.Controllers
{
    [Authorize]
    [Route("api/file")]
    [ApiController]
    public class FileController(IFileService fileService, IApiResponseHelper apiResponseHelper) : ControllerBase
    {
        private readonly IFileService _fileService = fileService;
        private readonly IApiResponseHelper _apiResponseHelper = apiResponseHelper;

        [HttpPost]
        public async Task<IActionResult> CreateNewFile(FileViewModel model)
        {
            try
            {
                var result = await _fileService.CreateFile(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "File created successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateFile(FileViewModel model)
        {
            try
            {
                var result = await _fileService.UpdateFile(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "File updated successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteFileById(int id)
        {
            try
            {
                var result = await _fileService.DeleteFileById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "File deleted successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("get-files")]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                var result = await _fileService.GetAllFiles();
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("get-files-for-employee/{id}")]
        public async Task<IActionResult> GetFilesForEmployee(int id)
        {
            try
            {
                var result = await _fileService.GetFilesForEmployee(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("get-files/ssr")]
        public async Task<IActionResult> GetAllFilesSsr(DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var result = await _fileService.GetAllFilesSsr(dataTableConfiguration);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetFileById(int id)
        {
            try
            {
                var result = await _fileService.GetFileById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("get-file-by-name/{fileName}")]
        public async Task<IActionResult> GetFilesByName(string fileName)
        {
            try
            {
                var result = await _fileService.GetFilesByName(fileName);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
