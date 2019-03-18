using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using ServiceStack;

namespace app.Repositories
{
    public class Bookmark
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Host { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Keywords { get; set; }
        public string Screenshot { get; set; }
        public DateTime? ScreenshotDate { get; set; }
        public string MetaTagsJson { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedById { get; set; }
        public bool IsDead { get; set; }
    }

    public class BookmarkDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Host { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Keywords { get; set; }
        public string Screenshot { get; set; }
        public DateTime? ScreenshotDate { get; set; }
        public List<Dictionary<string, object>> MetaTags { get; set; }
        public DateTime CreateDate { get; set; }
        public UserInfo CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public UserInfo ModifiedBy { get; set; }
        public bool IsDead { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }

    public interface IBookmarksRepository
    {
        IEnumerable<Bookmark> GetAll();
        Bookmark GetById(Guid id);
        Task<Bookmark> Create(Bookmark bookmark);
        Task<Bookmark> Update(Bookmark bookmark);
        void Delete(Guid id);
    }

    public class BookmarksRepository : IBookmarksRepository
    {
        private DataContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IScreenshotTaker _screenshotTaker;

        public BookmarksRepository(DataContext context, IScreenshotTaker screenshotTaker, IHostingEnvironment env)
        {
            _context = context;
            _screenshotTaker = screenshotTaker;
            _env = env;
        }

        public IEnumerable<Bookmark> GetAll()
        {
            return _context.Bookmarks;
        }

        public Bookmark GetById(Guid id)
        {
            return _context.Bookmarks.Find(id);
        }

        public async Task<Bookmark> Create(Bookmark bookmark)
        {
            if (string.IsNullOrWhiteSpace(bookmark.Url))
                throw new AppException("Url is required");

            if (_context.Bookmarks.Any(x => x.Url == bookmark.Url))
                throw new AppException("Url already exists");

            bookmark.Id = Guid.NewGuid();

            await TryPopulateBookmarkAsync(bookmark);
            await TryTakeScreenshotAsync(bookmark);

            bookmark.CreateDate = DateTime.UtcNow;
            bookmark.ModifiedDate = DateTime.UtcNow;

            _context.Bookmarks.Add(bookmark);
            _context.SaveChanges();

            return bookmark;
        }

        public async Task<Bookmark> Update(Bookmark bookmark)
        {
            var existing = _context.Bookmarks.Find(bookmark.Id);

            if (existing == null)
                throw new AppException("User not found");

            if (bookmark.Url != existing.Url)
            {
                // username has changed so check if the new username is already taken
                if (_context.Bookmarks.Any(x => x.Url == bookmark.Url))
                    throw new AppException("Url already exists");
            }

            // update user properties
            existing.PopulateWithNonDefaultValues(bookmark);

            await TryPopulateBookmarkAsync(existing);
            await TryTakeScreenshotAsync(existing);

            bookmark.ModifiedDate = DateTime.UtcNow;

            _context.Bookmarks.Update(existing);
            _context.SaveChanges();

            return existing;
        }

        public void Delete(Guid id)
        {
            var bookmark = _context.Bookmarks.Find(id);
            if (bookmark != null)
            {
                _context.Bookmarks.Remove(bookmark);
                _context.SaveChanges();
            }
        }

        private async Task TryPopulateBookmarkAsync(Bookmark bookmark)
        {
            try
            {
                var uri = new Uri(bookmark.Url);
                bookmark.Host = uri.Host;

                var html = "";
                using(var client = new HttpClient())
                {
                    var response = await client.GetAsync(bookmark.Url);
                    html = await response.Content.ReadAsStringAsync();
                }

                if (string.IsNullOrWhiteSpace(html))
                    return;

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var headTag = htmlDocument.DocumentNode.SelectSingleNode("//head");
                if (headTag == null)
                    return;

                var titleTag = headTag.SelectSingleNode("//title");
                if (titleTag != null)
                {
                    bookmark.Title = titleTag.InnerText;
                }

                var metaTagsList = new List<Dictionary<string, object>>();
                foreach (var metaTag in headTag.Descendants("meta"))
                {
                    var attName = metaTag.GetAttributeValue("name", null);
                    if (attName == "description")
                    {
                        bookmark.Description = metaTag.GetAttributeValue("content", "");
                    }
                    else if (attName == "keywords")
                    {
                        bookmark.Keywords = metaTag.GetAttributeValue("content", "");
                    }
                    else if (attName == "author")
                    {
                        bookmark.Author = metaTag.GetAttributeValue("content", "");
                    }
                    else if (metaTag.HasAttributes)
                    {
                        var tags = new Dictionary<string, object>();
                        foreach (var att in metaTag.Attributes)
                        {
                            tags.TryAdd(att.Name, att.Value);
                        }
                        metaTagsList.Add(tags);
                    }
                }

                if (metaTagsList.Any())
                {
                    bookmark.MetaTagsJson = metaTagsList.ToJson();
                }
            }
            catch
            {
                // we ignore errors
            }
        }

        private async Task TryTakeScreenshotAsync(Bookmark bookmark)
        {
            if (_screenshotTaker == null)
                return;

            var uploadsDirectory = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "data", "uploads"));
            if (!Directory.Exists(uploadsDirectory))
                Directory.CreateDirectory(uploadsDirectory);

            var screenshotFileNameFormat = bookmark.Id.ToString("N") + "-{0}.png";

            var iteration = 0;
            var screenshotFileName = string.Format(screenshotFileNameFormat, iteration++);
            var screenshotFilePath = Path.Combine(uploadsDirectory, screenshotFileName);
            while (File.Exists(screenshotFilePath))
            {
                screenshotFileName = string.Format(screenshotFileNameFormat, iteration++);
                screenshotFilePath = Path.Combine(uploadsDirectory, screenshotFileName);
            }

            if (await _screenshotTaker.TryTakeScreenshotAsync(bookmark.Url, screenshotFilePath))
            {
                bookmark.Screenshot = "/uploads/" + screenshotFileName;
                bookmark.ScreenshotDate = DateTime.UtcNow;
            }
        }
    }
}