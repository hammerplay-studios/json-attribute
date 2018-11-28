using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hammerplay.Utils.JsonAttribute {
	[AttributeUsage(AttributeTargets.Field)]
	public class JsonAttribute : Attribute {
		private string parameterPath;

		public string ParameterPath {
			get { return parameterPath; }
		}

		public JsonAttribute(string parameterPath) {
			this.parameterPath = parameterPath;
		}
	}
}
