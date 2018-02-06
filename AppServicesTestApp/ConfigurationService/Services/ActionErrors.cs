using System;

namespace ConfigurationService.Services
{
    public sealed class ActionErrors<TData> where TData: class 
    {
        public ActionErrors(string message, TData data = null)
        {
            Message = message;
            Data = data;
        }

        public string Message { get; }

        public TData Data { get; }
    }
}