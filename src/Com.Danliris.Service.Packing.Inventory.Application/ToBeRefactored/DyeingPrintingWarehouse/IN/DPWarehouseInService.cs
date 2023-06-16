﻿using Com.Danliris.Service.Packing.Inventory.Infrastructure.Repositories.DyeingPrintingAreaMovement;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.DyeingPrintingAreaInput.Warehouse.PreOutputWarehouse;
using System.Linq;
using Com.Danliris.Service.Packing.Inventory.Infrastructure.Utilities;
using Com.Danliris.Service.Packing.Inventory.Application.CommonViewModelObjectProperties;
using Com.Danliris.Service.Packing.Inventory.Infrastructure.Repositories.DyeingPrintingWarehouse;
using Com.Danliris.Service.Packing.Inventory.Application.Utilities;
using Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.DyeingPrintingAreaInput.Warehouse.List;

using Com.Danliris.Service.Packing.Inventory.Data.Models.DyeingPrintingWarehouse;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.DyeingPrintingAreaInput.Warehouse.Create;
using Com.Danliris.Service.Packing.Inventory.Infrastructure;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using Com.Danliris.Service.Packing.Inventory.Infrastructure.IdentityProvider;
using Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.DyeingPrintingWarehouse.IN.ViewModel;

namespace Com.Danliris.Service.Packing.Inventory.Application.ToBeRefactored.DyeingPrintingWarehouse.IN
{
    public class DPWarehouseInService : IDPWarehouseInService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDyeingPrintingAreaOutputProductionOrderRepository _outputProductionOrderRepository;
        private readonly IDPWarehousePreInputRepository _dPWarehousePreInputRepository;
        private readonly IDPWarehouseInputRepository _dPWarehouseInputRepository;
        private readonly IDPWarehouseSummaryRepository _dPWarehouseSummaryRepository;
        private readonly PackingInventoryDbContext _dbContext;
        private readonly DbSet<DPWarehouseInputModel> _dbSet;
        private readonly DbSet<DPWarehouseInputItemModel> _dbSetItems;
        private readonly DbSet<DPWarehouseSummaryModel> _dbSetSummary;
        private readonly DbSet<DPWarehouseMovementModel> _dbSetMovement;
        private readonly IIdentityProvider _identityProvider;
        private const string UserAgent = "Repository";

        public DPWarehouseInService(PackingInventoryDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbSet = dbContext.Set<DPWarehouseInputModel>();
            _dbSetSummary = dbContext.Set<DPWarehouseSummaryModel>();
            _dbSetMovement = dbContext.Set<DPWarehouseMovementModel>();
            _dbSetItems = dbContext.Set<DPWarehouseInputItemModel>();
            _outputProductionOrderRepository = serviceProvider.GetService<IDyeingPrintingAreaOutputProductionOrderRepository>();
            _dPWarehousePreInputRepository = serviceProvider.GetService<IDPWarehousePreInputRepository>();
            _dPWarehouseInputRepository = serviceProvider.GetService<IDPWarehouseInputRepository>();
            _dPWarehouseSummaryRepository = serviceProvider.GetService<IDPWarehouseSummaryRepository>();
            _identityProvider = serviceProvider.GetService<IIdentityProvider>();
            _dbContext = dbContext;
        }

        public List<OutputPreWarehouseItemListViewModel> PreInputWarehouse(string packingCode)
        {
            //List<OutputPreWarehouseItemListViewModel> queryResult;

            var queryResult = new List<OutputPreWarehouseItemListViewModel>();
            var query = _dPWarehousePreInputRepository.ReadAll().OrderByDescending(s => s.LastModifiedUtc).
                                                                   Where(s => s.ProductPackingCode.Contains(packingCode)
                                                                  );

            if (query != null)
            {
                queryResult = query.Select(p => new OutputPreWarehouseItemListViewModel()
                {
                    Id = p.Id,
                    ProductionOrder = new ProductionOrder()
                    {
                        Id = p.ProductionOrderId,
                        No = p.ProductionOrderNo,
                        Type = p.ProductionOrderType,
                        OrderQuantity = p.ProductionOrderOrderQuantity,
                        CreatedUtc = p.CreatedUtcOrderNo
                    },
                    MaterialWidth = p.MaterialWidth,
                    MaterialOrigin = p.MaterialOrigin,
                    FinishWidth = p.FinishWidth,
                    MaterialConstruction = new MaterialConstruction()
                    {
                        Id = p.MaterialConstructionId,
                        Name = p.MaterialConstructionName
                    },
                    MaterialProduct = new Material()
                    {
                        Id = p.MaterialId,
                        Name = p.MaterialName
                    },
                    ProcessType = new CommonViewModelObjectProperties.ProcessType()
                    {
                        Id = p.ProcessTypeId,
                        Name = p.ProcessTypeName
                    },
                    YarnMaterial = new CommonViewModelObjectProperties.YarnMaterial()
                    {
                        Id = p.YarnMaterialId,
                        Name = p.YarnMaterialName
                    },
                    CartNo = p.CartNo,
                    Buyer = p.Buyer,
                    BuyerId = p.BuyerId,
                    Construction = p.Construction,
                    Unit = p.Unit,
                    Color = p.Color,
                    Motif = p.Motif,
                    UomUnit = p.UomUnit,
                    Remark = p.Remark,
                    Grade = p.Grade,
                    Balance = p.BalanceRemains,
                    InputQuantity = p.Balance,
                    PackingInstruction = p.PackingInstruction,
                    PackagingType = p.PackagingType,
                    PackagingQty = p.PackagingQtyRemains,
                    PackagingLength = p.PackagingLength,
                    InputPackagingQty = p.PackagingQty,
                    PackagingUnit = p.PackagingUnit,
                    Description = p.Description,


                    Qty = p.PackagingLength,
                    ProductSKUId = p.ProductSKUId,
                    FabricSKUId = p.FabricSKUId,
                    ProductSKUCode = p.ProductSKUCode,

                    ProductPackingId = p.ProductPackingId,
                    FabricPackingId = p.FabricPackingId,
                    ProductPackingCode = p.ProductPackingCode,

                    PreviousOutputPackagingQty = p.PackagingQty,

                }).ToList();


            }
            //else { 

            //}
            return queryResult;
        }


        public ListResult<IndexViewModel> Read(int page, int size, string filter, string order, string keyword)
        {
            //var query = _inputRepository.ReadAll().Where(s => s.Area == GUDANGJADI &&
            //                                             s.DyeingPrintingAreaInputProductionOrders.Any(d => !d.HasOutputDocument && d.Balance > 0));
            var query = _dPWarehouseInputRepository.ReadAll().Where(s => s.Area == DyeingPrintingArea.GUDANGJADI);

            List<string> SearchAttributes = new List<string>()
            {
                "BonNo"
            };

            query = QueryHelper<DPWarehouseInputModel>.Search(query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<DPWarehouseInputModel>.Filter(query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<DPWarehouseInputModel>.Order(query, OrderDictionary);
            var data = query.Skip((page - 1) * size).Take(size).Select(s => new IndexViewModel()
            {
                Area = s.Area,
                BonNo = s.BonNo,
                Date = s.Date,
                Id = s.Id,
                Shift = s.Shift,
                Group = s.Group,
            });

            return new ListResult<IndexViewModel>(data.ToList(), page, size, query.Count());
        }


        public async Task<int> Create(DPInputWarehouseCreateViewModel viewModel)
        {
            int result = 0;

            var model = _dPWarehouseInputRepository.GetDbSet()
                //.Include(s => s.DyeingPrintingAreaInputProductionOrders)
                .FirstOrDefault(s => s.Area == DyeingPrintingArea.GUDANGJADI &&
                s.Date.AddHours(7).ToString("dd/MM/yyyy") == DateTime.Now.Date.AddHours(7).ToString("dd/MM/yyyy") &&
                s.Shift == "DAILY SHIFT" &&
                s.Group == "");

            //var dateData = viewModel.Date;

            var ids = _dPWarehouseInputRepository.GetDbSet().Where(s => s.Area == DyeingPrintingArea.GUDANGJADI).Select(x => x.Id).ToList();

            if (model != null)
            {
                result = await UpdateExistingWarehouse(viewModel, model.Id, model.BonNo);
            }
            else
            {
                result = await InsertNewWarehouse(viewModel);
            }


            return result;


        }

        public async Task<int> InsertNewWarehouse(DPInputWarehouseCreateViewModel viewModel)
        {
            int Created = 0;



            using (var transaction = this._dbContext.Database.BeginTransaction())
            {
                try
                {
                    int totalCurrentYearData = _dPWarehouseInputRepository.ReadAllIgnoreQueryFilter().Count(s => s.Area == DyeingPrintingArea.GUDANGJADI &&
                                                                                             s.CreatedUtc.Year == DateTime.Now.Date.Year);

                    string bonNo = GenerateBonNo(totalCurrentYearData + 1, DateTime.Now.Date);



                    var model = new DPWarehouseInputModel(
                                DateTime.Now.Date,
                                "GUDANG JADI",
                                "DAILY SHIFT",
                                bonNo,
                                "",
                                viewModel.DyeingPrintingWarehouseInItems.Select(s => new DPWarehouseInputItemModel(
                                   s.ProductionOrder.Id,
                                   s.ProductionOrder.No,
                                   s.MaterialProduct.Id,
                                   s.MaterialProduct.Name,
                                   s.MaterialConstruction.Id,
                                   s.MaterialConstruction.Name,
                                   s.MaterialWidth,
                                   s.BuyerId,
                                   s.Buyer,
                                   s.Construction,
                                   s.Unit,
                                   s.Color,
                                   s.Motif,
                                   s.UomUnit,
                                   s.Remark,
                                   s.Grade,
                                   s.Sendquantity * s.PackagingLength,
                                   s.PackingInstruction,
                                   s.ProductionOrder.Type,
                                   s.ProductionOrder.OrderQuantity,
                                   s.PackagingType,
                                   (decimal)s.Sendquantity,
                                   s.PackagingLength,
                                   s.PackagingUnit,
                                   s.Area,
                                   s.Remark,
                                   s.Id,
                                   s.ProductSKUId,
                                   s.FabricSKUId,
                                   s.ProductSKUCode,
                                   s.ProductPackingId,
                                   s.FabricPackingId,
                                   s.ProductPackingCode,
                                   s.ProcessType.Id,
                                   s.ProcessType.Name,
                                   s.YarnMaterial.Id,
                                   s.YarnMaterial.Name,
                                   s.FinishWidth,
                                   s.MaterialOrigin,
                                   s.ProductionOrder.CreatedUtc

                                   )).ToList()
                        );

                    //model.Area.Trim();
                    model.FlagForCreate(_identityProvider.Username, UserAgent);

                    _dbSet.Add(model);

                    var listSummary = new List<DPWarehouseSummaryModel>();

                    var IdSummary = new List<int>();

                    foreach (var item in model.DPWarehouseInputItems)
                    {
                        item.FlagForCreate(_identityProvider.Username, UserAgent);

                        #region save or update summary table
                        var modelSummary = _dPWarehouseSummaryRepository.GetDbSet().FirstOrDefault(s => s.ProductPackingCode.Contains(item.ProductPackingCode) && s.TrackId == 0);
                        if (modelSummary == null)
                        {

                            modelSummary = new DPWarehouseSummaryModel(
                                        item.Balance,
                                        item.Balance,
                                        0,
                                        item.BuyerId,
                                        item.Buyer,
                                        "",
                                        item.Color,
                                        item.Grade,
                                        item.Construction,
                                        item.MaterialConstructionId,
                                        item.MaterialConstructionName,
                                        item.MaterialId,
                                        item.MaterialName,
                                        item.MaterialWidth,
                                        item.Motif,
                                        item.PackingInstruction,
                                        item.PackagingQty,
                                        item.PackagingQty,
                                        0,
                                        item.PackagingLength,
                                        item.PackagingType,
                                        item.PackagingUnit,
                                        item.ProductionOrderId,
                                        item.ProductionOrderNo,
                                        item.ProductionOrderType,
                                        item.ProductionOrderOrderQuantity,
                                        item.CreatedUtcOrderNo,
                                        item.ProcessTypeId,
                                        item.ProcessTypeName,
                                        item.YarnMaterialId,
                                        item.YarnMaterialName,
                                        item.Unit,
                                        item.UomUnit,
                                        0,
                                        item.Description,
                                        item.ProductSKUId,
                                        item.FabricSKUId,
                                        item.ProductSKUCode,
                                        item.ProductPackingId,
                                        item.FabricPackingId,
                                        item.ProductPackingCode

                                );
                            modelSummary.FlagForCreate(_identityProvider.Username, UserAgent);

                            listSummary.Add(modelSummary);

                            _dbSetSummary.Add(modelSummary);
                        }
                        else
                        {
                            double balanceUpdate = modelSummary.Balance + item.Balance;
                            double balanceRemainsUpdate = modelSummary.BalanceRemains + item.Balance;
                            decimal packagingQtyUpdate = modelSummary.PackagingQty + item.PackagingQty;
                            decimal packagingQtyRemainsUpdate = modelSummary.PackagingQtyRemains + item.PackagingQty;
                            //modelSummary.FlagForUpdate(_identityProvider.Username, UserAgent);
                            //modelSummary.SetBalanceRemains(balanceUpdate, _identityProvider.Username, UserAgent);
                            //var modelSummaries = _dbSetSummary.FirstOrDefault(entity => entity.Id == modelSummary.Id);
                            //modelSummaries.BalanceRemains = balanceUpdate;
                            modelSummary.Balance = balanceUpdate;
                            modelSummary.BalanceRemains = balanceRemainsUpdate;
                            modelSummary.PackagingQty = packagingQtyUpdate;
                            modelSummary.PackagingQtyRemains = packagingQtyRemainsUpdate;
                            //EntityExtension.FlagForUpdate(modelSummaries, _identityProvider.Username, UserAgent);
                            EntityExtension.FlagForUpdate(modelSummary, _identityProvider.Username, UserAgent);
                            listSummary.Add(modelSummary);


                        }
                        #endregion

                        #region update dpWarehousePreInput
                        var modelPreInput = _dPWarehousePreInputRepository.GetDbSet().FirstOrDefault(s => s.ProductPackingCode.Contains(item.ProductPackingCode));



                        modelPreInput.BalanceReceipt = modelPreInput.BalanceReceipt + item.Balance;
                        modelPreInput.BalanceRemains = modelPreInput.BalanceRemains - item.Balance;
                        modelPreInput.PackagingQtyReceipt = modelPreInput.PackagingQtyReceipt + item.PackagingQty;
                        modelPreInput.PackagingQtyRemains = modelPreInput.PackagingQtyRemains - item.PackagingQty;

                        EntityExtension.FlagForUpdate(modelPreInput, _identityProvider.Username, UserAgent);

                        #endregion

                    }

                    Created = await _dbContext.SaveChangesAsync();

                    var modelItem = new DPWarehouseInputItemModel();
                    await createMovement(model, listSummary);


                    transaction.Commit();

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }
        public async Task<int> UpdateExistingWarehouse(DPInputWarehouseCreateViewModel viewModel, int modelId, string bonNo)
        {
            int Created = 0;

            var listSummary = new List<DPWarehouseSummaryModel>();
            var listItem = new List<DPWarehouseInputItemModel>();

            using (var transaction = this._dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in viewModel.DyeingPrintingWarehouseInItems)
                    {
                        var modelItem = new DPWarehouseInputItemModel(
                                               item.ProductionOrder.Id,
                                               item.ProductionOrder.No,
                                               item.MaterialProduct.Id,
                                               item.MaterialProduct.Name,
                                               item.MaterialConstruction.Id,
                                               item.MaterialConstruction.Name,
                                               item.MaterialWidth,
                                               item.BuyerId,
                                               item.Buyer,
                                               item.Construction,
                                               item.Unit,
                                               item.Color,
                                               item.Motif,
                                               item.UomUnit,
                                               item.Remark,
                                               item.Grade,
                                               item.Sendquantity * item.PackagingLength,
                                               item.PackingInstruction,
                                               item.ProductionOrder.Type,
                                               item.ProductionOrder.OrderQuantity,
                                               item.PackagingType,
                                               (decimal)item.Sendquantity,
                                               item.PackagingLength,
                                               item.PackagingUnit,
                                               item.Area,
                                               item.Remark,
                                               modelId,
                                               item.ProductSKUId,
                                               item.FabricSKUId,
                                               item.ProductSKUCode,
                                               item.ProductPackingId,
                                               item.FabricPackingId,
                                               item.ProductPackingCode,
                                               item.ProcessType.Id,
                                               item.ProcessType.Name,
                                               item.YarnMaterial.Id,
                                               item.YarnMaterial.Name,
                                               item.FinishWidth,
                                               item.MaterialOrigin,
                                               item.ProductionOrder.CreatedUtc

                                               );

                        modelItem.FlagForCreate(_identityProvider.Username, UserAgent);
                        listItem.Add(modelItem);
                        _dbSetItems.Add(modelItem);

                        #region save or update summary table
                        
                        var modelSummary = _dPWarehouseSummaryRepository.GetDbSet().FirstOrDefault(s => s.ProductPackingCode.Contains(item.ProductPackingCode) && s.TrackId == 0);
                        if (modelSummary == null)
                        {

                            modelSummary = new DPWarehouseSummaryModel(
                                        item.Sendquantity * item.PackagingLength,
                                        item.Sendquantity * item.PackagingLength,
                                        0,
                                        item.BuyerId,
                                        item.Buyer,
                                        "",
                                        item.Color,
                                        item.Grade,
                                        item.Construction,
                                        item.MaterialConstruction.Id,
                                        item.MaterialConstruction.Name,
                                        item.MaterialProduct.Id,
                                        item.MaterialProduct.Name,
                                        item.MaterialWidth,
                                        item.Motif,
                                        item.PackingInstruction,
                                        (decimal)item.Sendquantity,
                                        (decimal)item.Sendquantity,
                                        0,
                                        item.PackagingLength,
                                        item.PackagingType,
                                        item.PackagingUnit,
                                        item.ProductionOrder.Id,
                                        item.ProductionOrder.No,
                                        item.ProductionOrder.Type,
                                        item.ProductionOrder.OrderQuantity,
                                        item.ProductionOrder.CreatedUtc,
                                        item.ProcessType.Id,
                                        item.ProcessType.Name,
                                        item.YarnMaterial.Id,
                                        item.YarnMaterial.Name,
                                        item.Unit,
                                        item.UomUnit,
                                        0,
                                        item.Description,
                                        item.ProductSKUId,
                                        item.FabricSKUId,
                                        item.ProductSKUCode,
                                        item.ProductPackingId,
                                        item.FabricPackingId,
                                        item.ProductPackingCode

                                );
                            modelSummary.FlagForCreate(_identityProvider.Username, UserAgent);

                            listSummary.Add(modelSummary);

                            _dbSetSummary.Add(modelSummary);
                        }
                        else
                        {
                            double balanceUpdate = modelSummary.Balance + (item.Sendquantity * item.PackagingLength);
                            double balanceRemainsUpdate = modelSummary.BalanceRemains + (item.Sendquantity * item.PackagingLength);
                            decimal packagingQtyUpdate = modelSummary.PackagingQty + (decimal) item.Sendquantity;
                            decimal packagingQtyRemainsUpdate = modelSummary.PackagingQtyRemains + (decimal) item.Sendquantity;
                            //modelSummary.FlagForUpdate(_identityProvider.Username, UserAgent);
                            //modelSummary.SetBalanceRemains(balanceUpdate, _identityProvider.Username, UserAgent);
                            //var modelSummaries = _dbSetSummary.FirstOrDefault(entity => entity.Id == modelSummary.Id);
                            //modelSummaries.BalanceRemains = balanceUpdate;
                            modelSummary.Balance = balanceUpdate;
                            modelSummary.BalanceRemains = balanceRemainsUpdate;
                            modelSummary.PackagingQty = packagingQtyUpdate;
                            modelSummary.PackagingQtyRemains = packagingQtyRemainsUpdate;
                            //EntityExtension.FlagForUpdate(modelSummaries, _identityProvider.Username, UserAgent);
                            EntityExtension.FlagForUpdate(modelSummary, _identityProvider.Username, UserAgent);
                            listSummary.Add(modelSummary);


                        }
                        #endregion

                        #region update dpWarehousePreInput
                        var modelPreInput = _dPWarehousePreInputRepository.GetDbSet().FirstOrDefault(s => s.ProductPackingCode.Contains(item.ProductPackingCode));



                        modelPreInput.BalanceReceipt = modelPreInput.BalanceReceipt + (item.Sendquantity * item.PackagingLength);
                        modelPreInput.BalanceRemains = modelPreInput.BalanceRemains - (item.Sendquantity * item.PackagingLength);
                        modelPreInput.PackagingQtyReceipt = modelPreInput.PackagingQtyReceipt + (decimal)item.Sendquantity;
                        modelPreInput.PackagingQtyRemains = modelPreInput.PackagingQtyRemains - (decimal)item.Sendquantity;

                        EntityExtension.FlagForUpdate(modelPreInput, _identityProvider.Username, UserAgent);

                        #endregion

                       
                    }

                    Created = await _dbContext.SaveChangesAsync();
                    await createMovementAv(listItem, listSummary, modelId, bonNo);


                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);

                }
            
            
            }

            return Created;
        }
        private string GenerateBonNo(int totalPreviousData, DateTimeOffset date)
        {
            return string.Format("{0}.{1}.{2}", DyeingPrintingArea.GJ, date.ToString("yy"), totalPreviousData.ToString().PadLeft(4, '0'));
        }

        private async Task<int> createMovement(DPWarehouseInputModel model, List<DPWarehouseSummaryModel> modelSum)
        {
            int count = 0;
            foreach (var item in model.DPWarehouseInputItems)
            {
                var IdSum = modelSum.FirstOrDefault(x => x.ProductPackingCode == item.ProductPackingCode && x.TrackId == 0);

                var modelMovement = new DPWarehouseMovementModel(
                            DateTime.Now,
                            DyeingPrintingArea.GUDANGJADI,
                            DyeingPrintingArea.IN,
                            model.Id,
                            item.Id,
                            model.BonNo,
                            IdSum.Id,
                            item.ProductionOrderId,
                            item.ProductionOrderNo,
                            item.Buyer,
                            item.Construction,
                            item.Unit,
                            item.Color,
                            item.Motif,
                            item.UomUnit,
                            item.Balance,
                            item.Grade,
                            item.ProductionOrderType,
                            item.Description,
                            item.PackagingType,
                            item.PackagingQty,
                            item.PackagingUnit,
                            item.PackagingLength,
                            item.MaterialOrigin,
                            0,
                            "",
                            "",
                            item.ProductPackingId,
                            item.ProductPackingCode

                            );

                modelMovement.FlagForCreate(_identityProvider.Username, UserAgent);

                _dbSetMovement.Add(modelMovement);


                

            }

            count = await _dbContext.SaveChangesAsync();
            return count;
        }

        private async Task<int> createMovementAv(List<DPWarehouseInputItemModel> modelItem, List<DPWarehouseSummaryModel> modelSum, int modelId, string modelBonNo)
        {
            int count = 0;
            
                

            foreach (var item in modelItem)
            {
                var IdSum = modelSum.FirstOrDefault(x => x.ProductPackingCode == item.ProductPackingCode && x.TrackId == 0);

                var modelMovement = new DPWarehouseMovementModel(
                            DateTime.Now,
                            DyeingPrintingArea.GUDANGJADI,
                            DyeingPrintingArea.IN,
                            modelId,
                            item.Id,
                            modelBonNo,
                            IdSum.Id,
                            item.ProductionOrderId,
                            item.ProductionOrderNo,
                            item.Buyer,
                            item.Construction,
                            item.Unit,
                            item.Color,
                            item.Motif,
                            item.UomUnit,
                            item.Balance,
                            item.Grade,
                            item.ProductionOrderType,
                            item.Description,
                            item.PackagingType,
                            item.PackagingQty,
                            item.PackagingUnit,
                            item.PackagingLength,
                            item.MaterialOrigin,
                            0,
                            "",
                            "",
                            item.ProductPackingId,
                            item.ProductPackingCode

                            );

                modelMovement.FlagForCreate(_identityProvider.Username, UserAgent);

                _dbSetMovement.Add(modelMovement);
            }
            count = await _dbContext.SaveChangesAsync();
            return count;
        }


    }




}
