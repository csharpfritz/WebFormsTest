using System;

namespace Fritz.WebFormsTest
{
    public class WebApplicationProxyException : ApplicationException
    {
        public WebApplicationProxyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
