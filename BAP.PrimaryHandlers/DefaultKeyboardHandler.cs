//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BAP.Types
//{
//	public class DefaultKeyboardHandler : IKeyboardHandler
//	{
//		public IBapKeyboardProvider CurrentKeyboard { get; internal set; }
//		private IServiceProvider services { get; set; }
//		public ILogger<DefaultKeyboardHandler> Logger { get; internal set; }

//		public DefaultKeyboardHandler(IServiceProvider serviceProvider, ILogger<DefaultKeyboardHandler> logger)
//		{
//			services = serviceProvider;
//			Logger = logger;
//			var keyboards = services.GetServices<IBapKeyboardProvider>();
//			foreach (var keyboard in keyboards)
//			{
//				//Todo - There needs to be a way to remember keyboards and load the same one that was loaded the previous time.
//				if (keyboard.GetType().Name.Contains("DefaultKeyboard"))
//				{
//					CurrentKeyboard = keyboard;
//				}
//			}
//			if (CurrentKeyboard == null)
//			{
//				CurrentKeyboard = keyboards.FirstOrDefault()!;
//			}
//		}

//		public bool ResetKeyboard()
//		{
//			if (CurrentKeyboard != null)
//			{
//				CurrentKeyboard.Reset();
//				return true;
//			}
//			return false;
//		}

//		public bool ReloadKeyboard()
//		{

//			if (CurrentKeyboard != null)
//			{
//				CurrentKeyboard = (IBapKeyboardProvider)services.GetRequiredService(CurrentKeyboard.GetType());
//				return true;
//			}
//			else
//			{
//				return false;
//			}
//		}

//		public bool SetSpecificKeyboard<T>() where T : IBapKeyboardProvider
//		{
//			var keyboardType = typeof(T);
//			if (CurrentKeyboard != null && CurrentKeyboard.GetType() == keyboardType)
//			{
//				return true;
//			}

//			if (CurrentKeyboard != null)
//			{
//				CurrentKeyboard.Dispose();
//				CurrentKeyboard = null;
//			}

//			CurrentKeyboard = services.GetRequiredService<T>();
//			return true;

//		}
//	}

//}
