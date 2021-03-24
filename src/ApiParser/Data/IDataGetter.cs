using System.Threading.Tasks;

namespace ApiParser.Data
{
    public interface IDataGetter
    {
        Task<string> GetData(string urlData);
    }
}
