﻿using System;
using System.Windows.Input;

namespace WinGoMapsX
{
    public class AppCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public static AppCommand GetInstance()
        {
            return new AppCommand() { CanExecuteFunc = obj => true };
        }
        public Predicate<object> CanExecuteFunc
        {
            get;
            set;
        }

        public Action<object> ExecuteFunc
        {
            get;
            set;
        }

        public bool CanExecute(object parameter) => CanExecuteFunc(parameter);

        public void Execute(object parameter) => ExecuteFunc(parameter);
        
    }
}

