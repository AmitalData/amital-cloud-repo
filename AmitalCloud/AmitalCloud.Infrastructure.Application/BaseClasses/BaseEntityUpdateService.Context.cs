using System;
using System.Diagnostics;
using System.Text;

namespace AmitalCloud.Infrastructure.Application.BaseClasses
{
    public abstract partial class BaseEntityUpdateService<TEntity, TEntityPM, TEntityParentPM, TEntityList, TkeyType>
    {
        protected virtual void AddContext(TEntityPM myTEntityPM)
        {
            if (EntityUpdateServiceContext.Current == null)
            {
                EntityUpdateServiceContext.Current = new EntityUpdateServiceDebugContext() { EntityUpdateService = this as object };
            }
        }
        protected void AddExternalTrace(string Trace)
        {
            if (IsMainUpdateService())
            {
                EntityUpdateServiceContext.Current.AddExternalTrace(Trace);
            }
        }
        private void AddStepTrace(string stepName)
        {
            if (IsMainUpdateService())
            {
                EntityUpdateServiceContext.Current.AddStepTrace(stepName);
            }
        }
        public string GetDebugTrace()
        {
            if (IsMainUpdateService())
            {
                return EntityUpdateServiceContext.Current.GetDebugTrace();
            }
            return null;
        }
        private bool IsMainUpdateService()
        {
            return EntityUpdateServiceContext.Current != null && EntityUpdateServiceContext.Current.EntityUpdateService == this;
        }
        private void RemoveContext(TEntityPM myTEntityPM)
        {
            if (EntityUpdateServiceContext.Current != null && EntityUpdateServiceContext.Current.EntityUpdateService == this)
            {
                EntityUpdateServiceContext.Current.EntityUpdateService = null;
                EntityUpdateServiceContext.Current = null;
            }
        }
        public void GetAncestorEntityUpdateService
            (out object AncestorEntityUpdateService)
        {
            AncestorEntityUpdateService = null;
            try
            {
                AncestorEntityUpdateService = EntityUpdateServiceContext.Current.EntityUpdateService;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get ancestor: {ex.Message}");
            }

        }
        public void GetAncestor
            (out object entityPOCO, out object entityPM, out object entityParentPM)

        {
            entityPOCO = null; entityPM = null; entityParentPM = null;
            try
            {
                var entityUpdateService = EntityUpdateServiceContext.Current.EntityUpdateService as dynamic;
                entityPOCO = entityUpdateService.EntityPOCO;
                entityPM = entityUpdateService.EntityPM;
            }
            catch (Exception)
            {
                //throw;
            }
        }
    }
    class EntityUpdateServiceContext
    {
        [ThreadStatic]
        public static EntityUpdateServiceDebugContext Current = null;
    }
    public class EntityUpdateServiceDebugContext
    {
        private const string ExternalTraceTag = "ExternalTrace";
        public object EntityUpdateService = null;
        private StringBuilder _sb;
        private Stopwatch _sw;
        public EntityUpdateServiceDebugContext()
        {
            _sb = new StringBuilder();
            _sw = Stopwatch.StartNew();
        }
        internal void AddStepTrace(string stepName)
        {
            lock (_sb)
            {
                _sb.AppendLine($"{stepName}:Took:{_sw.ElapsedMilliseconds}");
                _sw.Restart();
            }
        }
        internal void AddExternalTrace(string Trace)
        {
            _sb.AppendLine($"{ExternalTraceTag}:{Trace}");
        }
        internal string GetDebugTrace()
        {
            return _sb.ToString();
        }
    }
}
