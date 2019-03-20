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
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;
using ServiceStack.Text;

namespace app.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookmarksController : ControllerBase
    {
        public const string HUBMESSAGE_BOOKMARK_CREATED = "bookmark_created";
        public const string HUBMESSAGE_BOOKMARK_UPDATED = "bookmark_updated";
        public const string HUBMESSAGE_BOOKMARK_DELETED = "bookmark_deleted";

        private readonly IHubContext<AppHub> _hubContext;
        private IAuthRepository _authRepository;
        private readonly AppSettings _appSettings;
        private readonly IBookmarksRepository _bookmarksRepository;

        public BookmarksController(
            IAuthRepository authRepository,
            IBookmarksRepository bookmarksRepository,
            IOptions<AppSettings> appSettings,
            IHubContext<AppHub> hubContext)
        {
            _authRepository = authRepository;
            _bookmarksRepository = bookmarksRepository;
            _appSettings = appSettings.Value;
            _hubContext = hubContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery(Name = "skip")] int skip = 0, [FromQuery(Name = "take")] int take = 0, [FromQuery(Name = "q")] string q = null)
        {
            var query =
                _bookmarksRepository.GetAll().OrderByDescending(d => d.CreateDate)
                .Where(r =>
                    r.Title.Contains(q ?? "", StringComparison.OrdinalIgnoreCase) || (
                        r.Description != null && r.Description.Contains(q ?? "", StringComparison.OrdinalIgnoreCase))
                );
            var bookmarks = (skip >= 0 && take > 0) ?
                query
                .Skip(skip)
                .Take(take)
                .ToList() :
                query.ToList();
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
            return Ok(GetDtoById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookmarkDto bookmarkDto)
        {
            var userId = int.Parse(this.User.Identity.Name);

            var bookmark = new Bookmark().PopulateWith(bookmarkDto);
            bookmark.CreatedById = userId;
            bookmark.ModifiedById = userId;

            BookmarkDto dto = null;
            try
            {
                // save 
                bookmark = await _bookmarksRepository.Create(bookmark);
                dto = GetDtoById(bookmark.Id);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }

            await _hubContext.SendMessage(new HubMessage
            {
                Action = HUBMESSAGE_BOOKMARK_CREATED,
                    Data = new Dictionary<string, object>
                    { { "bookmark", dto }
                    }
            });

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookmarkDto bookmarkDto)
        {
            var userId = int.Parse(this.User.Identity.Name);

            var bookmark = new Bookmark().PopulateWith(bookmarkDto);
            bookmark.Id = id;
            bookmark.ModifiedById = userId;

            BookmarkDto dto = null;
            try
            {
                // save 
                bookmark = await _bookmarksRepository.Update(bookmark);
                dto = GetDtoById(bookmark.Id);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }

            await _hubContext.SendMessage(new HubMessage
            {
                Action = HUBMESSAGE_BOOKMARK_UPDATED,
                    Data = new Dictionary<string, object>
                    { { "bookmark", dto }
                    }
            });

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            BookmarkDto dto = null;
            try
            {
                _bookmarksRepository.Delete(id);
                dto = new BookmarkDto { Id = id };
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }

            await _hubContext.SendMessage(new HubMessage
            {
                Action = HUBMESSAGE_BOOKMARK_DELETED,
                    Data = new Dictionary<string, object>
                    { { "bookmark", dto }
                    }
            });

            return Ok();
        }

        private BookmarkDto GetDtoById(Guid id)
        {
            var bookmark = _bookmarksRepository.GetById(id);
            var createdBy = _authRepository.GetById(bookmark.CreatedById)?.ConvertTo<UserInfo>();
            var modifiedBy = _authRepository.GetById(bookmark.ModifiedById)?.ConvertTo<UserInfo>();

            var dto = new BookmarkDto().PopulateWith(bookmark);
            dto.CreatedBy = createdBy;
            dto.ModifiedBy = modifiedBy;
            dto.MetaTags = bookmark.MetaTagsJson?.FromJson<List<Dictionary<string, object>>>();

            return dto;
        }
    }
}