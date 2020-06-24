using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Services
{
    public class MemoryConnectionService : IConnectionService
    {
        private Dictionary<string, int> UserIds = new Dictionary<string, int>();
        private Dictionary<int, string> ConnectionIds = new Dictionary<int, string>();

        public void Attach(string connectionId, int userId)
        {
            if (UserIds.ContainsKey(connectionId))
            {
                UserIds.Remove(connectionId);
            }
            if (ConnectionIds.ContainsKey(userId))
            {
                ConnectionIds.Remove(userId);
            }
            UserIds[connectionId] = userId;
            ConnectionIds[userId] = connectionId;
        }

        public void Detach(string connectionId)
        {
            if (UserIds.ContainsKey(connectionId))
            {
                int userId = UserIds[connectionId];
                UserIds.Remove(connectionId);
                ConnectionIds.Remove(userId);
            }
        }

        public string GetConnectionId(int userId)
        {
            return ConnectionIds.TryGetValue(userId, out string connectionId) ? connectionId : null;
        }

        public int? GetUserId(string connectionId)
        {
            return UserIds.TryGetValue(connectionId, out int userId) ? (int?) userId : null;
        }

        public bool IsOnline(int userId)
        {
            return ConnectionIds.ContainsKey(userId);
        }
    }
}
