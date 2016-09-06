﻿using Anchi.ERP.Common;
using Anchi.ERP.Domain.Customers;
using Anchi.ERP.Domain.Employees;
using Anchi.ERP.Domain.RepairOrder.Enum;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Anchi.ERP.Domain.RepairOrder
{
    /// <summary>
    /// 维修单
    /// </summary>
    public class RepairOrder : BaseDomain
    {
        /// <summary>
        /// 应收金额
        /// </summary>
        [Required]
        public decimal Amount
        {
            get; set;
        }

        private DateTime completeOn;
        /// <summary>
        /// 完工时间
        /// </summary>
        public DateTime CompleteOn
        {
            get
            {
                if (completeOn < SqlDateTime.Min)
                    completeOn = SqlDateTime.Min;

                return completeOn;
            }
            set
            {
                completeOn = value;
            }
        }

        private DateTime settlementOn;
        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime SettlementOn
        {
            get
            {
                if (settlementOn < SqlDateTime.Min)
                    settlementOn = SqlDateTime.Min;

                return settlementOn;
            }
            set
            {
                settlementOn = value;
            }
        }

        /// <summary>
        /// 维修单状态
        /// </summary>
        [Required]
        public EnumRepairOrderStatus Status
        {
            get; set;
        }

        /// <summary>
        /// 结算状态
        /// </summary>
        [Required]
        public EnumSettlementStatus SettlementStatus
        {
            get; set;
        }

        /// <summary>
        /// 客户信息
        /// </summary>
        public virtual Customer Customer
        {
            get; set;
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        [Required]
        public Guid CustomerId
        {
            get; set;
        }

        /// <summary>
        /// 接待人ID
        /// </summary>
        [Required]
        public Guid ReceptionById
        {
            get; set;
        }

        /// <summary>
        /// 接待人信息
        /// </summary>
        public virtual Employee ReceptionBy
        {
            get; set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000)]
        public string Remark
        {
            get; set;
        }

        private IList<RepairOrderItem> itemList;
        /// <summary>
        /// 维修项目列表
        /// </summary>
        public virtual IList<RepairOrderItem> ItemList
        {
            get
            {
                if (itemList == null)
                    itemList = new List<RepairOrderItem>();

                return itemList;
            }
            set
            {
                itemList = value;
            }
        }

        private IList<RepairProductItem> productList;
        /// <summary>
        /// 配件明细列表
        /// </summary>
        public virtual IList<RepairProductItem> ProductList
        {
            get
            {
                if (productList == null)
                    productList = new List<RepairProductItem>();

                return productList;
            }
            set
            {
                productList = value;
            }
        }
    }
}