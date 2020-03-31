using System;
using System.Text;

namespace Beginor.NetCoreApp.Common {

    public class Jwt {
        public string Secret { get; set; }
        public TimeSpan ExpireTimeSpan { get; set; }
        public byte[] SecretKey {
            get {
                if (string.IsNullOrEmpty(Secret)) {
                    throw new InvalidOperationException("Secret is empty!");
                }
                return Encoding.ASCII.GetBytes(Secret);
            }
        }
    }

}
