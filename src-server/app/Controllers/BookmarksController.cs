using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using app.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;
using ServiceStack.Text;

namespace app.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BookmarksController : ControllerBase
    {
        private IAuthRepository _authRepository;
        private readonly AppSettings _appSettings;
        private readonly IBookmarksRepository _bookmarksRepository;

        public BookmarksController(
            IAuthRepository authRepository,
            IBookmarksRepository bookmarksRepository,
            IOptions<AppSettings> appSettings)
        {
            _authRepository = authRepository;
            _bookmarksRepository = bookmarksRepository;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var bookmarks = _bookmarksRepository.GetAll().OrderByDescending(d => d.CreateDate).ToList();
            var createdByIds = bookmarks.Select(b => b.CreatedById).Distinct();
            var modifiedByIds = bookmarks.Select(b => b.ModifiedById).Distinct();
            var users = _authRepository.GetAll().Where(u => createdByIds.Contains(u.Id) || modifiedByIds.Contains(u.Id)).ToList();

            var dtos = new List<BookmarkDto>();
            foreach (var bookmark in bookmarks)
            {
                var dto = new BookmarkDto().PopulateWith(bookmark);
                dto.CreatedBy = users.FirstOrDefault(u => u.Id == bookmark.CreatedById)?.ConvertTo<UserInfo>();
                dto.ModifiedBy = users.FirstOrDefault(u => u.Id == bookmark.ModifiedById)?.ConvertTo<UserInfo>();
                dto.MetaTags = bookmark.MetaTagsJson?.FromJson<List<Dictionary<string, object>>>();
                dtos.Add(dto);
            }

            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var bookmark = _bookmarksRepository.GetById(id);
            var createdBy = _authRepository.GetById(bookmark.CreatedById)?.ConvertTo<UserInfo>();
            var modifiedBy = _authRepository.GetById(bookmark.ModifiedById)?.ConvertTo<UserInfo>();

            var dto = new BookmarkDto().PopulateWith(bookmark);
            dto.CreatedBy = createdBy;
            dto.ModifiedBy = modifiedBy;
            dto.MetaTags = bookmark.MetaTagsJson?.FromJson<List<Dictionary<string, object>>>();

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookmarkDto bookmarkDto)
        {
            var userId = int.Parse(this.User.Identity.Name);

            var bookmark = new Bookmark().PopulateWith(bookmarkDto);
            bookmark.CreatedById = userId;
            bookmark.ModifiedById = userId;

            try
            {
                // save 
                bookmark = await _bookmarksRepository.Create(bookmark);
                return GetById(bookmark.Id);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookmarkDto bookmarkDto)
        {
            var userId = int.Parse(this.User.Identity.Name);

            var bookmark = new Bookmark().PopulateWith(bookmarkDto);
            bookmark.Id = id;
            bookmark.ModifiedById = userId;

            try
            {
                // save 
                bookmark = await _bookmarksRepository.Update(bookmark);
                return GetById(bookmark.Id);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _bookmarksRepository.Delete(id);
            return Ok();
        }
    }
}