using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XueLeMeBackend.Data;

namespace XueLeMeBackend.Services
{
    public class TopicService
    {
        public TopicService(XueLeMeContext context)
        {
            Context = context;
        }

        public XueLeMeContext Context { get; }

    }
}
