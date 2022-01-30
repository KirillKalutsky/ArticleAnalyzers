using System.Threading.Tasks;

namespace TextAnalizator
{
    public interface IAnalyzer
    {
        public Task<string> AnalizeCategoryAsync(string text, string defaultCategory);
    }
}