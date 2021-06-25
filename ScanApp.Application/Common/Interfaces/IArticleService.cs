using ScanApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IArticleService
    {
        Task<List<Article>> GetArticlesAsync();
    }
}