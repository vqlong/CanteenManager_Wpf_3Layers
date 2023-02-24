using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDataAccess
{
    public class BillDetailDAO : IBillDetailDAO
    {
        private BillDetailDAO() { }

        public List<BillDetail> GetListBillDetailByBillId(int billId)
        {
            using var context = new CanteenContext();

            var data = context.BillInfos.Where(bi => bi.BillId == billId)
                                        .Select(bi => new { FoodName = bi.Food.Name, CategoryName = bi.Food.Category.Name, FoodCount = bi.FoodCount, Price = bi.Food.Price, TotalPrice = bi.FoodCount * bi.Food.Price })
                                        .ToList();

            List<BillDetail> listBillDetail = new List<BillDetail>(data.Count);

            foreach (var item in data)
            {
                BillDetail billDetail = new BillDetail(item.FoodName, item.CategoryName, item.FoodCount, item.Price, item.TotalPrice);

                listBillDetail.Add(billDetail);
            }

            return listBillDetail;
        }

        public List<BillDetail> GetListBillDetailByTableId(int tableId)
        {
            using var context = new CanteenContext();

            var data = context.BillInfos.Where(bi => bi.Bill.TableId == tableId && bi.Bill.Status == 0)
                                        .Select(bi => new { FoodName = bi.Food.Name, CategoryName = bi.Food.Category.Name, FoodCount = bi.FoodCount, Price = bi.Food.Price, TotalPrice = bi.FoodCount * bi.Food.Price })
                                        .ToList();

            List<BillDetail> listBillDetail = new List<BillDetail>(data.Count);

            foreach (var item in data)
            {
                BillDetail billDetail = new BillDetail(item.FoodName, item.CategoryName, item.FoodCount, item.Price, item.TotalPrice);

                listBillDetail.Add(billDetail);
            }

            return listBillDetail;
        }
    }
}
