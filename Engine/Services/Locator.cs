using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine.Services
{
	public interface IServiceLocator
	{
		T Resolve<T>();
	}

    public class Locator : IServiceLocator
    {
        private readonly Dictionary<Type, Func<object>> services;

        public Locator()
        {
            this.services = new Dictionary<Type, Func<object>>();
        }

        public void Register<T>(Func<T> resolver)
        {
            this.services[typeof(T)] = () => resolver();
        }

        public T Resolve<T>()
        {
            return (T)this.services[typeof(T)]();
        }
    }
}
