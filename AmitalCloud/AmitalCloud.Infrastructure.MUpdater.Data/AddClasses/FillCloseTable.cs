using AmitalCloud.Infrastructure.Domain.Interfaces;

namespace AmitalCloud.Infrastructure.MUpdater.Data.AddClasses
{
    public class FillCloseTables
    {
        public void FillCloseTable<TPOCO, TDetails, TRepository>(TRepository repo, Dictionary<string, TPOCO> dic)
            where TDetails : ICloseTable<TPOCO, TDetails>, new()
            where TRepository : IRepository<TPOCO> 
            where TPOCO : 
           class,new()
        {

            if (repo == null)
            {
                throw new Exception("repo == null");
            }
            var listHC = (new TDetails()).GetAll();

            foreach (var itemDetails in listHC)
            {

                var strCode = itemDetails.Code;
                if (dic.Keys.Contains(strCode))
                {
                    var updatePoco = dic[strCode]; 
                    itemDetails.MapPoco(updatePoco);

                    repo.Update(updatePoco);
                }
                else
                {
                    var newPoco = new TPOCO();
                    itemDetails.MapPoco(newPoco);
                    //dic.Add(strCode, newPoco);
                    repo.Insert(newPoco);
                }
            }
            repo.SubmitChanges();
        }
    }
}
