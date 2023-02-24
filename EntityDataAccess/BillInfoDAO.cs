using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDataAccess
{
    public class BillInfoDAO : IBillInfoDAO
    {
        private BillInfoDAO() { }

        public int InsertBillInfo(int billId, int foodId, int foodCount)
        {
            using var context = new CanteenContext();

            //Kiểm tra xem cái Bill này đã thanh toán chưa, chưa thanh toán mới được thêm BillInfo vào
            var billStatus = context.Bills.FirstOrDefault(b => b.Id == billId).Status;
            if (billStatus == 1) return -1;

            //Kiểm tra xem cái món này đã có chưa
            var countBillInfo = context.BillInfos.Count(bi => bi.BillId == billId && bi.FoodId == foodId);

            //Nếu chưa có thì thêm mới
            if (countBillInfo == 0)
            {
                
                if (foodCount > 0)
                {
                    var newBillInfo = new BillInfo { BillId = billId, FoodId = foodId, FoodCount = foodCount };
                    context.BillInfos.Add(newBillInfo);
                    context.SaveChanges();
                    return newBillInfo.Id;
                }
                else
                {
                    return -1;
                }
            }

            //Nếu có rồi thì update số lượng món đã gọi
            var billInfo = context.BillInfos.FirstOrDefault(bi => bi.BillId == billId && bi.FoodId == foodId);
            var currentFoodCount = billInfo.FoodCount;
            var newFoodCount = currentFoodCount + foodCount;

            //Theo thiết kế @foodCount truyền vào có thể âm, nếu @newFoodCount <= 0 thì xoá món đó khỏi hoá đơn
            if (newFoodCount <= 0)
            {
                context.BillInfos.Remove(billInfo);
                context.SaveChanges();

                ////Sau mỗi lần xoá BillInfo, đếm xem cái Bill này còn BillInfo nào không, nếu không còn cái nào thì xoá luôn Bill
                ////SqlServer đã có trigger tự xoá
                //var count = context.BillInfos.Count(bi => bi.BillId == billId);
                //if (count == 0) context.Bills.Remove(bill);
                //context.SaveChanges();

                return -1;
            }

            billInfo.FoodCount = newFoodCount;
            context.SaveChanges();
            return billInfo.Id;
        }
    }
}
