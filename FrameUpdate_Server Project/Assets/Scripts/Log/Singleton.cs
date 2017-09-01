using UnityEngine;
using System.Collections;


/// <summary>
/// Be aware this will not prevent a non singleton constructor5.
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;     
	private static object _lock = new object();
	public static T Instance
	{       
		get       
		{         
			if (applicationIsQuitting) 
			{
				return null;         
			}
			lock(_lock)
			{ 
				if (_instance == null)
				{
					_instance = (T) FindObjectOfType(typeof(T));
					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) "+ typeof(T).ToString();
						DontDestroyOnLoad(singleton);
                        
					} 
					else 
					{

					}
				}
				return _instance;
			}
		}
	}
	
	private static bool applicationIsQuitting = false;
	/// <summary>
	/// When unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.    
	/// If any script calls Instance after it have been destroyed,     
	///   it will create a buggy ghost object that will stay on the Editor scene    
	///   even after stopping playing the Application. Really bad!    
	/// So, this was made to be sure we're not creating that buggy ghost object.    
	/// </summary>
	public void OnDestroy () 
	{
		if( ClearMode == false )
			applicationIsQuitting = true;
		ClearMode = false;
	}
	
	private static bool ClearMode = false;
	public static void DestorySingleton()
	{
		lock(_lock)
		{
			if( _instance != null )
			{
				ClearMode = true;
				GameObject.Destroy( _instance.gameObject );
				_instance = null;
			}
		}
	}
}
