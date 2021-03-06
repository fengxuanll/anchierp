﻿using Anchi.ERP.Common.Filter;
using Anchi.ERP.Domain.PurchaseOrders;
using Anchi.ERP.Domain.PurchaseOrders.Filter;
using Anchi.ERP.Service.Employees;
using Anchi.ERP.Service.Purchases;
using Anchi.ERP.Service.Suppliers;
using Anchi.ERP.ServiceModel.Purchases;
using Anchi.ERP.UI.Web.Filter;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Anchi.ERP.UI.Web.Controllers
{
    /// <summary>
    /// 采购管理
    /// </summary>
    [UserAuthorize]
    public class PurchaseController : BaseController
    {
        #region 构造函数和属性
        public PurchaseController(PurchaseService purchaseService, EmployeeService employeeService, SupplierService supplierService)
        {
            this.PurchaseService = purchaseService;
            this.EmployeeService = employeeService;
            this.SupplierService = supplierService;
        }

        PurchaseService PurchaseService { get; }
        EmployeeService EmployeeService { get; }
        SupplierService SupplierService { get; }
        #endregion

        #region 添加采购单
        /// <summary>
        /// 添加采购单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.EmployeeList = EmployeeService.FindNormalList();

            var model = new PurchaseOrder();
            model.PurchaseOn = DateTime.Now;
            return View("Edit", model);
        }
        #endregion

        #region 采购单管理
        /// <summary>
        /// 采购单管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult List()
        {
            ViewBag.EmployeeList = EmployeeService.FindNormalList();
            return View("Index");
        }

        /// <summary>
        /// 查询维修项目列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult List(FindPurchaseOrderFilter filter)
        {
            var result = this.PurchaseService.FindList(filter);
            return new BetterJsonResult(result, true);
        }
        #endregion

        #region 修改采购单
        /// <summary>
        /// 修改采购单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            ViewBag.EmployeeList = EmployeeService.FindNormalList();

            var model = PurchaseService.Get(id);
            return View("Edit", model);
        }

        /// <summary>
        /// 获取采购单详细信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEditModel(Guid Id)
        {
            var model = PurchaseService.GetModel(Id);
            return new BetterJsonResult(model, true);
        }
        #endregion

        #region 保存采购单
        /// <summary>
        /// 保存采购单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Save(PurchaseOrder model)
        {
            try
            {
                if (model.Id == Guid.Empty)
                    model.CreatedById = base.CurrentUser.Id;

                PurchaseService.SaveOrUpdate(model);
                return new BetterJsonResult();
            }
            catch (Exception ex)
            {
                return new BetterJsonResult(ex.Message);
            }
        }
        #endregion

        #region 结算采购单
        /// <summary>
        /// 结算采购单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Settlement()
        {
            return View();
        }

        /// <summary>
        /// 采购结算
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SettlementOrder(PurchaseSettlementModel model)
        {
            try
            {
                PurchaseService.Settlement(model);
                return new BetterJsonResult();
            }
            catch (Exception ex)
            {
                return new BetterJsonResult(ex.Message);
            }
        }
        #endregion

        #region 设置已到货
        /// <summary>
        /// 设置已到货
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public ActionResult SetArrival(IList<Guid> idList)
        {
            try
            {
                PurchaseService.SetArrival(idList);
                return new BetterJsonResult();
            }
            catch (Exception ex)
            {
                return new BetterJsonResult(ex.Message);
            }
        }
        #endregion

        #region 取消采购单
        /// <summary>
        /// 取消采购单
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Cancel(IList<Guid> idList)
        {
            try
            {
                PurchaseService.CancelOrder(idList);
                return new BetterJsonResult();
            }
            catch (Exception ex)
            {
                return new BetterJsonResult(ex.Message);
            }
        }
        #endregion
    }
}