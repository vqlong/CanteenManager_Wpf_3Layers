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
    public class TableDAO : ITableDAO
    {
        private TableDAO() { }

        /// <summary>
        /// Gộp bàn, bằng cách chuyển các món ăn trên bàn này vào bàn kia.
        /// </summary>
        /// <param name="firstTableId"></param>
        /// <param name="secondTableId"></param>
        /// <returns></returns>
        public bool CombineTable(int firstTableId, int secondTableId)
        {
            using var context = new CanteenContext();

            var firstBill = context.Bills.Where(b => b.Status == 0 && b.TableId == firstTableId).FirstOrDefault();
            var secondBill = context.Bills.Where(b => b.Status == 0 && b.TableId == secondTableId).FirstOrDefault();

            //Nếu có bàn không có người thì thôi
            if (firstBill == null || secondBill == null) return false;

            //Bắt đầu gộp
            var result = context.Database.ExecuteSqlInterpolated($"UPDATE BillInfo SET BillId = {secondBill.Id} WHERE BillId = {firstBill.Id};");
           
            if (result <= 0) return false;

            ////Xoá cái Bill thứ 1 (lúc này tất cả thức ăn đã chuyển sang Bill thứ 2)
            ////SqlServer đã có trigger tự xoá
            //context.Database.ExecuteSqlInterpolated($"DELETE FROM Bill WHERE Id = {firstBill.Id};");
            ////context.Bills.Remove(firstBill);

            //Khi gộp bàn sẽ xuất hiện các món trùng lặp với nhau
            //Lấy ra các FoodID với số lần trùng lặp
            var data = context.BillInfos.Where(bi => bi.BillId == secondBill.Id)
                                        .GroupBy(bi => bi.FoodId)
                                        .Select(g => new 
                                        { 
                                            FoodId = g.Key, 
                                            MaxId = g.Max(bi => bi.Id), 
                                            Count = g.Count(), 
                                            TotalFoodCount = g.Sum(bi => bi.FoodCount) 
                                        })
                                        .ToList();
            foreach (var item in data)
            {
                //Trường hợp count > 1 tức là món này bị trùng nhau, xuất hiện hơn 1 lần
                if (item.Count > 1)
                {
                    //Lấy ra max ID của món này để tí nữa giữ lại và update, các ID khác xoá hết cho khỏi trùng nhau
                    context.Database.ExecuteSqlInterpolated($"UPDATE BillInfo SET FoodCount = {item.TotalFoodCount} WHERE BillId = {secondBill.Id} AND FoodId = {item.FoodId} AND Id = {item.MaxId};");

                    context.Database.ExecuteSqlInterpolated($"DELETE FROM BillInfo WHERE BillId = {secondBill.Id} AND FoodId = {item.FoodId} AND Id != {item.MaxId};");

                }
            }

            return true;
        }

        public bool DeleteTable(int id)
        {
            using var context = new CanteenContext();

            context.Tables.Attach(new Table { Id = id, UsingState = UsingState.StopServing }).Property(t => t.UsingState).IsModified = true;
            if (context.SaveChanges() == 1) return true;
            return false;
        }

        public List<Table> GetListTable()
        {
            using var context = new CanteenContext();

            return context.Tables.ToList();
        }

        public List<Table> GetListTableUsing()
        {
            using var context = new CanteenContext();

            return context.Tables.Where(t => t.UsingState == UsingState.Serving).ToList();
        }

        public Table GetTableById(int tableId)
        {
            using var context = new CanteenContext();

            return context.Tables.FirstOrDefault(t => t.Id == tableId);
        }

        public Table InsertTable()
        {
            using var context = new CanteenContext();

            var count = context.Tables.Count();
            var table = new Table { Name = "Bàn " + ++count };

            context.Tables.Add(table);

            if (context.SaveChanges() == 1) return table;
            return null;
        }

        /// <summary>
        /// Chuyển bàn, bằng cách tráo đổi TableId của 2 Bill trên mỗi bàn.
        /// </summary>
        /// <param name="firstTableId"></param>
        /// <param name="secondTableId"></param>
        /// <returns></returns>
        public bool SwapTable(int firstTableId, int secondTableId)
        {
            using var context = new CanteenContext();

            var firstBill = context.Bills.Where(b => b.Status == 0 && b.TableId == firstTableId).FirstOrDefault();
            var secondBill = context.Bills.Where(b => b.Status == 0 && b.TableId == secondTableId).FirstOrDefault();

            //Nếu bàn thứ nhất được chọn có người thì mới tiến hành chuyển
            if (firstBill != null)
            {
                firstBill.TableId = secondTableId;

                if (secondBill != null) secondBill.TableId = firstTableId;
            }

            if (context.SaveChanges() >= 1) return true;
            return false;
        }

        public bool UpdateTable(int id, string name, UsingState usingState)
        {
            using var context = new CanteenContext();
            context.Tables.Update(new Table { Id = id, Name = name, UsingState = usingState });
            if (context.SaveChanges() == 1) return true;
            return false;
        }
    }
}
