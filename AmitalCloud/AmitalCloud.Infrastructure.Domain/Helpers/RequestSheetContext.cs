using System;

namespace AmitalCloud.Infrastructure.Domain.Helpers
{

    public class RequestSheetContext //like ServiceLocator
    {
        RequestSheetContextModel _RequestSheetContextModel = null;
        private RequestSheetContext()
        {

        }
        [ThreadStatic]
        private static RequestSheetContext _Current;
        public static RequestSheetContext Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new RequestSheetContext();
                }
                return _Current;
            }

        }

        public void Dispose()
        {
            _RequestSheetContextModel = null;
        }

        public RequestSheetContextModel GetContextOrDefault()
        {
            return _RequestSheetContextModel ?? new RequestSheetContextModel();
        }
        public void SetRSContext(RequestParamsBase requestParamsBase)
        {

            _RequestSheetContextModel = null;
            if (requestParamsBase == null)
            {
                return;
            }
            _RequestSheetContextModel = new RequestSheetContextModel()
            {
                Tenant = requestParamsBase.Tenant,
                CustomsRequestsSheetId = requestParamsBase.CustomsRequestsSheetId,
                MainInterfaceCode = requestParamsBase.MainInterfaceCode,
                RequestParams = requestParamsBase,
            };
        }


    }
    public class RequestSheetContextModel //: RequestParamsBase
    {

        public int Tenant { get; set; }
        public string InterfaceTypeCode { get; set; }
        public string MainInterfaceCode { get; set; }
        public string CustomsRequestsSheetId { get; set; }

        public string SignByX509SubjectName { get; set; }

        public string GetUserFromRequestParam()
        {
            if (this.RequestParams != null)
            {
                return this.RequestParams.LoggingUserId;
            }
            return null;
        }

        public RequestParamsBase RequestParams { get; set; }// new feature 2017 06 04 
    }
    public interface SetRequestParamsBase
    {
        void SetRequestParamsBase(RequestParamsBase requestParamsBase);
    }
    public interface GetRequestParamsBase
    {
        RequestParamsBase GetRequestParamsBase();
    }

}
