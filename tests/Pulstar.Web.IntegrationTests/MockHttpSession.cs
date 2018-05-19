namespace Pulstar.Web.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class MockHttpSession : ISession
    {
        private Dictionary<string, object> _sessionStorage;

        public MockHttpSession(Dictionary<string, object> sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public IEnumerable<string> Keys
        {
            get { return _sessionStorage.Keys; }
        }

        public bool IsAvailable { get; set; }

        public string Id { get; set; }

        public object this[string name]
        {
            get { return _sessionStorage[name]; }
            set { _sessionStorage[name] = value; }
        }

        public void Clear()
        {
            _sessionStorage.Clear();
        }

        public void Remove(string key)
        {
            _sessionStorage.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _sessionStorage[key] = value;
        }

        public string GetString(string name)
        {
            return _sessionStorage[name] as string;
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            if (_sessionStorage[key] != null)
            {
                value = Encoding.ASCII.GetBytes(_sessionStorage[key].ToString());
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public Task LoadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
