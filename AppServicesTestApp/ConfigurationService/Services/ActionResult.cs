using System.Collections.Generic;
using System.Linq;

namespace ConfigurationService.Services
{
    public class ActionResult<TResult, TData> where TData: class
    {
        public ActionResult(TResult resut, IEnumerable<ActionErrors<TData>> errors = null)
        {
            Result = resut;
            Errors = errors ?? Enumerable.Empty<ActionErrors<TData>>();
        }

        public TResult Result { get; }

        public IEnumerable<ActionErrors<TData>> Errors { get; }

        public bool IsValid()
        {
            return CheckValid();
        }

        protected virtual bool CheckValid()
        {
            return !Errors.Any();
        }
    }
}
