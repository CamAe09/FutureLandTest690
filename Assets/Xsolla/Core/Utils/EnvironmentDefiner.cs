using UnityEngine;

namespace Xsolla.Core
{
	public static class EnvironmentDefiner
	{
		public static bool IsEditor
		{
			get
			{
				switch (Application.platform)
				{
					case RuntimePlatform.LinuxEditor:
					case RuntimePlatform.OSXEditor:
					case RuntimePlatform.WindowsEditor:
						return true;
					default:
						return false;
				}
			}
		}

		public static bool IsStandalone
		{
			get
			{
				switch (Application.platform)
				{
					case RuntimePlatform.LinuxPlayer:
					case RuntimePlatform.OSXPlayer:
					case RuntimePlatform.WindowsPlayer:
						return true;
					default:
						return false;
				}
			}
		}

		public static bool IsStandaloneOrEditor => (IsStandalone || IsEditor);
		public static bool IsAndroid => Application.platform == RuntimePlatform.Android;
		public static bool IsWebGL => Application.platform == RuntimePlatform.WebGLPlayer;
	}
}
