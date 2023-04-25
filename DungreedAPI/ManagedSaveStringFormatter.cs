using System;
using UnityEngine;

namespace DungreedAPI
{
    internal readonly struct ManagedSaveStringFormatter<TData>
    {
		private readonly string format;
		private readonly Func<TData, int> getId;
		private readonly Func<TData, string> getName;

		internal ManagedSaveStringFormatter(string format, Func<TData, int> getId, Func<TData, string> getName)
		{
			this.format = format;
			this.getId = getId;
			this.getName = getName;
		}

		internal string IdFormat(TData data)
        {
			return string.Format(format, getId(data));
        }

		internal string NameFormat(TData data)
		{
			return string.Format(format, getName(data));
		}
	}
}
