using System;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Data.Helpers
{

    public class AmitalCloudDomainScope : IDisposable
    {

        [ThreadStatic]
        private static Dictionary<string, IAmitalCloudContextEntity> _Context = new Dictionary<string, IAmitalCloudContextEntity>();

        private readonly bool _IsParentContext;
        private readonly string _myEnum;

        public AmitalCloudDomainScope(IAmitalCloudContextEntity myContextEntity = null)
        {
            if (myContextEntity == null)
            {
                throw new Exception("myContextEntity is empty");
            }
            var myType = myContextEntity.GetType().Name;
            _myEnum = myType;
            if (Context.ContainsKey(myType))
            {
                throw new InvalidOperationException("Only a single Domain Context can be created!");
            }


            //myLogtitudeDomainScope.MyEnum = myType;
            _IsParentContext = true;
            Context.Add(myType, myContextEntity);
        }
        public static TAmitalCloudContextEntity GetCurrent<TAmitalCloudContextEntity>()
            where TAmitalCloudContextEntity : class, IAmitalCloudContextEntity
        {
            var myType = typeof(TAmitalCloudContextEntity).Name;
            return GetCurrent(myType) as TAmitalCloudContextEntity;

        }
        private static IAmitalCloudContextEntity GetCurrent(string my_type)
        {
            IAmitalCloudContextEntity myIContextEntity = null;
            try
            {
                if (Context == null) return null;
                if (!Context.ContainsKey(my_type)) return null;
                myIContextEntity = Context[my_type];
            }
            catch
            {


            }
            return myIContextEntity;

        }
        public IAmitalCloudContextEntity Current
        {
            get { return AmitalCloudDomainScope.GetCurrent(_myEnum); }
        }

        public static Dictionary<string, IAmitalCloudContextEntity> Context
        {
            get
            {
                return _Context = _Context ?? new Dictionary<string, IAmitalCloudContextEntity>();
            }

            set
            {
                _Context = value;
            }
        }

        public void Dispose()
        {
            if (!_IsParentContext) return;
            Context.Remove(_myEnum);
        }
    }

    public interface IAmitalCloudContextEntity
    {

    }
    public class ExceptionInErrorLog : Exception, IAmitalCloudContextEntity
    {
        public string ErrorlogId { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {

            return String.Format(
@"An unhandled exception has been caught (our ref :{0})  
{1}", ErrorlogId, Message);
        }
    }
}
