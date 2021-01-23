﻿using System;

namespace TRGE.Core
{
    class ChecksumMismatchException : Exception
    {
        public ChecksumMismatchException()
            : base() { }

        public ChecksumMismatchException(string message)
            : base(message) { }

        public ChecksumMismatchException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}