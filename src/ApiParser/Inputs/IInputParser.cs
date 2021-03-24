using System.Collections.Generic;

namespace ApiParser.Inputs
{
    public interface IInputParser
    {
        IEnumerable<string[]> Parse(string inputFilePath);
    }
}