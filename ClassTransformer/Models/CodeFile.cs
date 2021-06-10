using System.Collections.Generic;

namespace ClassTransformer.Models
{
    public class CodeFile
    {
        public IEnumerable<CodeClass> Classes { get; set; } 
        public IEnumerable<CodeEnum> Enums { get; set; }
    }
}
