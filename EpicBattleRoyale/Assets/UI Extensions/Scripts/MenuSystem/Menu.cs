﻿namespace UnityEngine.UI.Extensions
{
    public abstract class Menu<T> : Menu where T : Menu<T>
    {
        public static T Instance { get; private set; }

        protected void Awake()
        {
            Instance = (T)this;
            OnInit();
        }

        protected void OnDestroy()
        {
            OnCleanUp();
            Instance = null;
        }

        protected static void Open()
        {
            if (Instance == null)
                Instance = MenuManager.Instance.CreateInstance(typeof(T).Name) as T;
            //MenuManager.Instance.CreateInstance<T>();
            else
                Instance.gameObject.SetActive(true);

            MenuManager.Instance.OpenMenu(Instance);
        }

        protected static void Close()
        {
            if (Instance == null)
            {
                Debug.LogErrorFormat("Trying to close menu {0} but Instance is null", typeof(T));
                return;
            }

            MenuManager.Instance.CloseMenu(Instance);

        }
        
        public override void OnBackPressed()
        {
            Close();
        }
    }

    public abstract class Menu : MonoBehaviour
    {
        [Tooltip("Destroy the Game Object when menu is closed (reduces memory usage)")]
        public bool DestroyWhenClosed = true;

        [Tooltip("Disable menus that are under this one in the stack")]
        public bool DisableMenusUnderneath = true;

        public abstract void OnBackPressed();

        public virtual void OnShow()
        {

        }

        public virtual void OnClose()
        {

        }

        public virtual void OnCleanUp()
        {

        }

        public virtual void OnInit()
        {

        }
    }
}