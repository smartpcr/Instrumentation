using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Instrumentation
{
	public enum Categories
	{
		Default
	}

	public enum Layers
	{
		Unknown,
		Entity,
		Repository,
		DataAccess,
		WebAPI,
		Worker
	}
}
