﻿using Anchi.ERP.Common.Filter;
using Anchi.ERP.Domain;
using Anchi.ERP.IRespository;
using Chloe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Anchi.ERP.Repository
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : IBaseRespository<T> where T : BaseDomain, new()
    {
        #region 根据ID获取数据
        /// <summary>
        /// 根据ID获取数据
        ///     不加载关联数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public virtual T Get(Guid Id)
        {
            using (var context = new AnchiDbContext())
            {
                return context.Query<T>().FirstOrDefault(item => item.Id == Id);
            }
        }

        /// <summary>
        /// 根据ID获取数据
        ///     加载关联数据，基仓储类不关联，由各子仓储类取重写实现
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public virtual T GetModel(Guid Id)
        {
            using (var context = new AnchiDbContext())
            {
                return context.Query<T>().FirstOrDefault(item => item.Id == Id);
            }
        }
        #endregion

        #region 修改数据
        /// <summary>
        /// 修改数据
        ///     不修改关联对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Update(T model)
        {
            using (var context = new AnchiDbContext())
            {
                context.Update(model);
            }
        }

        /// <summary>
        /// 修改数据
        ///     修改关联对象，基仓储不修改关联，由各子仓储类重写修改
        /// </summary>
        /// <param name="model"></param>
        public virtual void UpdateModel(T model)
        {
            using (var context = new AnchiDbContext())
            {
                context.Update(model);
            }
        }
        #endregion

        #region 创建数据
        /// <summary>
        /// 创建数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void Create(T model)
        {
            using (var context = new AnchiDbContext())
            {
                context.Insert(model);
            }
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="IdArray"></param>
        public virtual bool Delete(params Guid[] IdArray)
        {
            using (var context = new AnchiDbContext())
            {
                context.CurrentSession.BeginTransaction();

                foreach (var Id in IdArray)
                {
                    context.Delete<T>(item => item.Id == Id);
                }

                context.CurrentSession.CommitTransaction();
            }
            return true;
        }
        #endregion

        #region 分页查找数据
        /// <summary>
        /// 分页查找数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual PagedQueryResult<TModel> FindPaged<TModel>(PagedQueryFilter filter) where TModel : new()
        {
            var result = new PagedQueryResult<TModel>();

            var sql = filter.SQL;
            var paramArray = ConvertParamDictToArray(filter.ParamDict);

            using (var context = new AnchiDbContext())
            {
                var pagedSql = string.Format("SELECT * FROM ({0}) temp LIMIT {1} OFFSET {2}",
                                        filter.SQL, filter.PageSize, filter.PageSize * filter.PageIndex);
                result.Data = context.SqlQuery<TModel>(pagedSql, paramArray).ToList();

                var countSql = string.Format("SELECT COUNT(1) FROM ({0}) AS temp", sql);
                result.TotalCount = context.SqlQuery<int>(countSql, paramArray).FirstOrDefault();
            }

            result.PageIndex = filter.PageIndex;
            result.PageSize = filter.PageSize;
            result.TotalPage = result.TotalCount / filter.PageSize;
            if (result.TotalCount % result.PageSize > 0)
                result.TotalPage++;

            return result;
        }
        #endregion

        #region 不分页查找数据
        /// <summary>
        /// 不分页查找数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<TModel> Find<TModel>(QueryFilter filter) where TModel : new()
        {
            var result = new List<TModel>();

            var sql = filter.SQL;
            var paramArray = ConvertParamDictToArray(filter.ParamDict);
            using (var context = new AnchiDbContext())
            {
                result = context.SqlQuery<TModel>(sql, paramArray).ToList();
            }

            return result;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 转换参数字典为Chloe参数数组
        /// </summary>
        /// <param name="paramDict"></param>
        /// <returns></returns>
        private static DbParam[] ConvertParamDictToArray(IDictionary<string, object> paramDict)
        {
            var paramList = new List<DbParam>();
            foreach (var item in paramDict)
            {
                var param = DbParam.Create(item.Key, item.Value);
                paramList.Add(param);
            }

            return paramList.ToArray();
        }
        #endregion 
    }
}
