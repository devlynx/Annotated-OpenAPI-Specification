using System;

namespace Periwinkle.Swashbuckle
{
    internal class SwaggerHeaderValidationException : Exception
    {
        public SwaggerHeaderValidationException() { }

        public SwaggerHeaderValidationException(string message) : base(message) { }

        public SwaggerHeaderValidationException(string message, Exception innerException) : base(message, innerException) { }

    }
}