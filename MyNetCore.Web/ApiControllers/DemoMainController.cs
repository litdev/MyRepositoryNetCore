﻿/**
*┌──────────────────────────────────────────────────────────────┐
*│　描    述：演示主体接口控制器
*│　作    者：杨习友
*│　版    本：1.0 使用Razor引擎自动生成
*│　创建时间：2021-03-28 16:06:58
*└──────────────────────────────────────────────────────────────┘
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyNetCore.IServices;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace MyNetCore.Web.ApiControllers
{
    /// <summary>
    /// 演示主体管理
    /// </summary>
	[PermissionHandler("演示", "演示主体", "demoMain", 10)]
    public class DemoMainController : BaseOpenApiController
    {
        private readonly ILogger<DemoMainController> _logger;
        private readonly IDemoMainServices _demoMainServices;

        public DemoMainController(ILogger<DemoMainController> logger, IDemoMainServices demoMainServices)
        {
            _logger = logger;
            _demoMainServices = demoMainServices;
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("get/pagelist")]
        [Permission("查看", "show")]
        public async Task<ApiResult> GetPageList(Model.RequestModel.DemoMainPageModel model)
        {
            var data = await _demoMainServices.GetPageListBasicAsync(model, out long total);

            return ApiResult.OK(total, data);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("get/info")]
        [Permission("查看", "show")]
        public async Task<ApiResult> GetInfo(int id)
        {
            var data = await _demoMainServices.GetModelFullAsync(id);

            return ApiResult.OK(data);
        }

        /// <summary>
        /// 添加或修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("modify")]
        [Permission("编辑", "modify")]
        public async Task<ApiResult> Post(Model.Entity.DemoMain model)
        {
            var data = await _demoMainServices.ModifyAsync(model);

            return ApiResult.OK(data);
        }

        /// <summary>
        /// 根据ids批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete, Route("delete")]
        [Permission("删除", "delete")]
        public async Task<ApiResult> Delete(string ids)
        {
            var affrows = await _demoMainServices.DeleteByIdsAsync(ids.SplitWithComma());

            return ApiResult.OK($"受影响的行数:{affrows}");
        }

    }
}
