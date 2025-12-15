using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detailly.Application.Common.Exceptions;
public class DetaillyTimeoutException : Exception
{
    public DetaillyTimeoutException(string message) : base(message) { }
}
