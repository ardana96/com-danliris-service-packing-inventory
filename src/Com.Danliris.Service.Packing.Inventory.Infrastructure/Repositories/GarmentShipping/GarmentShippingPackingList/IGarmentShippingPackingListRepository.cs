﻿using Com.Danliris.Service.Packing.Inventory.Data;
using Com.Danliris.Service.Packing.Inventory.Data.Models.Garmentshipping.GarmentShippingPackingList;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Packing.Inventory.Infrastructure.Repositories.GarmentShipping.GarmentShippingPackingList
{
    public interface IGarmentShippingPackingListRepository : IRepository<GarmentShippingPackingListModel>
    {
        Task<GarmentShippingPackingListModel> ReadByInvoiceNoAsync(string no);
        IQueryable<GarmentShippingPackingListModel> Query { get; }
        Task<int> SaveChanges();
    }
}