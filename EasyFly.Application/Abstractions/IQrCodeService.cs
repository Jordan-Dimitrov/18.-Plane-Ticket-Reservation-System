using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Abstractions
{
    public interface IQrCodeService
    {
        public byte[] GenerateQRCode(string inputText, int size);
    }
}
