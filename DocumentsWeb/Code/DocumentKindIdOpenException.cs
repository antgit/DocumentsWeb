using System;
using System.Runtime.Serialization;

namespace DocumentsWeb
{
    [Serializable]
    public class DocumentKindIdOpenException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DocumentKindIdOpenException()
        {
        }

        public DocumentKindIdOpenException(string message) : base(message)
        {
        }

        public DocumentKindIdOpenException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DocumentKindIdOpenException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}