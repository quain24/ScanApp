using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScanApp.Infrastructure.Services
{
    public class ArticleService : IArticleService
    {
        private List<Article> _articles;

        public ArticleService()
        {
            var Articles = File.ReadAllText("Articles.json");
            _articles = JsonSerializer.Deserialize<List<Article>>(Articles);
        }

        public async Task<List<Article>> GetArticlesAsync()
        {
            return await Task.FromResult(_articles);
        }
    }
}