using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.ErrorService
{
    public class ErrorDto
    {
        public List<string> Eroors { get; private set; }
        public bool IsShow { get; private set; }

        public ErrorDto()
        {
            Eroors = new List<string>();
        }
        public ErrorDto(string eroors, bool isShow)
        {
            Eroors.Add(eroors);
            isShow = true;
        }
        public ErrorDto(List<string> eroors, bool ısShow)
        {
            Eroors = eroors;
            IsShow = ısShow;
        }
    }
}
