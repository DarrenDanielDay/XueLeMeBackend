using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Services
{
    public interface IConnectionService
    {
        public void Attach(string connectionId, int userId);
        public void Detach(string connectionId);
        public int? GetUserId(string connectionId);
        public string GetConnectionId(int userId);
        public bool IsOnline(int userId);
    }
}
