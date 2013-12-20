using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.AstoundingDock.Services
{
    public class ServiceManager
    {
        static readonly ServiceManager _instance = new ServiceManager();
        static readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

        ServiceManager() { } // Private constructor

        public static T RegisterService<T>(IService service)
        {
            return _instance.Register<T>(service);
        }

        public static T GetService<T>() where T : IService
        {
            return _instance.Get<T>();
        }

        public void RemoveService<T>() where T : IService
        {
            _instance.Remove<T>();
        }

        public void ClearServices()
        {
            _instance.Clear();
        }

        T Register<T>(IService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            lock (_services)
            {
                if (_services.ContainsKey( typeof(T) ))
                    throw new ArgumentException("Service already registered");
                _services[typeof(T)] = service;
            }
            return (T)service;
        }

        T Get<T>() where T : IService
        {
            lock (_services)
            {
                if (_services.ContainsKey( typeof(T) ))
                    return (T)_services[typeof(T)];
                else
                    throw new ArgumentException("Service not registered: " + typeof(T));
            }
        }

        void Remove<T>() where T : IService
        {
            lock (_services)
            {
                if (_services.ContainsKey( typeof(T) ))
                    _services.Remove(typeof(T));
            }
        }

        void Clear()
        {
            lock (_services)
            {
                _services.Clear();
            }
        }
    }
}
