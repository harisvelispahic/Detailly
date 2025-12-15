using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detailly.Application.Common.Exceptions;
public class DetaillyExternalServiceException : Exception
{
    public DetaillyExternalServiceException(string message) : base(message) { }
}
