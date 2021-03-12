﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyNetCore.IServices;
using MyNetCore.IRepository;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace MyNetCore.Web.API
{
    /// <summary>
    /// 二维码生成
    /// </summary>
    [HiddenApi]
    public class QRController : BaseOpenAPIController
    {
        private readonly ILogger<QRController> _logger;

        public QRController(ILogger<QRController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="_qr"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpGet, Route("get")]
        public async Task<FileContentResult> Get([FromServices] IQRCodeServices _qr, string content)
        {
            //var buffer = await _qr.GenerateQRCode(content);
            var iconPath = _hostEnvironment.ContentRootPath;//D:\\WorkSpace\\GitHub\\MyNetCore\\MyNetCore.Web
            iconPath += "\\wwwroot\\favicon.ico";
            if (System.IO.File.Exists(iconPath))
            {
                var buffer = await _qr.GenerateQRCode(content, iconPath);
                return File(buffer, "image/jpeg");
            }
            else
            {
                var buffer = await _qr.GenerateQRCode(content);
                return File(buffer, "image/jpeg");
            }
        }
    }
}
